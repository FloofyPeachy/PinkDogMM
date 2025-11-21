using System;
using Godot.Collections;
using PinkDogMM_Gd.Core.Commands;
using PinkDogMM_Gd.Core.Schema;

namespace PinkDogMM_Gd.Core.Actions.All.TheModel;

public class GroupPartAction : IAction
{
    private bool unGroup = false;
    private Model model;

    public string Icon => "icon_add";
    public void Execute()
    {
        foreach (var obj in model.State.SelectedObjects)
        {
            //model.Items.Add("")
        }
    }

    public void SetArguments(Dictionary arguments)
    {
        model = arguments["model"].As<Model>()?? throw new InvalidOperationException();
        unGroup = arguments.ContainsKey("ungroup") && arguments["ungroup"].AsBool();
    }

    public void Undo()
    {
        throw new System.NotImplementedException();
    }

    public bool AddToStack => true;
}