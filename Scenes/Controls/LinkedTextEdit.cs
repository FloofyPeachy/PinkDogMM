using System;
using Godot;

namespace PinkDogMM_Gd.Scenes.Controls;

public partial class LinkedTextEdit : Control
{
	private LineEdit _lineEdit;

	[Export] public Node TargetNodePath;
	[Export] public string TargetProperty;

	private Node _target;

	public override void _Ready()
	{
		_lineEdit = GetNode<LineEdit>("LineEdit");
		_target = TargetNodePath;

		_lineEdit.TextChanged += OnTextChanged;
	}

	private void OnTextChanged(string newText)
	{
		_target?.Set(TargetProperty, newText);
	}

	public override void _Process(double delta)
	{
		if (_target == null) return;
		var value = _target.Get(TargetProperty).ToString();
		if (_lineEdit.Text != value)
			_lineEdit.Text = value;
	}
	
}
