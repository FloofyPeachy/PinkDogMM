using Godot;
using Godot.Collections;
using PinkDogMM_Gd.Core;
using Vector2 = System.Numerics.Vector2;

namespace PinkDogMM_Gd._3D.Tools.Textures;

public partial class TextureTool3D : Tool3D
{
    private MeshInstance3D guide;


    public override void Selected()
    {
        var color = Colors.Gray;
        color.A = 128f / 256f;
        guide = new MeshInstance3D()
        {
            Mesh = new BoxMesh()
            {
                Size = Vector3.One / 16
            },
            MaterialOverride = new StandardMaterial3D()
            {
                Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
                AlbedoColor = color
            }
        };
        AddChild(guide);
    }

    public override Dictionary Execute()
    {
        guide.QueueFree();
        return new Dictionary();
    }

    public override void MouseClick(Godot.Vector2 position, MouseButton buttonIndex, bool pressed, bool doubl)
    {
        if (!pressed)
        {
            ActionRegistry.Finish();
            return;
        }
        var node = GetNodeAtMouse();
        if (node == null) return;
        var dataTool = new MeshDataTool();
        dataTool.CreateFromSurface((node.Value.Item2.GetParent() as MeshInstance3D)?.Mesh as ArrayMesh, 0);

        var uvs = MeshPickingHelpers.GetUvCoords(dataTool, node.Value.Item2, node.Value.Item3,
            node.Value.Item1);
        if (uvs == null) return;
        
        var realUv = uvs * Model.Textures[Model.State.CurrentTexture].Size;
        var dict = new Dictionary();
        dict.Add("pos", realUv.Value);
        
        ActionRegistry.Tick(dict);
        
    }

    public override void MouseMotion(Godot.Vector2 vector2, MouseButtonMask? buttonMask)
    {
        var node = GetNodeAtMouse();
        guide.Visible = node != null;
        
        if (node == null) return;
        var valueItem3 = (node.Value.Item3 * 32).Round() / 32;
        guide.GlobalPosition =valueItem3;
        GD.Print(valueItem3);
        if ((buttonMask & MouseButtonMask.Left) == 0) return;
        
      
       
        var dataTool = new MeshDataTool();
        dataTool.CreateFromSurface((node.Value.Item2.GetParent() as MeshInstance3D)?.Mesh as ArrayMesh, 0);
   
        var uvs = MeshPickingHelpers.GetUvCoords(dataTool, node.Value.Item2, node.Value.Item3,
            node.Value.Item1);
        if (uvs == null) return;
        
        var realUv = uvs * Model.Textures[Model.State.CurrentTexture].Size;
        var dict = new Dictionary();
        dict.Add("pos", realUv.Value);
        
        ActionRegistry.Tick(dict);
    }
}