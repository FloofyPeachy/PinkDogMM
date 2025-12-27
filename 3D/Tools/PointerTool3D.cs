using Godot;
using Godot.Collections;
using PinkDogMM_Gd._3D.Tools;
using PinkDogMM_Gd.Core.Commands;

namespace PinkDogMM_Gd._3D.Gear;

public partial class PointerTool3D : Tool3D
{
    public override void Tick(Dictionary arguments)
    {
        base.Tick(arguments);
    }

    public override void MouseClick(Vector2 position, MouseButton buttonIndex, bool pressed)
    {
        if (buttonIndex != MouseButton.Left || !pressed) return;

        var idAtMouse = GetIdAtMouse();
        if (idAtMouse != -1)
        {
            ActionRegistry.Execute("TheModel/SelectPart",
                new Dictionary { { "model", Model }, { "id", idAtMouse} });
            ActionRegistry.Start("Tools/SizeTool",
                new Dictionary { { "model", Model }});
            /*ActionRegistry.Start("Tools/MoveTool",
                new Dictionary { { "model", Model }, { "id", idAtMouse} });*/
        }
        else
        {
            ActionRegistry.Execute("TheModel/SelectPart",
                new Dictionary { { "model", Model }, { "id", idAtMouse} });
            
        }
      
        
    }
    
    public override void MouseMotion(Vector2 position, MouseButtonMask? buttonMask)
    {
        var objectAtMouse = GetObjectAtMouse();
        var id = objectAtMouse?.Item2 ?? (Model.State.Hovering?.Id ?? -1);
      
    
        if (DragDelta.GetValueOrDefault() == new Vector2(1,1) && Model.State.SelectedObjects.Count != 0)
        {
            ActionRegistry.Start("Tools/MoveTool",
                new Dictionary { { "model", Model }});
        }
        if (id != -1)
        {
            DisplayServer.CursorSetShape(DisplayServer.CursorShape.PointingHand);
        }
        else
        {
            DisplayServer.CursorSetShape(DisplayServer.CursorShape.Arrow);
        }
    }
    public override Dictionary Execute()
    {
        return base.Execute();
    }
}