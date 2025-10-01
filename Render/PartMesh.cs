using System.Linq;
using Godot;
using PinkDogMM_Gd.Core;
using PinkDogMM_Gd.Core.Schema;

namespace PinkDogMM_Gd.Render;

public partial class PartMesh(Part part) : MeshInstance3D
{
    public Part part = part;
    private MeshInstance3D outlineMesh;
    private StaticBody3D staticBody;
    private PartManipulator? _manipulator;
    
    private AppState appState;
    
    private bool selected;
    
    
    /*MeshInstance3D mesh = new MeshInstance3D()
                {
                    Name = part.Name,
                    Mesh = MeshGenerator.MeshFromPart(part,
                        appState.ActiveModel.HasTextures
                            ? appState.ActiveModel.Textures["Default"].Size
                            : new Vector2()),
                    Scale = new Vector3(0.0625f, 0.0625f, 0.0625f),
                    Position = new Vector3(part.Position.Z, -part.Position.Y, part.Position.X) * 0.0625f
                };

                mesh.CreateConvexCollision();
                var child = mesh.GetChild<StaticBody3D>(0);
                child.SetMeta("id", part.Id);

                mesh.MaterialOverride = new StandardMaterial3D()
                {
                    ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
                    AlbedoTexture = appState.ActiveModel.HasTextures
                        ? appState.ActiveModel.Textures["Default"].Image
                        : null,
                    VertexColorUseAsAlbedo = !appState.ActiveModel.HasTextures,
                    TextureFilter = BaseMaterial3D.TextureFilterEnum.Nearest,
                    CullMode = BaseMaterial3D.CullModeEnum.Disabled,
                };


                //Outline too!!!
                MeshInstance3D outlineMesh = new MeshInstance3D()
                {
                    Name = part.Name + "Outline",
                    Mesh = MeshGenerator.OutlineMeshFromPart(part),
                    Scale = new Vector3(0.0625f, 0.0625f, 0.0625f),
                    Position = new Vector3(part.Position.Z, -part.Position.Y, part.Position.X) * 0.0625f
                };
                outlineMesh.MaterialOverride = new StandardMaterial3D()
                {
                    ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
                    VertexColorUseAsAlbedo = true,
                };

                AddChild(outlineMesh);
                /*var shader = new ShaderMaterial();
                shader.Shader = GD.Load<Shader>("res://Assets/outline.gdshader");
                shader.Set("color", Colors.Brown);

                mesh.MaterialOverride = shader;#1#
                AddChild(mesh);

                meshes.Add(mesh);*/
    public override void _Ready()
    {
        appState = (GetNode("/root/AppState") as AppState)!;
        
        Name = part.Name;
        Scale = new Vector3(0.0625f, 0.0625f, 0.0625f);
        part.PropertyChanged += (sender, args) =>
        {
            UpdateMesh(args.PropertyName == "Size");
        };
        //manipulator = new PartManipulator(this);
       // AddChild(_manipulator);
       
        SetMesh();
        
    }

    public void SetSelected(bool selected)
    {
        this.selected = selected;
        ((outlineMesh.MaterialOverride as StandardMaterial3D)!).AlbedoColor = selected ? Colors.Yellow : Colors.Gray;
        if (selected)
        {
            //manipulator.Visible = true;
        }
        else
        {
          //  _manipulator.Visible = false;
        }
    }

    public void SetVisibility(bool visible)
    {
        /*var color = ((outlineMesh.MaterialOverride as StandardMaterial3D)!).AlbedoColor;
        color.A = 0.5f;
        ((this.MaterialOverride as StandardMaterial3D)!).AlbedoColor = color;*/
        Visible = visible;
    }
    private void UpdateMesh(bool rebuildMesh)
    {
        if (rebuildMesh)
        {
            Mesh = MeshGenerator.MeshFromPart(part, new Vector2(512, 512));
            outlineMesh.Mesh = MeshGenerator.OutlineMeshFromPart(part);
            RemoveChild(staticBody);
            CreateConvexCollision();

            var child = GetChildren().Last() as StaticBody3D;
            child!.SetMeta("id", part.Id);
            staticBody = child;
            PL.I.Info("Rebuilt mesh for " + part.Name + "!");
        }
        Position = new Vector3(part.Position.Z, -part.Position.Y, part.Position.X) * 0.0625f;
        outlineMesh.GlobalPosition = new Vector3(part.Position.X, -part.Position.Y, -part.Position.Z) * 0.0625f;
        outlineMesh.Name = part.Name + " Outline";
        
        PL.I.Info("Updated mesh for " + part.Name + "!");
    }
    /*
    private void CreateMesh()
    {
        Mesh = MeshGenerator.MeshFromPart(part, new Vector2(512, 512));
        
        Position = new Vector3(part.Position.Z, -part.Position.Y, part.Position.X) * 0.0625f;
        CreateConvexCollision();
        
        var child = GetChild<StaticBody3D>(0);
        child.SetMeta("id", part.Id);
        staticBody = child;
        
        MaterialOverride = new StandardMaterial3D()
        {
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
            AlbedoTexture = appState.ActiveModel.HasTextures
                ? appState.ActiveModel.Textures["Default"].Image
                : null,
            VertexColorUseAsAlbedo = !appState.ActiveModel.HasTextures,
            TextureFilter = BaseMaterial3D.TextureFilterEnum.Nearest,
            CullMode = BaseMaterial3D.CullModeEnum.Disabled,
        };

        CreateOutlineMesh();

        AddChild(outlineMesh);
        
    }
    */

    private void CreateOutlineMesh()
    {
        outlineMesh = new MeshInstance3D();
        outlineMesh.Name = part.Name + " Outline";
        outlineMesh.Mesh = MeshGenerator.OutlineMeshFromPart(part);
        outlineMesh.GlobalPosition = new Vector3(part.Position.Z, -part.Position.Y, part.Position.X) * 0.0625f;
        outlineMesh.Scale = Vector3.One * 0.0625f;
        outlineMesh.MaterialOverride = new StandardMaterial3D()
        {
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
            AlbedoColor = Colors.Gray
        };
        
        
    }

    public override void _PhysicsProcess(double delta)
    {
        this.Scale = this.Scale.Lerp(Vector3.One * 0.0625f, (float)delta * 16.0f);
    }

    private void SetMesh()
    {
        PL.I.Info("Generated mesh for " + part.Name + "!");
        Mesh = MeshGenerator.MeshFromPart(part, new Vector2(512, 512));
        
        Position = new Vector3(part.Position.Z, -part.Position.Y, part.Position.X) * 0.0625f;
        CreateConvexCollision();
        
        var child = GetChild<StaticBody3D>(0);
        child.SetMeta("id", part.Id);
        child.InputRayPickable = true;
        
        /*child.CollisionLayer = 0;
        child.InputEvent += (camera, @event, position, normal, idx) =>
        {
            PL.I.Info("clicked!!");
        };*/
        
        MaterialOverride = new StandardMaterial3D()
        {
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
            AlbedoTexture = appState.ActiveModel.HasTextures
                ? appState.ActiveModel.Textures["Default"].Image
                : null,
            VertexColorUseAsAlbedo = !appState.ActiveModel.HasTextures,
            TextureFilter = BaseMaterial3D.TextureFilterEnum.Nearest,
            CullMode = BaseMaterial3D.CullModeEnum.Disabled,
        };

        outlineMesh = new MeshInstance3D();
        outlineMesh.Name = part.Name + "Outline";
        outlineMesh.Mesh = MeshGenerator.OutlineMeshFromPart(part);
        outlineMesh.Position = new Vector3(part.Position.X, -part.Position.Y, part.Position.Z) * 0.0625f;
        outlineMesh.Scale = Vector3.One;
        outlineMesh.MaterialOverride = new StandardMaterial3D()
        {
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
            VertexColorUseAsAlbedo = true,
        };
        
        AddChild(outlineMesh);
        this.Scale = new Vector3(0.1f, 0.1f, 0.1f);
        
    }
}