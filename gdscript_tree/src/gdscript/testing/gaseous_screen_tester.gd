extends AspectRatioContainer

@export var key_to_press: Key

@export var gfx: ColorRect

@export var duration: float = 1.0
@export var alpha_multiplier_curve: Curve
@export var min_base_alpha_curve: Curve
@export var erosion_curve: Curve
@export var noise_a_offset_curve: Curve
@export var noise_b_offset_curve: Curve

var previous_pressed: bool

var original_material: Material
var material_copy: Material

var noise_a_offset_y: float = 0.0
var noise_b_offset_y: float = 0.0

func _ready() -> void :
	previous_pressed = false

	original_material = gfx.material;

	material_copy = original_material.duplicate(true)
	gfx.material = material_copy

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
	noise_a_offset_y = randf()
	noise_b_offset_y = randf()

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
	var min_base_alpha_curve_val = min_base_alpha_curve.sample(interpolation)
	var noise_a_offset_curve_val = noise_a_offset_curve.sample(interpolation)
	var noise_b_offset_curve_val = noise_b_offset_curve.sample(interpolation)
	var erosion_curve_val = erosion_curve.sample(interpolation)

	material_copy.set("shader_parameter/alpha_multiplier", alpha_multiplier_curve_val)
	material_copy.set("shader_parameter/min_base_alpha", min_base_alpha_curve_val)
	material_copy.set("shader_parameter/noise_a_static_offset", Vector2(noise_a_offset_curve_val, noise_a_offset_y))
	material_copy.set("shader_parameter/noise_b_static_offset", Vector2(noise_b_offset_curve_val, noise_b_offset_y))
	material_copy.set("shader_parameter/erosion_base", erosion_curve_val)
