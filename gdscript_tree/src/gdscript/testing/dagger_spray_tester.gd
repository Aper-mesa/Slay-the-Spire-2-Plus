extends Node2D

@export var key_to_press: Key

@export var dagger_particles: Array[GPUParticles2D]
@export var impact_particles: Array[GPUParticles2D]

@export var impact_container: Node2D
@export var player_center: Node2D
@export var enemy_center: Node2D

@export var spray_interval: float = 0.5
@export var impact_delay: float = 0.2

@export var repeats: int = 2

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
	for j in range(0, repeats):
		for i in dagger_particles:
			i.restart()

		await wait_for_seconds(impact_delay)

		for i in impact_particles:
			i.restart()

		playground_camera.shake(4, spray_interval + impact_delay + 0.5);

		await wait_for_seconds(spray_interval)


func wait_for_seconds(duration: float) -> void :
	var timer = 0

	while timer < duration:
		timer += get_process_delta_time()
		await get_tree().process_frame
