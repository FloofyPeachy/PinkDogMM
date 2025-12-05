using System;
using Godot;
using PinkDogMM_Gd.Core;

namespace PinkDogMM_Gd._3D.Tools;

public partial class AddTool3D : Tool3D
{
    private MeshInstance3D ghostPart;
    private int _stage = 0;

    public override void _Ready()
    {
        base._Ready();
        ghostPart = new MeshInstance3D()
        {
            Mesh = new BoxMesh()
            {
                Size = new Vector3(1, 1, 1) / 1 / 16
            }
        };
        AddChild(ghostPart);
    }

    public override void MouseClick(MouseButton buttonIndex, bool pressed)
    {
    }

    public override void MouseMotion(Vector2 position)
    {
        if (DragDelta.HasValue)
        {
            var initial = PlanePosFromMouse(DragStart.Value, default) * 16;
            var deltaPos = ((PlanePosFromMouse(DragStart.Value) * 16) - Model.State.GridMousePosition).Round();

            ghostPart.Mesh = new BoxMesh()
            {
                Size = new Vector3(Math.Abs(deltaPos.X), 1, Math.Abs(deltaPos.Z)) / 16f
            };

// Instead of modifying the existing global position, compute a fresh one:
            var newZ = (initial.Z - Math.Min(deltaPos.Z, 0)) / 16f;

            ghostPart.GlobalPosition = new Vector3(
                ghostPart.GlobalPosition.X,    // or initial.X if it should be locked
                ghostPart.GlobalPosition.Y,    // same idea here
                newZ
            );
            
        }
        else
        {
            ghostPart.Mesh = new BoxMesh()
            {
                Size = new Vector3(1, 1, 1) / 1 / 1 / 16
            };
            var
                pos = DragStart.HasValue
                    ? (PlanePosFromMouse(DragStart.Value, default) * 16)
                    : (PlanePosFromMouse(position, default) * 16)
                    .Round(); /*(PlanePosFromMouse(position, ctrlPressed ? Plane.PlaneYZ : new Plane() ) * (ctrlPressed ? 1 : 16)).Round().LH();*/

            ghostPart.GlobalPosition =
                new Vector3((float)Math.Round(pos.X, 2), (float)Math.Round(pos.Y, 2), (float)Math.Round(pos.Z, 2)) / 1 / 16;
        }
        
        //GD.Print(DragStart.GetValueOrDefault() + ":" + position);

       
    }
}