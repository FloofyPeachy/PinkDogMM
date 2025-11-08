extends Button


func _on_button_pressed() -> void:
	
	var state = get_node("/root/ActionRegistry") as ActionRegistry
	state.Execute("TheModel/AddPart")
	


func _on_pressed() -> void:
	var state = get_node("/root/ActionRegistry") as ActionRegistry
	var model = Model.Get(self)
	state.Execute("TheModel/AddPart", {"model" : model})
	model.State.ShowEventText("Added part!")


func _on_test_helpframe_pressed() -> void:
	var state = get_node("/root/ActionRegistry") as ActionRegistry
	state.Execute("TheModel/AddHelpframe", ["/home/peachy/Downloads/siemens_charger_venture_amtrak_cascades.jpg"])
	
