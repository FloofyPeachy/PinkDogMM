using Godot;
using System;
using PinkDogMM_Gd.Scenes.Controls;
using Texture = PinkDogMM_Gd.Core.Schema.Texture;

[Tool]
public partial class TexturePreview(Texture? texture, ViewportExternal vp) : VBoxContainer
{
    
    

    public override async void _Ready()
    {
        base._Ready();

        var textureRect = new TextureRect()
        {
            Texture = ImageTexture.CreateFromImage(await vp.TakeScreenshot(new Vector2I((int)this.Size.X, (int)this.Size.Y), Vector2.Zero, 10, projection: Camera3D.ProjectionType.Orthogonal)),
            ExpandMode = TextureRect.ExpandModeEnum.FitHeightProportional
        };
        AddChild(textureRect);
           AddChild(new Label()
           {
               Text = texture?.Name ?? "None",
           });
           var button = new Button();
           button.Pressed += async () =>
           {
               textureRect.Texture = ImageTexture.CreateFromImage(await vp.TakeScreenshot(new Vector2I((int)this.Size.X, (int)this.Size.Y), new Vector2(-0.5f,-0.5f), 20,
                   projection: Camera3D.ProjectionType.Orthogonal, position: new Vector3(0,0.5f, 0)));
               
           };
           AddChild(button);

           (await vp.TakeScreenshot(new Vector2I((int)this.Size.X, (int)this.Size.Y), Vector2.Zero, 10, projection: Camera3D.ProjectionType.Orthogonal)).SavePng("deeznuts.png");
        
     
    }

    public override void _EnterTree()
    {
        base._EnterTree();
    }

    public void CreatePreview()
    {
        
        var vp = GetNode("..//").GetParent().GetNode("../").GetParent().GetNode("../").GetParent().GetParent().GetNode<ViewportExternal>("ViewportExternal");
        //GetNode<TextureRect>("TextureRect").Texture = ImageTexture.CreateFromImage(vp.TakeScreenshot(Vector2.Zero, 20));
    }
}
