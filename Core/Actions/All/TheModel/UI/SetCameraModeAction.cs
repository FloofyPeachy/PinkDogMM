using System;
using Godot.Collections;
using PinkDogMM_Gd.Core.Commands;
using PinkDogMM_Gd.Core.Schema;
using PinkDogMM_Gd.Render;

namespace PinkDogMM_Gd.Core.Actions.All.TheModel.UI;

public class SetCameraModeAction : IAction
{
    private Model model;
    private Mode? mode;
    private Projection? projection;
    public string Icon { get; }
    public void Execute()
    {
        if (projection != null) model.State.Camera.Projection = (Projection)projection;
        if (mode != null) model.State.Camera.Mode = (Mode)mode;
    }

    public void SetArguments(Dictionary arguments)
    {
        model = arguments["model"].As<Model>() ?? throw new InvalidOperationException();
        projection = arguments["projection"].As<Projection>();
        
        
    }

    public void Undo()
    {
        throw new System.NotImplementedException();
    }

    public bool AddToStack => false;
}