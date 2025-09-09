using Godot;
using System;
using PinkDogMM_Gd.Core;
using PinkDogMM_Gd.Core.Actions;

public partial class HistoryPanel : PanelContainer

{
	private AppState? appState;
	private ItemList historyList;
	public override void _Ready()
	{
		appState = GetNode("/root/AppState") as AppState;
		historyList = FindChild("HistoryList") as ItemList ?? throw new InvalidOperationException();
		appState.ActiveModelChanged += (index) =>
		{
			
		};
		appState.ActionExecuted += (sender, action) =>
		{
			BuildHistory();
		};

	}

	public void BuildHistory()
	{
		historyList.Clear();
		foreach (var action in appState.ActiveModel?.State.History.GetHistory() ?? throw new InvalidOperationException())
		{
			historyList.AddItem(action.TextPrefix);
		}
	}
	public void UndoButtonPressed()
	{
		appState.ActiveModel?.State.History.Undo();
	}
	
	public void RedoButtonPressed()
	{
		appState.ActiveModel?.State.History.Redo();
	}
}
