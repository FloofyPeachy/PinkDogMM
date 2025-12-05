using System;
using Godot;
using Godot.Collections;
using PinkDogMM_Gd.Core.Commands;
using PinkDogMM_Gd.Core.Schema;

namespace PinkDogMM_Gd.Core.Actions.All.TheModel;

public class MovePartAction : IStagedAction
{
    public string Icon => "icon_move_parts";
    private Dictionary positions = new Dictionary();
    private Dictionary initalPositions = new Dictionary();
    private Model model;
    public void Start()
    {
        
    }
    public void Tick(Dictionary arguments)
    {
        positions = arguments;
        
        foreach (var keyValuePair in positions)
        {
            var part = model.GetItemById(keyValuePair.Key.AsInt32());
            if (!initalPositions.ContainsKey(keyValuePair.Key.AsInt32()))
            {
                initalPositions.Add(keyValuePair.Key.AsInt32(), part.Position.AsVector3());
            }
          
            var pos = keyValuePair.Value.AsVector3();
            part.Position.X = pos.X;
            part.Position.Y = pos.Y;
            part.Position.Z = pos.Z;
        }
    }
    public void Execute()
    {
        foreach (var keyValuePair in positions)
        {
            var part = model.GetItemById(keyValuePair.Key.AsInt32());
            
            var pos = keyValuePair.Value.AsVector3();
            part.Position.X = pos.X;
            part.Position.Y = pos.Y;
            part.Position.Z = pos.Z;
        }
    }

    public void SetArguments(Dictionary arguments)
    {
        model = arguments["model"].As<Model>()?? throw new InvalidOperationException();
        //positions = arguments["positions"].AsGodotDictionary();
    }

    public void Undo()
    {
        foreach (var keyValuePair in initalPositions)
        {
            var part = model.GetItemById(keyValuePair.Key.AsInt32());
            
            var pos = keyValuePair.Value.AsVector3();
            part.Position.X = pos.X;
            part.Position.Y = pos.Y;
            part.Position.Z = pos.Z;
            
        }
    }

    public bool AddToStack => true;


    public Dictionary Result { get; }
}