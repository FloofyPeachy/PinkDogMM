using Godot;
using Godot.Collections;
using PinkDogMM_Gd.Core.Schema;

namespace PinkDogMM_Gd.Render;

public partial class PartManipulator(PartMesh mesh) : Node3D
{
    Vector3 DragDirection = new Vector3();

    public override void _Ready()
    {
        base._Ready();
        GD.Print("Readuy!!");
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion motion)
        {
           
      
           
            if (DragDirection == Vector3.Up || DragDirection == Vector3.Down)
            {
                /*//GD.Print(vector3.Value);
                mesh.part.Size.Y = mesh.part.Size.Y + motion.Relative.Y *  0.01f;
                /*if (motion.Relative.Y * 2f > 0)
                {
                    mesh.part.Size.Y  = mesh.Pa
                }
                else
                {
                    mesh.part.Size.Y -= 1f;
                }#1#*/
              
            } else if (DragDirection == Vector3.Right || DragDirection == Vector3.Left)
            {
                mesh.part.Size.X += motion.Relative.X * 0.001f;
                //mesh.part.Size.X = Mathf.RoundToInt(mesh.part.Size.X);
                GD.Print(Mathf.RoundToInt(mesh.part.Size.X));
                //mesh.part.Size.X = Mathf.Round(mesh.part.Size.X);
                /*GD.Print(motion.Relative.X );
                mesh.part.Size.X += Mathf.Round(motion.Relative.X * 0.01f);
                motion.Relative.Length*/
                var cam = GetParent().GetParent().GetNode("../PivotXY2/PivotY/PivotX/Camera3D") as Camera3D;
               // GD.Print(cam.ProjectRayNormal(motion.Position));
                /*GD.Print(DeltaOnAxis(motion.Position, DragDirection, motion.Relative.Length()));
                mesh.part.Size.X += DeltaOnAxis(motion.Position, DragDirection, motion.Relative.Length());*/

            } else if (DragDirection == Vector3.Forward || DragDirection == Vector3.Back)
            {
                /*mesh.part.Size.Z = mesh.part.Size.Z + motion.Relative.X *  0.01f;*/
                
            }
            /*Vector3? normal = GetNodeAtMouse()?.Item1;
            Dictionary<Vector3, string> axes = new();
            axes.Add(Vector3.Right, "Right");
            axes.Add(Vector3.Left, "Left");
            axes.Add(Vector3.Up, "Up");
            axes.Add(Vector3.Down, "Down");
            axes.Add(Vector3.Forward, "Forward");
            axes.Add(Vector3.Back, "Back");



            foreach (var vector3 in axes)
            {
                if (normal == vector3.Key)
                {

                    if (vector3.Key == Vector3.Up || vector3.Key == Vector3.Down)
                    {
                        GD.Print(vector3.Value);
                        mesh.part.Size.Y += motion.Relative.Y;
                    }
                }
            }*/
        }
        else if (@event is InputEventMouseButton button)
        {
            if (button.ButtonIndex != MouseButton.Left) return;
            if (button.Pressed)
            {
                Vector3? normal = GetNodeAtMouse()?.Item1;

                if (normal != null)
                {
                    DragDirection = normal.Value;
                    GD.Print("Dragging now!" + DragDirection);
                }
            }
            else
            {
                DragDirection = Vector3.Zero;
                  GD.Print("Not dragging now!");
            }
        }
    }
    float Snap(float value, float step)
    {
        return Mathf.Round(value / step) * step;
    }

    private float DeltaOnAxis(Vector2 mousePos, Vector3 axis, float length)
    {
        var cam = GetParent().GetParent().GetNode("../PivotXY2/PivotY/PivotX/Camera3D") as Camera3D;
        var rayOrigin = cam.ProjectRayOrigin(mousePos);
        var rayDir = cam.ProjectRayNormal(mousePos);

        return axis.Dot(rayDir) * length;
        
    }
    private (Vector3, Node3D)? GetNodeAtMouse()
    {
        var result = new Dictionary();
        var spaceState = GetWorld3D().DirectSpaceState;
        var cam = GetParent().GetParent().GetNode("../PivotXY2/PivotY/PivotX/Camera3D") as Camera3D;
        var mousePos = GetViewport().GetMousePosition();

        var origin = cam.ProjectRayOrigin(mousePos);
        var end = origin + cam.ProjectRayNormal(mousePos) * 10000;
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


        return (hitNormal.Round(), result["collider"].As<Node3D>());
    }
}