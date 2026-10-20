extends Node2D

@export var key_to_press: Key

@export var potion_sprite: Node2D

@export var flight_time: float
@export var horizontal_curve: Curve
@export var vertical_curve: Curve
@export var max_height: float

@export var rotation_influence_curve: Curve
@export var rotation_amount: float

@export var source_node: Node2D
@export var source_offset: Vector2
@export var destinaton_node: Node2D
@export var destination_offset: Vector2

@export var splash_particles: Array[GPUParticles2D]
@export var target_spine_sprite: SpineSprite

@export var overlay_duration: float
@export var potion_liquid_overlay_material: Material

var previous_pressed: bool
var playing_vfx: bool

var source_position:
	get:
		return source_node.global_position + source_offset

var destination_position:
	get:
		return destinaton_node.global_position + destination_offset

func _ready() -> void :
	potion_sprite.visible = false
	previous_pressed = false
	playing_vfx = false

	target_spine_sprite.normal_material = null

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
	if playing_vfx:
		return

	playing_vfx = true

	var start_pos = source_position
	var end_pos = destination_position

	potion_sprite.global_position = start_pos
	potion_sprite.rotation_degrees = randf() * 360

	potion_sprite.visible = true

	var timer = 0

	while timer < flight_time:
		var delta_time = get_process_delta_time()

		var interpolation = timer / flight_time

		var horizontal_curve_val = horizontal_curve.sample(interpolation)
		var vertical_curve_val = vertical_curve.sample(interpolation)

		var rotation_influence_val = rotation_influence_curve.sample(interpolation)

		potion_sprite.rotate(deg_to_rad(rotation_influence_val * rotation_amount * delta_time))

		var target_pos = lerp(start_pos, end_pos, horizontal_curve_val) + (Vector2.UP * vertical_curve_val * max_height)
		potion_sprite.global_position = target_pos

		timer += delta_time
		await get_tree().process_frame

	potion_sprite.visible = false

	for i in splash_particles:
		i.self_modulate = Color.WHITE
		i.restart()

	var material_instance = potion_liquid_overlay_material.duplicate()
	target_spine_sprite.normal_material = material_instance

	material_instance.set("shader_parameter/overlay_influence", 1.0)

	timer = 0

	while timer < overlay_duration:
		var delta_time = get_process_delta_time()

		var interpolation = timer / overlay_duration
		material_instance.set("shader_parameter/overlay_influence", 1.0 - interpolation)

		timer += delta_time
		await get_tree().process_frame

	material_instance = null
	target_spine_sprite.normal_material = null

	playing_vfx = false
