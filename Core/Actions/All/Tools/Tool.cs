using System;

using Godot;
using Godot.Collections;
using PinkDogMM_Gd._3D.Tools;
using PinkDogMM_Gd.Core.Commands;
using PinkDogMM_Gd.Core.Schema;

namespace PinkDogMM_Gd.Core.Actions.All.Tools;

/*
 * A tool is something you can select that helps you do a thing in the 3D space.
 * Toggle them with a keybind or a button press.
 */
public enum Tools {
    Pointer,
    Move
}

public abstract class Tool : IStagedAction
{
    private Node3D worldRoot;
    private Tool3D tool3DInstance;
    public Model Model;
    private Dictionary _result;
    
    void IStagedAction.Start()
    {
        tool3DInstance = Activator.CreateInstance(Tool3D()) as Tool3D ?? throw new InvalidOperationException();
        worldRoot.AddChild(tool3DInstance);
    }

    public virtual void Tick(Dictionary arguments)
    {
        //tool3DInstance.Tick(arguments);
    }
    
    public virtual void Execute()
    {
        //All done! Cleanup.
        _result = tool3DInstance.Execute();
        tool3DInstance.Free();
    }

  
    public virtual void Undo()
    {
        //Do something with the result.
        throw new NotImplementedException();
    }
    public void SetArguments(Dictionary arguments)
    {
        worldRoot = arguments["worldRoot"].As<Node3D>();
        Model = arguments["model"].As<Model>();
    }

    virtual public bool AddToStack => true;

    virtual public string Icon => "icon_move_parts";

   
    virtual public bool Sticky()
    {
        return false;
    }

    virtual public Type Tool3D()
    {
        return GetType();
    }

    virtual public Dictionary Result => _result;
}