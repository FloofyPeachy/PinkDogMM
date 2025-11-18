using System.Collections.Generic;
using System.Linq;
using Godot;
using PinkDogMM_Gd.Core;

namespace PinkDogMM_Gd.UI.Viewport;

public partial class CornerNode(int cornerIndex) : Node3D
{
   

    public MeshInstance3D CornerMesh;
    public Label3D Label;

    public override void _Ready()
    {
        base._Ready();
        CornerMesh = new MeshInstance3D();
        CornerMesh.Mesh = new BoxMesh()
        {
            Size = new Vector3(0.2f, 0.2f, 0.2f)
        };
        CornerMesh.MaterialOverride = new StandardMaterial3D()
        {
            AlbedoColor = Consts.cornerToColor[cornerIndex]
        };
        CornerMesh.CreateConvexCollision();
        CornerMesh.GetChildren().Last().SetMeta("corner", cornerIndex);
        CornerMesh.Rotation = new Vector3(0, 0, 90);

        Label = new Label3D();
        Label.Text = (cornerIndex + 1).ToString();
        Label.FontSize = 128;

        Label.Modulate = Consts.cornerToColor[cornerIndex];
        //Label.Rotation = new Vector3(0, -90, 0);
        AddChild(Label);
        AddChild(CornerMesh);
    }

    public void SetFocused(bool focused)
    {
        Label.Rotation = new Vector3(0, 0, 0);

        Label.Position = cornerIndex switch
        {
            0 => new Vector3(-0.3f, 0.3f, -0.3f),
            1 => new Vector3(-0.3f, 0.5f, 0.3f),
            2 => new Vector3(0.3f, 0.5f, 0.3f),
            3 => new Vector3(0.3f, 0.3f, -0.3f),
            4 => new Vector3(-0.3f, -0.3f, -0.3f),
            5 => new Vector3(-0.3f, -0.5f, 0.3f),
            6 => new Vector3(0.3f, -0.5f, 0.3f),
            7 => new Vector3(0.3f, -0.3f, -0.3f),
            _ => Label.Position
        };

        if (cornerIndex is 3 or 0 or 7 or 4)
        {
            //Label.Rotation = new Vector3(0, 57, 0);
        }
        CornerMesh.MaterialOverride = new StandardMaterial3D()
        {
            AlbedoColor = focused ? Colors.Yellow : Consts.cornerToColor[cornerIndex]
        };
    }
}