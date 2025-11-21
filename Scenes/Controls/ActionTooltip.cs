using Godot;
using System;

public partial class ActionTooltip : Control
{
    [Export] public string Title;
    [Export] public string Description;
    [Export] public string Icon;

    public override void _Ready()
    {
        GetNode<Label>("VBoxContainer/HBoxContainer/Title").Text = Title;
        GetNode<TextureRect>("VBoxContainer/HBoxContainer/TextureRect").Texture = GD.Load<Texture2D>(
            $"res://Assets/placeholders/{Icon}.png");
        GetNode<Label>("VBoxContainer/Description").Text = Description;
    }

    public void Setup(string title, string description, string icon)
    {
        Title = title;
        Description = description;
        Icon = icon;
    }
}
