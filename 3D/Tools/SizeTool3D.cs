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
    public override void MouseClick(MouseButton buttonIndex, bool pressed)
    {
        if (pressed)
        {
            Capture();
        }
        else
        {
            Uncapture();
        }
        if (buttonIndex == MouseButton.Left && !pressed)
        {
            Model.State.Camera.UpdateCamera();
            _initalSizes = null;
            
            ActionRegistry.Start("Tools/PointerTool",
                new Dictionary { { "model", Model }});
        }
    }

    public override void MouseMotion(Vector2 position)
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
        for (var index = 0; index < Model.State.SelectedObjects.Count; index++)
        {
            var renderable = Model.State.SelectedObjects[index];
            var newSize = renderable.Size.AsVector3();
            var newPos = renderable.Position.AsVector3();
            Vector3 distance = CurrentWorldPos.GetValueOrDefault() - FirstWorldPos.GetValueOrDefault();
            GD.Print(CurrentWorldPos);
            var v = (distance).Clamp(-128, 128).Round();
            
            if (Model.State.ActiveAxis is Axis.X or Axis.All)
            {
                newSize.X = Math.Abs(v.X);
                newPos.X = _initalSizes[index].X + Math.Min(v.X, 0);
            }

            if (Model.State.ActiveAxis is Axis.Y or Axis.All)
            {
                /*newSize.Y = newSize.Y;
                newPos.Y = _initalSizes[index].Y + Math.Min(v.Y, 0);*/
            }

            if (Model.State.ActiveAxis is Axis.Z or Axis.All)
            {
                newSize.Z = Math.Abs(v.Z);
                newPos.Z = _initalSizes[index].Z + Math.Min(v.Z, 0);
            }

            posSizes.Add(renderable.Id, new Array() {newSize, newPos});
            GD.Print(distance);
        }

        ActionRegistry.Tick(posSizes);
     
    }
}