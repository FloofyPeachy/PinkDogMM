using System.Linq;
using Godot;
using PinkDogMM_Gd.Core.Commands;
using PinkDogMM_Gd.Core.Configuration;
using PinkDogMM_Gd.Core.Schema;

namespace PinkDogMM_Gd.Core.Actions.All.TheModel;

public class AddPartAtMouseAction(AppState state) : IAction
{
    
    private Part part;
    public void Execute()
    {
        part = new Shapebox();
        part.Id = state.ActiveModel!.TotalPartCount + 1;
        part.Name = "Part " + (state.ActiveModel!.TotalPartCount + 1);
        var pos = state.ActiveEditorState.WorldMousePosition.Round();
        part.Position = new Vector3L(pos.X, -pos.Y, -pos.Z);
        state.ActiveModel!.PartGroups.First().Value.Add(part);
        state.ActiveEditorState?.SelectPart(part);
    }

    public void Undo()
    {
        throw new System.NotImplementedException();
    }

    public string TextPrefix => "Added Part";
    public bool AddToStack => true;
    
    public static int DefaultKeys => KeyCombo.KeyAndModifiers((int)Key.Insert, KeyModifiers.None);
}
