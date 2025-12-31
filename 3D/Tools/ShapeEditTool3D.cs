using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using PinkDogMM_Gd.Core;
using PinkDogMM_Gd.Core.Schema;
using PinkDogMM_Gd.UI.Viewport;

namespace PinkDogMM_Gd._3D.Tools;

public partial class ShapeEditTool3D : Tool3D
{
    private List<Vector3>? _initalSizes;
    private List<MeshInstance3D> gizmos = [];
    private Shapebox? part;
    private List<CornerNode> corners = [];
    public bool[] selectedIndicies = [false, false, false, false, false, false, false, false];

    public override void Selected()
    {
        part = Model.State.SelectedObjects.First() as Shapebox;
        if (part == null) return;
        for (var i = 0; i < 8; i++)
        {
            var corner = new CornerNode(this, i);
            corner.SetMeta("corner", i);
            corners.Add(corner);
            AddChild(corner);
        }

        part.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName.Contains("Shapebox"))
            {
                PositionCorners();
            }
        };

        this.Scale = new Vector3(0.01f, 0.01f, 0.01f);
        PositionCorners();
    }


    public override void _PhysicsProcess(double delta)
    {
        PositionCorners();
        this.Scale = this.Scale.Lerp(Vector3.One, (float)delta * 24.0f);
    }


    public void CornerClick(int index, bool pressed)
    {
        selectedIndicies[index] = pressed;

        if (pressed)
        {
            Capture();
        }
        else
        {
            Uncapture();
        }
    }

    public void CornerHover(int index, bool isit)
    {

    }

    public override void MouseClick(Vector2 position, MouseButton buttonIndex, bool pressed, bool doubl = false)
    {
        if (buttonIndex == MouseButton.Left) {
            if (!pressed) Uncapture();
            
        }
    }

    public override void MouseMotion(Vector2 position, MouseButtonMask? buttonMask)
    {
        for (var index = 0; index < selectedIndicies.Length; index++)
        {
            var selectedIndicy = selectedIndicies[index];
            if (!selectedIndicy) return;
            var vpDelta = WorldPosDelta;
            part.Corners[index].X += (float)Math.Round(vpDelta.X);
            GD.Print(vpDelta.Round());
        }
    }

    public void PositionCorners()
    {
        if (part == null) return;
        this.Position = part.Position.AsVector3().LHS();
        if (part is Shapebox shapebox)
        {
            for (var index = 0; index < corners.Count; index++)
            {
                var node = corners[index];
                var corner = shapebox.GetCornerPosition(index);
                node.Position = new Vector3(corner.Z, corner.Y, corner.X) / 2 /1/8/* ((new Vector3(corner.Z, corner.Y, corner.X) / 1 / 20) /
                                 new Vector3(model.State.Camera.Zoom, model.State.Camera.Zoom, model.State.Camera.Zoom))*/;
                node.SetFocused(false);
                
                
            }
        }
    }
}