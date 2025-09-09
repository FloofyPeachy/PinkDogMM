using Godot;

namespace PinkDogMM_Gd.UltraBinder;

public partial class PropertyBinded : Node3D
{
	public override void _Ready()
	{
		base._Ready();
		GD.Print("Ready!!");
	}
}
