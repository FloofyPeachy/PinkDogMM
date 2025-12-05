using Godot;
using System;
using System.Text;
using PinkDogMM_Gd.Core;

public partial class DebugText : Label
{
	private AppState state;
	public override void _Ready()
	{
		base._Ready();
		state = GetNode("/root/AppState") as AppState;
	}

	public override void _Process(double d)
	{
		var builder = new StringBuilder().Append("Pink Dog Minecraft Modeller - Alpha\n")
			.Append(Engine.GetFramesPerSecond())
			.Append("fps\n\n");
		if (state.ActiveModel != null)
		{
			builder.Append("Camera: " + state.ActiveModel.State.Camera.ToString());
			builder.Append("\nMouse Pos: " + state.ActiveModel.State.WorldMousePosition + "");
			builder.Append("\nPos on Grid: " + state.ActiveModel.State.GridMousePosition + "\n");
			builder.Append("\nModel: " + state.ActiveModel.Name);
			builder.Append("\nFocused corner: " + state.ActiveModel.State.FocusedCorner);
			builder.Append("\nMode: " + state.ActiveModel.State.Mode);
			builder.Append("\nTool: " + state.ActiveModel.State.CurrentTool);
			builder.Append("\nSelected: " +  String.Join(", ", state.ActiveModel.State.SelectedObjects));
			builder.Append("\nHovering: " + state.ActiveModel.State.Hovering);
			builder.Append("\nHovered side: " + state.ActiveModel.State.HoveredSide);
			builder.Append("\nHovered axis: " + state.ActiveModel.State.HoveredAxis);
			builder.Append("Texture: " + state.ActiveModel.State.CurrentTexture);
			if (state.ActiveEditorState.Mode == EditorMode.ShapeEdit)
			{
				builder.Append("\nPress ENTER to exit Shape Edit mode.");
			}
		}
		else
		{
			builder.Append("No model loaded");
		}

		Text = builder.ToString();


	}
}
