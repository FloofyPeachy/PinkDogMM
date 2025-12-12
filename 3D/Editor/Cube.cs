using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using PinkDogMM_Gd.Core.Configuration;
using PinkDogMM_Gd.Core.Schema;
using PinkDogMM_Gd.Render;
using Projection = PinkDogMM_Gd.Render.Projection;

public partial class Cube : MeshInstance3D
{
    private Model model;
    public override void _Ready()
    {
        List<Color> colors = [
            PDMMTheme.Bottom,
            PDMMTheme.Left,
            PDMMTheme.Right,
            PDMMTheme.Front,
            PDMMTheme.Back,
            PDMMTheme.Top,
        ];
        model = Model.Get(this);
        SurfaceTool st = new SurfaceTool();

        // Begin defining the mesh as triangles
        st.Begin(Mesh.PrimitiveType.Triangles);

     
        // Define the 8 vertices of a unit cube
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(-0.5f, -0.5f, 0.5f),  // 0: Front-bottom-left
            new Vector3(0.5f, -0.5f, 0.5f),   // 1: Front-bottom-right
            new Vector3(0.5f, 0.5f, 0.5f),    // 2: Front-top-right
            new Vector3(-0.5f, 0.5f, 0.5f),   // 3: Front-top-left
            new Vector3(-0.5f, -0.5f, -0.5f), // 4: Back-bottom-left
            new Vector3(0.5f, -0.5f, -0.5f),  // 5: Back-bottom-right
            new Vector3(0.5f, 0.5f, -0.5f),   // 6: Back-top-right
            new Vector3(-0.5f, 0.5f, -0.5f)    // 7: Back-top-left
        };
        List<int[]> faces =
        [
            (int[])[3, 2, 1, 0], (int[])[4, 5, 6, 7], (int[])[0, 1, 5, 4], (int[])[1, 2, 6, 5], (int[])[2, 3, 7, 6], (int[])[4, 7, 3, 0]
        ];
        
        for (var i = 0; i < faces.Count; i++)
        {
            st.SetColor(colors[i]);
            for (int j = 0; j < 4; j++)
            {
                st.AddVertex(vertices[faces[i][j]]);
            }
            st.AddIndex(i * 4);
            st.AddIndex(i * 4 + 1);
            st.AddIndex(i * 4 + 2);
            st.AddIndex(i * 4);
            st.AddIndex(i * 4 + 2);
            st.AddIndex(i * 4 + 3);
        }
        
        st.GenerateNormals();
        st.GenerateTangents();
       
        
        Mesh = st.Commit();
        MaterialOverride = new StandardMaterial3D()
        {
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
            VertexColorUseAsAlbedo = true,
            CullMode = BaseMaterial3D.CullModeEnum.Back,
        };
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        this.Rotation = new Vector3(  -model.State.Camera.Rotation.Y + 1.6f, 0,   model.State.Camera.Rotation.X);
        GetViewport().GetCamera3D().Projection = model.State.Camera.Projection == Projection.Perspective
            ? Camera3D.ProjectionType.Orthogonal
            : Camera3D.ProjectionType.Orthogonal;
    }
}
