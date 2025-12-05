using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using PinkDogMM_Gd._3D;
using PinkDogMM_Gd.Core;
using PinkDogMM_Gd.Core.Actions;
using PinkDogMM_Gd.Core.Schema;
using PinkDogMM_Gd.UI.Viewport;

namespace PinkDogMM_Gd.Scenes;

public partial class ViewportInteraction : Control
{
    private Camera3D camera;
    private PopupMenu popupMenu;
    private ActionRegistry actionRegistry;
    private Vector2 CurrentMousePos = Vector2.Zero;
    private Vector3 InitialWorldPos = Vector3.Zero;
    private Vector3 InitalVec3 = Vector3.Zero;
    private Vector2 CapturedMousePos = Vector2.Zero;
    private Vector2 DragStart = Vector2.Zero;
    private Model model;
    private Axis axis;

    public override void _Ready()

    {
        camera = GetNode<Camera3D>("../PivotXY2/PivotY/PivotX/Camera3D");
        model = Model.Get(this);
        actionRegistry = GetNode<ActionRegistry>("/root/ActionRegistry");
        popupMenu = GetNode<PopupMenu>("PopupMenu");
        /*model.State.ModeChanged += (sender, mode) =>
        {
            if (mode == EditorMode.Resize)
            {
                CapturedMousePos = GetViewport().GetMousePosition();
                Input.SetMouseMode(Input.MouseModeEnum.Captured);
                InitalVec3 = model.State.SelectedObjects.First().Size.AsVector3();
            }
            else
            {
                CapturedMousePos = Vector2.Zero;
                InitialWorldPos = Vector3.Zero;
                InitalVec3 = Vector3.Zero;
                model.State.ActiveAxis = Axis.All;
                Input.SetMouseMode(Input.MouseModeEnum.Visible);
            }
        };*/
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public override void _Draw()
    {
        if (DragStart != Vector2.Zero)
        {
            GD.Print("ds: " + DragStart);
            var mouse = GetViewport().GetMousePosition();
            var rectPos = new Vector2(Mathf.Min(DragStart.X, mouse.X), Mathf.Min(DragStart.Y, mouse.Y));
            var rectSize = (DragStart - mouse).Abs();
            DrawRect(new Rect2(rectPos, rectSize), Color.FromHtml("#a986f7"), false);
        }
    }

    public void _UnhandledInput(InputEvent @event)
    {
        //PL.I.Info("This event!!");
        //var partAtMouse = GetObjectAtMouse();

        switch (@event)
        {
            /*case InputEventMouseButton button:
            {
                switch (button.ButtonIndex)
                {
                    case MouseButton.Right:
                        break;
                    case MouseButton.Left:
                        if (model.State.Hovering == null && !button.Pressed)
                        {
                            actionRegistry.Execute("TheModel/SelectPart",
                                new Dictionary { { "model", model }, { "id", -1 } });
                        }


                        CapturedMousePos = button.Pressed ? button.Position : Vector2.Zero;


                        if (!button.Pressed)
                        {
                            var part = GetObjectAtMouse();
                            if (part != null && model.GetItemById(part.Value.Item2) != null)
                            {
                                actionRegistry.Execute("TheModel/SelectPart",
                                    new Dictionary { { "model", model }, { "id", part.Value.Item2} });
                            }
                        }
                        if (!button.Pressed)
                        {
                            if (model.State.Mode == EditorMode.ShapeEdit) return;
                            Input.SetDefaultCursorShape(Input.CursorShape.Arrow);
                            actionRegistry.Execute("Editor/SwitchMode",
                                new Dictionary { { "model", model }, { "mode", (int)EditorMode.Normal} });
                            DragStart = Vector2.Zero;
                            QueueRedraw();
                        }
                        else
                        {
                            if (button.DoubleClick)
                            {
                                var part = GetObjectAtMouse();
                                if (part != null && model.GetItemById(part.Value.Item2) != null )
                                {
                                    actionRegistry.Execute("TheModel/SelectPart",
                                        new Dictionary { { "model", model }, { "id", part.Value.Item2} });
                                    actionRegistry.Execute("Editor/SwitchMode",
                                        new Dictionary { { "model", model }, { "mode", (int)EditorMode.ShapeEdit} });
                                }
                               
                            }
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

                    }#1#
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
                    #1#


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
                            model.State.SelectedObjects.Add(objec);#2#
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
                        Input.MouseMode = Input.MouseModeEnum.Captured;#2#
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
                        Input.MouseMode = Input.MouseModeEnum.Visible;#2#
                    }#1#

                        break;
                }

                if (model.State.Mode == EditorMode.Move) HandleMoveMode(@event);
                if (model.State.Mode == EditorMode.Resize) HandleScaleMode(@event);
                if (model.State.Mode == EditorMode.Paint) HandlePaintMode(@event);
                break;
            }
            case InputEventMouseMotion motion:
                model.State.WorldMousePosition = WorldPosFromMouse3(motion.Position).LH();

                model.State.GridMousePosition = PlanePosFromMouse(motion.Position);
                if (CapturedMousePos != Vector2.Zero) GD.Print(CapturedMousePos - motion.Position);
                var delta = (CapturedMousePos - motion.Position).Abs();
                if (CapturedMousePos != Vector2.Zero && model.State.CurrentTool != null)
                {
                    if (model.State.SelectedObjects.Count != 0 && delta > new Vector2(5, 5))
                    {
                        //Start dragging.
                        GD.Print("now dragging part!");
                        Input.SetDefaultCursorShape(Input.CursorShape.Move);
                        //model.State.ChangeMode(EditorMode.Move);

                    actionRegistry.Start("Tools/MoveTool",
                        new Dictionary { { "model", model }});
                    }
                    else
                    {
                        GD.Print("now dragging selection!");
                        var topLeft = new Vector2(Mathf.Min(DragStart.X, motion.Position.X),
                            Mathf.Min(DragStart.Y, motion.Position.Y));
                        var bottomRight = new Vector2(Mathf.Max(DragStart.X, motion.Position.X),
                            Mathf.Max(DragStart.Y, motion.Position.Y));
                        var topRight = new Vector2(bottomRight.X, topLeft.Y);
                        var bottomLeft = new Vector2(topLeft.X, bottomRight.Y);
                        Vector2[] points = [topLeft, bottomRight, topRight, bottomLeft];
                        var cam = camera;
                        var camPos = cam.GlobalPosition;

                        List<Plane> planes = new();

// Project each point into rays
                        Vector3[] origins = new Vector3[points.Length];
                        Vector3[] directions = new Vector3[points.Length];

                        for (int i = 0; i < points.Length; i++)
                        {
                            origins[i] = cam.ProjectRayOrigin(points[i]);
                            directions[i] = cam.ProjectRayNormal(points[i]);
                        }

// Define the side planes in a loop
// Side planes connect adjacent points around the rectangle
                        for (int i = 0; i < 4; i++)
                        {
                            int next = (i + 1) % 4; // wrap around to form a loop
                            planes.Add(new Plane(camPos, camPos + directions[i], camPos + directions[next]));
                        }

// Optional: near and far planes
                        planes.Add(new Plane(origins[0], origins[1], origins[3])); // near
                        planes.Add(new Plane(origins[0] + directions[0] * cam.Far,
                            origins[1] + directions[1] * cam.Far,
                            origins[3] + directions[3] * cam.Far));

                        foreach (var modelAllPart in model.AllObjects)
                        {
                            var renderable = modelAllPart;
                            Vector3 half = renderable.Size.AsVector3() * 0.5f;
                            Vector3 min = -half;
                            Vector3 max = half;
                            Aabb localAabb = new Aabb(min, max - min);

// Apply world position (scale as needed)
                            Vector3 worldPos = new Vector3(renderable.Position.Z, -renderable.Position.Y, renderable.Position.X) * 0.0625f;
                            localAabb.Position += worldPos;

// Check against frustum planes
                            bool result = true;
                            foreach (var plane in planes)
                            {
                                if (!localAabb.IntersectsPlane(plane))
                                {
                                    result = false;
                                    break;
                                }
                            }

                            if (result)
                            {
                                GD.Print("intersect!!");
                            }
                          
                        }

                        if (DragStart == Vector2.Zero) DragStart = motion.Position;
                        QueueRedraw();
                    }
                }

                if (model.State.Mode == EditorMode.Move) HandleMoveMode(@event);
                if (model.State.Mode == EditorMode.Resize) HandleScaleMode(@event);
                if (model.State.Mode == EditorMode.Paint) HandlePaintMode(@event);
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
            if (model.State.Mode == EditorMode.Resize) HandleScaleMode(@event);#1#
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
                }#2#
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
                }#2#
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
                    -(float)Math.Round(result.Value.X)#2#
                /*
                actualPart.ShapeboxX[model.State.FocusedCorner] =
                    (float)Math.Round(delta.X);
                    #2#

                //PL.I.Info(CurrentMousePos);

                //PL.I.Info(result);
                //var newSize = (Captu.Round() + (result!.Value * 16).Round());

                /*actualPart.ShapeboxX[model.State.FocusedCorner] =
                    -(float)Math.Round(result.Value.X);#2#

                //PL.I.Info((float)Math.Round(result.Value.X * 8));
                /*PL.I.Info(AmountMoved);
                AmountMoved += CapturedMousePos;
                PL.I.Info(AmountMoved);#2#
                //actualPart.ShapeboxX[model.State.FocusedCorner] = (float)Math.Round(AmountMoved.X * 0.2f);



                lastProjection = result.Value;
            }#1#


                break;*/
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
                        case Key.P:
                            model.State.ChangeMode(EditorMode.Paint);
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
                        case Key.F5:
                            if (model.Textures.Count != 0)
                            {
                                model.State.ReloadModel(true);
                            }

                            break;

                        case Key.G:
                            actionRegistry.Execute("TheModel/GroupPart",
                                new Dictionary { { "model", model }, { "ungroup",false} });
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
                        case Key.F3:
                            List<Part> parts = [];
                            parts.AddRange(model.AllObjects.Select(modelAllPart => modelAllPart as Part));

                            Texturerer.DrawTexture(parts);
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

    private void HandlePaintMode(InputEvent @event)
    {
        switch (@event)
        {
            case InputEventMouseButton button:
            {
                var node = GetNodeAtMouse();
                if (node == null) break;
                
                var dataTool = new MeshDataTool();
                dataTool.CreateFromSurface((node.Value.Item2.GetParent() as MeshInstance3D)?.Mesh as ArrayMesh, 0);

                var uvs = MeshPickingHelpers.GetUvCoords(dataTool, node.Value.Item2, node.Value.Item3,
                    node.Value.Item1);
                GD.Print(uvs);
                model.State.CurrentTexture = model.Textures.Count - 1;
                if (model.State.CurrentTexture != -1)
                {
                    var img = model.Textures[model.State.CurrentTexture].Image!;
                    var realUv = uvs! * model.Textures[model.State.CurrentTexture]!.Size;
                    var image = img.GetImage();
                    
                    GD.Print(image.GetPixelv(new Vector2I((int)realUv.Value.X, (int)realUv.Value.Y)));
                    image.SetPixelv(new Vector2I((int)realUv.Value.X, (int)realUv.Value.Y), Colors.Orange);
                    img.Update(image);
                }
               
                break;
                
            }

            case InputEventMouseMotion motion:
            {
                var node = GetNodeAtMouse();
                if (node == null) break;
                
                var dataTool = new MeshDataTool();
                dataTool.CreateFromSurface((node.Value.Item2.GetParent() as MeshInstance3D)?.Mesh as ArrayMesh, 0);

                var uvs = MeshPickingHelpers.GetUvCoords(dataTool, node.Value.Item2, node.Value.Item3,
                    node.Value.Item1);
                GD.Print(uvs);
                model.State.CurrentTexture = model.Textures.Count - 1;
                if (model.State.CurrentTexture != -1)
                {
                    var img = model.Textures[model.State.CurrentTexture].Image!;
                    var realUv = uvs! * model.Textures[model.State.CurrentTexture]!.Size;
                    var image = img.GetImage();
                    if (realUv.HasValue)
                    {
                        GD.Print(image.GetPixelv(new Vector2I((int)realUv.Value.X, (int)realUv.Value.Y)));
                        image.SetPixelv(new Vector2I((int)realUv.Value.X, (int)realUv.Value.Y), Colors.Orange);
                    }
                    img.Update(image);
                
                   
                }

                break;
            }
        }
    }
    Vector3 GetBarycentric(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 v0 = b - a, v1 = c - a, v2 = p - a;
        float d00 = v0.Dot(v0);
        float d01 = v0.Dot(v1);
        float d11 = v1.Dot(v1);
        float d20 = v2.Dot(v0);
        float d21 = v2.Dot(v1);
        float denom = d00 * d11 - d01 * d01;
        float v = (d11 * d20 - d01 * d21) / denom;
        float w = (d00 * d21 - d01 * d20) / denom;
        float u = 1.0f - v - w;
        return new Vector3(u, v, w);
    }
    public void HandleScaleMode(InputEvent @event)
    {
        switch (@event)
        {
            case InputEventMouseButton button:
            {
                /*GD.Print(button.Pressed);
            Input.MouseMode = button.Pressed ? Input.MouseModeEnum.Captured : Input.MouseModeEnum.Visible;*/
                break;
            }

            case InputEventMouseMotion motion:
            {
                CapturedMousePos += motion.Relative;
                model.State.WorldMousePosition = WorldPosFromMouse3(CapturedMousePos) * 16;
                GD.Print("cm:" + CapturedMousePos);
                if (model.State.SelectedObjects.Count != 0)
                {
                    foreach (var part in model.State.SelectedObjects)
                    {
                        if (InitialWorldPos == Vector3.Zero) InitialWorldPos = model.State.WorldMousePosition;
                        Vector3 distance = InitialWorldPos - model.State.WorldMousePosition;
                        GD.Print("d:" + distance);
                        /*if (InitialWorldPos == model.State.WorldMousePosition && Math.Sign(distance.X) == 1)
                        {
                            distance.X = -1;
                        }
                        else
                        {
                            distance.X = 1;
                        }*/
                        if (distance == Vector3.Zero);
                        var v = (distance).Clamp(-128, 128).Round();

                        GD.Print("v:" + v);
                        if (model.State.ActiveAxis is Axis.X or Axis.All)
                        {
                            part.Size.X = Math.Abs(v.X);
                            part.Position.X = InitalVec3.X + v.X;
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
                var positions = new Godot.Collections.Dictionary();
                
                foreach (var renderable in model.State.SelectedObjects)
                {
                    var newPos = Vector3.Zero;
                    if (model.State.ActiveAxis is Axis.X or Axis.All)
                    {
                        newPos.X = pos.X;
                    }

                    if (model.State.ActiveAxis is Axis.Y or Axis.All)
                    {
                        newPos.Y = pos.Y;
                    }

                    if (model.State.ActiveAxis is Axis.Z or Axis.All)
                    {
                        newPos.Z = pos.Z;
                    }
                    
                    positions.Add(renderable.Id, newPos);
                }
                actionRegistry.Tick(positions);
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

    private Vector3 PlanePosFromMouse(Vector2 mousePos)
    {
        var result = new Dictionary();
        var spaceState = camera.GetWorld3D().DirectSpaceState;
        
        var origin = camera.ProjectRayOrigin(mousePos);
        var end = origin + camera.ProjectRayNormal(mousePos) * 1000.0f;

        var plane = new Plane();
        var positionY = model.State.Hovering?.Position.Y;
        
        plane.Y = positionY ?? -1.395f;
        var intersection = plane.IntersectsRay(origin, end);
        if (positionY != null && intersection != null) intersection = intersection.Value with { Y = -positionY.Value / 1/16 };
        
        return intersection ?? Vector3.Zero;
    }

    private Vector3 WorldPosFromPos(Vector2 point)
    {
        return camera.ProjectRayOrigin(point);
    }

    public void InputOnGrid(Node camera,
        Godot.InputEvent @event,
        Vector3 eventPosition,
        Vector3 normal,
        long shapeIdx)
    {
        GD.Print("on grid!!");
        model.State.GridMousePosition = eventPosition;
    }
    private bool GetHovering()
    {
        /*(Vector3, Node3D)? collider = GetNodeAtMouse();*/
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

    private (Vector3, Node3D, Vector3, int)? GetNodeAtMouse()
    {
        var result = new Dictionary();
        var spaceState = camera.GetWorld3D().DirectSpaceState;

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

        return (hitNormal.Round(), result["collider"].As<Node3D>(), result["position"].AsVector3(), result["face_index"].AsInt32());
    }


    private (int, Node3D)? GetCornerAtMouse()
    {
        var result = new Dictionary();
        var spaceState = camera.GetWorld3D().DirectSpaceState;
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


    private (Vector3, int, Vector3)? GetObjectAtMouse()
    {
        (Vector3, Node3D, Vector3, int)? nodeAtMouse = GetNodeAtMouse();
        if (nodeAtMouse == null) return null;
        if (!nodeAtMouse.Value.Item2.HasMeta("id")) return null;
        if (nodeAtMouse.Value.Item2.GetParent().GetParent() is PartNode pn)
        {
            pn.SetHovering(true);
        }
        return (nodeAtMouse.Value.Item1, nodeAtMouse.Value.Item2.GetMeta("id").AsInt32(), nodeAtMouse.Value.Item3);
    }

    /*private (Vector3, int, Node3D)? GetObjectAtMouse1()
{
    (Vector3, Node3D)? nodeAtMouse = GetNodeAtMouse();
    if (nodeAtMouse == null) return null;
    if (!nodeAtMouse.Value.Item2.HasMeta("id")) return null;
    return (nodeAtMouse.Value.Item1, nodeAtMouse.Value.Item2.GetMeta("id").AsInt32(), nodeAtMouse.Value.Item2);
}*/
}