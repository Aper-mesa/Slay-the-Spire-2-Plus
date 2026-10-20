extends Node2D

@export var key_to_press: Key

@export var flame_duration: float = 0.65

@export var flame_particles: Array[GPUParticles2D]
@export var end_particles: Array[GPUParticles2D]

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
	for i in flame_particles:
		i.restart()

	await wait_for_seconds(flame_duration)

	for i in end_particles:
		i.restart()

func wait_for_seconds(duration: float) -> void :
	var timer = 0

	while timer < duration:
		timer += get_process_delta_time()
		await get_tree().process_frame
