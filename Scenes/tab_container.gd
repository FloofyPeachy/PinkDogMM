extends TabContainer
@onready var icon_texture_1 = preload("res://Assets/placeholders/icon_tools.png")
@onready var icon_texture_2 = preload("res://Assets/placeholders/icon_mod_tb.png")

func _ready():
	# Set icon for the first tab (index 0)
	set_tab_icon(0, icon_texture_1)
	set_tab_title(0, "")
	# Set icon for the second tab (index 1)
	set_tab_icon(1, icon_texture_2)
	set_tab_title(1, "")
	
	var state = get_node("/root/AppState") as AppState
	state.mode_changed.connect()
	
func _on_mode_changed(mode):
	print(mode)
