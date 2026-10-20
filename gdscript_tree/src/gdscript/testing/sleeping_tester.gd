extends Node2D

@export var key_to_press: Key
@export var burst_particles: Array[GPUParticles2D]
@export var continuous_particles: Array[GPUParticles2D]

var previous_pressed: bool

func _ready():
	previous_pressed = false

func _unhandled_input(event: InputEvent) -> void :
	if event is InputEventKey:
		var current_pressed = event.pressed

		if previous_pressed != current_pressed\
		and event.keycode == key_to_press:
			if current_pressed:
				start_sleep()
			else:
				end_sleep()

		previous_pressed = current_pressed

func start_sleep() -> void :
	for i in burst_particles:
		i.restart()

	for i in continuous_particles:
		i.restart()
		i.emitting = true

func end_sleep() -> void :
	for i in continuous_particles:
		i.emitting = false
