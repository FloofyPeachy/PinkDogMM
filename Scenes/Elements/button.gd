extends Button


func _on_button_pressed() -> void:
	
	var state = get_node("/root/ActionRegistry") as ActionRegistry
	state.Execute("TheModel/AddPart", ["Default"] as Array)
	


func _on_pressed() -> void:
	var state = get_node("/root/ActionRegistry") as ActionRegistry
	state.Execute("TheModel/AddPart", ["Default"] as Array)
