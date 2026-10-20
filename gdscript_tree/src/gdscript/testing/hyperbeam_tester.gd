@tool extends Node2D


@export var key_to_press: Key

@export var anticipation_particles: Array[GPUParticles2D]
@export var late_anticipation_particles: Array[GPUParticles2D]
@export var hit_particles: Array[GPUParticles2D]
@export var hit_line: Line2D
@export var laser_container: Node2D
@export var end_particles: Array[GPUParticles2D]
@export var impact_particles: Array[GPUParticles2D]
@export var impact_end_particles: Array[GPUParticles2D]

@export var playground_camera: VfxPlaygroundCamera

var previous_pressed: bool

func _ready() -> void :
	previous_pressed = false

	for i in impact_particles:
		i.visible = false

	for i in hit_particles:
		i.visible = false

	hit_line.visible = false

	laser_container.visible = false

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
	for i in impact_particles:
		i.visible = false

	for i in hit_particles:
		i.visible = false

	hit_line.visible = false

	laser_container.visible = false

	for i in anticipation_particles:
		i.restart()

	await wait_for_seconds(0.425)

	for i in late_anticipation_particles:
		i.restart()

	await wait_for_seconds(0.1)

	playground_camera.shake(10, 1);

	for i in impact_particles:
		i.visible = true

	for i in hit_particles:
		i.visible = true

	for i in hit_particles:
		i.restart()

	hit_line.visible = true

	laser_container.visible = true

	await wait_for_seconds(0.5)

	playground_camera.shake(10, 0.5);

	for i in impact_particles:
		i.visible = false

	for i in hit_particles:
		i.visible = false

	hit_line.visible = false

	laser_container.visible = false

	for i in end_particles:
		i.restart()

	for i in impact_end_particles:
		i.restart()

func wait_for_seconds(duration: float) -> void :
	var timer = 0

	while timer < duration:
		timer += get_process_delta_time()
		await get_tree().process_frame
