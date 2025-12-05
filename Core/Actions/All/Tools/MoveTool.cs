using System;
using Godot.Collections;
using PinkDogMM_Gd._3D.Tools;

namespace PinkDogMM_Gd.Core.Actions.All.Tools;

public class MoveTool : Tool
{
    private Dictionary positions = new Dictionary();
    private Dictionary initalPositions = new Dictionary();
    
    public override void Execute()
    {
        foreach (var keyValuePair in positions)
        {
            var part = Model.GetItemById(keyValuePair.Key.AsInt32());
            
            var pos = keyValuePair.Value.AsVector3();
            part.Position.X = pos.X;
            part.Position.Y = pos.Y;
            part.Position.Z = pos.Z;
        }
        
        base.Execute();
    }
    public override void Tick(Dictionary arguments)
    {
        
        positions = arguments;
        
        foreach (var keyValuePair in positions)
        {
            var part = Model.GetItemById(keyValuePair.Key.AsInt32());
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
    public override void Undo()
    {
        foreach (var keyValuePair in initalPositions)
        {
            var part = Model.GetItemById(keyValuePair.Key.AsInt32());
            
            var pos = keyValuePair.Value.AsVector3();
            part.Position.X = pos.X;
            part.Position.Y = pos.Y;
            part.Position.Z = pos.Z;
            
        }
    }
    public override string Icon => "icon_move_parts";
    
    public override bool AddToStack => true;
    
    public override bool Sticky() => false;
    public override Type Tool3D() => typeof(MoveTool3D);
}