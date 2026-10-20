extends ColorRect

@export var key_to_press: Key

@export var duration: float = 1.0
@export var alpha_multiplier_curve: Curve
@export var noise_additional_offset_curve: Curve
@export var gradient: Gradient

var previous_pressed: bool

var original_material: Material
var material_copy: Material

func _ready() -> void :
	previous_pressed = false

	original_material = material;

	material_copy = original_material.duplicate(true)
	material = material_copy;

	randomize_initial_offset()
	set_properties(1)

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

	set_properties(0)

	while timer < duration:
		var interpolation = timer / duration

		set_properties(interpolation)

		timer += get_process_delta_time()
		await get_tree().process_frame

	set_properties(1)



func randomize_initial_offset() -> void :
	material_copy.set("shader_parameter/noise_initial_offset", Vector2(randf(), randf()))

func set_properties(interpolation: float) -> void :
	var alpha_multiplier_curve_val = alpha_multiplier_curve.sample(interpolation)
	var noise_addition_offset_curve_val = noise_additional_offset_curve.sample(interpolation)
	var gradient_sample = gradient.sample(interpolation)

	material_copy.set("shader_parameter/alpha_multiplier", alpha_multiplier_curve_val)
	material_copy.set("shader_parameter/noise_additional_offset", noise_addition_offset_curve_val)
	material_copy.set("shader_parameter/main_color", gradient_sample)
