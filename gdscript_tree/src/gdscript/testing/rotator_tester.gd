extends Node2D

@export var key_to_press: Key

@export var rotation_speed: float;

var previous_pressed: bool

var active: bool = true

func _process(_delta: float) -> void :
	rotation_degrees += rotation_speed * (_delta if active else 0.0)

func _unhandled_input(event: InputEvent) -> void :
	if not visible:
		return

	if event is InputEventKey:
		var current_pressed = event.pressed

		if current_pressed and previous_pressed != current_pressed\
		and event.keycode == key_to_press:
			active = !active

		previous_pressed = current_pressed
