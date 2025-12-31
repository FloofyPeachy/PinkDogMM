using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using PinkDogMM_Gd.Core.Schema;

namespace PinkDogMM_Gd.Core.Commands;

public partial class HistoryStack : Resource
{
    private Stack<IAction> undoStack = new Stack<IAction>();
    private Stack<IAction> redoStack = new Stack<IAction>();
    private IStagedAction? activeAction;
    public event EventHandler<IAction>? ActionExecuted;
    public event EventHandler<IAction>? ActionUndone;
    public event EventHandler<IAction>? ActionRedone;

    public event EventHandler<(string, IAction)>? ActionStarted;
    
    
    public int Position => undoStack.Count - redoStack.Count;
    
    public void Clear()
    {
        undoStack.Clear();
        redoStack.Clear();
    }
    
    public void Undo()
    {
        if (undoStack.Count == 0) return;
        var action = undoStack.Pop();
        action.Undo();
        redoStack.Push(action);
        OnActionUndone(action);
    }

    public void Start(string path, IStagedAction action)
    {
        Finish();
        activeAction = action;
        activeAction.Start();
        OnActionStarted((path, action));
    }

    public void Tick(Dictionary arguments)
    {
        activeAction?.Tick(arguments);
    }

    public void Finish()
    {
        if (activeAction != null) Execute(activeAction);
        activeAction = null;
    }
    public void Redo()
    {
      
        if (redoStack.Count == 0)
            return;

        var action = redoStack.Pop();

        // Execute the action first
        action.Execute();
        OnActionRedone(action);

        // Only push it if it should be tracked for undo
        if (action.AddToStack)
            undoStack.Push(action);
    }
    
    public void Execute(IAction action)
    {
       
        action.Execute();
        
        if (!action.AddToStack) return;
        undoStack.Push(action);
        redoStack.Clear();
        OnActionExecuted(action);
    }
    
  
    public TResult Execute<TResult>(IAction<TResult> action)
    {
        action.Execute();
        
         undoStack.Push(action);
         redoStack.Clear();
         OnActionExecuted(action);
         return action.Result;
    }
    public void UpdateHistory()
    {
        undoStack.Clear();
        redoStack.Clear();
    }
    
    public List<IAction> GetHistory() => undoStack.Concat(redoStack).ToList();
    
    protected virtual void OnActionExecuted(IAction e)
    {
        ActionExecuted?.Invoke(this, e);
    }

    protected virtual void OnActionUndone(IAction e)
    {
        ActionUndone?.Invoke(this, e);
    }

    protected virtual void OnActionRedone(IAction e)
    {
        ActionRedone?.Invoke(this, e);
    }


    protected virtual void OnActionStarted((string, IAction) e)
    {
        ActionStarted?.Invoke(this, e);
    }
}