using Godot;
using System;
using PinkDogMM_Gd.Core;

public partial class SideTabs : TabContainer
{

	public override void _Ready()
	{
		SetTabIcon(0,  GD.Load<Texture2D>("res://Assets/placeholders/icon_tools.png"));
		SetTabTitle(0, "");
		SetTabIcon(1,  GD.Load<Texture2D>("res://Assets/placeholders/icon_mod_tb.png"));
		SetTabTitle(1, "");

		GetNode<AppState>("/root/AppState").ModeChanged += (sender, mode) =>
		{
			if (mode == EditorMode.ShapeEdit)
			{
				SetCurrentTab(1);
			}
		};
	}
}
