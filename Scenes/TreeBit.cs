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
	private Dictionary<Renderable, TreeItem> items = [];
	private Model model;
	public override void _Ready()
	{

		model = Model.Get(this);
		model.Items.CollectionChanged += (sender, tuple) =>
		{
			PopulateTree();
		};
		this.ItemSelected += () =>
		{
			
		};
		/*model.PartGroups.NestedCollectionChanged += (sender, args) => PopulateTree();
		model.PartGroups.ItemsChanged += (sender, args) => PopulateTree();
		model.PartGroups.ItemChanged += (sender, args) =>
		{
			PopulateTree();
		};

		model.State.PartSelectionChanged += (sender, tuple) =>
		{
			//this.SetSelected(items[tuple.Item1], 0);

		};

		model.State.AllPartsUnselected += (sender, args) =>
		{
			selected.Clear();
		};
		PopulateTree();*/
	}


	private void AddToTree(TreeNode<Renderable> item, TreeItem parent)
	{
		if (item.Children.Count == 0)
		{
			var partItem = this.CreateItem(parent);
			partItem.SetText(0, item.Name);
			return;
		};
		foreach (var partChild in item.Children)
		{
			AddToTree(partChild, parent);
		}
	}
	private void PopulateTree()
	{
		this.Clear();
		var tree = this;
		var root = tree.CreateItem(null); // root item, hidden if hide_root = true
		root.SetText(0,model.Name);


		AddToTree(model.Items, root);
		/*foreach (var groupKey in model.PartGroups)
		{
			// Create a group (parent) node
			var groupItem = tree.CreateItem(root);
			groupItem.SetText(0, groupKey.Key);

			// Get the parts for this group
			var parts = model.PartGroups[groupKey.Key];

			// Add each part as a child node under group
			foreach (var part in parts)
			{
				if (items.ContainsKey(part)) continue;
				var partItem = tree.CreateItem(groupItem);
				if (selected.Contains(part.Id)) partItem.Select(0);
				partItem.SetText(0, part.Name); // assuming Part has a Name property
				//partItem.SetMetadata(0, part.Id);
				items.Add(part, partItem);
			}
		}*/
	}
}
