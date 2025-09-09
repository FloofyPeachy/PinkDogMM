using Godot;
using System;

public partial class MainArea : HSplitContainer
{
	public override void _Ready()
	{
		base._Ready();
		
		GetWindow().FilesDropped += files =>
		{
			GD.Print("File!!");
		};
	}
	
}
