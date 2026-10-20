@tool
class_name DevToolsPlugin extends EditorPlugin

const CACHE_SCRIPT_PATH: String = "res://addons/dev_tools/clear_godot_cache.py"

enum MenuItem{
	CLEAR_NUGET_CACHE = 0, 
	CLEAR_IMPORT_CACHE = 1, 
	CLEAR_ALL_CACHES = 2, 
}

var _menu_button: MenuButton
var _result_dialog: AcceptDialog
var _confirm_dialog: ConfirmationDialog
var _pending_include_mono: bool = false
var _python_command: String = ""
var _clear_args: PackedStringArray = PackedStringArray()
var _clear_pending: bool = false


func _enter_tree() -> void :
	_menu_button = MenuButton.new()
	_menu_button.text = "Dev Tools"
	_menu_button.tooltip_text = "Developer tools and utilities"

	var popup: PopupMenu = _menu_button.get_popup()
	popup.add_item("Clear NuGet Cache", MenuItem.CLEAR_NUGET_CACHE)
	popup.add_separator()
	popup.add_item("Clear Import Cache and Restart", MenuItem.CLEAR_IMPORT_CACHE)
	popup.add_item("Clear All Caches and Restart", MenuItem.CLEAR_ALL_CACHES)
	popup.id_pressed.connect(_on_menu_item_pressed)

	add_control_to_container(EditorPlugin.CONTAINER_TOOLBAR, _menu_button)

	_result_dialog = AcceptDialog.new()
	_result_dialog.title = "Dev Tools"
	add_child(_result_dialog)

	_confirm_dialog = ConfirmationDialog.new()
	_confirm_dialog.title = "Clear Godot Cache"
	_confirm_dialog.confirmed.connect(_on_clear_confirmed)
	add_child(_confirm_dialog)


func _exit_tree() -> void :


	if _clear_pending:

		if OS.create_process(_python_command, _clear_args, OS.get_name() == "Windows") == -1:
			push_error("Clear Godot Cache: cannot start %s. The cache is not deleted and the editor will not start again." % _python_command)

	remove_control_from_container(EditorPlugin.CONTAINER_TOOLBAR, _menu_button)
	_menu_button.queue_free()

	_result_dialog.queue_free()
	_confirm_dialog.queue_free()


func _on_menu_item_pressed(id: int) -> void :
	match id:
		MenuItem.CLEAR_NUGET_CACHE:
			_clear_nuget_cache()
		MenuItem.CLEAR_IMPORT_CACHE:
			_clear_godot_cache(false)
		MenuItem.CLEAR_ALL_CACHES:
			_clear_godot_cache(true)


func _show_result(text: String) -> void :
	_result_dialog.dialog_text = text
	_result_dialog.popup_centered()


func _get_dotnet_path() -> String:
	var output: Array = []
	var find_cmd: String = "where" if OS.get_name() == "Windows" else "which"
	var exit_code: int = OS.execute(find_cmd, ["dotnet"], output)
	if exit_code == 0:

		for line in str(output[0]).split("\n"):
			var path: String = line.strip_edges()
			if not path.is_empty() and FileAccess.file_exists(path):
				return path

	return ""


func _get_python_path() -> String:
	var find_cmd: String = "where" if OS.get_name() == "Windows" else "which"

	for name in ["python3", "python", "py"]:
		var found: Array = []
		if OS.execute(find_cmd, [name], found) != 0:
			continue

		for line in str(found[0]).split("\n"):
			var path: String = line.strip_edges()
			if path.is_empty() or not FileAccess.file_exists(path):
				continue



			var version: Array = []
			if OS.execute(path, ["--version"], version) != 0:
				continue
			if str(version[0]).strip_edges().begins_with("Python 3"):
				return path

	return ""


func _clear_nuget_cache() -> void :
	var dotnet_path: String = _get_dotnet_path()

	if dotnet_path.is_empty():
		_show_result("Cannot find the dotnet CLI.\n\nMake sure that dotnet is in PATH.")
		return

	var output: Array = []

	var temp_dir: String = OS.get_cache_dir()
	var exit_code: int
	if OS.get_name() == "Windows":
		var cmd: String = "cd /d \"%s\" && \"%s\" nuget locals all --clear" % [temp_dir, dotnet_path]
		exit_code = OS.execute("cmd", ["/c", cmd], output, true)
	else:
		var cmd: String = "cd '%s' && '%s' nuget locals all --clear" % [temp_dir, dotnet_path]
		exit_code = OS.execute("bash", ["-c", cmd], output, true)

	var result_text: String = ""
	for line in output:
		result_text += str(line)

	if exit_code == 0:
		_show_result("The NuGet cache is deleted.\n\n" + result_text)
	else:
		_show_result("The operation failed (exit code: %d).\n\nPath: %s\n\n%s" % [exit_code, dotnet_path, result_text])


func _clear_godot_cache(include_mono: bool) -> void :
	_python_command = _get_python_path()
	if _python_command.is_empty():
		_show_result("Cannot find Python 3.\n\nInstall Python 3 and make sure that it is in PATH.")
		return

	_pending_include_mono = include_mono
	if include_mono:
		_confirm_dialog.dialog_text = (
			"Delete all of the .godot directory and start the editor again.\n\n"
			+ "The editor imports all assets again and builds the C# code again. This takes time.\n"
			+ "The open tabs and the dock layout are also reset."
		)
	else:
		_confirm_dialog.dialog_text = (
			"Delete the import, filesystem and UID caches, and start the editor again.\n\n"
			+ "The editor imports all assets again. The C# build output, the open tabs and the dock layout stay."
		)
	_confirm_dialog.dialog_text += "\n\nThe editor must close. Unsaved work or a running game stops the operation."
	_confirm_dialog.popup_centered()


func _on_clear_confirmed() -> void :
	var editor_node: Node = EditorInterface.get_base_control().get_parent()
	if editor_node == null or editor_node.get_class() != "EditorNode":
		_show_result("Cannot find the editor node to close the editor. No files were deleted.")
		return


	_clear_args = [
		ProjectSettings.globalize_path(CACHE_SCRIPT_PATH), 
		"--pid", str(OS.get_process_id()), 
		"--project", ProjectSettings.globalize_path("res://"), 
		"--editor", OS.get_executable_path(), 
	]

	var root: Node = EditorInterface.get_edited_scene_root()
	if root != null and not root.scene_file_path.is_empty():
		_clear_args.append("--scene")
		_clear_args.append(root.scene_file_path)

	if _pending_include_mono:
		_clear_args.append("--include-mono")

	_clear_pending = true






	editor_node.notification.call_deferred(NOTIFICATION_WM_CLOSE_REQUEST)



	_abort_clear.call_deferred()


func _abort_clear() -> void :
	_clear_pending = false
	_clear_args = PackedStringArray()
	push_warning("Clear Godot Cache: the editor did not close. No files were deleted.")
