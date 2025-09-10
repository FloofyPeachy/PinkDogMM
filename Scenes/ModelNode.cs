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

	public override void _Ready()
	{
		appState = GetNode("/root/AppState") as AppState;
		Debug.Assert(appState != null, nameof(appState) + " != null");

		appState.ActiveModelChanged += (index) =>
		{
			List<Part> unbuiltParts = appState.ActiveModel!.AllParts.Where((part => !parts.ContainsKey(part))).ToList();
			
				//meshes.Where((mesh => appState.ActiveModel.GetPartById(mesh.part.Id) == null)).ToList();
			foreach (var part in unbuiltParts) {
				PartNode newOne = new PartNode(part);
				parts.Add(part, newOne);
				AddChild(newOne);
			}

			List<KeyValuePair<Part, PartNode>> deezParts = parts.Where((pair => !appState.ActiveModel.AllParts.Contains(pair.Key))).ToList();
			foreach (var mesh in parts)
			{
				mesh.Value.Visible = appState.ActiveModel.AllParts.Contains(mesh.Key);
			}

			//appState.ActiveModel.PartGroups.NestedItemChanged += (sender, args) => BuildVisuals();
			appState.ActiveModel.PartGroups.NestedCollectionChanged += (sender, args) =>
			{
				if (args.Item2.Added)
				{
					PartNode newOne = new PartNode(args.Item2.Item as Part);
					parts.Add(args.Item2.Item as Part, newOne);
					AddChild(newOne);
				}
				
			};

			appState.ActiveModel.Helpers.ItemsChanged += (sender, changed) =>
			{
				GetParent().AddChild(new HelpframeNode(changed.Item as Helpframe ?? throw new InvalidOperationException()));
			};
			
			appState.PartSelected += (sender, part) =>
			{
				parts[part].SetSelected(true);
			};
		
			appState.PartUnselected += (sender, part) =>
			{
				parts[part].SetSelected(false);
			};

			appState.AllPartsUnselected += (sender, args) =>
			{
				foreach (var keyValuePair in parts)
				{
					parts[keyValuePair.Key].SetSelected(false);	
				}
				
			};

			appState.IsPeekingChanged += (sender, b) =>
			{
				foreach (var keyValuePair in parts)
				{
					if (!b)
					{
						if (!appState.ActiveEditorState.SelectedParts.Contains(keyValuePair.Key))
						{
							parts[keyValuePair.Key].SetVisibility(true);
							continue;
						}
					}
					
					if (!appState.ActiveEditorState.SelectedParts.Contains(keyValuePair.Key))
					{
						parts[keyValuePair.Key].SetVisibility(!b);
					}
					
					
				}
			};

			appState.ModeChanged += (sender, mode) =>
			{
			
				if (mode == EditorMode.ShapeEdit)
				{
					_editedPart = appState.ActiveEditorState.SelectedParts.First() as Part;
					appState.ActiveEditorState.SelectedParts.Clear();
					appState.ActiveEditorState.SelectPart(_editedPart);
					parts[_editedPart].SetBeingEdited(true);
				}
				else
				{
					parts[_editedPart].SetBeingEdited(false);
				}
			};
			//appState.ActiveModel.PartGroups.ItemChanged += (in NotifyCollectionChangedEventArgs<KeyValuePair<string, BubblingObservableList<Part>>> args) => BuildVisuals();
			//appState.ActiveModel.PartGroups.ItemChanged += i => BuildVisuals();
		};
	}

	private void OnButtonPressed()
	{
		foreach (var findChild in this.GetChildren())
		{
			RemoveChild(findChild);
		}
	}
	
}
