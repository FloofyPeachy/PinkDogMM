using Godot;
using PinkDogMM_Gd.Core.Commands;
using PinkDogMM_Gd.Core.Configuration;
using PinkDogMM_Gd.Core.Schema;

namespace PinkDogMM_Gd.Core.Actions.All.TheModel;

public class DeletePartAction(int id, AppState state) : IAction
{
    
    public void Execute()
    {
        var part = state.ActiveModel!.GetPartById(id);
    }

    public void Undo()
    {
        throw new System.NotImplementedException();
    }

    public bool AddToStack { get; }
    public string TextPrefix => "Deleted Part";
    
    public static int DefaultKeys => KeyCombo.KeyAndModifiers((int)Key.Delete, Modifiers.None);
}