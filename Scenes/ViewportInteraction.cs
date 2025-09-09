using Godot;
using System;
using System.Linq;
using Godot.Collections;
using PinkDogMM_Gd.Core;
using PinkDogMM_Gd.Core.Actions.All.TheModel;
using PinkDogMM_Gd.Core.Schema;

public partial class ViewportInteraction : Node3D
{
	private Camera3D camera;
	private AppState? appState;
	private PopupMenu popupMenu;
	Vector3 DragDirection = new Vector3();
	private int DraggingPart = 0;
	private Vector3 AmountMoved = Vector3.Zero;
	private Vector3 lastProjection = Vector3.Zero;
	private Vector2 CapturedMousePos = Vector2.Zero;
	private Vector2 CurrentMousePos = Vector2.Zero;
	private Vector3 InitalSize = Vector3.Zero;
	public override void _Ready()
	{
		camera = GetNode<Camera3D>("../PivotXY2/PivotY/PivotX/Camera3D");
		appState = GetNode<AppState>("/root/AppState");
		popupMenu = GetNode<PopupMenu>("PopupMenu");
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		//GD.Print("This event!!");
		var partAtMouse = GetPartAtMouse();


		switch (@event)
		{
			case InputEventMouseButton button:
			{


				switch (button.ButtonIndex)
				{
					case MouseButton.Right:
						break;
					case MouseButton.Left:

						
						if (button.Pressed)
						{
							if (button.IsDoubleClick())
							{
								if (partAtMouse == null) return;
								if (appState.ActiveEditorState.Mode == EditorMode.Normal)
								{
									(int, Node3D)? cornerAtMouse = GetCornerAtMouse();
									GD.Print(cornerAtMouse);
									appState.ActiveEditorState.ToggleShapeEditMode();
									return;
								}
							}
							
							
							

							if (appState.ActiveEditorState.Mode != EditorMode.ShapeEdit)
							{
								appState.ActiveModel?.State.History.Execute(partAtMouse == null
									? new SelectPartAction(null, appState)
									: new SelectPartAction(partAtMouse.Value.Item2, appState));
							}
							else
							{
								(int, Node3D)? cornerAtMouse = GetCornerAtMouse();
								if (cornerAtMouse != null)
								{
									appState.ActiveEditorState.FocusedCorner = cornerAtMouse.Value.Item1;
									CapturedMousePos = button.Position;
									CurrentMousePos = button.Position;
									Input.MouseMode = Input.MouseModeEnum.Captured;
									//InitalSize = appState.ActiveModel.GetPartById()
									return;
								}
							}
						
							
							if (partAtMouse == null) return;
							
							
							/*
							DragDirection = partAtMouse.Value.Item1;
							DraggingPart = partAtMouse.Value.Item2;
							var actualPart2 = appState?.ActiveModel?.GetPartById(DraggingPart);
							InitalSize = actualPart2.Size.AsVector3();
							CapturedMousePos = button.Position;
							Input.MouseMode = Input.MouseModeEnum.Captured;*/


						
						}
						else
						{
							CapturedMousePos = Vector2.Zero;
							Input.MouseMode = Input.MouseModeEnum.Visible;
							AmountMoved = Vector3.Zero;
							CurrentMousePos = Vector2.Zero;
							/*AmountMoved = Vector3.Zero;
							lastProjection = Vector3.Zero;
							InitalSize = Vector3.Zero;
							CapturedMousePos = Vector2.Zero;
							Input.MouseMode = Input.MouseModeEnum.Visible;*/
						}

						break;
				}

				break;
			}
			case InputEventMouseMotion motion:
				
				if (!Input.IsMouseButtonPressed(MouseButton.Left)) return;

				if (appState.ActiveEditorState.Mode != EditorMode.ShapeEdit)
				{
					var actualPart = appState?.ActiveModel?.GetPartById(DraggingPart)?.Item2;
					if (actualPart == null) return;
					CapturedMousePos += motion.Relative;
					
					if (WorldPosFromMouse(actualPart, out var result)) return;
				
					var newSize = (InitalSize.Round() + (result!.Value * 16).Round());

					if (Math.Abs(DragDirection.X) == 1f)
					{
						//actualPart.Position = new Vector3L((result.Value * 16));
						actualPart.Size.X = newSize.X;
					}

					if (Math.Abs(DragDirection.Y) == 1f)
					{
						actualPart.Size.Y = newSize.Y;
					}

					if (Math.Abs(DragDirection.Z) == 1f)
					{
						actualPart.Size.Z = -newSize.Z;
					}
					lastProjection = result.Value;
				}
				else
				{
					//We are shape editing!!
					if (CapturedMousePos == Vector2.Zero) return;
					if (appState?.ActiveEditorState.SelectedParts.First() is not Shapebox actualPart) return;
					CurrentMousePos += motion.Relative;
					//GD.Print(motion.Relative.Y);
					if (!WorldPosFromMouse(actualPart, out var result)) return;
				
					
					actualPart.ShapeboxX[appState.ActiveEditorState.FocusedCorner] =
						-(float)Math.Round(result.Value.X);
					actualPart.ShapeboxZ[appState.ActiveEditorState.FocusedCorner] =
						(float)Math.Round(result.Value.Z);
					/*actualPart.ShapeboxX[appState.ActiveEditorState.FocusedCorner] =
						-(float)Math.Round(result.Value.X)*/
					/*
					actualPart.ShapeboxX[appState.ActiveEditorState.FocusedCorner] =
						(float)Math.Round(delta.X);
						*/
							
					//GD.Print(CurrentMousePos);
				
					//GD.Print(result);
					//var newSize = (Captu.Round() + (result!.Value * 16).Round());
					
					/*actualPart.ShapeboxX[appState.ActiveEditorState.FocusedCorner] =
						-(float)Math.Round(result.Value.X);*/
						
					//GD.Print((float)Math.Round(result.Value.X * 8));
					/*GD.Print(AmountMoved);
					AmountMoved += CapturedMousePos;
					GD.Print(AmountMoved);*/
					//actualPart.ShapeboxX[appState.ActiveEditorState.FocusedCorner] = (float)Math.Round(AmountMoved.X * 0.2f);
					
					
					 
					lastProjection = result.Value;
				}
				
				
				break;
			case InputEventKey key:
				if (key is { Pressed: true, Echo: false })
				switch (key.Keycode)
				{
					case Key.Shift:
						appState.ActiveEditorState.TogglePeek();
						break;
					case Key.Enter:
					{
						if (appState.ActiveEditorState.Mode == EditorMode.ShapeEdit)
						{
							appState.ActiveEditorState.ToggleShapeEditMode();
							appState.ActiveEditorState.UnselectAllParts();
						}

						break;
					}
					case Key.D:
						if (appState.ActiveEditorState.Mode != EditorMode.ShapeEdit) break;
						appState.ActiveEditorState.FocusedCorner = 
							(appState.ActiveEditorState.FocusedCorner + 1 + 8) % 8;
						break;
					case Key.A:
					{
						if (appState.ActiveEditorState.Mode != EditorMode.ShapeEdit) break;
						appState.ActiveEditorState.FocusedCorner = 
							(appState.ActiveEditorState.FocusedCorner - 1 + 8) % 8;
						break;
					}
						
					default:
						break;
				}

				break;
		
	}

		/*if (@event is InputEventMouseButton button)
		{
			if (button.ButtonIndex == MouseButton.WheelUp) camera.Translate(new Vector3(0, 0, -ZoomSpeed));
			if (button.ButtonIndex == MouseButton.WheelDown) camera.Translate(new Vector3(0, 0, ZoomSpeed));
			var partAtMouse = GetPartAtMouse();

			if (button.ButtonIndex == MouseButton.Right)
			{
				if (button.Pressed)
				{
					ClickPosition = button.Position;
					RightButtDown = true;
				}
				else
				{
					if (RightButtDown && button.Position.DistanceTo(ClickPosition) < DragThreshold)
					{
						popupMenu.Clear();
						if (partAtMouse != null)
						{
							Part partObj = appState.ActiveModel?.GetPartById(partAtMouse.Value)!;

							popupMenu.AddItem(partObj.Name);
							popupMenu.AddSeparator();
							popupMenu.AddItem("Duplicate");
							popupMenu.AddItem("Delete");
						}
						else
						{
						}

						popupMenu.Popup(new Rect2I((Vector2I)button.Position, Vector2I.Zero));
					}

					RightButtDown = false;
				}
			}
			else
			{
				if (button.ButtonIndex == MouseButton.Left && button.Pressed)
				{
					appState.ActiveModel?.State.History.Execute(new SelectPartAction(partAtMouse, appState));
				}
			}
		}
		*/
	}

	private bool WorldPosFromMouse(Part actualPart, out Vector3? result)
	{
		var rayOrigin = camera.ProjectRayOrigin(CurrentMousePos);
		var rayDir = camera.ProjectRayNormal(CurrentMousePos);
		var to = rayOrigin + rayDir * 1000;

		var plane = new Plane(Vector3.Up, -(actualPart.Position.Y * 0.0625f));
		result = plane.IntersectsRay(rayOrigin, to);
		//GD.Print(result);
		return result != null;
	}

	private bool GetHovering()
	{
		(Vector3, Node3D)? collider = GetNodeAtMouse();
		/*if (collider != null)
		{
		//    GD.Print(collider.Value.Item1);
			if (collider == null) return true;
			var mesh = (MeshInstance3D)collider.Value.Item3.GetParent();
			var verts = mesh.Mesh.GetFaces();
			System.Collections.Generic.Dictionary<int, List<Vector3>> triangleToSide = new();

			var enumerator = verts.Chunk(6).GetEnumerator();
			int leIndex = 0;
			/*
		Vector3[] leSide = enumerator.Current;
		while (enumerator.MoveNext())
		{
			foreach (var vector3 in leSide) triangleToSide[leIndex].Add(mesh.ToGlobal(vector3));

			leIndex++;

		}
		#1#


			// Build ArrayMesh
			var arrayMesh = new ArrayMesh();
			var arrays = new Godot.Collections.Array();
			arrays.Resize((int)Mesh.ArrayType.Max);
			arrays[(int)Mesh.ArrayType.Vertex] = verts;
			arrayMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);

			var meshDataTool = new MeshDataTool();
			meshDataTool.CreateFromSurface(arrayMesh, 0);
			int side = -1;

			for (var i = 0; i < 5; i++)
			{
				var a = meshDataTool.GetVertex(meshDataTool.GetFaceVertex(i, 0));
				var b = meshDataTool.GetVertex(meshDataTool.GetFaceVertex(i, 1));
				var c = meshDataTool.GetVertex(meshDataTool.GetFaceVertex(i, 2));

				var direction =
					(collider.Value.Item2 -
					 GetNode<Camera3D>("../PivotXY2/PivotY/PivotX/Camera3D").GlobalTransform.Origin).Normalized();
				var intersectsTriangle = Geometry3D.RayIntersectsTriangle(
					GetNode<Camera3D>("../PivotXY2/PivotY/PivotX/Camera3D").GlobalTransform.Origin,
					direction,
					mesh.ToGlobal(a),
					mesh.ToGlobal(b),
					mesh.ToGlobal(c)
				);

				if (intersectsTriangle.Obj != null)
				{
					var normal = meshDataTool.GetFaceNormal(i);
					if (normal.Dot(Vector3.Right) > 0.9) side = i;
					if (normal.Dot(Vector3.Left) > 0.9) side = i;
					if (normal.Dot(Vector3.Up) > 0.9) side = i;
					if (normal.Dot(Vector3.Down) > 0.9) side = i;
					if (normal.Dot(Vector3.Forward) > 0.9) side = i;
					if (normal.Dot(Vector3.Back) > 0.9) side = i;



				}
			}
		}*/

		/*int side = 0;
		for (var i = 0; i < verts.Length; i += 3)
		{
			var faceIndex = i / 3;
			var a = mesh.ToGlobal(verts[i]);
			var b = mesh.ToGlobal(verts[i + 1]);
			var c = mesh.ToGlobal(verts[i + 2]);
			Vector3 direction =
				(collider.Value.Item2 - GetNode<Camera3D>("../PivotXY2/PivotY/PivotX/Camera3D").GlobalTransform.Origin)
				.Normalized();

			var intersectsTriangle = Geometry3D.RayIntersectsTriangle(
				GetNode<Camera3D>("../PivotXY2/PivotY/PivotX/Camera3D").GlobalTransform.Origin,
				direction,
				a,
				b,
				c
			);

			if (intersectsTriangle.Obj != null)
			{

				GD.Print("Triangle");
				for
				/*var normal = meshDataTool.GetFaceNormal(faceIndex);
				var angle = Mathf.RadToDeg(new Vector3(100, 100, 100).AngleTo(normal));

				if (angle > 90 && angle < 180)
				{
					side = faceIndex;
					GD.Print(side);
				}#1#
			}
		}*/

		return false;
	}

	private (Vector3, Node3D)? GetNodeAtMouse()
	{
		var result = new Dictionary();
		var spaceState = GetWorld3D().DirectSpaceState;
		var mousePos = GetViewport().GetMousePosition();

		var origin = camera.ProjectRayOrigin(mousePos);
		var end = origin + camera.ProjectRayNormal(mousePos) * 1000.0f;
		var query = PhysicsRayQueryParameters3D.Create(origin, end);
		query.CollideWithAreas = true;
		query.CollideWithBodies = true;
		result = spaceState.IntersectRay(query);
		if (result.Count == 0) return null;

		Vector3 hitNormal = result["normal"].AsVector3();


		Vector3[] axes =
		{
			Vector3.Right,
			Vector3.Left,
			Vector3.Up,
			Vector3.Down,
			Vector3.Forward,
			Vector3.Back
		};

		appState.ActiveEditorState.HoveredSide = hitNormal.Round();

		return (hitNormal.Round(), result["collider"].As<Node3D>());
	}

	private (int, Node3D)? GetCornerAtMouse()
	{
		var result = new Dictionary();
		var spaceState = GetWorld3D().DirectSpaceState;
		var mousePos = GetViewport().GetMousePosition();

		var origin = camera.ProjectRayOrigin(mousePos);
		var end = origin + camera.ProjectRayNormal(mousePos) * 1000.0f;
		var query = PhysicsRayQueryParameters3D.Create(origin, end);
		query.CollideWithAreas = true;
		query.CollideWithBodies = true;
		result = spaceState.IntersectRay(query);
		if (result.Count == 0) return null;

	
		Node3D collider = result["collider"].As<Node3D>();
		if (collider.HasMeta("corner"))
		{
			return (collider.GetMeta("corner").AsInt32(), result["collider"].As<Node3D>());
		}
		

		return null;
	}


	private (Vector3, int)? GetPartAtMouse()
	{
		(Vector3, Node3D)? nodeAtMouse = GetNodeAtMouse();
		if (nodeAtMouse == null) return null;
		if (!nodeAtMouse.Value.Item2.HasMeta("id")) return null;
		return (nodeAtMouse.Value.Item1, nodeAtMouse.Value.Item2.GetMeta("id").AsInt32());
	}
}
