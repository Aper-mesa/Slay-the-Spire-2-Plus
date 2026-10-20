extends Control

@export var key_to_press: Key

@export var material_container: Control
@export var particles_container: Node2D
@export var particles: Array[GPUParticles2D]
@export var quick_anticipation_particles: Array[GPUParticles2D]
@export var quick_particles: Array[GPUParticles2D]
@export var quick_erosion_container: Node2D

@export var exhaust_duration: float = 1.0
@export var curve: Curve
@export var erosion_base_range: Vector2
@export var particle_y_range: Vector2

@export var quick_anticipation_duration: float = 0.25

var previous_pressed: bool

func _ready() -> void :
	previous_pressed = false

	material_container.self_modulate = Color(1.0, 1.0, 1.0, 1.0)
	quick_erosion_container.modulate = Color(1.0, 1.0, 1.0, 1.0)

	set_particles_playing(false)

func _unhandled_input(event: InputEvent) -> void :
	if not visible:
		return

	if event is InputEventKey:
		var current_pressed = event.pressed

		if current_pressed and previous_pressed != current_pressed\
		and event.keycode == key_to_press:
			if event.shift_pressed:
				play_vfx()
			else:
				play_vfx_quick()

		previous_pressed = current_pressed

func wait_for_seconds(duration: float) -> void :
	var timer = 0

	while timer < duration:
		timer += get_process_delta_time()
		await get_tree().process_frame

func set_particles_playing(is_playing: bool) -> void :
	for i in particles:
		i.emitting = is_playing

func set_progress(progress: float) -> void :
	var curve_val = curve.sample(progress)

	var erosion_base_val = lerp(erosion_base_range.x, erosion_base_range.y, curve_val)
	var particle_y_val = lerp(particle_y_range.x, particle_y_range.y, curve_val)

	material_container.set("instance_shader_parameters/erosion_base", erosion_base_val)
	particles_container.position = Vector2(0.0, particle_y_val)

func play_vfx() -> void :
	set_particles_playing(false)
	set_progress(0)

	if material_container.self_modulate == Color(1.0, 1.0, 1.0, 0.0):
		var tween = get_tree().create_tween()
		tween.tween_property(material_container, "self_modulate", Color(1.0, 1.0, 1.0, 1.0), 0.5)

		await tween.finished
		await wait_for_seconds(0.5)

	var timer = 0

	set_particles_playing(true)

	material_container.set("instance_shader_parameters/erosion_texture_x_offset", randf())

	while timer < exhaust_duration:
		var interpolation = timer / exhaust_duration
		set_progress(interpolation)

		timer += get_process_delta_time()
		await get_tree().process_frame

	set_progress(1)
	set_particles_playing(false)

	material_container.self_modulate = Color(1.0, 1.0, 1.0, 0.0)

func play_vfx_quick() -> void :
	set_particles_playing(false)
	set_progress(0)

	if material_container.self_modulate == Color(1.0, 1.0, 1.0, 0.0):
		var tween = get_tree().create_tween()
		tween.tween_property(material_container, "self_modulate", Color(1.0, 1.0, 1.0, 1.0), 0.5)

		await tween.finished
		await wait_for_seconds(0.5)

	for i in quick_anticipation_particles:
		i.restart()

	await wait_for_seconds(quick_anticipation_duration)

	for i in quick_particles:
		i.restart()

	material_container.self_modulate = Color(1.0, 1.0, 1.0, 0.0)
