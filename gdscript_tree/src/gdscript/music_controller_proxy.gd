
class_name MusicControllerProxy extends Node

var _musicEv: FmodEvent
var _currentTrack

var _ambienceEv: FmodEvent
var _currentAmbience

var _bank_loader: FmodBankLoader


var _loaded_bank_path: String = ""

func update_music(track):
	_currentTrack = track
	stop_music()

	if !FmodServer.check_event_path(_currentTrack):
		printerr("cannot find music path: " + _currentTrack)
		return

	_musicEv = FmodServer.create_event_instance(_currentTrack)
	_musicEv.start()

func update_music_parameter(label, labelIndex):
	if _musicEv == null:
		printerr("missing music track: " + _currentTrack)
		return

	_musicEv.set_parameter_by_name(label, labelIndex)

func update_global_parameter(label, labelIndex):
	FmodServer.set_global_parameter_by_name(label, labelIndex)

func stop_music():
	if _musicEv != null:
		_musicEv.stop(0)
		_musicEv.release()
		_musicEv = null

func update_ambience(track):
	_currentAmbience = track
	stop_ambience()

	if !FmodServer.check_event_path(_currentAmbience):
		printerr("cannot find ambience path: " + _currentAmbience)
		return

	_ambienceEv = FmodServer.create_event_instance(_currentAmbience)
	_ambienceEv.start()

func update_campfire_ambience(trackIndex):
	if !FmodServer.check_event_path(_currentAmbience):
		printerr("cannot find ambience path: " + _currentAmbience)
		return

	_ambienceEv.set_parameter_by_name("Campfire", trackIndex)

func stop_ambience():
	if _ambienceEv != null:
		_ambienceEv.stop(0)
		_ambienceEv.release()
		_ambienceEv = null






func load_act_bank(bank_path: String, verify_event: String) -> bool:
	if _loaded_bank_path == bank_path and FmodServer.check_event_path(verify_event):
		return true
	unload_act_banks()
	if not FileAccess.file_exists(bank_path):
		printerr("music bank not found: " + bank_path)
		return false
	_bank_loader = FmodBankLoader.new()
	_bank_loader.bank_paths = [bank_path]
	add_child(_bank_loader)
	if not FmodServer.check_event_path(verify_event):
		printerr("music bank failed to load: " + bank_path)
		unload_act_banks()
		return false
	_loaded_bank_path = bank_path
	return true

func unload_act_banks():
	if _bank_loader:
		remove_child(_bank_loader)








		_bank_loader.free()
		_bank_loader = null
	_loaded_bank_path = ""
