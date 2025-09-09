extends Control

@onready var file_dialog = $FileDialog


func _on_file_id_pressed(id: int) -> void:
	file_dialog.use_native_dialog = true;
	file_dialog.file_mode = 0;
	file_dialog.popup()


func _on_file_dialog_file_selected(path: String) -> void:
	print(path)
	AppState.ExecuteLoadSaveAction(path)


func _on_file_dialog_confirmed() -> void:
	print("confimred"); # Replace with function body.
