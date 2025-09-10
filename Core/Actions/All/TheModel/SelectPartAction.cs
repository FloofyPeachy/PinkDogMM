using System.Linq;
using Godot;
using PinkDogMM_Gd.Core.Commands;
using PinkDogMM_Gd.Core.Schema;

namespace PinkDogMM_Gd.Core.Actions.All.TheModel;

public class SelectPartAction(int? id, AppState state) : IAction
{
    public static int DefaultKeys => -1;

    public void Execute()
    {
        
        if (id == null || state.ActiveModel!.GetPartById(id.Value) == null)
        {
            state.ActiveEditorState?.UnselectAllParts();
            return;
        }

        if (!Input.IsKeyPressed(Key.Shift)) state.ActiveEditorState?.UnselectAllParts();
        state.ActiveEditorState?.SelectPart(state.ActiveModel!.GetPartById(id.Value)?.Item2!);
        
    }

    public void Undo()
    {
        if (id == null || state.ActiveModel!.GetPartById(id.Value) == null)
        {
            state.ActiveEditorState?.UnselectAllParts();
            return;
        }
        state.ActiveEditorState?.UnselectPart(state.ActiveModel!.GetPartById(id.Value)?.Item2);
        
    }
   
    public bool AddToStack => false;
    public string TextPrefix => "Selected Part";
}