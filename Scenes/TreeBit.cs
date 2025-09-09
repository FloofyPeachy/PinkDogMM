using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;
using PinkDogMM_Gd.Core;
using PinkDogMM_Gd.Core.Schema;

namespace PinkDogMM_Gd.Scenes;

public partial class TreeBit : Tree
{
	private List<int> selected = [];
	private List<TreeItem> items = [];
	public override void _Ready()
	{
		AppState appState = GetNode("/root/AppState") as AppState;
		Debug.Assert(appState != null, nameof(appState) + " != null");
		appState.ActiveModelChanged += (index) =>
		{
			HandleModel();
			UpdateEverything();
		};
		appState.PartSelected += (sender, part) =>
		{
			selected.Add(part.Id);
		};
		
		appState.PartUnselected += (sender, part) =>
		{
			selected.Remove(part.Id);
		};

		appState.AllPartsUnselected += (sender, args) =>
		{
			selected.Clear();
		};

	}

	public void HandleModel()
	{
		AppState appState = GetNode("/root/AppState") as AppState;
		Model? model = appState.ActiveModel;
		if (model == null) return;
		model.PartGroups.ItemChanged += (sender, args) =>
		{
			PopulateTree(model);
		};

		model.PartGroups.ItemsChanged += (sender, args) => PopulateTree(model);
		/*model.State.SelectedParts.Changed += () =>
		{
			selected.Clear();
			foreach (var i in model.State.SelectedParts)
			{
				selected.Add(i.Id);
			}

			PopulateTree(model);

		};

		model.PartGroups.ItemChanged += index =>
		{
			GD.Print("item changed!!");
			UpdateEverything();
		};

		foreach (var modelPartGroup in model.PartGroups)
		{
			modelPartGroup.Value.Changed += UpdateEverything;
		}*/
	}

	public void UpdateEverything()
	{
		AppState appState = GetNode("/root/AppState") as AppState;
		PopulateTree(appState.ActiveModel);
	}

	private void PopulateTree(Model model)
	{
		this.Clear();
		var tree = this;
		var root = tree.CreateItem(null); // root item, hidden if hide_root = true
		root.SetText(0,model.Name);
		foreach (var groupKey in model.PartGroups)
		{
			// Create a group (parent) node
			var groupItem = tree.CreateItem(root);
			groupItem.SetText(0, groupKey.Key);

			// Get the parts for this group
			var parts = model.PartGroups[groupKey.Key];

			// Add each part as a child node under group
			foreach (var part in parts)
			{
				var partItem = tree.CreateItem(groupItem);
				if (selected.Contains(part.Id)) partItem.Select(0);
				partItem.SetText(0, part.Name); // assuming Part has a Name property
				//partItem.SetMetadata(0, part.Id);
			}
		}
	}
}
