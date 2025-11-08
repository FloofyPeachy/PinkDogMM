using System.Linq;
using Godot;
using Godot.Collections;
using PinkDogMM_Gd.Core;
using PinkDogMM_Gd.Core.Actions;
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
    private ActionRegistry actionRegistry;
    private Model model;
    private bool _selected;
    private bool _beingEdited;
        public override void _Ready()
    {
        Name = part.Name;
        Scale = new Vector3(0.0625f, 0.0625f, 0.0625f);
        part.PropertyChanged += (sender, args) =>
        {
            UpdateMesh(args.PropertyName.Contains("Size") || args.PropertyName.Contains("Shapebox"));
        };
        
        model = Model.Get(this);
        model.State.TextureChanged += (sender, texture) =>
        {
            UpdateMesh(false);
        };
        SetMesh();
        Scale = new Vector3(0.001f, 0.001f, 0.0001f);
        actionRegistry = GetNode<ActionRegistry>("/root/ActionRegistry");
    }

    public void SetSelected(bool selected)
    {
        this._selected = selected;
        
        ((_outlineMesh.MaterialOverride as StandardMaterial3D)!).AlbedoColor = selected ? Colors.Yellow : Colors.Gray;
        
    }
    public void SetHovering(bool selected)
    {
        if (_selected) return;
        ((_outlineMesh.MaterialOverride as StandardMaterial3D)!).AlbedoColor = selected ? Colors.Orange : Colors.Gray;
        
        
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
    public void UpdateMesh(bool rebuildMesh)
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
            

            PL.I.Debug("Rebuilt mesh for " + part.Name + "!");
        }
        _partMesh.MaterialOverride = new StandardMaterial3D()
        {
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
            AlbedoTexture = (model.HasTextures && model.State.CurrentTexture != -1)
                ? ( model.Textures[model.State.CurrentTexture].Image) : null,
            VertexColorUseAsAlbedo = (model.State.CurrentTexture == -1),
            TextureFilter = BaseMaterial3D.TextureFilterEnum.Nearest,
            CullMode = BaseMaterial3D.CullModeEnum.Disabled,
        };
        _outlineMesh.Name = part.Name + " Outline";
        
        PL.I.Debug("Updated mesh for " + part.Name + "!");
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
        _outlineMesh.GlobalPosition = new Vector3(part.Position.X, -part.Position.Y, part.Position.Z) * 0.0625f;
        _outlineMesh.Scale = Vector3.One * 0.0625f;
        _outlineMesh.MaterialOverride = new StandardMaterial3D()
        {
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
            AlbedoColor = Colors.Gray
        };
        
        
    }

    public override void _PhysicsProcess(double delta)
    {
    
        Position = this.Position.Lerp(new Vector3(part.Position.Z, -part.Position.Y, part.Position.X) * 0.0625f, (float)delta * 16.0f);
        /*if (Input.IsMouseButtonPressed(MouseButton.Left))
        {
            Position = new Vector3(part.Position.Z, -part.Position.Y, part.Position.X) * 0.0625f;
        }*/
        /*else
        {
            /*this.Position.Lerp(new Vector3(-part.Position.X, part.Position.Y, part.Position.Z) * 0.0625f,
                (float)delta * 24.0f);#1#
        }*/
        //Position = new Vector3(part.Position.Z, -part.Position.Y, part.Position.X) * 0.0625f/*this.Position.Lerp(new Vector3(part.Position.Z, -part.Position.Y, part.Position.X) * 0.0625f, (float)delta * 24.0f)*/;
       RotationDegrees = this.RotationDegrees.Lerp(new Vector3(0,0,0), (float)delta * 24.0f);
       this.RotationOrder = EulerOrder.Zyx;
        this.Scale = this.Scale.Lerp(Vector3.One * 0.0625f, (float)delta * 16.0f);
        _outlineMesh.GlobalPosition = _outlineMesh.GlobalPosition.Lerp(part.Position.AsVector3().LHS(), (float)delta * 24.0f);
        
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
        
        _partMesh.CreateConvexCollision();
        
        var child = _partMesh.GetChild<StaticBody3D>(0);
        child.SetMeta("id", part.Id);
        child.InputRayPickable = true;

        child.MouseEntered += () =>
        {
            model.State.Hovering = part;
            ((_outlineMesh.MaterialOverride as StandardMaterial3D)!).AlbedoColor = Colors.Orange;
        };
        child.MouseExited += () =>
        {
            model.State.Hovering = null;
            ((_outlineMesh.MaterialOverride as StandardMaterial3D)!).AlbedoColor = Colors.White;
        };
        child.InputEvent += ChildOnInputEvent;
        
        _partMesh.MaterialOverride = new StandardMaterial3D()
        {
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
            AlbedoTexture = (model.HasTextures && model.State.CurrentTexture != -1)
                ? model.Textures[model.State.CurrentTexture].Image : null,
            VertexColorUseAsAlbedo = model.State.CurrentTexture == -1,
            TextureFilter = BaseMaterial3D.TextureFilterEnum.Nearest,
            CullMode = BaseMaterial3D.CullModeEnum.Disabled,
        };

       
        _outlineMesh.Name = part.Name + "Outline";
        _outlineMesh.Mesh = MeshGenerator.OutlineMeshFromPart(part);
        _outlineMesh.Position = new Vector3(part.Position.X, -part.Position.Y, part.Position.Z) * 0.0625f;
        _outlineMesh.Scale = Vector3.One;
        _outlineMesh.MaterialOverride = new StandardMaterial3D()
        {
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
            VertexColorUseAsAlbedo = false,
        };

        AddChild(_cornerNodes);
        AddChild(_partMesh);
        AddChild(_outlineMesh);
        
        Scale = new Vector3(0.0625f, 0.0625f, 0.0625f);
        PL.I.Info("Generated mesh for " + part.Name + "!");
    }

    private void ChildOnInputEvent(Node camera, InputEvent @event, Vector3 eventPosition, Vector3 normal, long shapeIdx)
    {
        if (@event is InputEventMouseButton click && click.ButtonIndex == MouseButton.Left && !click.Pressed)
        {
            GD.Print(part.Id);
            actionRegistry.Execute("TheModel/SelectPart",
                new Dictionary { { "model", model }, { "id", part.Id} });
            GetViewport().SetInputAsHandled();
        } 
    }
}