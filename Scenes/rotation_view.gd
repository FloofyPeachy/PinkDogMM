extends SubViewportContainer
@onready var context_menu = $PopupMenu

func _ready():
	# Connect the id_pressed signal
	context_menu.id_pressed.connect(_on_context_menu_id_pressed)
	# Ensure the Control node can receive mouse input
	mouse_filter = MOUSE_FILTER_STOP

func _gui_input(event):
	if event is InputEventMouseButton:
		if event.button_index == MOUSE_BUTTON_RIGHT and event.is_pressed():
			context_menu.popup(Rect2(event.global_position.x, event.global_position.y, 0,0))


func _on_context_menu_id_pressed(id):
	# Handle the actions for each menu item ID
	match id:
		0:
			var state = get_node("/root/ActionRegistry") as ActionRegistry
			state.Execute("TheModel/UI/SetCameraMode", {"model" : Model.Get(self), "projection" : 0})
			# Add your custom logic here
		1:
			print("Option 2 pressed!")
			var state = get_node("/root/ActionRegistry") as ActionRegistry
			state.Execute("TheModel/UI/SetCameraMode", {"model" : Model.Get(self), "projection" : 1})
			# Add your custom logic here
		# ... more cases
	
