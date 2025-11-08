using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;
using PinkDogMM_Gd.Core.Commands;
using PinkDogMM_Gd.Core.Configuration;
using PinkDogMM_Gd.Core.LoadSave;
using PinkDogMM_Gd.Core.Schema;

namespace PinkDogMM_Gd.Core.Actions.All.Editor;

[Tool]
public class LoadModelAction : IAction<Model>
{
    public static int DefaultKeys => KeyCombo.KeyAndModifiers((int)Key.O, KeyModifiers.Ctrl);

    private string path = "";
    
    public void Execute()
    {
        if (path == null) return;
        if (path.EndsWith(".mtb")) Result = new ToolboxLoader().Load(path);
        Console.WriteLine($"Loaded model from {path}");
    }

    public void SetArguments(Dictionary arguments)
    {
        path = arguments["path"].AsString();
    }

    public void Undo()
    {
    }
    
    public bool AddToStack => true;
    public string Id => "LoadModel";
    public string TextPrefix => "Loaded";

    public Model Result { get; private set; }
}