extends PanelContainer


















signal feedback_submitted(feedback: SentryFeedback)

signal feedback_cancelled()


@export var logo_visible: bool = true:
	set(value):
		logo_visible = value
		_update_controls()


@export var name_visible: bool = true:
	set(value):
		name_visible = value
		_update_controls()


@export var email_visible: bool = true:
	set(value):
		email_visible = value
		_update_controls()


@export var minimum_words: int = 2


@onready var _message_edit: TextEdit = %MessageEdit
@onready var _name_edit: LineEdit = %NameEdit
@onready var _email_edit: LineEdit = %EmailEdit
@onready var _submit_button: Button = %SubmitButton
@onready var _character_counter: Label = %CharacterCounter


func _ready() -> void :
	_update_controls()
	_on_message_edit_text_changed()


func _update_controls() -> void :
	if is_node_ready():
		%Logo.visible = logo_visible
		%EmailSection.visible = email_visible
		%NameSection.visible = name_visible


func _on_submit_button_pressed() -> void :
	var feedback: = SentryFeedback.new()
	feedback.message = _message_edit.text
	feedback.name = _name_edit.text
	feedback.contact_email = _email_edit.text

	SentrySDK.capture_feedback(feedback)

	feedback_submitted.emit(feedback)


	_message_edit.text = ""


func _on_message_edit_text_changed() -> void :
	var message: String = _message_edit.text
	if message.length() > 4096:
		var col: int = _message_edit.get_caret_column()
		message = message.substr(0, 4096)
		_message_edit.text = message
		_message_edit.set_caret_column(col)
	_submit_button.disabled = _count_words(message) < minimum_words
	_character_counter.text = str(message.length()) + "/4096"
	_character_counter.visible = message.length() > 3000


func _count_words(text: String) -> int:
	var words: PackedStringArray = text.strip_edges().split(" ", false)
	var clean_words: PackedStringArray = []
	for word in words:
		if word.strip_edges() != "":
			clean_words.append(word)
	return clean_words.size()


func _on_cancel_button_pressed() -> void :
	feedback_cancelled.emit()


func _on_visibility_changed() -> void :
	if visible and _message_edit:
		_message_edit.grab_focus()
