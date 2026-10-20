@tool
extends Node

@export var play: bool:
	set(value):
		if value:
			for i in particles:
				i.restart()
		play = false

@export var particles: Array[GPUParticles2D]
