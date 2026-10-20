class_name ParticlesContainer extends Node2D

@export var particles: Array[GPUParticles2D]

func set_emitting(emitting: bool) -> void :
	for i in particles:
		i.emitting = emitting

func restart() -> void :
	for i in particles:
		i.restart()
