extends Control

var following = false
var dragging_start_position = Vector2()

	
func _on_TitleBar_gui_input(event):
	if event is InputEventMouseButton:
			if event.get_button_index() == 1:
					if event.pressed:
						following = !following
						dragging_start_position = get_local_mouse_position()
						DisplayServer.window_start_drag()
					
						 
					else:
						following = !following
			elif event.get_button_index() == 2:
					following = false;
					print("fgd")
					DisplayServer.window_show_titlebar_menu();
					



func _process(_delta):
	if following:
		var mouse_position = get_global_mouse_position()
		var window_position = Vector2(DisplayServer.window_get_position())
	
		DisplayServer.window_set_position(window_position + (mouse_position - dragging_start_position))

func _on_CloseButton_pressed():
	get_tree().quit()

func _on_minimize_button_pressed():
	DisplayServer.window_set_mode(DisplayServer.WINDOW_MODE_MINIMIZED)


func _on_button_2_pressed() -> void:
	DisplayServer.beep(); # Replace with function body.


func _on_test_helpframe_pressed() -> void:
	pass # Replace with function body.
