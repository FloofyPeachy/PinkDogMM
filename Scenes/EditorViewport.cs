using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;
using PinkDogMM_Gd.Core;
using PinkDogMM_Gd.Core.Actions.All.TheModel;
using PinkDogMM_Gd.Core.Schema;

public partial class EditorViewport : Node3D
{
	private const float RayLength = 1000.0f;
	private AppState appState;
	private bool menuUp;
	
	public override void _Ready()
	{
		appState = GetNode("/root/AppState") as AppState;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (appState.ActiveModelIndex == -1) return;
		if (GetWorld3D().DirectSpaceState == null) return;
		var partAtMouse = GetPartAtMouse();
		
		if (@event is InputEventMouseMotion)
		{
			if (Input.IsMouseButtonPressed(MouseButton.Left))
			{
				appState.ActiveModel?.State.History.Execute(new SelectPartAction(partAtMouse, appState));
			}
			if (partAtMouse != null)
			{
				if (GetHovering()) return;
			}
			else
			{
				appState.ActiveModel?.State.Hovering.Clear();
			}
		}
		if (@event is InputEventMouseButton button)
		{
			if (!button.Pressed) return;
			switch (button.ButtonIndex)
			{
				case MouseButton.Left:
					//Select part!!
					appState.ActiveModel?.State.History.Execute(new SelectPartAction(partAtMouse, appState));

					break;
				case MouseButton.Right:
					var menu = new PopupMenu();

					if (partAtMouse != null)
					{
						Part partObj = appState.ActiveModel?.GetPartById(partAtMouse.Value).Value.Item2!;

						menu.AddItem(partObj.Name);
					}
					else
					{

					}
					AddChild(menu);
					menu.Popup();
					break;
			}
		}
		/*if (@event is not InputEventMouseButton button) return;
		if (!button.Pressed) return;
		switch (button.ButtonIndex)
		{
			case MouseButton.Left:
				//Select part!!
				appState.ActiveModel?.State.History.Execute(new SelectPartAction(partAtMouse, appState));

				break;
			case MouseButton.Right:
				var menu = new PopupMenu();

				if (partAtMouse != null)
				{
					Part partObj = appState.ActiveModel?.GetPartById(partAtMouse.Value)!;

					menu.AddItem(partObj.Name);
				}
				else
				{

				}
				AddChild(menu);
				menu.Popup();
				break;
		}*/


		/*
		if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed &&
			eventMouseButton.ButtonIndex == MouseButton.Left)
		{
			appState.ActiveModel.State.History.Execute(new SelectPartAction(partAtMouse, appState));
		}

		if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed &&
			eventMouseButton.ButtonIndex == MouseButton.Left)
		{

		}*/
		/*if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed &&
			eventMouseButton.ButtonIndex == MouseButton.Left)
		{
			var spaceState = GetWorld3D().DirectSpaceState;
			var cam = GetNode<Camera3D>("../PivotXY/Camera3D");
			var mousePos = GetViewport().GetMousePosition();

			var origin = cam.ProjectRayOrigin(mousePos);
			var end = origin + cam.ProjectRayNormal(mousePos) * RayLength;
			var query = PhysicsRayQueryParameters3D.Create(origin, end);
			query.CollideWithAreas = true;
			query.CollideWithBodies = true;

			var result = spaceState.IntersectRay(query);
			if (result.Count == 0) return;
			GD.Print(result);
			GD.Print("Clicked on: " + result["collider"]);
		}*/
	}

	private bool GetHovering()
	{
		(Vector2, Vector3, Node3D)? collider = GetNodeAtMouse();
		if (collider == null) return true;
		var mesh = (MeshInstance3D)collider.Value.Item3.GetParent();
		var verts = mesh.Mesh.GetFaces();
				
		// Build ArrayMesh
		var arrayMesh = new ArrayMesh();
		var arrays = new Godot.Collections.Array();
		arrays.Resize((int)Mesh.ArrayType.Max);
		arrays[(int)Mesh.ArrayType.Vertex] = verts;
		arrayMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
				
		var meshDataTool = new MeshDataTool();
		meshDataTool.CreateFromSurface(arrayMesh, 0);
		int side = 0;
		for (var i = 0; i < verts.Length; i+=3)
		{
			var faceIndex = i / 3;
			var a = mesh.ToGlobal(verts[i]);
			var b = mesh.ToGlobal(verts[i + 1]);
			var c = mesh.ToGlobal(verts[i + 2]);
			Vector3 direction = (collider.Value.Item2 - GetNode<Camera3D>("../PivotXY2/PivotY/PivotX/Camera3D").GlobalTransform.Origin).Normalized();

			var intersectsTriangle = Geometry3D.RayIntersectsTriangle(
				GetNode<Camera3D>("../PivotXY2/PivotY/PivotX/Camera3D").GlobalTransform.Origin,
				direction,
				a,
				b,
				c
			);

			if (intersectsTriangle.Obj != null)
			{
				var normal = meshDataTool.GetFaceNormal(faceIndex);
				var angle = Mathf.RadToDeg(new Vector3(100,100,100).AngleTo(normal));

				if (angle > 90 && angle < 180)
				{
					side = faceIndex;
					//GD.Print(side);
				}
			}

		}

		return false;
	}

	private (Vector2, Vector3, Node3D)? GetNodeAtMouse()
	{
		var result = new Dictionary();
		var spaceState = GetWorld3D().DirectSpaceState;
		var cam = GetNode<Camera3D>("../PivotXY2/PivotY/PivotX/Camera3D");
		var mousePos = GetViewport().GetMousePosition();

		var origin = cam.ProjectRayOrigin(mousePos);
		var end = origin + cam.ProjectRayNormal(mousePos) * RayLength;
		var query = PhysicsRayQueryParameters3D.Create(origin, end);
		query.CollideWithAreas = true;
		query.CollideWithBodies = true;
		result = spaceState.IntersectRay(query);
		if (result.Count == 0) return null;
		return (result["normal"].As<Vector2>(), result["position"].AsVector3(), result["collider"].As<Node3D>());
	}


	private int? GetPartAtMouse() => GetNodeAtMouse()?.Item3.GetMeta("id").AsInt32();
}
