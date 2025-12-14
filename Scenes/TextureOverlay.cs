using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using PinkDogMM_Gd.Core.Schema;
using PinkDogMM_Gd.Render;

public partial class TextureOverlay : Control
{
    public Model model;
    public Label InfoLabel;
    public Dictionary<Part, List<Rect2>> ranges = [];
    public List<Part> hovering = [];
    public override void _Ready()
    {
        
        model = Model.Get(GetParent().Owner);
       
        model.State.ObjectSelectionChanged += StateOnObjectSelectionChanged;
        model.State.ObjectHoveringChanged += (sender, renderable) =>
        {
            QueueRedraw();
        };
    }

    private void StateOnObjectSelectionChanged(object? sender, (Renderable, bool) e)
    {
        if (e.Item1 is Part)
        {
            MakeRanges();
        }
    }

    public override void _Input(InputEvent @event)
    {
        
        if (MakeInputLocal(@event) is InputEventMouseButton button)
        {
            if (model.Textures.Count == 0) return;
            if (!button.Pressed) return;
            var image = model.Textures[0].Image.GetImage();
            image.SetPixel((int)button.Position.X, (int)button.Position.Y, Colors.Black);
            model.Textures[0].Image.Update(image);
            QueueRedraw();
            /*foreach (var keyValuePair in ranges)
            {
                var image = model.Textures[0].Image.GetImage();
                image.SetPixel((int)button.Position.X, (int)button.Position.Y, Colors.Orange);
                model.Textures[0].Image.Update(image);
                /*foreach (var rect2 in keyValuePair.Value)
                {
                    if (rect2.HasPoint(button.Position))
                    {
                        GD.Print("in range!! " + button.Position);
                        model.State.SelectObject(keyValuePair.Key.Id);
                        return;
                    }
                }#1#

                QueueRedraw();
            }*/

        } else if (MakeInputLocal(@event) is InputEventMouseMotion motion)
        {
            InfoLabel = GetParent().GetParent().GetParent().GetParent().GetNode<Label>("Panel/Label");
            InfoLabel.Text = (model.Textures.Count != 0 ? model.Textures[0].Size.ToString() : "") + " (" + motion.Position.Round().X + "," + motion.Position.Round().Y+ ")" ;
            
            /*foreach (var keyValuePair in ranges)
            {
                bool inIt = false;
                
                foreach (var rect2 in keyValuePair.Value)
                {
                    if (rect2.HasPoint(motion.Position))
                    {
                        GD.Print("in range!! " + motion.Position);
                        inIt = true;
                        break;
                    }
                }

                if (inIt)
                {
                    hovering.Add(keyValuePair.Key);
                }
                else
                {
                    hovering.Remove(keyValuePair.Key);
                }
                
                QueueRedraw();
            }*/
           
            
        }
        
    }

    public override void _Draw()
    {
        foreach (var keyValuePair in ranges)
        {
            var part = keyValuePair.Key;
            var color = model.State.SelectedObjects.Contains(part) ? Colors.Yellow :
                (model.State.Hovering == part) ? Colors.Orange : Colors.White;
           
            
                foreach (var rect2 in keyValuePair.Value)
                {
                    DrawLine(rect2.Position, rect2.Size, color);
                }
        }
    }

    private void MakeRanges()
    {
        ranges.Clear();
        foreach (var obj in model.AllObjects)
        {
            if (obj is not Part part) continue;
            var uvs = MeshGenerator
                .GenerateCubeUVs(part, model.Textures[0].Size).ToList();
            var rectSize = model.Textures[0].Size;
            var range = new List<Rect2> {};
            foreach (var quad in uvs.Chunk(4))
            {
                var p0 = quad[0] * rectSize;
                var p1 = quad[1] * rectSize;
                var p2 = quad[2] * rectSize;
                var p3 = quad[3] * rectSize;
                
                range.Add(new Rect2(p0, p1));
                range.Add(new Rect2(p1, p2));
                range.Add(new Rect2(p2, p3));
                range.Add(new Rect2(p3, p0));
                
            }
            
            ranges.Add(part, range);
        }
    }
}