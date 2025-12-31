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
	
	private Vector2 _lastMousePos;
	private bool _dragging = false;
	private AppState appState;
	private Part? _editedPart;
	private Model model;

	public override void _PhysicsProcess(double delta)
	{
		
	}
	

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
		
		model.CollectionChanged += (sender, args) =>

		{
			if (args.Item2 is not Part part) return;
			var partNode = new PartNode(part);
			if (args.Item1)
			{
				//parts.Add(part, partNode);
				AddChild(partNode);
			}
			else
			{
				
				//parts[part].QueueFree();
				//parts.Remove(part);
			}

		};

		model.State.ObjectSelectionChanged += (sender, tuple) =>
		{
			//if (tuple.Item1 is Part part) parts[part].SetSelected(tuple.Item2); 
			
		};
			
		model.State.IsPeekingChanged += (sender, b) =>
		{
			/*foreach (var keyValuePair in parts)
			{
				if (!b)
				{
					if (!model.State.SelectedObjects.Contains(keyValuePair.Key))
					{
						parts[keyValuePair.Key].SetVisibility(true);
						continue;
					}
				}
					
				if (!model.State.SelectedObjects.Contains(keyValuePair.Key))
				{
					parts[keyValuePair.Key].SetVisibility(!b);
				}
					
					
			}*/
		};

		/*model.State.ObjectHoveringChanged += (sender, renderable) =>
		{
			foreach (var keyValuePair in parts)
			{
				keyValuePair.Value.SetHovering(keyValuePair.Key == renderable);
			}
		};*/
		
		/*model.State.ModeChanged += (sender, mode) =>
		{
			
			if (mode == EditorMode.ShapeEdit)
			{
				_editedPart = model.State.SelectedObjects.First() as Part;
				model.State.SelectedObjects.Clear();
				model.State.SelectPart(_editedPart);
				
			}
			else
			{
				//parts[_editedPart].SetBeingEdited(false);
			}
		};*/

		model.State.ModelReloaded += (sender, b) =>
		{
			RebuildAll();
		};
		
		foreach (var modelAllPart in model.AllObjects)
		{
			var newOne = new PartNode(modelAllPart as Part);
			//parts.Add(modelAllPart as Part, newOne);
			
			Callable.From(() =>
			{
				AddChild(newOne); 
				newOne.Owner = this;
			}).CallDeferred();
			
			
		}
	}

	public void RebuildAll()
	{
	
		
		foreach (var modelAllPart in model.AllObjects)
		{
			var newOne = new PartNode(modelAllPart as Part);
			//parts.Add(modelAllPart as Part, newOne);
			
			Callable.From(() =>
			{
				AddChild(newOne); 
				newOne.Owner = this;
			}).CallDeferred();
			
			
		}
	}
	public void RefreshAll()
	{
		
	}

	
	private void OnButtonPressed()
	{
		foreach (var findChild in this.GetChildren())
		{
			RemoveChild(findChild);
		}
	}
	
}
