using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using PinkDogMM_Gd.Core.Schema;
using Image = Godot.Image;
using ImageTexture = Godot.ImageTexture;
using Vector2 = Godot.Vector2;

namespace PinkDogMM_Gd.Core.LoadSave;

public class ToolboxLoader : ModelLoader
{
    public new Model Load(string path)
    {
        Model model = new Model();
        model.PartGroups.Add("Import", new BubblingObservableList<Part>());
        using ZipArchive archive = ZipFile.OpenRead(path);
        foreach (var entry in archive.Entries)
        {
            //this is it! 
            if (entry.Name.EndsWith("Model.txt"))
            {
                StreamReader reader = new StreamReader(entry.Open());
                string text = reader.ReadToEnd().Replace(",", "."); //Replace the punctuation cuz German is dumb.

                var split = text.Split(Environment.NewLine);
                for (var index = 0; index < split.Length; index++)
                {
                    var line = split[index];
                    string[] lineSplit = line.Split("|");
                    if (lineSplit[0] == "ModelClassName") model.Name = line.Split("|")[1].ReplaceLineEndings("");

                    if (!line.StartsWith("Element")) continue;
                    model.PartGroups["Import"].Add(
                        lineSplit[5] == "Shapebox" ? DeserialiseShapebox(lineSplit, index) : DeserialisePart(lineSplit, index));
                }
            } else if (entry.Name.EndsWith("Model.png"))
            {
                StreamReader reader = new StreamReader(entry.Open());
                byte[] imageBytes = Utils.ReadAllBytes(reader.BaseStream);
                
                var image = new Image();

                image.LoadPngFromBuffer(imageBytes);
                model.Textures.Add("Default", new Texture(new Vector2(image.GetWidth(), image.GetHeight()), ImageTexture.CreateFromImage(image)));
            }
            else
            {
                continue;
            }
        }
        
        return model;
    }
    
    
    public Part DeserialisePart(string[] line, int index)
    {
        var part = new Part();
        //It's a very complicated and weird thing, but here we go:

        part.Name = line[3];
        part.Id = index;
        part.Position = new Vector3L(float.Parse(line[6]), float.Parse(line[7]), float.Parse(line[8]));
        part.Size = new Vector3L(int.Parse(line[9]), int.Parse(line[10]), int.Parse(line[11]));
        part.Rotation = new Vector3L(float.Parse(line[12]), float.Parse(line[13]), float.Parse(line[14]));
        part.Offset = new Vector3L();    
        
        part.Texture = new Vector2L(int.Parse(line[18]), int.Parse(line[19]));

        part.Visible = line[98] == "1";
        return part;
    }
    
    public Shapebox DeserialiseShapebox(string[] line, int index)
    {
        var part = new Shapebox
        {
            //It's a very complicated and weird thing, but here we go:
            Name = line[3],
            Id = index,
            Position = new Vector3L(float.Parse(line[6]), float.Parse(line[7]), float.Parse(line[8])),
            Size = new Vector3L(int.Parse(line[9]), int.Parse(line[10]), int.Parse(line[11])),
            Rotation = new Vector3L(float.Parse(line[12]), float.Parse(line[13]), float.Parse(line[14])),
            Offset = new Vector3L(),
            Texture = new Vector2L(int.Parse(line[18]), int.Parse(line[19])),
            
            ShapeboxX = new BubblingObservableList<float>(line.Skip(20).Take(8).Select(float.Parse).ToList()),
            ShapeboxY = new BubblingObservableList<float>(line.Skip(28).Take(8).Select(float.Parse).ToList()),
            ShapeboxZ = new BubblingObservableList<float>(line.Skip(36).Take(8).Select(float.Parse).ToList()),

            Visible = line[98] == "1"
        };
        
        return part;
    }
}