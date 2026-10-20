extends Node2D

@export var key_to_press: Key

@export var continuous_particles: Array[GPUParticles2D]
@export var one_shot_particles: Array[GPUParticles2D]

@export var scream_duration: float = 1.0
@export var camera_shake_intensity: float = 5

@export var playground_camera: VfxPlaygroundCamera

var previous_pressed: bool

func _ready() -> void :
	previous_pressed = false

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
	for i in one_shot_particles:
		i.lifetime = scream_duration
		i.restart()

	for i in continuous_particles:
		i.restart()
		i.emitting = true

	if camera_shake_intensity > 0:
		playground_camera.shake(camera_shake_intensity, scream_duration);

	await wait_for_seconds(scream_duration)

	for i in continuous_particles:
		i.emitting = false

func wait_for_seconds(duration: float) -> void :
	var timer = 0

	while timer < duration:
		timer += get_process_delta_time()
		await get_tree().process_frame
