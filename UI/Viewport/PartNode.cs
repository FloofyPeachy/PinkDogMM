using System.Linq;
using Godot;
using PinkDogMM_Gd.Core;
using PinkDogMM_Gd.Core.Schema;
using PinkDogMM_Gd.Render;

namespace PinkDogMM_Gd.UI.Viewport;

public partial class PartNode(Part part) : Node3D
{
    public Part Part = part;
    private MeshInstance3D? _partMesh;
    private MeshInstance3D? _outlineMesh;
    private StaticBody3D? _staticBody;
    private CornerNodes _cornerNodes;
    private AppState _appState = null!;
    private bool _selected;
    private bool _beingEdited;
        public override void _Ready()
    {
        _appState = (GetNode("/root/AppState") as AppState)!;
        
        Name = part.Name;
        Scale = new Vector3(0.0625f, 0.0625f, 0.0625f);
        part.PropertyChanged += (sender, args) =>
        {
            UpdateMesh(args.PropertyName == "Size" || args.PropertyName.Contains("Shapebox"));
        };
        
       
        SetMesh();
        Scale = new Vector3(0.001f, 0.001f, 0.0001f);
    }

    public void SetSelected(bool selected)
    {
        this._selected = selected;
        
        ((_outlineMesh.MaterialOverride as StandardMaterial3D)!).AlbedoColor = selected ? Colors.Yellow : Colors.Gray;
        
    }

    public void SetBeingEdited(bool edited)
    {
        this._beingEdited = edited;
        _cornerNodes.Visible = edited;

    }

    public void SetVisibility(bool visible)
    {
        /*var color = ((outlineMesh.MaterialOverride as StandardMaterial3D)!).AlbedoColor;
        color.A = 0.5f;
        ((this.MaterialOverride as StandardMaterial3D)!).AlbedoColor = color;*/
        
        _partMesh.Visible = visible;
    }
    private void UpdateMesh(bool rebuildMesh)
    {
        if (rebuildMesh)
        {
            _partMesh.Mesh = MeshGenerator.MeshFromPart(part, new Vector2(512, 512));
            _outlineMesh.Mesh = MeshGenerator.OutlineMeshFromPart(part);
            
            _partMesh.RemoveChild(_staticBody);
            _partMesh.CreateConvexCollision();

            var child = _partMesh.GetChildren().Last() as StaticBody3D;
            child!.SetMeta("id", part.Id);
            _staticBody = child;
            GD.Print("Rebuilt mesh for " + part.Name + "!");
        }
      
        _outlineMesh.Name = part.Name + " Outline";
        
        GD.Print("Updated mesh for " + part.Name + "!");
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
        _outlineMesh = new MeshInstance3D();
        _outlineMesh.Name = part.Name + " Outline";
        _outlineMesh.Mesh = MeshGenerator.OutlineMeshFromPart(part);
        _outlineMesh.GlobalPosition = new Vector3(part.Position.Z, -part.Position.Y, part.Position.X) * 0.0625f;
        _outlineMesh.Scale = Vector3.One * 0.0625f;
        _outlineMesh.MaterialOverride = new StandardMaterial3D()
        {
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
            AlbedoColor = Colors.Gray
        };
        
        
    }

    public override void _PhysicsProcess(double delta)
    {
        this.Scale = this.Scale.Lerp(Vector3.One * 0.0625f, (float)delta * 16.0f);
        Position = this.Position.Lerp(new Vector3(part.Position.Z, -part.Position.Y, part.Position.X) * 0.0625f, (float)delta * 24.0f);
        _outlineMesh.Position = _outlineMesh.GlobalPosition.Lerp(new Vector3(part.Position.X, -part.Position.Y, part.Position.Z) * 0.0625f, (float)delta * 24.0f);
    }

    private void SetMesh()
    {
        _partMesh = new MeshInstance3D();
        _outlineMesh = new MeshInstance3D();
        _cornerNodes = new CornerNodes(part)
        {
            Visible = false
        };
        _partMesh.Mesh = MeshGenerator.MeshFromPart(part, new Vector2(512, 512));
        
        Position = new Vector3(part.Position.Z, -part.Position.Y, part.Position.X) * 0.0625f;
        _partMesh.CreateConvexCollision();
        
        var child = _partMesh.GetChild<StaticBody3D>(0);
        child.SetMeta("id", part.Id);
        child.InputRayPickable = true;
       
        _partMesh.MaterialOverride = new StandardMaterial3D()
        {
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
            AlbedoTexture = _appState.ActiveModel.HasTextures
                ? _appState.ActiveModel.Textures["Default"].Image
                : null,
            VertexColorUseAsAlbedo = !_appState.ActiveModel.HasTextures,
            TextureFilter = BaseMaterial3D.TextureFilterEnum.Nearest,
            CullMode = BaseMaterial3D.CullModeEnum.Disabled,
        };

       
        _outlineMesh.Name = part.Name + "Outline";
        _outlineMesh.Mesh = MeshGenerator.OutlineMeshFromPart(part);
        _outlineMesh.Position = new Vector3(part.Position.X, -part.Position.Y, -part.Position.Z) * 0.0625f;
        _outlineMesh.Scale = Vector3.One;
        _outlineMesh.MaterialOverride = new StandardMaterial3D()
        {
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
            VertexColorUseAsAlbedo = true,
        };

        AddChild(_cornerNodes);
        AddChild(_partMesh);
        AddChild(_outlineMesh);
        
        Scale = new Vector3(0.0625f, 0.0625f, 0.0625f);
        GD.Print("Generated mesh for " + part.Name + "!");
    }
}