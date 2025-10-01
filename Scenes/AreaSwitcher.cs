using Godot;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using PinkDogMM_Gd.Core;
using PinkDogMM_Gd.Core.Schema;
using PinkDogMM_Gd.Scenes;

public partial class AreaSwitcher : MarginContainer
{
	private AppState? appState;
	private List<HSplitContainer> areas = [];
	public override void _Ready()
	{
		appState = GetNode("/root/AppState") as AppState;
		appState.Models.CollectionChanged += ModelsOnCollectionChanged;
		appState.ActiveModelChanged2 += AppStateOnActiveModelChanged;
	}

	private void AppStateOnActiveModelChanged(int from, int to)
	{
		if (from != -1) 	areas[from].Visible = false;
		areas[to].Visible = true;
	}

	private void ModelsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		if (e.Action == NotifyCollectionChangedAction.Add)
		{
			if (e.NewItems == null) return;
			var model = e.NewItems[0] as Model;
			Debug.Assert(model != null, nameof(model) + " != null");
			appState.addingModel = model;
			PackedScene sceneToLoad = GD.Load<PackedScene>("res://Scenes/ModelArea.tscn");
		
			var modelArea = sceneToLoad.Instantiate<HSplitContainer>();
			AddChild(modelArea);
			areas.Add(modelArea);
			PL.I.Info("Added " + model.Name + " ModelArea");
			
		}
	}
}
