extends Node2D

@export var key_to_press: Key

@export var minion_sprite: Sprite2D
@export var minion_textures: Array[Texture2D]

@export var minion_animator: AnimationPlayer
@export var minion_animations: Array[String]

@export var minion_vfx: Array[ParticlesContainer]
@export var falling_trail: Node2D
@export var falling_vfx: ParticlesContainer
@export var impact_vfx: ParticlesContainer

@export var flight_time: float
@export var falling_vfx_entry_time: float

@export var horizontal_curve: Curve
@export var vertical_curve: Curve
@export var texture_curve: Curve
@export var max_height: float

@export var source_node: Node2D
@export var source_offset: Vector2
@export var destinaton_node: Node2D
@export var destination_offset: Vector2

@export var playground_camera: VfxPlaygroundCamera

var previous_pressed: bool
var playing_vfx: bool

var previous_pos: Vector2
var previous_index: int = -1

var source_position:
	get:
		return source_node.global_position + source_offset

var destination_position:
	get:
		return destinaton_node.global_position + destination_offset

func _ready() -> void :
	set_minion_visible(false)

	previous_pressed = false
	playing_vfx = false

	previous_pos = global_position

	for i in minion_vfx:
		if i == null:
			continue

		i.set_emitting(false)

func _unhandled_input(event: InputEvent) -> void :
	if not visible:
		return

	if event is InputEventKey:
		var current_pressed = event.pressed

		if current_pressed and previous_pressed != current_pressed\
		and event.keycode == key_to_press:
			play_vfx()

		previous_pressed = current_pressed

func set_minion_visible(minion_visible: bool) -> void :
	minion_sprite.self_modulate = Color(1, 1, 1, 1) if minion_visible else Color(1, 1, 1, 0)

func update_minion_sprite(index: int) -> void :
	if previous_index == index:
		return

	previous_index = index

	var texture = minion_textures[clampi(index, 0, minion_textures.size() - 1)]

	minion_sprite.texture = texture

	var anim_string = minion_animations[clampi(index, 0, minion_animations.size() - 1)]

	if minion_animator.current_animation != anim_string:
		minion_animator.play(anim_string)

	for i in len(minion_vfx):
		if minion_vfx[i] == null:
			continue

		if i == index:
			minion_vfx[i].restart()

func play_vfx() -> void :
	if playing_vfx:
		return

	playing_vfx = true

	var start_pos = source_position
	var end_pos = destination_position

	update_minion_sprite(0)
	minion_sprite.global_position = start_pos

	set_minion_visible(true)

	var timer = 0

	var is_playing_falling_vfx = false

	while timer < flight_time:
		var interpolation = timer / flight_time

		var horizontal_curve_val = horizontal_curve.sample(interpolation)
		var vertical_curve_val = vertical_curve.sample(interpolation)
		var sprite_curve_val = texture_curve.sample(interpolation)

		update_minion_sprite(floor(sprite_curve_val))

		var target_pos = lerp(start_pos, end_pos, horizontal_curve_val)
		target_pos += (Vector2.UP * vertical_curve_val * max_height)

		falling_vfx.global_position = target_pos

		if timer >= falling_vfx_entry_time and !is_playing_falling_vfx:
			falling_vfx.restart()
			falling_trail.visible = true
			is_playing_falling_vfx = true

		minion_sprite.global_position = target_pos

		timer += get_process_delta_time()
		await get_tree().process_frame

	set_minion_visible(false)

	playground_camera.shake(5, 0.5);

	impact_vfx.global_position = destinaton_node.global_position
	impact_vfx.restart()

	falling_trail.visible = false
	falling_vfx.set_emitting(false)

	playing_vfx = false
