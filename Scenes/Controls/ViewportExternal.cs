using System.Threading.Tasks;
using Godot;
using PinkDogMM_Gd.Core;
using PinkDogMM_Gd.Core.Schema;

namespace PinkDogMM_Gd.Scenes.Controls;

public partial class ViewportExternal : SubViewport
{
    private Node3D pivotXy;
    private LiteModelNode modelNode;
    private Node3D pivotY;
    private Node3D pivotX;
    private Camera3D camera;
    
    private int textureIdx = -1;
    private Camera3D.ProjectionType projectionType;
    public override void _Ready()
    {
        var model = Model.Get(this);
        
        base._Ready();
        this.Size = new Vector2I(1005, 642);
        
        modelNode = new LiteModelNode();
        modelNode.Name = "LiteModelNode";
        pivotX = GetNode<Node3D>("WorldEnvironment/PivotXY2/PivotY/PivotX");
        pivotY = GetNode<Node3D>("WorldEnvironment/PivotXY2/PivotY");
        camera = GetNode<Camera3D>("WorldEnvironment/PivotXY2/PivotY/PivotX/Camera3D");
        pivotXy = GetNode<Node3D>("WorldEnvironment/PivotXY2");
        this.SetUpdateMode(SubViewport.UpdateMode.Disabled);
        GetNode<WorldEnvironment>("WorldEnvironment").SetMeta("model", model);
        GetNode<WorldEnvironment>("WorldEnvironment").AddChild(modelNode);
        
    }
    

    public void SetCamera(Vector3 position, Vector2 rotation, float zoom, Camera3D.ProjectionType projection)
    {
        pivotX = GetNode<Node3D>("WorldEnvironment/PivotXY2/PivotY/PivotX");
        pivotY = GetNode<Node3D>("WorldEnvironment/PivotXY2/PivotY");
        camera = GetNode<Camera3D>("WorldEnvironment/PivotXY2/PivotY/PivotX/Camera3D");
        pivotXy = GetNode<Node3D>("WorldEnvironment/PivotXY2");
        
        pivotX.Rotation = Vector3.Zero;
        pivotY.Rotation = Vector3.Zero;
        pivotY.Rotate(pivotY.Transform.Basis.Y, rotation.Y);
        pivotX.Rotate(pivotX.Transform.Basis.X,  rotation.X);
        camera.Projection = projectionType;
        camera.Position = projectionType == Camera3D.ProjectionType.Orthogonal ? new Vector3(position.X,position.Y,100) : new Vector3(position.X,position.Y,zoom);
        camera.Size = 10;
        camera.Current = true;
    }

    public async Task<Image> TakeScreenshot(Vector2I size, Vector2 rotation, float zoom, Camera3D.ProjectionType projection = Camera3D.ProjectionType.Perspective, Vector3 position = new Vector3(),SubViewport? viewport = null, int textureIdx = -1)
    {
        this.Size = size;
        viewport ??= this;
        this.projectionType = projection;
        SetCamera(position, rotation, zoom, projection);
        modelNode.Free();
        modelNode = new LiteModelNode();
        var model = Model.Get(this);

        GetNode<WorldEnvironment>("WorldEnvironment").SetMeta("model", model);
        GetNode<WorldEnvironment>("WorldEnvironment").AddChild(modelNode);
        modelNode.BuildMeshes(textureIdx);
        this.SetUpdateMode(SubViewport.UpdateMode.Always);
        await ToSignal(RenderingServer.Singleton, RenderingServerInstance.SignalName.FramePostDraw); 
        this.SetUpdateMode(SubViewport.UpdateMode.Disabled);
        var takeScreenshot = GetTexture().GetImage();
        takeScreenshot.SavePng("result.png");
       
        return takeScreenshot;
    }
}