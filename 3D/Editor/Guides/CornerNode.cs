using System.Collections.Generic;
using System.Linq;
using Godot;
using PinkDogMM_Gd._3D.Tools;
using PinkDogMM_Gd.Core;

namespace PinkDogMM_Gd.UI.Viewport;

public partial class CornerNode(ShapeEditTool3D tool, int cornerIndex) : Node3D
{
   

    public MeshInstance3D cornerMesh;
    public Label3D Label;
    public bool hovering = false;
    public Color color;
    
    public override void _Ready()
    {
        base._Ready();
        color = Consts.cornerToColor[cornerIndex];
        cornerMesh = new MeshInstance3D();
        cornerMesh.Mesh = new BoxMesh()
        {
            Size = new Vector3(0.02f, 0.02f, 0.02f)
        };
        cornerMesh.MaterialOverride = new StandardMaterial3D()
        {
            AlbedoColor = color
        };
        cornerMesh.CreateConvexCollision();
        cornerMesh.GetChildren().Last().SetMeta("corner", cornerIndex);
        (cornerMesh.GetChildren().Last() as StaticBody3D).InputEvent += (camera, @event, position, normal, idx) =>
        {
            if (@event is InputEventMouseButton)
            {
                tool.CornerClick(cornerIndex, @event.IsPressed());
            }
        };
        
        (cornerMesh.GetChildren().Last() as StaticBody3D).MouseEntered += () =>
        {
            tool.CornerHover(cornerIndex, true);
            hovering = true;
        };
        (cornerMesh.GetChildren().Last() as StaticBody3D).MouseExited += () =>
        {
            tool.CornerHover(cornerIndex, false);
            hovering = false;
        };
        cornerMesh.Rotation = new Vector3(0, 0, 90);

        Label = new Label3D();
        Label.Text = (cornerIndex + 1).ToString();
        Label.FontSize = 32;

        Label.Modulate = color;
        //Label.Rotation = new Vector3(0, -90, 0);
        AddChild(Label);
        AddChild(cornerMesh);
    }

    public override void _PhysicsProcess(double delta)
    {
    
        ((StandardMaterial3D)cornerMesh.MaterialOverride).AlbedoColor =
            ((StandardMaterial3D)cornerMesh.MaterialOverride).AlbedoColor.Lerp(
             hovering ? color.Lightened(0.5f) : color, (float)delta * 24.0f);
    }

    public void SetFocused(bool focused)
    {
        Label.Rotation = new Vector3(0, 0, 0);

        Label.Position = cornerIndex switch
        {
            0 => new Vector3(-0.1f, 0.1f, -0.1f),
            1 => new Vector3(-0.1f, 0.3f, 0.1f),
            2 => new Vector3(0.1f, 0.3f, 0.1f),
            3 => new Vector3(0.1f, 0.1f, -0.1f),
            4 => new Vector3(-0.1f, -0.1f, -0.1f),
            5 => new Vector3(-0.1f, -0.3f, 0.1f),
            6 => new Vector3(0.1f, -0.3f, 0.1f),
            7 => new Vector3(0.1f, -0.1f, -0.1f),
            _ => Label.Position
        };

        if (cornerIndex is 3 or 0 or 7 or 4)
        {
            //Label.Rotation = new Vector3(0, 57, 0);
        }
        cornerMesh.MaterialOverride = new StandardMaterial3D()
        {
            AlbedoColor = focused ? Colors.Yellow : Consts.cornerToColor[cornerIndex]
        };
    }
}