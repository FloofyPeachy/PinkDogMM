using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Godot;
using PinkDogMM_Gd.Core.Schema;
using PinkDogMM_Gd.Render;

namespace PinkDogMM_Gd._3D;

public class Texturerer
{
    public static void DrawPart(Image img, Part part)
    {
        var textureSize = new Vector2(512, 512);
        var uvs = MeshGenerator.GenerateCubeUVs(part, textureSize);

        /*foreach (var vector2 in uvs)
        {
            var px = Pixel(vector2, textureSize);
            img.SetPixel((int)px.X, (int)px.Y, Colors.Orange);
            GD.Print("drawing at" + px);
        }*/
    }
   
    public static void DrawTexture(List<Part> parts)
    {
        var img = Image.CreateEmpty(512, 512, false, Image.Format.Rgba8)!;
        img.Fill(Colors.Transparent);
        //img.FillRect(new Rect2I(new Vector2I(100, 100), new Vector2I(100,100)), Colors.Orange);
        for (var index = 0; index < parts.Count; index++)
        {
            var part = parts[index];
            var uvs = MeshGenerator.GenerateCubeUVs(part, new Vector2(512, 512));
            foreach (var vector2Se in uvs.Chunk(3))
            {
                List<Vector2> vertices = [];
                vertices.AddRange(vector2Se.Select(Pixel));

                if (vertices.Count < 3) return;

                // Step 1: Find Ymin and Ymax of the polygon
                float minY = vertices.Min(v => v.Y);
                float maxY = vertices.Max(v => v.Y);

                for (int y = (int)minY; y <= (int)maxY; y++)
                {
                    List<float> intersections = new List<float>();

                    // Step 2: ScanLine intersects with each edge of the polygon
                    for (int i = 0; i < vertices.Count; i++)
                    {
                        Vector2 p1 = vertices[i];
                        Vector2 p2 = vertices[(i + 1) % vertices.Count]; // Loop back to the start for the last edge

                        if ((p1.Y <= y && p2.Y > y) || (p1.Y > y && p2.Y <= y))
                        {
                            // Calculate the x-coordinate of the intersection point
                            if (Mathf.IsEqualApprox(p2.Y, p1.Y)) continue; // Avoid division by zero for horizontal lines
                            float x = p1.X + (y - p1.Y) * (p2.X - p1.X) / (p2.Y - p1.Y);
                            intersections.Add(x);
                        }
                    }

                    // Step 3: Sort the intersection points in increasing order of X
                    intersections.Sort();

                    // Step 4: Fill pixels between pairs of intersections
                    for (int i = 0; i < intersections.Count - 1; i += 2)
                    {
                        float xStart = intersections[i];
                        float xEnd = intersections[i + 1];
                        // Draw a horizontal line (Rect2 of height 1) for the scanline segment
                        img.FillRect(new Rect2I((int)xStart, y, (int)(xEnd - xStart + 1), 1), Colors.Orange);
                        img.SavePng("detexture22" + index + "_finalone.png");
                        //DrawRect(new Rect2(xStart, y, xEnd - xStart + 1, 1), color);
                    }
                }
            
            }
            foreach (var vector2 in uvs)
            {
                var px = Pixel(vector2);
               
               
                GD.Print("drawing at" + px);
                if (index == parts.Count - 1)
                {
                    img.SavePng("detexture" + index + "_finalone.png");
                }
            }
        
            /*img.FillRect(new Rect2I(new Vector2I(100, 100), new Vector2I(100, 100)), Colors.Orange);
         
            if (index == parts.Count - 1)
            {
                img.SavePng("detexture" + index + "_finalone.png");
            }
        
            /*img.FillRect(new Rect2I(new Vector2I(100, 100), new Vector2I(100,100)), Colors.Orange);
            DrawPart(img, part);
            img.FillRect(new Rect2I(new Vector2I(100, 100), new Vector2I(100,100)), Colors.Orange);#1#*/
        }

        var img2 = new Image();
        img2.CopyFrom(img);
      //  img2.SavePng("detexture_other.png");
     //   img.SavePng("detexture.png");
    }

    static Vector2 Pixel(Vector2 uv)
    {
        return new Vector2(
            uv.X * 512,
            (uv.Y) * 512 // because you flipped V
        );
    }
    public static IEnumerable<Vector2> GetPointsOnLine(int x0, int y0, int x1, int y1)
    {
        bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
        if (steep)
        {
            Swap(ref x0, ref y0);
            Swap(ref x1, ref y1);
        }

        if (x0 > x1)
        {
            Swap(ref x0, ref x1);
            Swap(ref y0, ref y1);
        }

        int dx = x1 - x0;
        int dy = Math.Abs(y1 - y0);
        int error = dx / 2;
        int ystep = (y0 < y1) ? 1 : -1;
        int y = y0;

        for (int x = x0; x <= x1; x++)
        {
            yield return steep ? new Vector2(y, x) : new Vector2(x, y);

            error -= dy;
            if (error < 0)
            {
                y += ystep;
                error += dx;
            }
        }
    }
    private static void Swap(ref int a, ref int b)
    {
        (a, b) = (b, a);
    }
    /*static Rect2I GetFaceRect(Vector2[] uvs, int baseIndex, Vector2 textureSize)
    {
        /#1#*var p0 = Pixel(uvs[baseIndex + 0], textureSize);
        var p1 = Pixel(uvs[baseIndex + 1], textureSize);
        var p2 = Pixel(uvs[baseIndex + 2], textureSize);#2#

        float x = p0.X;
        float y = p0.Y;

        float w = p1.X - p0.X;
        float h = p2.Y - p1.Y; // because V is flipped

        return new Rect2I((int)x, (int)y, (int)w, (int)h);#1#
    }*/
}