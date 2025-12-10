using Godot;
using System;
using PinkDogMM_Gd.Core;
using PinkDogMM_Gd.Core.Schema;

public partial class GizmoMove : Node3D
{
    private MeshInstance3D xGizmo;
    private MeshInstance3D yGizmo;
    private MeshInstance3D zGizmo;

    private StaticBody3D xBody;
    private StaticBody3D yBody;
    private StaticBody3D zBody;
    private AppState appState;
    private Model model;

    private Color xColor = Color.Color8(58, 179, 218, 1);
    private Color yColor = Color.Color8(128, 199, 47, 1);
    private Color zColor = Color.Color8(176, 45, 36, 1);

    
    public override void _Ready()
    {
        xGizmo = new MeshInstance3D();
        yGizmo = new MeshInstance3D();
        zGizmo = new MeshInstance3D();

        AddChild(xGizmo);
        AddChild(yGizmo);
        AddChild(zGizmo);
        xGizmo.Mesh = GetMesh(EditorMode.Move, Axis.X);
        yGizmo.Mesh = GetMesh(EditorMode.Move, Axis.Y);
        zGizmo.Mesh = GetMesh(EditorMode.Move, Axis.Z);
        
        appState = GetNode<AppState>("/root/AppState");
        model = Model.Get(this);
        
        xGizmo.CreateConvexCollision();
        yGizmo.CreateConvexCollision();
        zGizmo.CreateConvexCollision();

        xBody = xGizmo.GetChild<StaticBody3D>(0);
        yBody = yGizmo.GetChild<StaticBody3D>(0);
        zBody = zGizmo.GetChild<StaticBody3D>(0);
        

        Scale = new Vector3(0.0625f, 0.0625f, 0.0625f);
        model.State.ObjectSelectionChanged += (sender, renderable) =>
        {
            if (renderable.Item2)
            {
                
                this.Position = ((renderable.Item1.Position.AsVector3().LHS() +
                                  (renderable.Item1.Size.AsVector3().LHS() / 2)));
                
                //this.Visible = true;
            }
            else
            {
              //  this.Visible = false;
            }
        };

   

        xBody.MouseEntered += () => { MouseInOutAxis(Axis.X, true); };
        xBody.MouseExited += () => { MouseInOutAxis(Axis.X, false); };
        xBody.InputEvent += (camera, @event, position, normal, idx) => { MouseEventOnAxis(Axis.X, @event); };

        yBody.MouseEntered += () => { MouseInOutAxis(Axis.Y, true); };
        yBody.MouseExited += () => { MouseInOutAxis(Axis.Y, false); };
        yBody.InputEvent += (camera, @event, position, normal, idx) => { MouseEventOnAxis(Axis.Y, @event); };

        zBody.MouseEntered += () => { MouseInOutAxis(Axis.Z, true); };
        zBody.MouseExited += () => { MouseInOutAxis(Axis.Z, false); };
        zBody.InputEvent += (camera, @event, position, normal, idx) => { MouseEventOnAxis(Axis.Z, @event); };
    }

    private BoxMesh GetMesh(EditorMode mode, Axis axis)
    {
        return new BoxMesh()
        {
            Size = new Vector3(axis == Axis.Z ? 1.0f : 0.032f, axis == Axis.Y ? 1.0f : 0.032f,
                axis == Axis.X ? 1.0f : 0.032f),
            Material = new StandardMaterial3D()
            {
                AlbedoColor = axis switch
                {
                    Axis.X => xColor,
                    Axis.Y => yColor,
                    _ => zColor
                }
            }
        };
    }

    private void MouseEventOnAxis(Axis axis, InputEvent evento)
    {
        if (evento is not InputEventMouseButton mouse) return;
        if (mouse.ButtonIndex == MouseButton.Left)
        {
            model.State.ChangeMode(mouse.Pressed ? EditorMode.Resize : EditorMode.Normal);
            model.State.ActiveAxis = mouse.Pressed ? axis : Axis.All;
        }
    }

    private void MouseInOutAxis(Axis axis, bool entered)
    {
        model.State.HoveredAxis = entered ? axis : Axis.All;
    
    }

    public override void _PhysicsProcess(double delta)
    {
        this.Visible = false;
        //this.Position = appState.ActiveEditorState.WorldMousePosition;
        base._PhysicsProcess(delta);
        //this.Visible = model.State.Mode != EditorMode.ShapeEdit;
        this.Scale = this.Scale.Lerp((model.State.SelectedObjects.Count != 0 && model.State.Mode != EditorMode.ShapeEdit
            ? new Vector3(model.State.Camera.Zoom,
                model.State.Camera.Zoom, model.State.Camera.Zoom)
            : new Vector3(0.01f, 0.01f, 0.01f)) / 1.5f, (float)delta * 24.0f);
        ((StandardMaterial3D)xGizmo.Mesh.SurfaceGetMaterial(0)).AlbedoColor =
            ((StandardMaterial3D)xGizmo.Mesh.SurfaceGetMaterial(0)).AlbedoColor.Lerp(
                model.State.HoveredAxis == Axis.X ? xColor.Lightened(0.3f) : xColor, (float)delta * 24.0f);

        ((StandardMaterial3D)yGizmo.Mesh.SurfaceGetMaterial(0)).AlbedoColor =
            ((StandardMaterial3D)yGizmo.Mesh.SurfaceGetMaterial(0)).AlbedoColor.Lerp(
                model.State.HoveredAxis == Axis.Y ? yColor.Lightened(0.3f) : yColor, (float)delta * 24.0f);

        ((StandardMaterial3D)zGizmo.Mesh.SurfaceGetMaterial(0)).AlbedoColor =
            ((StandardMaterial3D)zGizmo.Mesh.SurfaceGetMaterial(0)).AlbedoColor.Lerp(
                model.State.HoveredAxis == Axis.Z ? zColor.Lightened(0.3f) : zColor, (float)delta * 24.0f);
    }
}