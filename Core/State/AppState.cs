using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Godot;
using Godot.Collections;
using Microsoft.Extensions.Logging;
using PinkDogMM_Gd.Core.Actions;
using PinkDogMM_Gd.Core.Actions.All.Editor;
using PinkDogMM_Gd.Core.Commands;
using PinkDogMM_Gd.Core.Configuration;
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
		PL.I.Info("deez nuts lol.");
	}
	
	public Model? ActiveModel => Models[ActiveModelIndex];
	public ModelEditorState/**/? ActiveEditorState => ActiveModel?.State;
	
	public ObservableCollection<Model> Models = new([]);

	//public Settings Settings = new();
	 //This method for doing things kinda sucks. We should change it..
	
	public HistoryStack History = new(); //for global editor state!!! not for models!!!!
	public Part? SelectedPart;
	
	[Signal] public delegate void ActiveModelChangedEventHandler(int index);
	[Signal] public delegate void ActiveModelChanged2EventHandler(int from, int to);
	
	public event EventHandler<IAction>? ActionExecuted;
	public event EventHandler<EditorMode>? ModeChanged;
	
	public event EventHandler<int>? FocusedCornerChanged;
	public Model? addingModel;
	public void ExecuteLoadSaveAction(string path)
	{
		var loadModelAction = new LoadModelAction();
		loadModelAction.SetArguments(new Dictionary(){ { "path", path } });
		
		Model model = History.Execute(loadModelAction);
		Models.Add(model);
		ActiveModelIndex = Models.Count - 1;
		
		
	}

	public void ResubscribeToEvents()
	{
	//	ActiveModel.State.PartSelectionChanged += OnPartSelectionChanged;
	//	ActiveModel.State.PartUnselected += OnPartUnselected;

		/*ActiveModel.State.History.ActionExecuted += OnActionExecuted;
		
		ActiveEditorState.ModeChanged += OnModeChanged;
		ActiveEditorState.FocusedCornerChanged += OnFocusedCornerChanged;*/

	}
	public void ExecuteNewModelAction()
	{
		Model model = History.Execute(new NewAction());
		Models.Add(model);
		ActiveModelIndex = Models.Count - 1;
	}
	
	protected virtual void OnActionExecuted(object? sender,IAction e)
	{
		ActionExecuted?.Invoke(this, e);
	}


	
	protected virtual void OnModeChanged(object? sender, EditorMode e)
	{
		ModeChanged?.Invoke(this, e);
	}

	protected virtual void OnFocusedCornerChanged(object? sender,int e)
	{
		FocusedCornerChanged?.Invoke(this, e);
	}
	
}
