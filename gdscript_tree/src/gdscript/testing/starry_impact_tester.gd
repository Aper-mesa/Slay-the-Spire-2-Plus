extends Node2D

@export var key_to_press: Key

@export var hit_particles: Array[GPUParticles2D]

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
	for i in hit_particles:
		i.restart()

	playground_camera.shake(5, 0.25);

func wait_for_seconds(duration: float) -> void :
	var timer = 0

	while timer < duration:
		timer += get_process_delta_time()
		await get_tree().process_frame
