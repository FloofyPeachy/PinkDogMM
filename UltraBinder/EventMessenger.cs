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

public partial class EventMessenger : Node
{
	private System.Collections.Generic.Dictionary<string, List<PropertyBinding>> listeners = [];
	private INotifyPropertyChanged _target = null!;

	public EventMessenger()
	{
		ChildEnteredTree += OnChildEnteredTree;
		typesToProps.Add(typeof(TextEdit), "Text");
		typesToProps.Add(typeof(SpinBox), "Value");
		typesToProps.Add(typeof(OptionButton), "Selected");
	}

	private System.Collections.Generic.Dictionary<Type, string> typesToProps = [];

	public override void _Ready()
	{
		var model = Model.Get(this);


		model.State.SelectedParts.CollectionChanged += (sender, args) =>
		{
			//_target?.PropertyChanged -= SelectedOnPropertyChanged;
			if (model.State.SelectedParts.Count == 0)
			{
				_target = null;
				return;
			}

			_target = model.State.SelectedParts[0];
			_target.PropertyChanged += SelectedOnPropertyChanged;
			SetInital();
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


	private void SetInital()
	{
		foreach (var keyValuePair in listeners)
		{
			var key = keyValuePair.Key;
			object? value = GetNestedPropertyValue(_target, key);


			foreach (var binding in keyValuePair.Value)
			{
				var controlProp = binding.Node.GetType().GetProperty(binding.NodeProperty);
				if (controlProp == null)
				{
					GD.Print("Couldn't update property.");
					return;
				}

				controlProp.SetValue(binding.Node, value);
			}
		}
	}

	private void SelectedOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (sender == null) return;
		foreach (var keyValuePair in listeners)
		{
			if (keyValuePair.Key != e.PropertyName) continue;

			var parts = e.PropertyName.Split('.');
			object? value = sender.GetType().GetProperty(e.PropertyName)?.GetValue(e.PropertyName);

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
					GD.Print("Couldn't update property.");
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

		foreach (var part in parts)
		{
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
				if (propInfo == null)
					return null;

				current = propInfo.GetValue(current);
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

		// walk to the second-to-last property
		for (int i = 0; i < parts.Length - 1; i++)
		{
			if (current == null)
				return;

			var propInfo = current.GetType().GetProperty(parts[i]);
			if (propInfo == null)
				return;
			current = propInfo.GetValue(current);
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
				value = Convert.ChangeType(value, finalProp.PropertyType);
			}

			finalProp.SetValue(current, value);
		}
	}

	public void OnChildEnteredTree(Node node)
	{
		Array<Node> children = node.GetChildren();
		if (children.Count == 0) return;
		//Try to connect things.

		foreach (var child in children)
		{
			ConnectNode(child);
		}
	}

	private void UpdateTargetProperty(Node node, string watchedField, string propertyToUpdate)
	{
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

			GD.Print(this.GetName() + " bound to " + watchedField);
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
