extends PanelContainer

var model : Model
func _ready() -> void:
	model = Model.Get(self)
	model.State.connect("EditorEventHappened", event_happened)

func event_happened(event: String):
		var tween = get_tree().create_tween()

		tween.tween_property(self, "modulate", Color.WHITE, 0.1).set_trans(Tween.TRANS_SINE)
		tween.tween_interval(3)
		tween.tween_property(self, "modulate", Color.TRANSPARENT, 0.1).set_trans(Tween.TRANS_SINE)
		$Label.text = event
		# Add your desired logic here
