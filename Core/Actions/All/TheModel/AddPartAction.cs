using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;
using PinkDogMM_Gd.Core.Commands;
using PinkDogMM_Gd.Core.Configuration;
using PinkDogMM_Gd.Core.Schema;

namespace PinkDogMM_Gd.Core.Actions.All.TheModel;

public class AddPartAction : IAction
{
    
    private Part part;
    private Model model;
    private bool byKey;
    
    public void Execute()
    {
        part = new Shapebox();
        part.Id = model.TotalPartCount + 1;
        part.Name = "Part " + (model.TotalPartCount + 1);
        if (byKey)
        {
            var pos = model.State.WorldMousePosition.Round();
            part.Position = new Vector3L(pos.X, -pos.Y, -pos.Z);
        }
        model.Items.Add(part, part.Name);
        model.State.SelectPart(part);
    }

    public void SetArguments(Dictionary arguments)
    {
        model = arguments["model"].As<Model>()?? throw new InvalidOperationException();
        byKey = arguments.ContainsKey("byKey") && arguments["byKey"].AsBool();
    }

    public void Undo()
    {
        model.Items.Remove(part);
    }

    public string TextPrefix => "Added Part";
    public bool AddToStack => true;
    
    public static int DefaultKeys => KeyCombo.KeyAndModifiers((int)Key.Insert, KeyModifiers.None);
}