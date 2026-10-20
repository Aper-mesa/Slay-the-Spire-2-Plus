extends Node2D

@export var key_to_press: Key

@export var emitting_particles: Array[GPUParticles2D]
@export var start_particles: Array[GPUParticles2D]
@export var end_particles: Array[GPUParticles2D]
@export var sweeping_particles: Array[GPUParticles2D]
@export var impact_particles: Array[GPUParticles2D]

@export var sweeping_index_curve: Curve

@export var emitting_particles_duration: float = 0.65

var previous_pressed: bool

func _ready() -> void :
	for i in emitting_particles:
		i.emitting = false

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

	var played_impact_particles = false

	for i in start_particles:
		i.restart()

	for i in emitting_particles:
		i.restart()
		i.emitting = true

	var sweep_previous_index = -1

	while timer < emitting_particles_duration:
		var interpolation = timer / emitting_particles_duration
		var sweep_current_index = sweeping_index_curve.sample(interpolation)
		sweep_current_index = floori(sweep_current_index)

		if sweep_previous_index != sweep_current_index and sweep_current_index >= 0 and sweep_current_index < sweeping_particles.size():
			sweeping_particles[sweep_current_index].restart()

			sweep_previous_index = sweep_current_index

		if interpolation >= 0.5 and !played_impact_particles:
			played_impact_particles = true

			for i in impact_particles:
				i.restart()

		timer += get_process_delta_time()
		await get_tree().process_frame

	for i in end_particles:
		i.restart()

	for i in emitting_particles:
		i.emitting = false
