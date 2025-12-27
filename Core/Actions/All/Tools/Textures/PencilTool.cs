using System.Collections.Generic;
using Godot.Collections;

namespace PinkDogMM_Gd.Core.Actions.All.Tools.Textures;

public class PencilTool : TextureTool
{
    private int _size = 1;

    public int Size
    {
        get => _size;
        set => _size = value;
    }

    public override void Tick(Dictionary arguments)
    {
         var position = arguments["pos"].AsVector2I();
         SetPixel(position);
        base.Tick(arguments);
    }
}