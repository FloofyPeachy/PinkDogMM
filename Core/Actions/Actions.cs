using System.Collections.Generic;
using Godot;
using Godot.Collections;
using PinkDogMM_Gd.Core.Actions;

namespace PinkDogMM_Gd.Core.Commands;

public interface IAction
{
    public string Icon { get; }
    public static int DefaultKeys { get; }
    void Execute();

    void SetArguments(Dictionary arguments);

    void Undo();

    public bool AddToStack { get; }
}

public interface IStagedAction : IAction<Dictionary>
{
    /*
     * Start to start.
     * Tick during.
     * Execute() when done.
     */
    void Start();
    void Tick(Dictionary arguments);
}


public interface ISubstackAction : IStagedAction {

    /*
     * Action with a built in HistoryStack, for your pleasure
     * Start() to start.
     * Execute() when done.
     * .Undo() to undo in the INTERNAL stack.
     * .Redo() to redo IN the INTERNAL stack.
     */

    new void Undo();
    void Redo();
}

public class ActionResult
{
    public ActionResult(bool success)
    {
        Data = null;
        Success = success;
    }


    public ActionResult(object? data, bool success)
    {
        Data = data;
        Success = success;
    }

    public object? Data { get; set; }
    public bool Success { get; set; } = true;
}

public interface IAction<out TResult> : IAction
{
    TResult Result { get; }
}