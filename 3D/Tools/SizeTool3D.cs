using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;
using PinkDogMM_Gd.Core;
using Array = Godot.Collections.Array;

namespace PinkDogMM_Gd._3D.Tools;

public partial class SizeTool3D : Tool3D
{
    private List<Vector3>? _initalSizes;
    private List<MeshInstance3D> gizmos = [];
    private Color xColor = Color.Color8(58, 179, 218, 1);
    private Color yColor = Color.Color8(128, 199, 47, 1);
    private Color zColor = Color.Color8(176, 45, 36, 1);
    private Vector3 _newSize;
    private Vector3 _newPos;
    private Vector3 _size;
    private Vector3 _pos;

    public override void Selected()
    {
        MakeGizmo();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!_later) UpdateGizmo();
    }

    public override void MouseClick(Vector2 position, MouseButton buttonIndex, bool pressed, bool doubl)
    {
        bool ok = false;
        if (pressed && buttonIndex == MouseButton.Left)
        {
            if (doubl)
            {
                ActionRegistry.Start("Tools/ShapeEditTool",
                    new Dictionary { { "model", Model }});
                return;
            }
            var node = GetNodeAtPos(position);
            if (node.HasValue)
            {
                if (!node.Value.Item2.GetParent().HasMeta("axis"))
                {
                    //not on axis, but on a part. trying to move?
                    ActionRegistry.Start("Tools/MoveTool",
                        new Dictionary { { "model", Model }});
                    return;
                }
                Capture();
                var variant = node.Value.Item2.GetParent().GetMeta("axis").AsInt32();
                var axis = variant switch
                {
                    0 => Axis.Z,
                    1 => Axis.Y,
                    2 => Axis.X,
                    _ => Axis.All
                };
                ok = true;
                GD.Print("axis:" +  axis);
                Model.State.ActiveAxis = axis;
                WorldPlane = axis == Axis.Y ? Plane.PlaneYZ : default;
                _newSize = Model.State.SelectedObjects.Count != 0
                    ? Model.State.SelectedObjects[0].Size.AsVector3()
                    : Vector3.One;
                _newPos = Model.State.SelectedObjects.Count != 0
                    ? Model.State.SelectedObjects[0].Position.AsVector3().LHS()
                    : Vector3.Zero;
            }
          
        }
        else
        {
            Uncapture();
            _newSize = Vector3.Zero;
        }
        
        var idAtMouse = GetIdAtMouse();
        if (idAtMouse == -1 && !ok)
        {
            Uncapture();
            Model.State.Camera.UpdateCamera();
            _initalSizes = null;

            foreach (var child in GetChildren())
            {
                child.QueueFree();
            }
            ActionRegistry.Start("Tools/PointerTool",
                new Dictionary { { "model", Model }});
        }
       
    }

    public void UpdateGizmo()
    {
        if (Model.State.SelectedObjects.Count != 0)
        {
            _size = Model.State.SelectedObjects[0].Size.AsVector3().LHS();
            _pos = Model.State.SelectedObjects[0].Position.AsVector3().LHS();
        }

        GD.Print(_size);
        
        for (var index = 0; index < gizmos.Count; index++)
        {
            var gizmo = gizmos[index];
            gizmo.Position = (GizmoPosition(index, _size + new Vector3(0.1f, 0.1f, 0.1f)) / 2);
        }
        this.Position = (_pos + _size / 2);
    }

    public Vector3 GizmoPosition(int index, Vector3 size)
    {
        int axis = index / 2;      // 0=X, 1=Y, 2=Z
        float sign = (index % 2 == 0) ? 1 : -1;

        return axis switch
        {
            0 => new Vector3(sign * size.X, 0, 0),
            1 => new Vector3(0, sign * size.Y, 0),
            2 => new Vector3(0, 0, sign * size.Z),
            _ => Vector3.Zero
        };
    }

    public Color GizmoColor(int index)
    {
        int axis = index / 2;      // 0=X, 1=Y, 2=Z
       
        return axis switch
        {
            0 => zColor,
            1 => yColor,
            2 => xColor,
            _ => Colors.Green
        };
    }

    public void MakeGizmo()
    {
     
        Vector3 pos = Model.State.SelectedObjects.Count != 0
            ? Model.State.SelectedObjects[0].Position.AsVector3()
            : Vector3.Zero;
        Vector3 size = Model.State.SelectedObjects.Count != 0
            ? Model.State.SelectedObjects[0].Size.AsVector3()
            : Vector3.Zero;
        for (var index = 0; index < 6; index++)
        {
            MeshInstance3D gizmo = new MeshInstance3D();
            var standardMaterial3D = new StandardMaterial3D();
            gizmo.Mesh = new BoxMesh()
            {
                Size = new Vector3(0.05f, 0.05f, 0.05f),
                Material = standardMaterial3D
            };
            gizmo.SetMeta("axis", (int)(index / 2));
            gizmo.Rotation = new Vector3(0, 0, 0);
            standardMaterial3D.AlbedoColor = GizmoColor(index);
            gizmo.Position = GizmoPosition(index, size);
            gizmo.CreateConvexCollision();
            AddGizmo(gizmo);
        }

    }

    private void AddGizmo(MeshInstance3D gizmo)
    {
        AddChild(gizmo);
        gizmos.Add(gizmo);
    }
    private void DeleteGizmo(MeshInstance3D gizmo)
    {
       gizmo.QueueFree();
       gizmos.Remove(gizmo);
    }
    public override void MouseMotion(Vector2 position, MouseButtonMask? buttonMask)
    {
        if (!Captured) return;
        if (_initalSizes == null)
        {
            _initalSizes = [];
            foreach (var renderable in Model.State.SelectedObjects)
            {
                _initalSizes.Add(renderable.Size.AsVector3());
            }
            //while we're at it..
            
        }
        
        var posSizes = new Godot.Collections.Dictionary();
        GD.Print(WorldPosDelta * 16);
        for (var index = 0; index < Model.State.SelectedObjects.Count; index++)
        {
            var renderable = Model.State.SelectedObjects[index];
           
          
            var frameDelta = WorldPosDelta;
           
            Vector3 v = WorldPosDelta;
            if (Model.State.ActiveAxis is not Axis.All)
            {
                /*if (Model.State.ActiveAxis != Axis.X) v.X = 1;
                if (Model.State.ActiveAxis != Axis.Y) v.Y = 1;
                if (Model.State.ActiveAxis != Axis.Z) v.Z = 1;*/
            }
            GD.Print(v);

            /*v = (v * 2).Clamp(-128, 128).Round();*/

            /*if (Model.State.ActiveAxis is Axis.X or Axis.All)
            {
                _newSize.X += v.X;
                _newPos.X  += v.X;
                //newPos.X  += Math.Min(v.X, 0);
            }

            if (Model.State.ActiveAxis is Axis.Y or Axis.All)
            {
                _newSize.Y += v.Y;
                _newPos.Y  += v.Y;
            }
            if (Model.State.ActiveAxis is Axis.Z or Axis.All)
            {
                _newSize.Z += v.Z;
                _newPos.Z  += v.Z;
            }*/
            if (Model.State.ActiveAxis != Axis.X) v.X = 0;
            if (Model.State.ActiveAxis != Axis.Y) v.Y = 0;
            if (Model.State.ActiveAxis != Axis.Z) v.Z = 0;

            _newSize += v;
            _newPos += v;
                
            GD.Print(_newSize);

            posSizes.Add(renderable.Id, new Array() {_newSize.Round().Abs().Max(0), _newPos.Round().Min(0)});
           
        }

        ActionRegistry.Tick(posSizes);
     
    }
}