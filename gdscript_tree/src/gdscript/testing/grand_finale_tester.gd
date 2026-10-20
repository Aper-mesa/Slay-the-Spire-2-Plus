extends Node2D

@export var key_to_press: Key

@export var silent: SpineSprite

@export var spotlight: Node2D

@export var spotlight_particles: Array[GPUParticles2D]
@export var anticipation_particles: Array[GPUParticles2D]
@export var slash_particles: Array[GPUParticles2D]
@export var hit_particles: Array[GPUParticles2D]
@export var end_particles: Array[GPUParticles2D]

@export var playground_camera: VfxPlaygroundCamera

var previous_pressed: bool

func _ready() -> void :
	previous_pressed = false

	spotlight.modulate = Color(0.0, 0.0, 0.0, 0.0)

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
	var spotlight_show_tween = get_tree().create_tween()
	spotlight_show_tween.tween_property(spotlight, "modulate", Color(1.0, 1.0, 1.0, 1.0), 1)

	play_particles(spotlight_particles)

	await wait_for_seconds(1.25)

	play_particles(anticipation_particles)

	await wait_for_seconds(0.25)

	silent.get_animation_state().set_animation("attack", false)
	silent.get_animation_state().add_animation("idle_loop", true)

	play_particles(slash_particles)

	await wait_for_seconds(0.125)

	play_particles(hit_particles)

	playground_camera.shake(10, 0.5);

	await wait_for_seconds(0.0125)

	play_particles(end_particles)

	var spotlight_end_tween = get_tree().create_tween()
	spotlight_end_tween.tween_property(spotlight, "modulate", Color(1.0, 1.0, 1.0, 0.0), 0.5)


func play_particles(particles: Array[GPUParticles2D]) -> void :
	for i in particles:
		i.restart()



func wait_for_seconds(duration: float) -> void :
	var timer = 0

	while timer < duration:
		timer += get_process_delta_time()
		await get_tree().process_frame
