using Godot;
using System;
using System.Linq;
using Godot.Collections;
using PinkDogMM_Gd.Core;
using PinkDogMM_Gd.Core.Actions;
using PinkDogMM_Gd.Core.Actions.All.TheModel;
using PinkDogMM_Gd.Core.Schema;
using PinkDogMM_Gd.Render;
using PinkDogMM_Gd.Scenes;

public partial class ViewportInteraction : Node3D
{
    private Camera3D camera;
    private PopupMenu popupMenu;
    private ActionRegistry actionRegistry;
    private Vector2 CurrentMousePos = Vector2.Zero;
    private Vector3 InitialWorldPos = Vector3.Zero;
    private Vector3 InitalVec3 = Vector3.Zero;
    private Vector2 CapturedMousePos = Vector2.Zero;
    private Model model;
    private Axis axis;
    public override void _Ready()
    
    {
        camera = GetNode<Camera3D>("../PivotXY2/PivotY/PivotX/Camera3D");
        model = Model.Get(this);
        actionRegistry = GetNode<ActionRegistry>("/root/ActionRegistry");
        popupMenu = GetNode<PopupMenu>("PopupMenu");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        var mousePos = GetViewport().GetMousePosition();
       
        DebugDraw2D.SetText("deez");
        var origin = camera.ProjectRayOrigin(mousePos);
        var end = origin + camera.ProjectRayNormal(mousePos) * 1000.0f;
        DebugDraw3D.DrawArrow(camera.Transform.Origin, end, Colors.Yellow);
    }

    public override void _Input(InputEvent @event)
    {
        //PL.I.Info("This event!!");
        //var partAtMouse = GetObjectAtMouse();
        
        switch (@event)
        {
            case InputEventMouseButton button:
            {
                switch (button.ButtonIndex)
                {
                    case MouseButton.Right:
                        break;
                    case MouseButton.Left:
                        if (model.State.Hovering == null && !button.Pressed)
                        {
                            actionRegistry.Execute("TheModel/SelectPart",
                                new Dictionary { { "model", model }, { "id", -1} });
                        }

                    
                            CapturedMousePos = button.Pressed ? button.Position : Vector2.Zero;
                        

                        if (!button.Pressed)
                        {
                            Input.SetDefaultCursorShape(Input.CursorShape.Arrow);
                            model.State.ChangeMode(EditorMode.Normal);
                        }
                        
                        
                    
                        /*
                        var part = GetObjectAtMouse();
                        if (part != null && model.GetItemById(part.Value.Item2) != null)
                        {
                            var p = model.GetItemById(part.Value.Item2);
                            if (model.State.SelectedObjects.Contains(p.Value) && button.Pressed)
                            {
                                model.State.ChangeMode(EditorMode.Move);
                                model.State.SetMoving(true);
                                model.State.ActiveAxis = Axis.All;
                            }
                            else
                            {
                                model.State.ChangeMode(EditorMode.Normal);
                                model.State.SetMoving(false);
                                Input.MouseMode = Input.MouseModeEnum.Visible;
                            }
                        }
                        else
                        {
                            model.State.ChangeMode(EditorMode.Normal);
                            model.State.SetMoving(false);
                            Input.MouseMode = Input.MouseModeEnum.Visible;
                        }
                        
                        var node = GetNodeAtMouse();
                        var onAxis = false;
                        if (node != null && node.Value.Item2.HasMeta("axis") && button.Pressed)
                        {
                            axis = (Axis)node.Value.Item2.GetMeta("axis").AsInt32();
                            model.State.ActiveAxis = axis;
                            model.State.ChangeMode(EditorMode.Resize);
                            onAxis = true;
                            CapturedMousePos = button.Position;
                            InitialWorldPos = WorldPosFromMouse3(button.Position);
                            InitalVec3 = model.State.SelectedObjects.First().Size.AsVector3();
                            Input.MouseMode = Input.MouseModeEnum.Captured;
                        }
                        else
                        {
                            if (model.State.Mode == EditorMode.Resize)
                            {
                                model.State.ActiveAxis = Axis.All;
                                model.State.ChangeMode(EditorMode.Normal);
                                model.State.SetMoving(false);
                                onAxis = false;
                                CapturedMousePos = Vector2.Zero;
                                InitialWorldPos = Vector3.Zero;
                                InitalVec3 = Vector3.Zero;
                                Input.MouseMode = Input.MouseModeEnum.Visible;
                            };
                         
                        }
                        
                        if (button.Pressed)
                        {

                            if (model.State.Mode != EditorMode.ShapeEdit && !onAxis)
                            {
                                    actionRegistry.Execute("TheModel/SelectPart",
                                        new Dictionary { { "model", model }, { "id", part?.Item2 ?? -1 } });
                            }
                            if (button.DoubleClick)
                            {
                                model.State.ChangeMode(EditorMode.ShapeEdit);
                                return;
                            }
                            
                        }*/
                        /*if (button.Pressed && part != null)
                        {
                            //Might be dragging? Start it.
                            DraggingPart = part.Value.Item2;
                            var actualPart = appState?.ActiveModel?.GetPartById(DraggingPart)?.Item2;
                            if (actualPart == null) return;
                            CapturedMousePos = button.Position;
                            CurrentMousePos = button.Position;
                            Input.MouseMode = Input.MouseModeEnum.Captured;
                            DragDirection = model.State.HoveredSide;
                            PL.I.Info(DragDirection);
                            InitalSize = actualPart.Size.AsVector3();
                            InitalPos = actualPart.Position.AsVector3();
                        }
                        else
                        {
                            CapturedMousePos = Vector2.Zero;
                            Input.MouseMode = Input.MouseModeEnum.Visible;
                            AmountMoved = Vector3.Zero;
                            CurrentMousePos = Vector2.Zero;
                        }
                        */


                        /*if (!button.Pressed)
                        {
                            if (button.IsDoubleClick())
                            {
                                if (partAtMouse == null) return;
                                if (model.State.Mode == EditorMode.Normal)
                                {
                                    (int, Node3D)? cornerAtMouse = GetCornerAtMouse();
                                    PL.I.Info(cornerAtMouse);
                                    model.State.ChangeMode(EditorMode.ShapeEdit);
                                    return;
                                }
                            }


                            if (model.State.Mode != EditorMode.ShapeEdit)
                            {
                                var nodeAtMouse = GetNodeAtMouse();

                                model?.State.History.Execute(partAtMouse == null
                                    ? new SelectPartAction(null, appState)
                                    : new SelectPartAction(partAtMouse.Value.Item2, appState));

                                //model.State.Mode = EditorMode.Move;


                                /*
                                if (nodeAtMouse == null) return;
                                var objec = model.Helpers[nodeAtMouse.Value.Item2.GetMeta("ido").AsInt32()];
                                model.State.SelectedObjects.Add(objec);#1#
                            }
                            else
                            {
                                (int, Node3D)? cornerAtMouse = GetCornerAtMouse();
                                if (cornerAtMouse != null)
                                {
                                    model.State.FocusedCorner = cornerAtMouse.Value.Item1;
                                    CapturedMousePos = button.Position;
                                    CurrentMousePos = button.Position;
                                    Input.MouseMode = Input.MouseModeEnum.Captured;
                                    //InitalSize = model.GetPartById()
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
                            Input.MouseMode = Input.MouseModeEnum.Captured;#1#
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
                            Input.MouseMode = Input.MouseModeEnum.Visible;#1#
                        }*/

                        break;
                }
                if (model.State.Mode == EditorMode.Move) HandleMoveMode(@event);
                if (model.State.Mode == EditorMode.Resize) HandleScaleMode(@event);
                break;
            }
            case InputEventMouseMotion motion:
                model.State.WorldMousePosition =  WorldPosFromMouse3(motion.Position) * 16;
                
                if (CapturedMousePos != Vector2.Zero) GD.Print(CapturedMousePos - motion.Position);
                if (CapturedMousePos != Vector2.Zero && (CapturedMousePos - motion.Position).Abs() > new Vector2(5,5) && model.State.Mode == EditorMode.Normal)
                {
                    //Start dragging.
                    GD.Print("now dragging!");
                    Input.SetDefaultCursorShape(Input.CursorShape.Move);
                    model.State.ChangeMode(EditorMode.Move);
                    var objectAtMouse = GetObjectAtMouse();

                    if (objectAtMouse != null)
                        actionRegistry.Execute("TheModel/SelectPart",
                            new Dictionary { { "model", model }, { "id", objectAtMouse.Value.Item2 } });
                }

                if (model.State.Mode == EditorMode.Move) HandleMoveMode(@event);
                if (model.State.Mode == EditorMode.Resize) HandleScaleMode(@event);
                /*var objectAtMouse = GetObjectAtMouse();

                if (objectAtMouse != null)
                {
                    GD.Print(model.GetItemById(objectAtMouse.Value.Item2)?.Value);
                }
                else
                {
                    GD.Print("null");
                }
                //model.State.Hovering = model.GetObjectById(objectAtMouse.Value.Item2).Value.Item2;
                model.State.WorldMousePosition =
                    WorldPosFromMouse3(GetViewport().GetMousePosition()) * 16;
                if (!Input.IsMouseButtonPressed(MouseButton.Left)) return;

                if (model.State.Mode == EditorMode.Move) HandleMoveMode(@event);
                if (model.State.Mode == EditorMode.Resize) HandleScaleMode(@event);*/
                /*if (model.State.Mode != EditorMode.ShapeEdit)
                {
                    if (partAtMouse == null) return;
                    var part = appState?.ActiveModel?.GetPartById(GetPartAtMouse().Value.Item2)?.Item2;
                    if (part == null) return;
                    CurrentMousePos += motion.Relative;

                    if (!WorldPosFromMouse2(part, out var result,DragDirection)) return;

                    if (result == null) return;
                    var scaledResult = (result!.Value * 16).Round();
                    PL.I.Info(scaledResult);

                    var final = InitalSize - scaledResult;
                    if (DragDirection == Vector3.Back) // dragging Z− side
                    {
                        var outcome = part.Size.Z + scaledResult.Z;
                        PL.I.Info(outcome);
                        if (Math.Sign(outcome) == -1)
                        {
                            part.Size.Z+=  scaledResult.Z;
                            part.Position.Z = outcome - 1;
                        }
                        else
                        {
                            part.Size.Z = outcome;
                        }

                    }

                    /*if (model.State.HoveredSide == Vector3.Forward ||
                        model.State.HoveredSide == Vector3.Back)
                    {
                        actualPart.Size.Z = newSize.Z;
                    }#1#
                    /*if (Math.Abs(DragDirection.X) == 1f)
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
                    }#1#
                    lastProjection = result.Value;
                }
                else
                {
                    //We are shape editing!!
                    if (CapturedMousePos == Vector2.Zero) return;
                    if (appState?.ActiveEditorState.SelectedParts.First() is not Shapebox actualPart) return;
                    CurrentMousePos += motion.Relative;
                    //PL.I.Info(motion.Relative.Y);
                    if (!WorldPosFromMouse(actualPart, out var result, Vector3.Up)) return;


                    actualPart.ShapeboxX[model.State.FocusedCorner] =
                        -(float)Math.Round(result.Value.X);
                    actualPart.ShapeboxZ[model.State.FocusedCorner] =
                        (float)Math.Round(result.Value.Z);
                    /*actualPart.ShapeboxX[model.State.FocusedCorner] =
                        -(float)Math.Round(result.Value.X)#1#
                    /*
                    actualPart.ShapeboxX[model.State.FocusedCorner] =
                        (float)Math.Round(delta.X);
                        #1#

                    //PL.I.Info(CurrentMousePos);

                    //PL.I.Info(result);
                    //var newSize = (Captu.Round() + (result!.Value * 16).Round());

                    /*actualPart.ShapeboxX[model.State.FocusedCorner] =
                        -(float)Math.Round(result.Value.X);#1#

                    //PL.I.Info((float)Math.Round(result.Value.X * 8));
                    /*PL.I.Info(AmountMoved);
                    AmountMoved += CapturedMousePos;
                    PL.I.Info(AmountMoved);#1#
                    //actualPart.ShapeboxX[model.State.FocusedCorner] = (float)Math.Round(AmountMoved.X * 0.2f);



                    lastProjection = result.Value;
                }*/


                break;
            case InputEventKey key:
                if (key is { Echo: false })
                    
                    switch (key.Keycode)
                    {
                        case Key.C:

                            if (key.Pressed)
                            {
                                if (Input.IsKeyPressed(Key.Ctrl))
                                {
                                    model.State.SwitchCameraProjection();
                                }
                                else
                                {
                                    model.State.SwitchCameraMode();
                                }
                            }
                            break;

                        case Key.X:
                            model.State.ActiveAxis = Axis.X;
                        break;
                        case Key.Y:
                            model.State.ActiveAxis = Axis.Y;
                            break;
                        case Key.Z:
                            model.State.ActiveAxis = Axis.Z;
                            break;
                        
                        case Key.G:
                            model.State.ChangeMode(EditorMode.Move);
                            break;
                        case Key.Capslock:
                            model.State.SetPeek(key.Pressed);
                            break;
                        case Key.Insert:
                            /*if (key.Pressed)
                                appState!.ActiveEditorState.History.Execute(new AddPartAtMouseAction(appState));
                            break;*/
                        case Key.Enter:
                        {
                            if (model.State.Mode == EditorMode.ShapeEdit)
                            {
                                model.State.ChangeMode(EditorMode.Normal);
                                model.State.UnselectAll();
                            }

                            break;
                        }
                        /*case Key.D:
                            if (model.State.Mode != EditorMode.ShapeEdit) break;
                            model.State.FocusedCorner =
                                (model.State.FocusedCorner + 1 + 8) % 8;
                            break;
                        case Key.A:
                        {
                            if (model.State.Mode != EditorMode.ShapeEdit) break;
                            model.State.FocusedCorner =
                                (model.State.FocusedCorner - 1 + 8) % 8;
                            break;
                        }*/
                        case Key.F1:
                        {
                            
                            
                            if (key.Pressed) GetNode<ModelNode>("../ModelNode").RefreshAll();
                            break;
                        }
                        case Key.F2: 
                            //if (key.Pressed) GetNode<ModelNode>("../ModelNode").RebuildAll();
                            GetViewport()
                                .GetTexture().GetImage().SavePng("deeznuts2.png");
                            model.State.ShowEventText("Screenshot taken!");
                            break;
                        
                            

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
                            Part partObj = model?.GetPartById(partAtMouse.Value)!;

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
                    model?.State.History.Execute(new SelectPartAction(partAtMouse, appState));
                }
            }
        }
        */
    }

    public void HandleScaleMode(InputEvent @event)
    {
        switch (@event)
        {
            case InputEventMouseButton button:
            {
                GD.Print(button.Pressed);
                Input.MouseMode = button.Pressed ? Input.MouseModeEnum.Captured : Input.MouseModeEnum.Visible;
                break;
            }
            
            case InputEventMouseMotion motion:
            {
               
                CapturedMousePos += motion.Relative;
               
                if (model.State.SelectedObjects.Count != 0)
                {
                    foreach (var part in model.State.SelectedObjects)
                    {
                        if (InitialWorldPos == Vector3.Zero) InitialWorldPos = model.State.WorldMousePosition;
                        Vector3 distance = InitialWorldPos - model.State.WorldMousePosition;
                        
                        var v = (InitalVec3 + distance).Clamp(-128, 128).Round();
                        
                    
                        if (model.State.ActiveAxis is Axis.X or Axis.All)
                        {
                            part.Size.X = Math.Abs(v.X);
                            part.Position.X = InitalVec3.X + Math.Min(v.X, 0);
                        }
                        if (model.State.ActiveAxis is Axis.Y or Axis.All)
                        {
                            part.Size.Y = Math.Abs(v.Y);
                            part.Position.Y = InitalVec3.Y + Math.Min(v.Y, 0);
                        }

                        if (model.State.ActiveAxis is not (Axis.Z or Axis.All)) continue;
                        part.Size.Z = Math.Abs(v.Z);
                        part.Position.Z = InitalVec3.Z + Math.Min(v.Z, 0);
                    }
                   
                }
                break;
            }
        }
    }
    public void HandleMoveMode(InputEvent @event)
    {

        switch (@event)
        {
            case InputEventMouseButton button:
            {
                if (!button.Pressed)
                {
                    //Finish!
                    //TODO: HistoryStack model.State.History.Execute(new PartPropertyAction());
                    //model.State.ChangeMode(EditorMode.Normal);
                }
               
                break;
            }
            
            case InputEventMouseMotion motion:
            {
                var pos = model.State.WorldMousePosition.Round().LH();
                foreach (var renderable in model.State.SelectedObjects)
                {
                    if (model.State.ActiveAxis is Axis.X or Axis.All)
                    {
                        renderable.Position.X = pos.X;
                    }
                    if (model.State.ActiveAxis is Axis.Y or Axis.All)
                    {
                        renderable.Position.Y = pos.Y;
                    }
                    if (model.State.ActiveAxis is Axis.Z or Axis.All)
                    {
                        renderable.Position.Z = pos.Z;
                    }
                }
               
                break;
            }
        }
    }
    void AdjustAlongAxis(ref float pos, ref float size, float newCoord, bool draggingMin)
    {
        float oldMin = pos;
        float oldMax = pos + size;

        if (draggingMin)
        {
            // Right side stays fixed
            pos = newCoord;
            size = oldMax - newCoord;
        }
        else
        {
            // Left side stays fixed
            size = newCoord - oldMin;
        }

        // Prevent negative size
        if (size < 0)
        {
            size = 0;
        }
    }

    private Vector3 WorldPosFromMouse3(Vector2 mousePos)
    {
        
        return camera.ProjectPosition(mousePos, camera.Position.Z);
    }
    
    private bool GetHovering()
    {
        (Vector3, Node3D)? collider = GetNodeAtMouse();
        /*if (collider != null)
        {
        //    PL.I.Info(collider.Value.Item1);
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

                PL.I.Info("Triangle");
                for
                /*var normal = meshDataTool.GetFaceNormal(faceIndex);
                var angle = Mathf.RadToDeg(new Vector3(100, 100, 100).AngleTo(normal));

                if (angle > 90 && angle < 180)
                {
                    side = faceIndex;
                    PL.I.Info(side);
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
        DebugDraw3D.DrawRay(origin, Vector3.Forward, 1000f, Colors.Yellow);
        var query = PhysicsRayQueryParameters3D.Create(origin, end);
        query.CollideWithAreas = true;
        query.CollideWithBodies = true;
        result = spaceState.IntersectRay(query);
        if (result.Count == 0) return null;

        Vector3 hitNormal = result["normal"].AsVector3();
        
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


    private (Vector3, int)? GetObjectAtMouse()
    {
        (Vector3, Node3D)? nodeAtMouse = GetNodeAtMouse();
        if (nodeAtMouse == null) return null;
        if (!nodeAtMouse.Value.Item2.HasMeta("id")) return null;
        return (nodeAtMouse.Value.Item1, nodeAtMouse.Value.Item2.GetMeta("id").AsInt32());
    }
    private (Vector3, int, Node3D)? GetObjectAtMouse1()
    {
        (Vector3, Node3D)? nodeAtMouse = GetNodeAtMouse();
        if (nodeAtMouse == null) return null;
        if (!nodeAtMouse.Value.Item2.HasMeta("id")) return null;
        return (nodeAtMouse.Value.Item1, nodeAtMouse.Value.Item2.GetMeta("id").AsInt32(), nodeAtMouse.Value.Item2);
    }
}