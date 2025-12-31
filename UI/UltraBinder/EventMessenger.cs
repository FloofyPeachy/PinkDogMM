using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Godot;
using Godot.Collections;
using PinkDogMM_Gd.Core;
using PinkDogMM_Gd.Core.Schema;

namespace PinkDogMM_Gd.UltraBinder;

class PropertyBinding()
{
	public string NodeProperty
	{
		get => _nodeProperty;
		set => _nodeProperty = value ?? throw new ArgumentNullException(nameof(value));
	}

	public Node Node
	{
		get => _node;
		set => _node = value ?? throw new ArgumentNullException(nameof(value));
	}

	private string _nodeProperty;
	private Node _node;
}
public partial class ModelPropertyBinding(Node node, string watchedField, string propertyToUpdate) : Node
{
	public Node node;
	public string watchedField;
	public string propertyToUpdate;
}

public partial class EventMessenger : Control
{
	private System.Collections.Generic.Dictionary<string, List<PropertyBinding>> listeners = [];
	private INotifyPropertyChanged _target = null!;
	private bool _settingInit = false;
	public EventMessenger()
	{
		ChildEnteredTree += OnChildEnteredTree;
		typesToProps.Add(typeof(TextEdit), "Text");
		typesToProps.Add(typeof(CheckBox), "ButtonPressed");
		typesToProps.Add(typeof(SpinBox), "Value");
		typesToProps.Add(typeof(OptionButton), "Selected");
	}

	private System.Collections.Generic.Dictionary<Type, string> typesToProps = [];

	public override void _Ready()
	{
		var model = Model.Get(this);


		model.State.SelectedObjects.CollectionChanged += (sender, args) =>
		{
			//_target?.PropertyChanged -= SelectedOnPropertyChanged;
			if (model.State.SelectedObjects.Count == 0)
			{
				_target = null;
				return;
			}
		
			_target = model.State.SelectedObjects[0];
			_target.PropertyChanged += SelectedOnPropertyChanged;
			SetInitial();
		};
		
	

		/*model.State.SelectedObjects.CollectionChanged += (sender, args) =>
		{
			//_target?.PropertyChanged -= SelectedOnPropertyChanged;
			if (model.State.SelectedObjects.Count == 0)
			{
				_target = null;
				return;
			}

			_target = model.State.SelectedObjects[0];
			_target.PropertyChanged += SelectedOnPropertyChanged;
			SetInital();
		};*/
	}


	private void SetInitial()
	{
		_settingInit = true;
		foreach (var keyValuePair in listeners)
		{
			var key = keyValuePair.Key;
			object? value = GetNestedPropertyValue(_target, key);


			foreach (var binding in keyValuePair.Value)
			{
				var controlProp = binding.Node.GetType().GetProperty(binding.NodeProperty);
				if (controlProp == null)
				{
					PL.I.Info("Couldn't update property.");
					return;
				}

				try
				{
					controlProp.SetValue(binding.Node, value);
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
					
				}
			}
		}
		_settingInit = false;
	}

	private void SelectedOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (sender == null) return;
		foreach (var keyValuePair in listeners)
		{
			if (keyValuePair.Key != e.PropertyName) continue;

			var parts = e.PropertyName.Split('.');
			object? value = sender.GetType().GetProperty(e.PropertyName)?.GetValue(sender);
			if (value == null)
			{
				//Try again..differently
				value = sender.GetType().GetProperty(parts[0])?.GetValue(sender)?.GetType().GetProperty(parts[1])
					?.GetValue(sender.GetType().GetProperty(parts[0])?.GetValue(sender));
			}
			foreach (var part in parts)
			{
				if (int.TryParse(part, out int index))
				{
					object? list = sender.GetType().GetProperty(parts[0]).GetValue(sender);
					List<PropertyInfo> matches =
						list.GetType().GetProperties().Where((info => info.Name == "Item")).ToList();
					if (matches.Count == 0 || matches[0].GetIndexParameters().Length <= 0) return;
					PropertyInfo realList = matches[0];

					value = realList.GetValue(list, [index]);
					break;
				}
			}

			foreach (var binding in keyValuePair.Value)
			{
				var controlProp = binding.Node.GetType().GetProperty(binding.NodeProperty);
				if (controlProp == null || value == null)
				{
					PL.I.Info("Couldn't update property.");
					return;
				}

				controlProp.SetValue(binding.Node, value);
			}
		}
	}

	public object? GetNestedPropertyValue(object target, string propertyPath)
	{
		if (target == null || string.IsNullOrEmpty(propertyPath))
			return null;

		var parts = propertyPath.Split('.');
		object? current = target;

		for (var i = 0; i < parts.Length; i++)
		{
			var part = parts[i];
			if (current == null)
				return null;

			if (int.TryParse(part, out int index))
			{
				var list = current as IList;

				if (list != null && list.Count < index)
					return null;

				current = list?[index];
			}
			else
			{
				
				var propInfo = current.GetType().GetProperty(part);
				current = propInfo?.GetValue(current) ?? current;
			}
		}

		return current;
	}

	public void SetNestedPropertyValue(object target, string propertyPath, object? value)
	{
		if (target == null || string.IsNullOrEmpty(propertyPath))
			return;

		var parts = propertyPath.Split('.');
		object? current = target;
		PropertyInfo? currentProp = null;
		// walk to the second-to-last property
		for (int i = 0; i < parts.Length; i++)
		{
			if (current == null)
				return;
			var part = parts[i];
			if (int.TryParse(part, out int index))
			{
				var list2 = current as IList;

				/*if (list2 != null && list2.Count < index)*/
					

					current = list2?[index];
			}
			else
			{
				
				var propInfo = current.GetType().GetProperty(part);
				if (propInfo?.GetValue(current) != null && i != parts.Length - 1)
				{
					current = propInfo?.GetValue(current) ?? current;
				}
			
				
			}
		}

		if (current == null)
			return;

		// get the final property
		if (current is IList list)
		{
			List<PropertyInfo> matches =
				current.GetType().GetProperties().Where((info => info.Name == "Item")).ToList();

			if (matches.Count == 0 || matches[0].GetIndexParameters().Length <= 0) return;
			if (value != null && !current.GetType().GenericTypeArguments[0].IsInstanceOfType(value))
			{
				value = Convert.ChangeType(value, current.GetType().GenericTypeArguments[0]);
			}

			matches[0].SetValue(current, value, [int.Parse(parts.Last())]);

			//list[int.Parse(parts[^1])] = value;
			return;
		}
		else
		{
			var finalProp = current.GetType().GetProperty(parts[^1]);
			if (finalProp == null || !finalProp.CanWrite)
				return;

			// handle type conversion if needed
			if (value != null && !finalProp.PropertyType.IsInstanceOfType(value))
			{
				value = finalProp.PropertyType.IsEnum ? Enum.ToObject(finalProp.PropertyType, value) : Convert.ChangeType(value, finalProp.PropertyType);
			}

			finalProp.SetValue(current, value);
		}
	}

	public void OnChildEnteredTree(Node node)
	{
		Array<Node> children = node.GetChildren();
		if (children.Count == 0)
		{
			//Just connect itself.
			ConnectNode(node);
			return;
		};
		//Try to connect things.

		foreach (var child in children)
		{
			ConnectNode(child);
		}
	}

	private void UpdateTargetProperty(Node node, string watchedField, string propertyToUpdate)
	{
		if (_settingInit) return;
		SetNestedPropertyValue(_target, watchedField, node.GetType().GetProperty(propertyToUpdate)?.GetValue(node));
	}

	private void ConnectNode(Node node)
	{
		if (node.HasMeta("WatchedField"))
		{
			string watchedField = node.GetMeta("WatchedField").AsString();
			string propertyToUpdate = typesToProps[node.GetType()];

			if (!listeners.ContainsKey(watchedField)) listeners.Add(watchedField, []);
			//We might have to do some watching.

			listeners[watchedField].Add(new PropertyBinding()
			{
				Node = node,
				NodeProperty = propertyToUpdate
			});
			if (node is TextEdit)
				node.Connect("text_changed",
					Callable.From(() => UpdateTargetProperty(node, watchedField, propertyToUpdate)));
			if (node is SpinBox)
				node.Connect("value_changed",
					Callable.From((double _) => UpdateTargetProperty(node, watchedField, propertyToUpdate)));
			if (node is CheckBox)
				node.Connect("toggled",
					Callable.From((double _) => UpdateTargetProperty(node, watchedField, propertyToUpdate)));
			if (node is OptionButton)
				node.Connect("item_selected",
					Callable.From((double _) => UpdateTargetProperty(node, watchedField, propertyToUpdate)));
			PL.I.Info(node.GetName() + " bound to " + watchedField);
		}

		Array<Node> children = node.GetChildren();
		if (children.Count == 0) return;
		//Try to connect things.
		foreach (var child in children)
		{
			ConnectNode(child);
		}
	}
}
