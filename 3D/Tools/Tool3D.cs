using Godot;
using Godot.Collections;
using PinkDogMM_Gd.Core;
using PinkDogMM_Gd.Core.Actions;
using PinkDogMM_Gd.Core.Schema;
using PinkDogMM_Gd.UI.Viewport;

namespace PinkDogMM_Gd._3D.Tools;

/*
 * Handy-dandy class for giving 3D world-space access to Tools.
 */
public abstract partial class Tool3D : Node3D
{
    public Camera3D Camera;
    public Model Model;

    public Vector2 CurrentMousePos = Vector2.Zero;
    
    public Vector3? FirstWorldPos = Vector3.Zero;
    public Vector3 WorldPosDelta = Vector3.Zero;
    public Vector3 LastWorldPos = Vector3.Zero;
    public Vector3? CurrentWorldPos = Vector3.Zero;
    
    public Plane WorldPlane = default;
    public Vector2? DragStart;
    public Vector2? DragDelta;
    
    public bool Captured = false;
    public ActionRegistry ActionRegistry;

    /*
     * Godot Functions
     */
    public override void _Ready()
    {
        Camera = GetNode<Camera3D>("../PivotXY2/PivotY/PivotX/Camera3D");
        ActionRegistry = GetNode<ActionRegistry>("/root/ActionRegistry");
        
        Model = Model.Get(this);
        Selected();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        
        switch (@event)
        {
            case InputEventMouseButton button:
            {
                if (button.ButtonIndex == MouseButton.Right) return;
                DragStart = button.Pressed ? button.Position : null;
                MouseClick(button.Position, button.ButtonIndex, button.Pressed);
                GD.Print(DragStart.GetValueOrDefault());
                FirstWorldPos = button.Pressed ? PlanePosFromMouse(button.Position) : null;
                break;
              
            }
            case InputEventMouseMotion motion:
            {
               
             
                // 1. update mouse position first
                if (!Captured)
                    CurrentMousePos = motion.Position;
                else
                    CurrentMousePos += motion.Relative;

// 2. compute current world position BEFORE updating LastWorldPos
               CurrentWorldPos = (PlanePosFromMouse(CurrentMousePos) * 16).LH();

// 3. initialize drag start once
                DragStart ??= motion.Position;

// 4. compute delta here
                WorldPosDelta = CurrentWorldPos.GetValueOrDefault() - LastWorldPos;

// 5. call your scaling logic
                MouseMotion(CurrentMousePos);

// 6. update world-pos for next frame
                CurrentWorldPos = CurrentWorldPos.GetValueOrDefault();
                LastWorldPos = CurrentWorldPos.GetValueOrDefault();

                break;
            }
        }
    }

/*
 * Event Functions (you override these)
 */

    public virtual void Selected()
    {
    }

    public virtual void Tick(Dictionary arguments)
    {
       
    }
    
    public virtual Dictionary Execute()
    {
        return new Dictionary();
    }
    
    public virtual void MouseClick(Vector2 position, MouseButton buttonIndex, bool pressed)
    {
    }

    public virtual void MouseMotion(Vector2 position)
    {
    }
    
/*
 * Helper Functions (you can use these)
 */
    public void Capture()
    {
        Input.SetMouseMode(Input.MouseModeEnum.Captured);
        Captured = true;
    }

    public void Uncapture()
    {
        Input.SetMouseMode(Input.MouseModeEnum.Visible);
        Captured = false;
    }


    public Vector3 WorldPosFromMouse(Vector2 mousePos)
    {
        return Camera.ProjectPosition(mousePos, Camera.Position.Z);
    }

    public Vector3 PlanePosFromMouse(Vector2 mousePos)
    {
        var result = new Dictionary();
        var spaceState = Camera.GetWorld3D().DirectSpaceState;

        var origin = Camera.ProjectRayOrigin(mousePos);
        var end = origin + Camera.ProjectRayNormal(mousePos) * 1000.0f;
        
        var positionY = Model.State.Hovering?.Position.Y;
        if (WorldPlane == Plane.PlaneYZ)
        {
            WorldPlane.X = -1.395f;
        }
        else
        {
            WorldPlane.Y = -1.395f;
        }
       
        var intersection = WorldPlane.IntersectsRay(origin, end);
        /*i/*f (positionY != null && intersection != null)
            intersection = intersection.Value with { Y = -positionY.Value / 1 / 16 };#1#*/

        return intersection ?? Vector3.Zero;
    }
    public (Vector3, Node3D, Vector3, int)? GetNodeAtPos(Vector2 position)
    {
        var spaceState = Camera.GetWorld3D().DirectSpaceState;
        
        var origin = Camera.ProjectRayOrigin(position);
        var end = origin + Camera.ProjectRayNormal(position) * 1000.0f;
        DebugDraw3D.DrawRay(origin, Vector3.Forward, 1000f, Colors.Yellow);
        var query = PhysicsRayQueryParameters3D.Create(origin, end);
        query.CollideWithAreas = true;
        query.CollideWithBodies = true;
        var result = spaceState.IntersectRay(query);
        if (result.Count == 0) return null;

        var hitNormal = result["normal"].AsVector3();

        return (hitNormal.Round(), result["collider"].As<Node3D>(), result["position"].AsVector3(),
            result["face_index"].AsInt32());
    }
    private (Vector3, Node3D, Vector3, int)? GetNodeAtMouse()
    {
        return GetNodeAtPos(GetViewport().GetMousePosition());
    }

    public (Vector3, int, Vector3)? GetObjectAtMouse()
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

    public int GetIdAtMouse()
    {
        var objectAtMouse = GetObjectAtMouse();
        return  objectAtMouse?.Item2 ?? (Model.State.Hovering?.Id ?? -1);
    }

    
}