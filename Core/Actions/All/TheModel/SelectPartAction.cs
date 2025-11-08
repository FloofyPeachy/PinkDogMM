using System;
using System.Linq;
using Godot;
using Godot.Collections;
using PinkDogMM_Gd.Core.Commands;
using PinkDogMM_Gd.Core.Schema;

namespace PinkDogMM_Gd.Core.Actions.All.TheModel;

public class SelectPartAction : IAction
{
    public static int DefaultKeys => -1;

    public Model model;
    public int id;
    public void Execute()
    {
        
        if (model!.GetItemById(id) == null)
        {
            model.State?.UnselectAll();
            return;
        }

        if (!Input.IsKeyPressed(Key.Shift)) model.State?.UnselectAll();
        model.State?.SelectObject(id);
        
    }

    public void SetArguments(Dictionary arguments)
    {
        model = arguments["model"].As<Model>() ?? throw new InvalidOperationException();
        id = arguments["id"].As<int>();
    }

    public void SetArguments(System.Collections.Generic.Dictionary<string, Variant> arguments)
    {
        throw new System.NotImplementedException();
    }

    public void SetArguments(System.Collections.Generic.Dictionary<string, object> arguments)
    {
        throw new System.NotImplementedException();
    }

    public void Undo()
    {
        /*if (id == null || model!.GetPartById(id.Value) == null)
        {
            model.State?.UnselectAllParts();
            return;
        }
        model.State?.UnselectPart(model!.GetPartById(id.Value)?.Item2);*/
        
    }
   
    public bool AddToStack => false;
    public string TextPrefix => "Selected Part";
}