using System;
using System.Collections.Generic;
using System.Drawing;
using Godot;
using Godot.Collections;
using PinkDogMM_Gd._3D.Tools;
using PinkDogMM_Gd._3D.Tools.Textures;
using Color = Godot.Color;

namespace PinkDogMM_Gd.Core.Actions.All.Tools.Textures;

public abstract class TextureTool : Tool
{
    private System.Collections.Generic.Dictionary<Vector2I, Color> _originalPixels = new();

    public System.Collections.Generic.Dictionary<Vector2I, Color> OriginalPixels
    {
        get => _originalPixels;
        set => _originalPixels = value ?? throw new ArgumentNullException(nameof(value));
    }

    public System.Collections.Generic.Dictionary<Vector2I, Color> AffectedPixels
    {
        get => _affectedPixels;
        set => _affectedPixels = value ?? throw new ArgumentNullException(nameof(value));
    }

    private System.Collections.Generic.Dictionary<Vector2I, Color> _affectedPixels = new();
    
    public static string Path = "Tools/Textures/TextureTool";
    public override string Icon => "icon_pinsel";
    
    public override bool AddToStack => true;
    
    public override bool Sticky() => true;
    public override Type Tool3D() => typeof(TextureTool3D);


    public void SetPixel(Vector2I position)
    {
        var color = Model.State.PaintState.PrimaryColor;
        if (Model.State.CurrentTexture == -1) return;
        var texture = Model.Textures[Model.State.CurrentTexture].Image!;
        var image = texture.GetImage();

        _originalPixels[position] = image.GetPixelv(position);

        image.SetPixelv(position, color);

        _affectedPixels[position] = color;
        
        texture.Update(image);
    }
    public override void Tick(Dictionary arguments)
    {
        
    }

    public override void Undo()
    {
        var texture = Model.Textures[Model.State.CurrentTexture].Image!;
        var image = texture.GetImage();

        foreach (var keyValuePair in _affectedPixels)
        {
            image.SetPixelv(keyValuePair.Key, _originalPixels[keyValuePair.Key]);
        }

        texture.Update(image);
    }

    
}