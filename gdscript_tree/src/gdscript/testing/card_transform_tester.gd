extends Control

@export var key_to_press: Key

@export var overlay: Control
@export var glow: Control
@export var reveal_particles: Array[GPUParticles2D]
@export var shine_particles: Array[GPUParticles2D]
@export var end_particles: Array[GPUParticles2D]

@export var overlay_show_duration: float = 0.75
@export var overlay_idle_duration: float = 0.125
@export var overlay_hide_duration: float = 0.25

@export var glow_fade_duration: float = 0.25
@export var glow_top_scale: float = 1.5

@export var shine_delay_duration: float = 0.125

@export var end_particles_delay: float = 0.4

@export var portrait: TextureRect
@export var first_texture: Texture
@export var second_texture: Texture

@export var description_label: RichTextLabel
@export var first_description: String
@export var second_description: String

@export var title_label: Label
@export var first_title: String
@export var second_title: String

var previous_pressed: bool

var iteration = 0

func _ready() -> void :
	previous_pressed = false
	overlay.self_modulate = Color(1.0, 1.0, 1.0, 0.0)
	glow.self_modulate = Color(1.0, 1.0, 1.0, 0.0)

	iteration = 0
	update_portrait()

func update_portrait() -> void :
	var use_first_option = iteration % 2 == 0

	portrait.texture = first_texture if use_first_option else second_texture

	var new_desc = "[center]" + (first_description if use_first_option else second_description) + "[/center]"

	description_label.text = new_desc

	var new_title = first_title if use_first_option else second_title

	title_label.text = new_title


func _unhandled_input(event: InputEvent) -> void :
	if not visible:
		return

	if event is InputEventKey:
		var current_pressed = event.pressed

		if current_pressed and previous_pressed != current_pressed\
		and event.keycode == key_to_press:
			play_vfx()

		previous_pressed = current_pressed

func play_vfx() -> void :
	var quick_mode = Input.is_key_pressed(KEY_SHIFT)

	if quick_mode:
		overlay.self_modulate = Color(1.0, 1.0, 1.0, 1.0)
	else:
		overlay.self_modulate = Color(1.0, 1.0, 1.0, 0.0)

	glow.self_modulate = Color(1.0, 1.0, 1.0, 0.0)

	var overlay_show_tween = get_tree().create_tween()

	if not quick_mode:
		overlay_show_tween.tween_property(overlay, "self_modulate", Color(1.0, 1.0, 1.0, 1.0), overlay_show_duration).set_ease(Tween.EASE_OUT)
		overlay_show_tween.tween_interval(overlay_idle_duration)

	overlay_show_tween.tween_property(overlay, "self_modulate", Color(1.0, 1.0, 1.0, 0.0), overlay_hide_duration).set_ease(Tween.EASE_OUT)

	if not quick_mode:
		await wait_for_seconds(overlay_show_duration + overlay_idle_duration)

	iteration += 1
	update_portrait()

	glow.self_modulate = Color(1.0, 1.0, 1.0, 1.0)
	glow.scale = Vector2.ONE

	var glow_tween = get_tree().create_tween()

	glow_tween.tween_property(glow, "scale", Vector2.ONE * glow_top_scale, glow_fade_duration).set_trans(Tween.TRANS_QUINT).set_ease(Tween.EASE_OUT)
	glow_tween.parallel().tween_property(glow, "self_modulate", Color(1.0, 1.0, 1.0, 0.0), glow_fade_duration).set_trans(Tween.TRANS_QUINT).set_ease(Tween.EASE_OUT)

	for i in reveal_particles:
		i.restart()

	await wait_for_seconds(shine_delay_duration)

	for i in shine_particles:
		i.restart()

	await wait_for_seconds(end_particles_delay)

	for i in end_particles:
		i.restart()

func wait_for_seconds(duration: float) -> void :
	var timer = 0

	while timer < duration:
		timer += get_process_delta_time()
		await get_tree().process_frame
