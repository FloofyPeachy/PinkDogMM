using System;
using Godot.Collections;
using PinkDogMM_Gd._3D.Tools;
using PinkDogMM_Gd.Core.Schema;
using Array = Godot.Collections.Array;

namespace PinkDogMM_Gd.Core.Actions.All.Tools;

public class SizeTool : Tool
{
    private Dictionary sizesPoses = new Dictionary();
    private Dictionary initPosesSizes = new Dictionary();
    
    public override void Execute()
    {
        foreach (var keyValuePair in sizesPoses)
        {
            var part = Model.GetItemById(keyValuePair.Key.AsInt32());

            var size = keyValuePair.Value.AsVector3Array()[0];
            var pos = keyValuePair.Value.AsVector3Array()[1];
            
            part.Size.X = size.X;
            part.Size.Y = size.Y;
            part.Size.Z = size.Z;

            part.Position.X = pos.X;
            part.Position.Y = pos.Y;
            part.Position.Z = pos.Z;
        }
        
        base.Execute();
    }
    public override void Tick(Dictionary arguments)
    {
        
        sizesPoses = arguments;
        
        foreach (var keyValuePair in sizesPoses)
        {
            var part = Model.GetItemById(keyValuePair.Key.AsInt32());
            if (!initPosesSizes.ContainsKey(keyValuePair.Key.AsInt32()))
            {
                initPosesSizes.Add(keyValuePair.Key.AsInt32(), new Array() {part.Size.AsVector3(), part.Position.AsVector3()});
            }
          
            var size = keyValuePair.Value.AsVector3Array()[0];
            var pos = keyValuePair.Value.AsVector3Array()[1];
            
            part.Size.X = size.X;
            part.Size.Y = size.Y;
            part.Size.Z = size.Z;
            
            part.Position.X = pos.X;
            part.Position.Y = pos.Y;
            part.Position.Z = pos.Z;
        }
    }
    public override void Undo()
    {
        foreach (var keyValuePair in initPosesSizes)
        {
            var part = Model.GetItemById(keyValuePair.Key.AsInt32());
            
            var size = keyValuePair.Value.AsVector3Array()[0];
            var pos = keyValuePair.Value.AsVector3Array()[1];
            
            part.Size.X = size.X;
            part.Size.Y = size.Y;
            part.Size.Z = size.Z;

            part.Position.X = pos.X;
            part.Position.Y = pos.Y;
            part.Position.Z = pos.Z;
            
        }
    }
    public override string Icon => "icon_mod_size";
    
    public override bool AddToStack => true;
    
    public override bool Sticky() => false;
    public override Type Tool3D() => typeof(SizeTool3D);
}