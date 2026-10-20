extends Control

@export var key_to_press: Key

@export var anticipation_particles: Array[GPUParticles2D]
@export var slash_start_particles: Array[GPUParticles2D]
@export var slash_end_particles: Array[GPUParticles2D]
@export var card_particles: Array[GPUParticles2D]

@export var slash_start_delay: float
@export var slash_end_delay: float

@export var card: Control

var previous_pressed: bool

func _ready() -> void :
	previous_pressed = false

	card.visible = true
	card.scale = Vector2.ONE

func _unhandled_input(event: InputEvent) -> void :
	if not visible:
		return

	if event is InputEventKey:
		var current_pressed = event.pressed

		if current_pressed and previous_pressed != current_pressed\
		and event.keycode == key_to_press:
			play_vfx()

		previous_pressed = current_pressed

func wait_for_seconds(duration: float) -> void :
	var timer = 0

	while timer < duration:
		timer += get_process_delta_time()
		await get_tree().process_frame

func play_vfx() -> void :
	if !card.visible:
		card.scale = Vector2.ZERO
		card.visible = true

		var show_tween = get_tree().create_tween()
		show_tween.tween_property(card, "scale", Vector2.ONE, 0.4).set_ease(Tween.EASE_OUT).set_trans(Tween.TRANS_BOUNCE)

		await show_tween.finished
		await wait_for_seconds(0.5)

	for i in anticipation_particles:
		i.restart()

	await wait_for_seconds(slash_start_delay)

	for i in slash_start_particles:
		i.restart()

	await wait_for_seconds(slash_end_delay)

	for i in slash_end_particles:
		i.restart()

	for i in card_particles:
		i.restart()

	card.visible = false
