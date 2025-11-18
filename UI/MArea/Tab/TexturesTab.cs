using Godot;
using System;
using PinkDogMM_Gd.Core;
using PinkDogMM_Gd.Core.Schema;
using PinkDogMM_Gd.Scenes.Controls;

public partial class TexturesTab : PanelContainer
{
    private Model? model;
    private VBoxContainer scrollCon;
    public override void _Ready()
    {
        base._Ready();
        model = Model.Get(this);
        scrollCon = GetNode<VBoxContainer>("ScrollContainer/VBoxContainer");
        GetParent().GetParent().GetParent().GetParent().GetParent()
            .GetNode<ViewportExternal>("ViewportExternal")._Ready();
        scrollCon.AddChild(new TexturePreview(null,
            GetParent().GetParent().GetParent().GetParent().GetParent()
                .GetNode<ViewportExternal>("ViewportExternal")));
        
        for (var index = 0; index < model.Textures.Count; index++)
        {
            var modelTexture = model.Textures[index];
            scrollCon.AddChild(new TexturePreview(modelTexture,
                GetParent().GetParent().GetParent().GetParent().GetParent()
                    .GetNode<ViewportExternal>("ViewportExternal")));
        }
       
     
        

        GD.Print("g");
    }
}
