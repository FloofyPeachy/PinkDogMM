using Godot;
using Godot.Collections;
using PinkDogMM_Gd.Core;

namespace PinkDogMM_Gd._3D.Tools;

public partial class MoveTool3D : Tool3D
{
    private MeshInstance3D ghostPart;
    
    public override void MouseClick(MouseButton buttonIndex, bool pressed)
    {
        if (buttonIndex == MouseButton.Left && !pressed)
        {
            Model.State.Camera.UpdateCamera();
            ActionRegistry.Start("Tools/PointerTool",
                new Dictionary { { "model", Model }});
        }
    }
    public override void MouseMotion(Vector2 position)
    {
        bool ctrlPressed = Input.IsPhysicalKeyPressed(Key.Ctrl);
        Model.State.ActiveAxis = ctrlPressed ? Axis.Y : Axis.All;
        var
            pos = /*(PlanePosFromMouse(position, ctrlPressed ? Plane.PlaneYZ : new Plane() ) * (ctrlPressed ? 1 : 16)).Round().LH();*/
                (PlanePosFromMouse(position, ctrlPressed ? Plane.PlaneYZ : default) * 16).Round().LH();
        var positions = new Godot.Collections.Dictionary();
       
        foreach (var renderable in Model.State.SelectedObjects)
        {
            var newPos = renderable.Position.AsVector3();
            if (Model.State.ActiveAxis is Axis.X or Axis.All)
            {
                newPos.X = pos.X;
            }

            if (Model.State.ActiveAxis is Axis.Y or Axis.All)
            {
                newPos.Y = pos.Y;
            }

            if (Model.State.ActiveAxis is Axis.Z or Axis.All)
            {
                newPos.Z = pos.Z;
            }
                    
            positions.Add(renderable.Id, newPos);
        }
        
        ActionRegistry.Tick(positions);
        
        
        DisplayServer.CursorSetShape(DisplayServer.CursorShape.Move);
    }
}