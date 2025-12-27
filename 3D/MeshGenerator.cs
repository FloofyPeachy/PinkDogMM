using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using PinkDogMM_Gd.Core;
using PinkDogMM_Gd.Core.Configuration;
using PinkDogMM_Gd.Core.Schema;

namespace PinkDogMM_Gd.Render;

public class MeshGenerator
{
    static Random rnd = new Random();

    //red (204, 51, 51)
    //red 2 (143, 36, 36)
    //green (57, 198, 57)
    //green 2 (40, 139, 40)
    //blue (51, 51, 204)
    //blue 2 (36, 36, 143)
    public static ArrayMesh MeshFromPart(Part part, Vector2 textureSize)
    {
        List<Color> colors =
        [
            PDMMTheme.Front,
            PDMMTheme.Back,
            PDMMTheme.Top,
            PDMMTheme.Bottom,
            PDMMTheme.Left,
            PDMMTheme.Right
        ];
        textureSize = new Vector2(512, 512);
        List<Vector3> vectors = CreatePartVectors(part);
        List<Vector2> uvs = GenerateCubeUVs(part, textureSize).ToList();
        List<(Vector3, Vector2)> vertexes = [];
        for (var i = 0; i < vectors.Count; i++)
        {
            vertexes.Add((vectors[i], uvs[i]));
        }

        List<Vector3> outputVex = [];


        var st = new SurfaceTool();
        st.Begin(Mesh.PrimitiveType.Triangles);
        st.SetSmoothGroup(UInt32.MaxValue);


        var enumerator = vertexes.Chunk(4).GetEnumerator();
        var min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        var max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        using (enumerator)
        {
            int sideIdx = -1;
            while (enumerator.MoveNext())
            {
                sideIdx++;
                Vector2[] uvFlip =
                [
                    new Vector2(1.0f, 1.0f),
                    new Vector2(1.0f, 1.0f),
                    new Vector2(-1.0f, -1.0f),
                    new Vector2(1.0f, 1.0f),
                    new Vector2(1.0f, 1.0f),
                    new Vector2(1.0f, 1.0f),
                ];
                (Vector3, Vector2)[] side = enumerator.Current;

               
                List<(Vector3, Vector2)> tri1 = [side[0], side[1], side[2]];
                List<(Vector3, Vector2)> tri2 = [side[0], side[2], side[3]];

                foreach (var vector3 in tri2.Concat(tri1))
                {
                    st.SetColor(colors[sideIdx]);
                    var uv = vector3.Item2;
                    var realUv = sideIdx switch
                    {
                        2 => new Vector2(uv.X, uv.Y),
                        /*1 => new Vector2(1f - uv.X, 1f - uv.Y),
                        2 => new Vector2(1f - uv.Y, uv.X),*/
                        _ => uv,
                    };
                    st.SetUV(realUv);
                    st.AddVertex(vector3.Item1);
                    var globalV = part.Position.AsVector3() * vector3.Item1;
                    min = min.Min(globalV);
                    max = max.Max(globalV);
                }
                
            }
        }


        /*
        for (var index = 0; index < vertexes.Length; index++)
        {
            var vector3 = vertexes[index];
            //var nVector3 = vertexData[index + 1];
            st.AddVertex(vector3);

        }*/
        part.Bounding = new Aabb(min, max - min);

        st.GenerateNormals();

        // st.GenerateTangents();

        return st.Commit();
        /*/*var cube = new BoxMesh();
        cube.Size = new Vector3(part.Size.X, part.Size.Y, part.Size.Z);#1#
        var mesh = new ArrayMesh();

        var meshData = new MeshDataTool();

        mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, cube.GetMeshArrays());
        meshData.CreateFromSurface(mesh, 0);
        /*List<Vector3> offsets = [];
        if (part is Shapebox shapebox)
        {
            offsets.Add(new Vector3(shapebox.ShapeboxX[0], shapebox.ShapeboxY[0], shapebox.ShapeboxZ[0]));
            offsets.Add(new Vector3(shapebox.ShapeboxX[1], shapebox.ShapeboxY[1], shapebox.ShapeboxZ[1]));
            offsets.Add(new Vector3(shapebox.ShapeboxX[2], shapebox.ShapeboxY[2], shapebox.ShapeboxZ[2]));
            offsets.Add(new Vector3(shapebox.ShapeboxX[3], shapebox.ShapeboxY[3], shapebox.ShapeboxZ[3]));
            offsets.Add(new Vector3(shapebox.ShapeboxX[4], shapebox.ShapeboxY[4], shapebox.ShapeboxZ[4]));
            offsets.Add(new Vector3(shapebox.ShapeboxX[5], shapebox.ShapeboxY[5], shapebox.ShapeboxZ[5]));
            offsets.Add(new Vector3(shapebox.ShapeboxX[6], shapebox.ShapeboxY[6], shapebox.ShapeboxZ[6]));
            offsets.Add(new Vector3(shapebox.ShapeboxX[7], shapebox.ShapeboxY[7], shapebox.ShapeboxZ[7]));

        }

        List<float> offsetRaw = [];
        foreach (var vector3 in offsets)
        {
            offsetRaw.Add(vector3.X);
            offsetRaw.Add(vector3.Y);
            offsetRaw.Add(vector3.Z);
        }

        int[] boxMeshToCorner = new int[]
        {
            8, 9, 10, 11,    // Front
            12, 13, 14, 15,  // Back
            16, 17, 18, 19,  // Left
            20, 21, 22, 23,  // Right
            4, 5, 6, 7,      // Top
            0, 1, 2, 3
        };

        Color[] cornerColors = new Color[8]; // colors per corner

// Example colors
        cornerColors[0] = Colors.Red;
        cornerColors[1] = Colors.Green;
        cornerColors[2] = Colors.Blue;
        cornerColors[3] = Colors.Yellow;
        cornerColors[4] = Colors.Magenta;
        cornerColors[5] = Colors.Cyan;
        cornerColors[6] = Colors.Orange;
        cornerColors[7] = Colors.Purple;


        var colors = new Color[meshData.GetVertexCount()];#1#
        /*for (int i = 0; i < meshData.GetFaceCount(); i++)
        {
            for (int i2 = 0; i2 < 6; i2++)
            {
                int the = meshData.GetFaceVertex(i, i2);
                Vector3 thet = meshData.GetVertex(the);
                if (part is Shapebox)
                {
                    toSet += new Vector3(offsets[i].X,offsets[i].Y + 1, offsets[i].Z + 2 );
                    meshData.SetVertex(meshData.GetFaceVertex(), toSet);
                }


            }

        }#1#

        for (int i = 0; i < meshData.GetVertexCount(); i++)
        {
            Vector3 vertex = meshData.GetVertex(i);

            if (i < 4)
            {
                meshData.SetVertex(i, vertex += new Vector3(0, 0.1f, 0));
                meshData.SetVertexColor(i, Colors.Red);
            } else if (i < 8)
            {
                meshData.SetVertexColor(i, Colors.Blue);
            } else if (i < 12)
            {
                meshData.SetVertexColor(i, Colors.Green);
            } else if (i < 16)
            {
                meshData.SetVertexColor(i, Colors.Yellow);
            }
            else if (i < 20)
            {
                meshData.SetVertexColor(i, Colors.Magenta);
            }
            else if (i < 24)
            {
                meshData.SetVertexColor(i, Colors.Cyan);
            }

        }

        mesh.ClearSurfaces();
        meshData.CommitToSurface(mesh);
        //newMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, meshData.com);

        return mesh;*/
    }

    public static ArrayMesh OutlineMeshFromPart(Part part)
    {
        var st = new SurfaceTool();
        st.Begin(Mesh.PrimitiveType.Lines);
        st.SetSmoothGroup(UInt32.MaxValue);
        List<Vector3> vectors = CreatePartVectors(part);

        foreach (var vector3 in vectors)
        {
            st.AddVertex(vector3);
        }


        return st.Commit();
    }

    static void WriteQuad(ref int i, float u0, float v0, float u1, float v1, bool flipU = false)
    {
        Vector2[] uvs = new Vector2[24];
        if (!flipU)
        {
            uvs[i++] = new(u0, v0);
            uvs[i++] = new(u1, v0);
            uvs[i++] = new(u1, v1);
            uvs[i++] = new(u0, v1);
        }
        else
        {
            uvs[i++] = new(u1, v0);
            uvs[i++] = new(u0, v0);
            uvs[i++] = new(u0, v1);
            uvs[i++] = new(u1, v1);
        }
    }
    public static Vector2[] GenerateCubeUVs(Part part, Vector2 textureSize)
    {
        // We'll follow a typical "cuboid unfolded" layout in the texture atlas
        int tx = part.TextureSize.XI;
        int ty = part.TextureSize.YI;
        int dx = (int)part.Size.X;

        int dy = (int)part.Size.Y;
        int dz = (int)part.Size.Z;

      
        
        
        Vector2[] uvs = new Vector2[24]; // 6 faces × 4 vertices

        /*
        Vector3[] vertexData = new Vector3[]
        {
            new (x1, y1, z1), new (x2, y2, z2), new(x3, y3, z3), new(x4, y4, z4),

    // Top face
            new(x5, y5, z5),
            new(x6, y6, z6), new(x7, y7, z7), new(x8, y8, z8),

    // Front face
            new(x1, y1, z1),
            new(x5, y5, z5), new(x6, y6, z6), new(x2, y2, z2),

    // Back face
            new(x4, y4, z4),
            new(x8, y8, z8), new(x7, y7, z7), new(x3, y3, z3),

    // Left face
            new(x1, y1, z1),
            new(x4, y4, z4),new(x8, y8, z8), new(x5, y5, z5),

    // Right face
            new(x2, y2, z2),
            new(x3, y3, z3), new(x7, y7, z7), new(x6, y6, z6)
        };
        */

        // Front face
        /*for (int i = 0; i < 24; i++)
        {
            WriteQuad(ref i, U(tx + dz, textureSize.X), V(ty, textureSize.Y), U(tx + dz + dx, textureSize.X),
                V(ty, textureSize.Y));
            WriteQuad(ref i, U(tx + dz + dx, textureSize.X), V(ty + dz, textureSize.Y), U(tx + dz, textureSize.X), V(ty + dz, textureSize.Y));
        }*/

        //
        int i = 0;
        uvs[i++] = new Vector2(U(tx + dz, textureSize.X), V(ty, textureSize.Y));
        uvs[i++] = new Vector2(U(tx + dz + dx, textureSize.X), V(ty, textureSize.Y));
        uvs[i++] = new Vector2(U(tx + dz + dx, textureSize.X), V(ty + dz, textureSize.Y));
        uvs[i++] = new Vector2(U(tx + dz, textureSize.X), V(ty + dz, textureSize.Y));

        // Back face
        uvs[i++] = new Vector2(U(tx + dz + dx + dz, textureSize.X), V(ty, textureSize.Y));
        uvs[i++] = new Vector2(U(tx + dz + dx + dz + dx, textureSize.X), V(ty, textureSize.Y));
        uvs[i++] = new Vector2(U(tx + dz + dx + dz + dx, textureSize.X), V(ty + dz, textureSize.Y));
        uvs[i++] = new Vector2(U(tx + dz + dx + dz, textureSize.X), V(ty + dz, textureSize.Y));

        // Front face
        /*uvs[i++] = new Vector2(U(tx + dz, textureSize.X), V(ty + dz, textureSize.Y));
        uvs[i++] = new Vector2(U(tx + dz + dx, textureSize.X), V(ty + dz, textureSize.Y));
        uvs[i++] = new Vector2(U(tx + dz + dx, textureSize.X), V(ty + dz + dy, textureSize.Y));
        uvs[i++] = new Vector2(U(tx + dz, textureSize.X), V(ty + dz + dy, textureSize.Y));*/

        float u0 = U(tx + dz, textureSize.X);
        float v0 = V(ty + dz, textureSize.Y);

        float u1 = U(tx + dz + dx, textureSize.X);
        float v1 = V(ty + dz + dy, textureSize.Y);
        
        uvs[i++] = new Vector2(u0, v0);
        uvs[i++] = new Vector2(u0, v1);
        uvs[i++] = new Vector2(u1, v1);
        uvs[i++] = new Vector2(u1, v0);

        u0 = U(tx + dz + dx + dz, textureSize.X);
        v0 = V(ty + dz, textureSize.Y);

        u1 = U(tx + dz + dx + dz + dx, textureSize.X);
        v1 = V(ty + dz + dy, textureSize.Y);
        
        uvs[i++] = new Vector2(u0, v0);
        uvs[i++] = new Vector2(u0, v1);
        uvs[i++] = new Vector2(u1, v1);
        uvs[i++] = new Vector2(u1, v0);
        
        /*uvs[i++] = new Vector2(U(tx + dz + dx + dz, textureSize.X), V(ty + dz, textureSize.Y));
        uvs[i++] = new Vector2(U(tx + dz + dx + dz + dx, textureSize.X), V(ty + dz, textureSize.Y));
        uvs[i++] = new Vector2(U(tx + dz + dx + dz + dx, textureSize.X), V(ty + dz + dy, textureSize.Y));
        uvs[i++] = new Vector2(U(tx + dz + dx + dz, textureSize.X), V(ty + dz + dy, textureSize.Y));*/

        // Left face
        uvs[i++] = new Vector2(U(tx, textureSize.X), V(ty + dz, textureSize.Y));
        uvs[i++] = new Vector2(U(tx + dz, textureSize.X), V(ty + dz, textureSize.Y));
        uvs[i++] = new Vector2(U(tx + dz, textureSize.X), V(ty + dz + dy, textureSize.Y));
        uvs[i++] = new Vector2(U(tx, textureSize.X), V(ty + dz + dy, textureSize.Y));

        // Right face
        uvs[i++] = new Vector2(U(tx + dz + dx, textureSize.X), V(ty + dz, textureSize.Y));
        uvs[i++] = new Vector2(U(tx + dz + dx + dz, textureSize.X), V(ty + dz, textureSize.Y));
        uvs[i++] = new Vector2(U(tx + dz + dx + dz, textureSize.X), V(ty + dz + dy, textureSize.Y));
        uvs[i++] = new Vector2(U(tx + dz + dx, textureSize.X), V(ty + dz + dy, textureSize.Y));

        return uvs;
    }

    /*static float U(int x, float textureWidth) => (float)x / textureWidth;
    static float V(int y, float textureHeight) => (float)y / textureHeight;*/
    static float U(int x, float textureWidth) => (float)x / textureWidth;
    static float V(int y, float textureHeight) => (float)y / textureHeight;

    public static List<Vector3> CreatePartVectors(Part part)
    {
        if (part is Shapebox shapebox) return CreateShapeboxVectors(shapebox);


        float z1 = part.Offset.X;
        float y1 = part.Offset.Y;
        float x1 = part.Offset.Z;

        float z2 = part.Size.X + part.Offset.X;
        float y2 = part.Offset.Y;
        float x2 = part.Offset.Z;

        float z3 = part.Size.X + part.Offset.X;
        float y3 = part.Offset.Y;
        float x3 = part.Size.Z + part.Offset.Z;

        float z4 = part.Offset.X;
        float y4 = part.Offset.Y;
        float x4 = part.Size.Z + part.Offset.Z;

        float z5 = part.Offset.X;
        float y5 = (-part.Size.Y) - part.Offset.Y;
        float x5 = part.Offset.Z;

        float z6 = part.Size.X + part.Offset.X;
        float y6 = (-part.Size.Y) - part.Offset.Y;
        float x6 = part.Offset.Z;

        float z7 = part.Size.X + part.Offset.X;
        float y7 = (-part.Size.Y) - part.Offset.Y;
        float x7 = part.Size.Z + part.Offset.Z;

        float z8 = part.Offset.X;
        float y8 = (-part.Size.Y) - part.Offset.Y;
        float x8 = part.Size.Z + part.Offset.Z;

        Vector3[] vertexData = new Vector3[]
        {
            new(x1, y1, z1), new(x2, y2, z2), new(x3, y3, z3), new(x4, y4, z4),

// Top face
            new(x5, y5, z5),
            new(x6, y6, z6), new(x7, y7, z7), new(x8, y8, z8),

// Front face
            new(x1, y1, z1),
            new(x5, y5, z5), new(x6, y6, z6), new(x2, y2, z2),

// Back face
            new(x4, y4, z4),
            new(x8, y8, z8), new(x7, y7, z7), new(x3, y3, z3),

// Left face
            new(x1, y1, z1),
            new(x4, y4, z4), new(x8, y8, z8), new(x5, y5, z5),

// Right face
            new(x2, y2, z2),
            new(x3, y3, z3), new(x7, y7, z7), new(x6, y6, z6)
        };

        /*Vector3[] vertexData = new Vector3[]
        {
            new (x3, y3, z3), new (x4, y4, z4), new (x1, y1, z1), new (x2, y2, z2),

            new (x6, y6, z6), new (x5, y5, z5), new (x8, y8, z8), new (x7, y7, z7),

            new (x1, y1, z1), new (x4, y4, z4), new (x8, y8, z8), new (x5, y5, z5),

            new (x3, y3, z3), new (x2, y2, z2), new (x6, y6, z6), new (x7, y7, z7),

            new (x2, y2, z2), new (x1, y1, z1), new (x5, y5, z5), new (x6, y6, z6),
            new (x4, y4, z4), new (x3, y3, z3), new (x7, y7, z7), new (x8, y8, z8),
        };*/

        //num1 = x size
        //num 2 = ysize

        /*new Vector3((float) (1.0 /  num1 * (
            Module_Definition.Parts[id].TextureX + Module_Definition.Parts[id].DimZ + Module_Definition.Parts[id].DimX)),
            1f / (float) num2 * (float) Module_Definition.Parts[id].TextureY, 0.0f),*/
        return vertexData.ToList();
    }

    public static List<Vector3> CreateShapeboxVectors(Shapebox part)
    {
        float z1 = part.Offset.X - part.Corners[0].X;
        float y1 = part.Offset.Y + part.Corners[0].Y;
        float x1 = part.Offset.Z - part.Corners[0].Z;

        float z2 = part.Size.X + part.Offset.X + part.Corners[1].X;
        float y2 = part.Offset.Y + part.Corners[1].Y;
        float x2 = part.Offset.Z - part.Corners[1].Z;

        float z3 = part.Size.X + part.Offset.X + part.Corners[2].X;
        float y3 = part.Offset.Y + part.Corners[2].Y;
        float x3 = part.Size.Z + part.Offset.Z + part.Corners[2].Z;

        float z4 = part.Offset.X - part.Corners[3].X;
        float y4 = part.Offset.Y + part.Corners[3].Y;
        float x4 = part.Size.Z + part.Offset.Z + part.Corners[3].Z;

        float z5 = part.Offset.X - part.Corners[4].X;
        float y5 = (-part.Size.Y) - part.Offset.Y - part.Corners[4].Y;
        float x5 = part.Offset.Z - part.Corners[4].Z;

        float z6 = part.Size.X + part.Offset.X + part.Corners[5].X;
        float y6 = (-part.Size.Y) - part.Offset.Y - part.Corners[5].Y;
        float x6 = part.Offset.Z - part.Corners[5].Z;

        float z7 = part.Size.X + part.Offset.X + part.Corners[6].X;
        float y7 = (-part.Size.Y) - part.Offset.Y - part.Corners[6].Y;
        float x7 = part.Size.Z + part.Offset.Z + part.Corners[6].Z;

        float z8 = part.Offset.X - part.Corners[7].X;
        float y8 = (-part.Size.Y) - part.Offset.Y - part.Corners[7].Y;
        float x8 = part.Size.Z + part.Offset.Z + part.Corners[7].Z;

        float[] colorData = new float[108];

        Vector3[] vertexData = new Vector3[]
        {
            new(x1, y1, z1), new(x2, y2, z2), new(x3, y3, z3), new(x4, y4, z4),

// Top face
            new(x5, y5, z5),
            new(x6, y6, z6), new(x7, y7, z7), new(x8, y8, z8),

// Front face
            new(x1, y1, z1),
            new(x5, y5, z5), new(x6, y6, z6), new(x2, y2, z2),

// Back face
            new(x4, y4, z4),
            new(x8, y8, z8), new(x7, y7, z7), new(x3, y3, z3),

// Left face
            new(x1, y1, z1),
            new(x4, y4, z4), new(x8, y8, z8), new(x5, y5, z5),

// Right face
            new(x2, y2, z2),
            new(x3, y3, z3), new(x7, y7, z7), new(x6, y6, z6)
        };
        /*
        Vector3[] vertexData = new Vector3[]
        {
            new (x3, y3, z3), new (x4, y4, z4), new (x1, y1, z1), new (x2, y2, z2),

            new (x6, y6, z6), new (x5, y5, z5), new (x8, y8, z8), new (x7, y7, z7),

            new (x1, y1, z1), new (x4, y4, z4), new (x8, y8, z8), new (x5, y5, z5),

            new (x3, y3, z3), new (x2, y2, z2), new (x6, y6, z6), new (x7, y7, z7),

            new (x2, y2, z2), new (x1, y1, z1), new (x5, y5, z5), new (x6, y6, z6),
            new (x4, y4, z4), new (x3, y3, z3), new (x7, y7, z7), new (x8, y8, z8),
        };
        */


        /*float[] x = new float[] { x1, x2, x3, x4, x5, x6, x7, x8 };
        float[] y = new float[] { y1, y2, y3, y4, y5, y6, y7, y8 };
        float[] z = new float[] { z1, z2, z3, z4, z5, z6, z7, z8 };

        for (int i = 0; i < 36; i++)
        {
            int index = i * 3;
            vertexData[index] = x[i % 8];
            vertexData[index + 1] = y[i % 8];
            vertexData[index + 2] = z[i % 8];
        }*/

        return vertexData.ToList();
    }
}