
class_name AudioManagerProxy extends Node

var master_bus: FmodBus
var sfx_bus: FmodBus
var ambient_bus: FmodBus
var bgm_bus: FmodBus

var music_track: FmodEvent
var loops: Dictionary


func play_loop(_path: String, _usesLoopParam: bool):
	if !FmodServer.check_event_path(_path):
		printerr("cannot find sfx path: " + _path)
		return

	var sfx: FmodEvent = FmodServer.create_event_instance(_path)
	if !loops.has(_path):
		loops[_path] = [];

	loops[_path].append([sfx, _usesLoopParam]);
	sfx.start()
	sfx.release()

func set_param(_path: String, _param: String, _val: float):
	if loops.has(_path):
		var arr = loops[_path]
		var sfx = arr[0]
		var event: FmodEvent = sfx[0]
		event.set_parameter_by_name(_param, _val)

func stop_loop(_path: String):
	if loops.has(_path):
		var arr = loops[_path]
		var sfx = arr.pop_at(0)
		var usesLoopParam: bool = sfx[1]
		var event: FmodEvent = sfx[0]
		if usesLoopParam:
			event.set_parameter_by_name("loop", 1)
		else:
			event.stop(1)

		if arr.size() <= 0:
			loops.erase(_path)

func stop_all_loops() -> void :
	for key in loops.keys():
		var arr = loops[key]
		for i in arr.size():
			stop_loop(key)

func play_one_shot(_path: String, _parameters: Dictionary, _volume: float = 1.0):
	if !FmodServer.check_event_path(_path):
		printerr("cannot find sfx path: " + _path)
		return


	var event_desc: FmodEventDescription = FmodServer.get_event(_path)
	var valid_params: Array[String] = []
	if event_desc:
		for param: FmodParameterDescription in event_desc.get_parameters():
			valid_params.append(param.get_name())

	var sfx: FmodEvent = FmodServer.create_event_instance(_path)


	for key in _parameters:
		if key in valid_params:
			sfx.set_parameter_by_name(key, _parameters[key])
		else:
			print("WARNING: FMOD parameter '%s' not found on event '%s'" % [key, _path])

	sfx.set_volume(_volume)
	sfx.start()
	sfx.release()

func play_music(_music: String):
	if !FmodServer.check_event_path(_music):
		printerr("cannot find music path: " + _music)
		return

	stop_music()
	music_track = FmodServer.create_event_instance(_music)
	music_track.start()

func stop_music():
	if music_track != null:
		music_track.stop(0)
		music_track.release()
		music_track = null

func update_music_parameter(label, labelIndex):
	if music_track == null:
		printerr("there is no music track")
		return

	music_track.set_parameter_by_name_with_label(label, labelIndex, false)

func set_master_volume(_val: float):
	if master_bus == null:
		master_bus = FmodServer.get_bus("bus:/master")
	if master_bus != null:
		master_bus.volume = _val

func set_sfx_volume(_val: float):
	if sfx_bus == null:
		sfx_bus = FmodServer.get_bus("bus:/master/sfx")
	if sfx_bus != null:
		sfx_bus.volume = _val

func set_ambience_volume(_val: float):
	if ambient_bus == null:
		ambient_bus = FmodServer.get_bus("bus:/master/ambience")
	if ambient_bus != null:
		ambient_bus.volume = _val

func set_bgm_volume(_val: float):
	if bgm_bus == null:
		bgm_bus = FmodServer.get_bus("bus:/master/music")
	if bgm_bus != null:
		bgm_bus.volume = _val
