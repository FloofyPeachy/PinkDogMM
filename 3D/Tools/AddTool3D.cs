using System;
using Godot;
using PinkDogMM_Gd.Core;

namespace PinkDogMM_Gd._3D.Tools;

public partial class AddTool3D : Tool3D
{
    private MeshInstance3D ghostPart;
    private int _stage = 0;
    private Vector3 _newSize;
    private Vector3 _newPos;
    private Vector3? _worldStart;
    
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

    public override void MouseClick(Vector2 position, MouseButton buttonIndex, bool pressed, bool doubl)
    {
        if (!pressed)
        {
            _worldStart = null;
        }
        else
        {
            _worldStart = CurrentWorldPos.Value.LHS();
        }
    }

    public override void MouseMotion(Vector2 position, MouseButtonMask? buttonMask)
    {

        if (WorldPosDelta != Vector3.Zero && Input.IsMouseButtonPressed(MouseButton.Left))
        {
            Vector3 v = WorldPosDelta.LH();
            Vector3 end = CurrentWorldPos.GetValueOrDefault();

            Vector3 min = _worldStart.Value.Min(end);
            Vector3 max = _worldStart.Value.Max(end);
            GD.Print("min: " + min);
            GD.Print("max: " + max);
            
            if (Input.IsKeyPressed(Key.Shift))
            {
                //Scale proportionally.
                _newPos = _worldStart.GetValueOrDefault();
                _newSize += v.Round();
                _newSize.Abs();
            }
            else
            {
                _newSize = max - min;
                _newPos = min;
            }
            
            
            ghostPart.GlobalPosition = _newPos;
        
            ghostPart.Mesh = new BoxMesh()
            {
                Size = _newSize / 16
            };
            
          //  GD.Print(_newPos.Round());
            //GD.Print(_newSize.Round());
        }
        else
        {
            _newSize = Vector3.Zero;
        }
        
        
        /*if (DragDelta.HasValue)
        {
            var initial = PlanePosFromMouse(DragStart.Value) * 16;
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
                    ? (PlanePosFromMouse(DragStart.Value) * 16)
                    : (PlanePosFromMouse(position) * 16)
                    .Round(); /*(PlanePosFromMouse(position, ctrlPressed ? Plane.PlaneYZ : new Plane() ) * (ctrlPressed ? 1 : 16)).Round().LH();#1#

            ghostPart.GlobalPosition =
                new Vector3((float)Math.Round(pos.X, 2), (float)Math.Round(pos.Y, 2), (float)Math.Round(pos.Z, 2)) / 1 / 16;
        }*/
        
        //GD.Print(DragStart.GetValueOrDefault() + ":" + position);

       
    }
}