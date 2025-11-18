using Godot;
using PinkDogMM_Gd.Core;
using PinkDogMM_Gd.Core.Schema;

namespace PinkDogMM_Gd.UI.Viewport;

public partial class HelpframeNode(Helpframe helpframe): Node3D
{
    private AppState _appState = null!;
    private Sprite3D? _helpframeSprite;
    public override void _Ready()
    {
        _appState = (GetNode("/root/AppState") as AppState)!;
        Position = helpframe.Position.AsVector3();
        Scale = new Vector3(0.0625f, 0.0625f, 0.0625f);
        
        //Create the mesh from the tata!!
        CreateSprite();
        helpframe.PropertyChanged += (sender, args) =>
        {
            Position = helpframe.Position.AsVector3() * 0.0625f;
        };
    }

    public void CreateSprite()
    {
        _helpframeSprite = new Sprite3D();
        _helpframeSprite.Texture = helpframe.Texture.Image;

        var body = new StaticBody3D();
        var collision = new CollisionShape3D();
        var boxShape = new BoxShape3D()
        {
            Size = new Vector3(helpframe.Texture.Size.X, helpframe.Texture.Size.Y, 0.1f)
        };
        
        body.SetMeta("ido", helpframe.Id);
        
        collision.Shape = boxShape;
        body.AddChild(collision);
        _helpframeSprite.AddChild(body);
        AddChild(_helpframeSprite);
    }
}