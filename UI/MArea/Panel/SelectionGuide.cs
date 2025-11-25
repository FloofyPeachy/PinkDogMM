using Godot;
using System;
using System.Linq;
using PinkDogMM_Gd.Core;
using PinkDogMM_Gd.Core.Schema;

public partial class SelectionGuide : PanelContainer
{
    private Model? model1;
    private Label label;
    public override void _EnterTree()
    {
        base._EnterTree();
        VisibilityChanged += () =>
        {

        };
        model1 = (GetNode("/root/AppState") as AppState).addingModel;
        if (model1 != null) 	this.SetMeta("model", model1);
        
    }

    public override void _Ready()
    {
        label = GetNode<Label>("Label");
        model1!.State.ObjectSelectionChanged += (sender, state) =>
        {
            label.Text = model1.State.SelectedObjects.Count switch
            {
                > 1 => String.Join(" + ", model1.State.SelectedObjects.Select(o => o.GetType().Name)),
                0 => "Nothing",
                _ => model1.State.SelectedObjects.First().GetType().Name
            };
        };
    }
}
