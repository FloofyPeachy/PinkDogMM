using Godot;

namespace PinkDogMM_Gd.Scenes;

public partial class Gimbal : Node3D
{
	private Node3D yawPivot;
	private Node3D pitchPivot;
	private Node3D rollPivot;
	private MeshInstance3D cube;

	public override void _Ready()
	{
		// Yaw pivot (rotates around Y)
		yawPivot = new Node3D();
		AddChild(yawPivot);

		// Pitch pivot (rotates around X)
		pitchPivot = new Node3D();
		yawPivot.AddChild(pitchPivot);

		// Roll pivot (rotates around Z)
		rollPivot = new Node3D();
		pitchPivot.AddChild(rollPivot);

		// Example mesh to visualize
		cube = new MeshInstance3D
		{
			Mesh = new BoxMesh(),
			Scale = new Vector3(0.5f, 0.5f, 0.5f)
		};
		rollPivot.AddChild(cube);
	}

	public override void _Process(double delta)
	{
		// Example: rotate each pivot independently
		yawPivot.RotateY(Mathf.DegToRad(30f) * (float)delta);   // Yaw
		pitchPivot.RotateX(Mathf.DegToRad(45f) * (float)delta); // Pitch
		rollPivot.RotateZ(Mathf.DegToRad(60f) * (float)delta);  // Roll
	}
}
