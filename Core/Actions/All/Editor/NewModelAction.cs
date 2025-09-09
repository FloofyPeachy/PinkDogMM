
using Godot;
using PinkDogMM_Gd.Core.Commands;
using PinkDogMM_Gd.Core.Configuration;
using PinkDogMM_Gd.Core.Schema;

namespace PinkDogMM_Gd.Core.Actions.All.Editor;

public class NewModelAction : IAction<Model>
{
    
    
    private Model model;
    public string TextPrefix => "Created Model";

    public void Execute()
    {
        model = new Model();
        model.PartGroups.Add("Default", new BubblingObservableList<Part>());
    }

    public void Undo()
    {
        model = null;
    }

    public bool AddToStack { get; }
    public string Id { get; }
    public Model Result => model;
    
    public int DefaultKeys => KeyCombo.KeyAndModifiers((int)Key.N, Modifiers.Ctrl);
}