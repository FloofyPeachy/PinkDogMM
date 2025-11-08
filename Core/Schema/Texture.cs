using Godot;

namespace PinkDogMM_Gd.Core.Schema;

public partial class Texture(Vector2 size, string name, ImageTexture image): Resource
{
    public Vector2 Size = size;
    public string Name = name;
    public ImageTexture? Image = image;
}