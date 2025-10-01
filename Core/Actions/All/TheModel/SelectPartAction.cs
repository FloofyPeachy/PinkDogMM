using System.Linq;
using Godot;
using PinkDogMM_Gd.Core.Commands;
using PinkDogMM_Gd.Core.Schema;

namespace PinkDogMM_Gd.Core.Actions.All.TheModel;

public class SelectPartAction(int? id, Model model) : IAction
{
    public static int DefaultKeys => -1;

    public void Execute()
    {
        
        if (id == null || model!.GetPartById(id.Value) == null)
        {
            model.State?.UnselectAllParts();
            return;
        }

        if (!Input.IsKeyPressed(Key.Shift)) model.State?.UnselectAllParts();
        model.State?.SelectPart(model!.GetPartById(id.Value)?.Item2!);
        
    }

    public void Undo()
    {
        if (id == null || model!.GetPartById(id.Value) == null)
        {
            model.State?.UnselectAllParts();
            return;
        }
        model.State?.UnselectPart(model!.GetPartById(id.Value)?.Item2);
        
    }
   
    public bool AddToStack => false;
    public string TextPrefix => "Selected Part";
}