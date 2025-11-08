extends HBoxContainer

var tc: PanelContainer
func _ready() -> void:
	tc = get_node("TabContainer")

func _on_tab_bar_tab_changed(tab: int) -> void:
	for i in tc.get_child_count():
			tc.get_child(i).visible = i == tab
