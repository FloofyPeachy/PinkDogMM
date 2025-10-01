using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;
using Godot.Collections;
using PinkDogMM_Gd.Core;
using PinkDogMM_Gd.Core.Actions.All.TheModel;
using PinkDogMM_Gd.Core.Schema;
using PinkDogMM_Gd.Render;
using PinkDogMM_Gd.UI.Viewport;

namespace PinkDogMM_Gd.Scenes;

public partial class ModelNode : Node3D
{
	private System.Collections.Generic.Dictionary<Part, PartNode> parts = [];
	private Vector2 _lastMousePos;
	private bool _dragging = false;
	private AppState appState;
	private Part? _editedPart;
	private Model model;
	public override void _Ready()
	{
		model = Model.Get(this);
		SetMeta("model", model);
		appState = GetNode("/root/AppState") as AppState;
		Debug.Assert(appState != null, nameof(appState) + " != null");
		
		model.Helpers.ItemsChanged += (sender, changed) =>
		{
			GetParent().AddChild(new HelpframeNode(changed.Item as Helpframe ?? throw new InvalidOperationException()));
		};
		
		model.PartGroups.NestedCollectionChanged += (sender, args) =>

		{
			var part = args.Item2.Item as Part;
			var partNode = new PartNode(part);
			if (args.Item2.Added)
			{
				parts.Add(args.Item2.Item as Part, partNode);
				AddChild(partNode);
			}
			else
			{
				parts[part].QueueFree();
				parts.Remove(part);
			}
		};

		model.State.PartSelectionChanged += (sender, tuple) =>
		{
			parts[tuple.Item1].SetSelected(tuple.Item2); 
		};
			
		model.State.IsPeekingChanged += (sender, b) =>
		{
			foreach (var keyValuePair in parts)
			{
				if (!b)
				{
					if (!model.State.SelectedParts.Contains(keyValuePair.Key))
					{
						parts[keyValuePair.Key].SetVisibility(true);
						continue;
					}
				}
					
				if (!model.State.SelectedParts.Contains(keyValuePair.Key))
				{
					parts[keyValuePair.Key].SetVisibility(!b);
				}
					
					
			}
		};

		model.State.ObjectHoveringChanged += (sender, renderable) =>
		{
			foreach (var keyValuePair in parts)
			{
				keyValuePair.Value.SetHovering(keyValuePair.Key == renderable);
			}
		};
		
		model.State.ModeChanged += (sender, mode) =>
		{
			
			if (mode == EditorMode.ShapeEdit)
			{
				_editedPart = model.State.SelectedParts.First() as Part;
				model.State.SelectedParts.Clear();
				model.State.SelectPart(_editedPart);
				parts[_editedPart].SetBeingEdited(true);
				
			}
			else
			{
				//parts[_editedPart].SetBeingEdited(false);
			}
		};
		
		foreach (var modelAllPart in model.AllParts)
		{
			var newOne = new PartNode(modelAllPart);
			parts.Add(modelAllPart, newOne);
			AddChild(newOne);
			newOne.Owner = this;
		}
	}

	public void RebuildAll()
	{
		foreach (var keyValuePair in parts)
		{
			keyValuePair.Value.QueueFree();
		}
		parts.Clear();
		
		foreach (var modelAllPart in model.AllParts)
		{
			var newOne = new PartNode(modelAllPart);
			parts.Add(modelAllPart, newOne);
			AddChild(newOne);
		}
	}
	public void RefreshAll()
	{
		foreach (var keyValuePair in parts)
		{
			keyValuePair.Value.UpdateMesh(true);
		}
	}
	private void OnButtonPressed()
	{
		foreach (var findChild in this.GetChildren())
		{
			RemoveChild(findChild);
		}
	}
	
}
