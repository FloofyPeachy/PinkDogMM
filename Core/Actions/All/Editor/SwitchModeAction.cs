using System;
using Godot.Collections;
using PinkDogMM_Gd.Core.Commands;
using PinkDogMM_Gd.Core.Schema;

namespace PinkDogMM_Gd.Core.Actions.All.Editor;

public class SwitchModeAction : IAction
{
    private EditorMode mode;
    private EditorMode oldMode;
    private Model model;
    private string icon = "icon_newblock_new";

    string IAction.Icon => icon;
    
    public void Execute()
    {
        oldMode = model.State.Mode;
        model.State.ChangeMode(mode);
        icon = mode switch
        {
            EditorMode.Normal => "icon_newblock_new",
            EditorMode.Move => "icon_mod_move",
            EditorMode.Resize => "icon_mod_size",
            EditorMode.Paint => "icon_paint",
            EditorMode.Rotate => "icon_mod_size",
            EditorMode.ShapeEdit => "icon_mod_edit",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public void SetArguments(Dictionary arguments)
    {
        model = arguments["model"].As<Model>()?? throw new InvalidOperationException();
        mode = (EditorMode)arguments["mode"].AsInt32();
    }

    public void Undo()
    {
        model.State.ChangeMode(oldMode);
    }

    public bool AddToStack => oldMode is EditorMode.ShapeEdit or EditorMode.Paint;
}