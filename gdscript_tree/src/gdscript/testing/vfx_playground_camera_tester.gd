class_name VfxPlaygroundCamera extends Node2D

var scene_tree: SceneTree

var shaking: bool
var base_shake_intensity: float = 1

var timer: float = 0

@export var pivot: Node2D

@export var shake_intensity_multiplier: float = 0
@export var shake_duration: float = 0
@export var shake_curve: Curve

@export var use_animated_timer: bool
@export var animated_timer: float:
	set(value):
		animated_timer = value
		force_shake(animated_timer)

var previous_shake_timer: float = -1.0

func _ready():
	scene_tree = get_tree()
	shaking = false

func _process(delta: float):
	if !use_animated_timer:
		if timer <= 0 or shake_duration <= 0:
			offset_pivot_local_pos(Vector2.ZERO)
		else:
			var interpolation = timer / shake_duration
			var curve_value = shake_curve.sample(1.0 - interpolation)
			var final_shake_intensity = base_shake_intensity\
			* shake_intensity_multiplier\
			* curve_value

			var random_point_in_sphere = get_random_point_in_sphere(final_shake_intensity)
			offset_pivot_local_pos(random_point_in_sphere)

			timer -= delta
	else:
		pass

func shake(intensity_mult: float, duration: float):
	self.shake_intensity_multiplier = intensity_mult
	self.shake_duration = duration

	timer = duration

func force_shake(shake_timer: float):
	if shake_timer <= 0 or shake_duration <= 0:
		offset_pivot_local_pos(Vector2.ZERO)
		return

	if shake_timer == previous_shake_timer:
		return

	previous_shake_timer = shake_timer

	var interpolation = shake_timer / shake_duration
	var curve_value = shake_curve.sample(1.0 - interpolation)
	var final_shake_intensity = base_shake_intensity\
	* shake_intensity_multiplier\
	* curve_value

	var random_point_in_sphere = get_random_point_in_sphere(final_shake_intensity)
	offset_pivot_local_pos(random_point_in_sphere)

func get_random_point_in_sphere(radius: float) -> Vector2:
	var theta = randf() * 2 * PI
	var r = radius;

	var sin_theta = sin(theta)
	var cos_theta = cos(theta)

	return Vector2(
		r * cos_theta, 
		r * sin_theta
	)

func offset_pivot_local_pos(offset: Vector2):
	pivot.position = offset
