using Godot;
using System;
using PinkDogMM_Gd.Core.Schema;

public partial class TextureMap : VBoxContainer
{
    public Model model;
    public TextureRect texture;
    public CanvasLayer layer;
    public Label InfoLabel;
    public override void _Ready()
    {
        model = Model.Get(this);
        texture = GetNode<TextureRect>("MarginContainer/ScrollZoomView/MarginContainer/TextureRect");
       
        model.State.TextureChanged += (sender, i) =>
        {
            texture.Texture = model.Textures[0].Image;
        };
        texture.Texture = model.Textures.Count != 0 ?  model.Textures[0].Image : null;
    }

    private void StateOnObjectSelectionChanged(object? sender, (Renderable, bool) e)
    {
        if (e.Item1 is Part)
        {
            QueueRedraw();
        }
    }


    
}
