using Godot;

namespace PinkDogMM_Gd.Scenes.Controls;

[Tool]
public partial class EditorCamera : Camera3D
{
	[Export] public Node3D Target; // the object to orbit around
	[Export] public float Distance = 100f;
	[Export] public float ZoomSpeed = 1f;
	[Export] public float RotateSpeed = 0.01f;
	[Export] public float MinDistance = 1.0f;
	[Export] public float MaxDistance = 20.0f;

	private float _yaw = 0f;
	private float _pitch = 0f;
	private Camera3D _camera;

	public override void _Ready()
	{
		_camera = this;
		UpdateCameraPosition();
		var model = GetNode<ModelNode>("../TheModel");
		GD.Print("g");
		Target = model;
		Fov = 30.0f; //
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		// Rotate with right mouse button drag
		if (@event is InputEventMouseMotion motion && Input.IsMouseButtonPressed(MouseButton.Right))
		{
			_yaw -= motion.Relative.X * 0.01f; 
			_pitch -= motion.Relative.Y * 0.01f;

			// Clamp pitch so it stays flatter
			_pitch = Mathf.Clamp(_pitch, -0.2f, 0.5f); 
			UpdateCameraPosition();
		}

		// Zoom with scroll wheel
		if (@event is InputEventMouseButton mouseButton)
		{
			if (mouseButton.ButtonIndex == MouseButton.WheelUp && mouseButton.Pressed)
				Distance = Mathf.Max(MinDistance, Distance - ZoomSpeed);

			if (mouseButton.ButtonIndex == MouseButton.WheelDown && mouseButton.Pressed)
				Distance = Mathf.Min(MaxDistance, Distance + ZoomSpeed);

			UpdateCameraPosition();
		}
	}

	private void UpdateCameraPosition()
	{
		if (Target == null || _camera == null) return;
		_camera.Position = new Vector3(0, 0, 0);
		_camera.LookAt(GlobalPosition, Vector3.Up);
	}
}
