using Godot;

namespace PinkDogMM_Gd.Core.Schema;

public class Texture(Vector2 size, ImageTexture image)
{
    public Vector2 Size = size;
    public ImageTexture? Image = image;
}