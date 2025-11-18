using System.Collections.Specialized;
using Godot;
using PinkDogMM_Gd.Core;
using PinkDogMM_Gd.Core.Schema;

namespace PinkDogMM_Gd.Scenes;

public partial class ModelTabs : TabBar
{
	
	private AppState _appState = null!;

	public override void _Ready()
	{
		_appState = (GetNode("/root/AppState") as AppState)!;
		_appState.Models.CollectionChanged += (sender, args) =>
		{
			if (args.Action == NotifyCollectionChangedAction.Add)
			{
				Model? model = args.NewItems?[0] as Model;
				AddTab(model.Name);
				SetTabCloseDisplayPolicy(CloseButtonDisplayPolicy.ShowActiveOnly);
			}
		};
		TabClosePressed += tab =>
		{
			GD.Print("Attempting to close!!");
		};
		_appState.ActiveModelChanged += index =>
		{
			SetCurrentTab(index);
		};
		TabClicked += tab =>
		{
			_appState.ActiveModelIndex = (int)tab;
		};

	}

	
}
