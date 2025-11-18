using Godot;
using System;
using PinkDogMM_Gd.Core.Schema;

public partial class TexturePanelPopup : PanelContainer
{
    private Model model;
    private bool shown = false;
    public override void _Ready()
    {
        base._Ready();
        model = Model.Get(this);
    }

    public void ButtonPressed()
    {
        GD.Print("Button pressed!");
        var list = GetNode<HBoxContainer>("HBoxContainer");
        shown = !shown;
        list.Visible = shown;

        if (shown)
        {
            /*var defaultPreview = new TexturePreview();
            defaultPreview.texture = null;
            list.AddChild(defaultPreview);
            
            foreach (var modelTexture in model.Textures)
            {
                var texturePreview = new TexturePreview();
                texturePreview.texture = modelTexture;
                list.AddChild(texturePreview);
            }*/
          
        }


    }
}
