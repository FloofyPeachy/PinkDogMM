using Godot;
using Godot.Collections;
using PinkDogMM_Gd.Core.Commands;
using PinkDogMM_Gd.Core.Configuration;
using PinkDogMM_Gd.Core.Schema;

namespace PinkDogMM_Gd.Core.Actions.All.Editor;

public class NewAction : IAction<Model>
{
    
    private Model model;
    public string Icon => "Created Model";

    public void Execute()
    {
        model = new Model();
        //model.PartGroups.Add("Default", new BubblingObservableList<Part>());
    }

    public void SetArguments(Dictionary arguments)
    {
        throw new System.NotImplementedException();
    }

    public void SetArguments(System.Collections.Generic.Dictionary<string, Variant> arguments)
    {
        
    }
    

    public void Undo()
    {
        model = null;
    }

    public bool AddToStack { get; }
    public string Id { get; }
    public Model Result => model;
    
    public static int DefaultKeys => KeyCombo.KeyAndModifiers((int)Key.N, KeyModifiers.Ctrl);
}