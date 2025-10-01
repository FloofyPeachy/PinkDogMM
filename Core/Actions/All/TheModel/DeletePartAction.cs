using System.Linq;
using Godot;
using PinkDogMM_Gd.Core.Commands;
using PinkDogMM_Gd.Core.Configuration;
using PinkDogMM_Gd.Core.Schema;

namespace PinkDogMM_Gd.Core.Actions.All.TheModel;

public class DeletePartAction(AppState state) : IAction
{
    private int id = -1;
    public void Execute()
    {
        
        if (id != -1)
        {
            var part = state.ActiveModel!.GetPartById(id);
        }
        else
        {
            foreach (var selectedPart in state.ActiveEditorState.SelectedParts.ToList())
            {
                state.ActiveModel.RemovePart(selectedPart.Id);
                state.ActiveEditorState.SelectedParts.Remove(selectedPart);
            }
        }
      
    }

    public void Undo()
    {
        throw new System.NotImplementedException();
    }

    public bool AddToStack { get; }
    public string TextPrefix => "Deleted Part";
    
    public static int DefaultKeys => KeyCombo.KeyAndModifiers((int)Key.Delete, KeyModifiers.None);

    public DeletePartAction(int id, AppState state) : this(state)
    {
        this.id = id;
    }
  
}