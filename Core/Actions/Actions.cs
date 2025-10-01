using Godot;
using PinkDogMM_Gd.Core.Actions;

namespace PinkDogMM_Gd.Core.Commands;

public interface IAction
{
   
    public string TextPrefix { get; }
    public static int DefaultKeys { get; }
    void Execute();
    
    void Undo();
    
    public bool AddToStack { get; }
    
    
}

public interface IStagedAction : IAction
{
    void Update();
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

public interface IAction<TResult> : IAction
{
    TResult Result { get; }
}