extends MenuBar

@onready var file = $File
func _ready():
	file.add_item("New")
	file.add_separator()
	file.add_item("Open")
	file.add_item("Open Recent")
	file.add_submenu_item("test", "Open Recent")
