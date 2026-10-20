extends Node2D

@export var key_to_press: Key

@export var line: Line2D
@export var duration: float = 2.0

@export var pos_a: Node2D
@export var pos_b: Node2D

@export var width_range: Vector2

@export var alpha_gradient: Gradient
@export var alpha_curve: Curve

@export var movement_curve: Curve

var previous_pressed: bool

func _ready() -> void :
	line.visible = false;

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
	var timer = 0

	line.visible = true;

	while timer < duration:
		var interpolation = timer / duration

		var movement_curve_val = movement_curve.sample(interpolation)

		var line_end_pos = lerp(pos_a.position, pos_b.position, movement_curve_val)

		line.set_point_position(1, line_end_pos)
		line.width = lerp(width_range.x, width_range.y, movement_curve_val)

		var alpha_curve_val = alpha_curve.sample(interpolation);
		line.self_modulate = alpha_gradient.sample(alpha_curve_val);

		timer += get_process_delta_time()
		await get_tree().process_frame

	line.visible = false;
