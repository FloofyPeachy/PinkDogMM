namespace PinkDogMM_Gd.Core.Schema;

public class Helpframe(Texture texture) : Renderable
{
    private Texture texture = texture;

    public Texture Texture
    {
        get => texture;
        set => SetField(ref texture, value);
    }
}