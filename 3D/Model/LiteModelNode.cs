using Godot;
using PinkDogMM_Gd.Core;
using PinkDogMM_Gd.Core.Schema;
using PinkDogMM_Gd.Render;

namespace PinkDogMM_Gd.Scenes;

public partial class LiteModelNode : Node3D
{
    private Model model;
    

    
    public void BuildMeshes(int textureIdx = -1)
    {
        model = Model.Get(this);
        foreach (var child in GetChildren(true))
        {
            child.Free();
        }
        GlobalRotationDegrees = new Vector3(0, 90, 0);
        foreach (var modAllPt in model.AllObjects)
        {
            var part = modAllPt as Part;
            AddChild(CreatePartMesh(part, textureIdx));
        }
    }
     private MeshInstance3D CreatePartMesh(Part part, int textureIdx)
    {
        var partMesh = new MeshInstance3D();
        partMesh.Mesh = MeshGenerator.MeshFromPart(part, new Vector2(512, 512));
        GlobalPosition = new Vector3(-part.Position.Z, -part.Position.Y, -part.Position.X) * 0.0625f;
        
        partMesh.MaterialOverride = new StandardMaterial3D()
        {
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
            AlbedoTexture = (model.HasTextures && textureIdx != -1)
                ? model.Textures[textureIdx].Image : null,
            VertexColorUseAsAlbedo = textureIdx == -1,
            TextureFilter = BaseMaterial3D.TextureFilterEnum.Nearest,
            CullMode = BaseMaterial3D.CullModeEnum.Disabled,
        };
        Scale = new Vector3(0.0625f, 0.0625f, 0.0625f);
        PL.I.Info("Generated lite mesh for " + part.Name + "!");
        return partMesh;
    }

    public void ManualRefresh()
    {
        
    }
    

}