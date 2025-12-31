using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Numerics;
using PinkDogMM_Gd.Core.Schema;
using GD = Godot.GD;
using Image = Godot.Image;
using ImageTexture = Godot.ImageTexture;
using Vector2 = Godot.Vector2;
using Vector3 = Godot.Vector3;

namespace PinkDogMM_Gd.Core.LoadSave;

public class ToolboxFormat : IModelFormat<string[]>
{

    public static Dictionary<string, (int, Type)> PartFormat =
        new Dictionary<string, (int, Type)>()
        {
            {"Element", (1, typeof(string))},
            {"GenID", (1, typeof(int))},
            {"PartID", (1, typeof(int))},
            
            {"Name", (1, typeof(string))},
            {"ModelPart", (1, typeof(string))},
            {"PartType", (1, typeof(string))},
            
            {"Position", (3, typeof(Vector3))},
            {"Size", (3, typeof(Vector3))},
            {"Rotation", (3, typeof(Vector3))},
            {"Offset", (3, typeof(Vector3))},
            {"Texture", (2, typeof(Vector2))},
            
            {"ShapeboxX", (8, typeof(float[]))},
            {"ShapeboxY", (8, typeof(float[]))},
            {"ShapeboxZ", (8, typeof(float[]))},
            
            {"Trapezoid", (2, typeof(string))},
            {"Flexbox", (4, typeof(string))},
            {"FlexTrap", (6, typeof(string))},
            
            {"ShapeX", (20, typeof(float[]))},
            {"ShapeY", (20, typeof(float[]))},
            
            {"ShapeCorners", (1, typeof(double))},
            {"GroupID", (1, typeof(string))},
            {"Visible", (1, typeof(bool))},
            {"Border", (1, typeof(bool))},
            {"Flip", (1, typeof(bool))},
            {"Export", (1, typeof(bool))},
            {"Padding", (8, typeof(string))}
        };
      

    public new Model Load(string path)
    {
        Model model = new Model();
        using ZipArchive archive = ZipFile.OpenRead(path);
        foreach (var entry in archive.Entries)
        {
            //this is it! 
            if (entry.Name.EndsWith("Model.txt"))
            {
                int partCount = 0;
                StreamReader reader = new StreamReader(entry.Open());
                string text = reader.ReadToEnd().Replace(",", "."); //Replace the punctuation cuz German is dumb.

                var split = text.Split(Environment.NewLine);
                for (var index = 0; index < split.Length; index++)
                {
                    var line = split[index];
                    string[] lineSplit = line.Split("|");
                    if (lineSplit[0] == "ModelClassName") model.Name = line.Split("|")[1].ReplaceLineEndings("");
                    if (!line.StartsWith("Element")) continue;
                    model.Add(
                        lineSplit[5] == "Shapebox" ? DeserialiseShapebox(lineSplit, partCount) : DeserialisePart(lineSplit, partCount), lineSplit[3]);
                    partCount++;
                    int totalLength = 0;
                    foreach (var keyValuePair in PartFormat)
                    {
                        totalLength += keyValuePair.Value.Item1;
                    }
                    GD.Print(totalLength);
                    PL.I.Debug(PartJson.PartToJson(DeserialiseShapebox(lineSplit, partCount)));
                } 
            } else if (entry.Name.EndsWith("Model.png"))
            {
                StreamReader reader = new StreamReader(entry.Open());
                byte[] imageBytes = Utils.ReadAllBytes(reader.BaseStream);
                
                var image = new Image();

                image.LoadPngFromBuffer(imageBytes);
            
                model.Textures.Add(new Texture(new Vector2(image.GetWidth(), image.GetHeight()), "Default", ImageTexture.CreateFromImage(image)));
            }
            else
            {
                continue;
            }
        }
        
        return model;
    }

    public static Part FromSmp(string[] line)
    {
        return new Part()
        {
            Name = line[3].Replace("cull", ""),
            Position = new Vector3L(float.Parse(line[6]), float.Parse(line[7]), float.Parse(line[8])),
            Size = new Vector3L(int.Parse(line[9]), int.Parse(line[10]), int.Parse(line[11])),
            Rotation = new Vector3L(float.Parse(line[12]), float.Parse(line[13]), float.Parse(line[14])),
            Offset = new Vector3L(),
            
            TextureSize = new Vector2L(int.Parse(line[18]), int.Parse(line[19])),
            Visible = line[98] == "1",
            Cull = line[3].Contains("cull")
        };
    }

    public object GetField(string name)
    {
        return "";
    }

    public static string[] ToSmp(Part part)
    {
        return [];
    }

    public void Save(string path, Model model)
    {
      
    }

    public Part DeserialisePart(string[] line, int index)
    {
        var part = new Part();
        //It's a very complicated and weird thing, but here we go:

        part.Name = line[3].Replace("cull", "");
        part.Position = new Vector3L(float.Parse(line[6]), float.Parse(line[7]), float.Parse(line[8]));
        part.Size = new Vector3L(int.Parse(line[9]), int.Parse(line[10]), int.Parse(line[11]));
        part.Rotation = new Vector3L(float.Parse(line[12]), float.Parse(line[13]), float.Parse(line[14]));
        part.Offset = new Vector3L();    
        
        part.TextureSize = new Vector2L(int.Parse(line[18]), int.Parse(line[19]));

        part.Visible = line[98] == "1";
        part.Cull = line[3].Contains("cull");
        return part;
    }
    
    public Shapebox DeserialiseShapebox(string[] line, int index)
    {
        var sections = new BubblingObservableList<Vector3L>();
        var shapeboxX = line.Skip(20).Take(8).Select(float.Parse).ToList();
        var shapeboxY = line.Skip(28).Take(8).Select(float.Parse).ToList();
        var shapeboxZ = line.Skip(36).Take(8).Select(float.Parse).ToList();
        
        
        for (var i = 0; i < 8; i++)
        {
            sections.Add(new Vector3L(shapeboxX[i], shapeboxY[i], shapeboxZ[i]));
        }
        var part = new Shapebox
        {
            //It's a very complicated and weird thing, but here we go:
            Name = line[3],
            Position = new Vector3L(float.Parse(line[6]), float.Parse(line[7]), float.Parse(line[8])),
            Size = new Vector3L(int.Parse(line[9]), int.Parse(line[10]), int.Parse(line[11])),
            Rotation = new Vector3L(float.Parse(line[12]), float.Parse(line[13]), float.Parse(line[14])),
            Offset = new Vector3L(),
            TextureSize = new Vector2L(int.Parse(line[18]), int.Parse(line[19])),
            Corners = sections,
            Visible = line[98] == "1"
        };
        
        return part;
    }
}