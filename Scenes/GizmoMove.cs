using Godot;
using System;
using PinkDogMM_Gd.Core;
using PinkDogMM_Gd.Core.Schema;

public partial class GizmoMove : Node3D
{
	private MeshInstance3D xGizmo;
	private MeshInstance3D yGizmo;
	private MeshInstance3D zGizmo;
	private AppState appState;
	private Model model;
	public override void _Ready()
	{
		xGizmo = GetNode<MeshInstance3D>("X");
		yGizmo = GetNode<MeshInstance3D>("Y");
		zGizmo = GetNode<MeshInstance3D>("Z");
		appState = GetNode<AppState>("/root/AppState");
		model = Model.Get(this);
		
		xGizmo.CreateConvexCollision();
		yGizmo.CreateConvexCollision();
		zGizmo.CreateConvexCollision();
		Scale = new Vector3(0.0625f, 0.0625f, 0.0625f);
		
		model.State.ObjectSelected += (sender, renderable) =>
		{
			this.Position = renderable.Position.AsVector3() * new Vector3(0.0625f, -0.0625f, 0.0625f);
		};

		xGizmo.GetChild<StaticBody3D>(0).InputEvent += (camera, @event, position, normal, idx) =>
		{
			PL.I.Info("x hover!!");
		};
	}
	

}
