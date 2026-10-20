extends Node

@export var key_to_press: Key
@export var particles: Array[GPUParticles2D]
@export var flip_particles: Array[GPUParticles2D]

var previous_pressed: bool

var rng: RandomNumberGenerator

func _ready():
	previous_pressed = false
	rng = RandomNumberGenerator.new()

func _unhandled_input(event: InputEvent) -> void :
	if event is InputEventKey:
		var current_pressed = event.pressed

		if event.pressed and previous_pressed != current_pressed\
		and event.keycode == key_to_press:
			for i in particles:
				i.restart()

			for i in flip_particles:
				i.scale = Vector2(1 if (rng.randf() > 0.5) else -1, 1)

		previous_pressed = current_pressed
