extends Node2D

@export var key_to_press: Key

@export var anticipation_particles: Array[GPUParticles2D]
@export var projectile_start_particles: Array[GPUParticles2D]
@export var projectile_particles: Array[GPUParticles2D]
@export var impact_particles: Array[GPUParticles2D]

@export var anticipation_container: Node2D
@export var anticipation_duration: float = 0.2
@export var projectile_start_pos: Node2D
@export var projectile_end_pos: Node2D
@export var projectile_container: Node2D
@export var projectile_position_offset: float = 100
@export var projectile_flight_duration: float = 0.2

@export var playground_camera: VfxPlaygroundCamera

var previous_pressed: bool

func _ready() -> void :
	projectile_container.visible = false
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

func get_target_direction() -> Vector2:
	var target_direction_vec3 = Quaternion.from_euler(Vector3(0, 0, deg_to_rad(-30))) * Vector3.UP
	var target_direction = Vector2(target_direction_vec3.x, target_direction_vec3.y).normalized()

	return target_direction

func get_top_position() -> Vector2:
	var target_direction = get_target_direction()
	var intersection = Geometry2D.line_intersects_line(global_position, target_direction, Vector2.ZERO, Vector2.RIGHT)

	if intersection == null:
		return global_position

	return intersection

func initialize_node_positions() -> void :
	var target_direction = get_target_direction()
	var top_position = get_top_position()

	anticipation_container.global_position = top_position
	projectile_start_pos.global_position = top_position + (target_direction * projectile_position_offset)
	projectile_end_pos.global_position = global_position + (target_direction * projectile_position_offset)

func play_vfx() -> void :
	projectile_container.visible = false

	initialize_node_positions()

	for i in anticipation_particles:
		i.restart()

	await wait_for_seconds(anticipation_duration)

	for i in projectile_start_particles:
		i.restart()

	projectile_container.global_position = projectile_start_pos.global_position
	projectile_container.visible = true

	for i in projectile_particles:
		i.restart()

	var timer = 0

	while timer < projectile_flight_duration:
		var interpolation = timer / projectile_flight_duration

		projectile_container.global_position = lerp(projectile_start_pos.global_position, projectile_end_pos.global_position, interpolation)
		timer += get_process_delta_time()

		await get_tree().process_frame

	projectile_container.visible = false

	for i in impact_particles:
		i.restart()

	playground_camera.shake(10, 0.5);

func wait_for_seconds(duration: float) -> void :
	var timer = 0

	while timer < duration:
		timer += get_process_delta_time()
		await get_tree().process_frame
