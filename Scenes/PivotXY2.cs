using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using PinkDogMM_Gd.Core;
using PinkDogMM_Gd.Core.Actions.All.TheModel;
using PinkDogMM_Gd.Core.Schema;

namespace PinkDogMM_Gd.Scenes;

public partial class PivotXY2 : Node3D
{
	private Node3D pivotY;
	private Node3D pivotX;
	private Camera3D camera;
	private AppState? appState;
	private PopupMenu popupMenu;
	private const float Sensitivity = 0.5f;
	private const float ZoomSpeed = 0.1f;
	private Vector3 ActualPosition;
	private const float RayLength = 1000.0f;
	private const float DragThreshold = 2f;
	private Vector2 ClickPosition = Vector2.Zero;
	private bool RightButtDown = false;
	private bool menuUp;

	public override void _Ready()
	{
		pivotY = GetNode<Node3D>("PivotY");
		popupMenu = GetNode<PopupMenu>("PopupMenu");
		pivotX = GetNode<Node3D>("PivotY/PivotX");
		camera = GetNode<Camera3D>("PivotY/PivotX/Camera3D");
		appState = GetNode("/root/AppState") as AppState;
		appState.ActiveModelChanged += (index) =>
		{
			appState.ActiveEditorState.Camera.Position.PropertyChanged += (sender, args) => { SetTranslation(); };
		};
	}

	public override void _PhysicsProcess(double delta)
	{
		this.GlobalPosition = GlobalPosition.Lerp(ActualPosition,
			(float)delta * 16.0f);
	}

	public void SetTranslation()
	{
		ActualPosition = appState.ActiveModel.State.Camera.Position.AsVector3() * 0.0625f;
	}

	public override void _Input(InputEvent @event)
	{

		if (@event is InputEventMouseButton button)
		{
			switch (button.ButtonIndex)
			{
				case MouseButton.WheelUp:
					camera.Translate(new Vector3(0, 0, -ZoomSpeed));
					GetViewport().SetInputAsHandled();
					break;
				case MouseButton.WheelDown:
					camera.Translate(new Vector3(0, 0, ZoomSpeed));
					GetViewport().SetInputAsHandled();
					break;
				default:
					break;
			}

			


		}
		if (@event is not InputEventMouseMotion motion) return;
		
		if (!Input.IsMouseButtonPressed(MouseButton.Right)) return;
		if (motion.Position.DistanceTo(ClickPosition) < DragThreshold) return;
		pivotY.Rotate(pivotY.Transform.Basis.Y, -motion.Relative.X * Mathf.DegToRad(Sensitivity));

// Vertical (around local X)
		pivotX.Rotate(pivotX.Transform.Basis.X, -motion.Relative.Y * Mathf.DegToRad(Sensitivity));
		GetViewport().SetInputAsHandled();
	}

	
}
