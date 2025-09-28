using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Godot;
using Godot.Collections;
using PinkDogMM_Gd.Core.Actions;
using PinkDogMM_Gd.Core.Actions.All.Editor;
using PinkDogMM_Gd.Core.Commands;
using PinkDogMM_Gd.Core.Schema;

namespace PinkDogMM_Gd.Core;

[Tool] 
public partial class AppState : Node
{
	
	int activeModelIndex = -1;
	public int ActiveModelIndex
	{
		get => activeModelIndex;
		set
		{
			var old = activeModelIndex;
			activeModelIndex = value;
			ResubscribeToEvents();
			EmitSignal(nameof(ActiveModelChanged2), old, activeModelIndex);
		}
	}

	AppState()
	{
		CallDeferred("ExecuteNewModelAction");
	}
	
	public Model? ActiveModel => Models[ActiveModelIndex];
	public ModelEditorState/**/? ActiveEditorState => ActiveModel?.State;
	
	public ObservableCollection<Model> Models = new([]);

	
	public HistoryStack History = new(); //for global editor state!!! not for models!!!!
	public Part? SelectedPart;
	
	[Signal] public delegate void ActiveModelChangedEventHandler(int index);
	[Signal] public delegate void ActiveModelChanged2EventHandler(int from, int to);
	public event EventHandler<Part>? PartSelected;
	public event EventHandler<Renderable>? ObjectSelected;
	public event EventHandler? AllPartsUnselected;
	public event EventHandler<Part>? PartUnselected;
	
	public event EventHandler<IAction>? ActionExecuted;
	public event EventHandler<IAction>? ActionUndone;
	public event EventHandler<IAction>? ActionRedone;
	
	public event EventHandler<bool>? IsPeekingChanged;
	public event EventHandler<EditorMode>? ModeChanged;
	
	public event EventHandler<int>? FocusedCornerChanged;
	public Model? addingModel;
	public void ExecuteLoadSaveAction(string path)
	{
		Model model = History.Execute(new LoadModelAction(path, this));
		Models.Add(model);
		ActiveModelIndex = Models.Count - 1;
		
		
	}

	public void ResubscribeToEvents()
	{
	//	ActiveModel.State.PartSelectionChanged += OnPartSelectionChanged;
	//	ActiveModel.State.PartUnselected += OnPartUnselected;
		ActiveModel.State.AllPartsUnselected += OnAllPartsUnselected;

		ActiveModel.State.History.ActionExecuted += OnActionExecuted;
		ActiveModel.State.History.ActionUndone += OnActionUndone;
		ActiveModel.State.History.ActionRedone += OnActionRedone;
		
		ActiveEditorState.IsPeekingChanged += OnIsPeekingChanged;
		ActiveEditorState.ModeChanged += OnModeChanged;
		ActiveEditorState.FocusedCornerChanged += OnFocusedCornerChanged;
		ActiveEditorState.ObjectSelected += OnObjectSelected;

	}
	public void ExecuteNewModelAction()
	{
		Model model = History.Execute(new NewModelAction());
		Models.Add(model);
		ActiveModelIndex = Models.Count - 1;
	}




	

	protected virtual void OnPartSelectionChanged(object? sender, Part e)
	{
		PartSelected?.Invoke(this, e);
	}

	protected virtual void OnAllPartsUnselected(object? sender, EventArgs args)
	{
		AllPartsUnselected?.Invoke(this, EventArgs.Empty);
	}

	protected virtual void OnPartUnselected(object? sender, Part e)
	{
		PartUnselected?.Invoke(this, e);
	}

	protected virtual void OnActionExecuted(object? sender,IAction e)
	{
		ActionExecuted?.Invoke(this, e);
	}

	protected virtual void OnActionUndone(object? sender,IAction e)
	{
		ActionUndone?.Invoke(this, e);
	}

	protected virtual void OnActionRedone(object? sender,IAction e)
	{
		ActionRedone?.Invoke(this, e);
	}

	protected virtual void OnIsPeekingChanged(object? sender,bool e)
	{
		IsPeekingChanged?.Invoke(this, e);
	}
	
	protected virtual void OnModeChanged(object? sender, EditorMode e)
	{
		ModeChanged?.Invoke(this, e);
	}

	protected virtual void OnFocusedCornerChanged(object? sender,int e)
	{
		FocusedCornerChanged?.Invoke(this, e);
	}

	protected virtual void OnObjectSelected(object? sender, Renderable e)
	{
		ObjectSelected?.Invoke(this, e);
	}
}
