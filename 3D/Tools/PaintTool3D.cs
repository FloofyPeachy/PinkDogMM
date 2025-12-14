using Godot;
using PinkDogMM_Gd.Core;
using Vector2 = System.Numerics.Vector2;

namespace PinkDogMM_Gd._3D.Tools;

public partial class PaintTool3D : Tool3D
{
    public override void MouseClick(Godot.Vector2 vector2, MouseButton buttonIndex, bool pressed)
    {
        if (buttonIndex != MouseButton.Left | !pressed) return;
        var node = GetNodeAtMouse();
        if (node == null) return;
        
        var dataTool = new MeshDataTool();
        dataTool.CreateFromSurface((node.Value.Item2.GetParent() as MeshInstance3D)?.Mesh as ArrayMesh, 0);

        var uvs = MeshPickingHelpers.GetUvCoords(dataTool, node.Value.Item2, node.Value.Item3,
            node.Value.Item1);
        
        Model.State.CurrentTexture = Model.Textures.Count - 1;
        if (Model.State.CurrentTexture != -1)
        {
            var img = Model.Textures[Model.State.CurrentTexture].Image!;
            var realUv = uvs! * Model.Textures[Model.State.CurrentTexture]!.Size;
            var image = img.GetImage();
                    
            GD.Print(image.GetPixelv(new Vector2I((int)realUv.Value.X, (int)realUv.Value.Y)));
            image.SetPixelv(new Vector2I((int)realUv.Value.X, (int)realUv.Value.Y), Colors.Black);
            img.Update(image);
        }

    }
}