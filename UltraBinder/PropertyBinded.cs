using Godot;
using PinkDogMM_Gd.Core;

namespace PinkDogMM_Gd.UltraBinder;

public partial class PropertyBinded : Node3D
{
	public override void _Ready()
	{
		base._Ready();
		PL.I.Info("Ready!!");
	}
}
