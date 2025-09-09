using System.Collections.Generic;
using Godot;
using PinkDogMM_Gd.Core;
using PinkDogMM_Gd.Core.Schema;

namespace PinkDogMM_Gd.UI.Viewport;

public partial class CornerNodes(Part part) : Node3D
{
    private List<CornerNode> corners;
    
    public override void _Ready()
    {
        //Assuming 8 corners for a shapebox box. Other shapes not here yet.
        corners = new List<CornerNode>();
        for (var i = 0; i < 8; i++)
        {
            var corner = new CornerNode(i);
            corners.Add(corner);
            AddChild(corner);
        }

        part.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName.Contains("Shapebox"))
            {
                PositionCorners();
            }
        };
        GetNode<AppState>("/root/AppState").FocusedCornerChanged += (sender, i) => { SetFocusedAll(i); };
        
        PositionCorners();
        VisibilityChanged += () =>
        {
            if (!Visible)
            {
                Scale = Vector3.One;
            }
            else
            {
                this.Scale = new Vector3(0.1f, 0.1f, 0.1f);
            }
        };
      
        SetFocusedAll(0);
    }

    private void SetFocusedAll(int i)
    {
        for (var index = 0; index < corners.Count; index++)
        {
            var node = corners[index];
            node.SetFocused(i == index);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Visible)
        {
            this.Scale = this.Scale.Lerp(Vector3.One, (float)delta * 32.0f);
        }
    }

    
    public void PositionCorners()
    {
        if (part is Shapebox shapebox)
        {
            for (var index = 0; index < corners.Count; index++)
            {
                var node = corners[index];
                var corner = shapebox.GetCornerPosition(index);
                //node.Position = new Vector3(shapebox.Size.X + shapebox.ShapeboxX[index], shapebox.Size.Y + shapebox.ShapeboxY[index], shapebox.Size.Z +  shapebox.ShapeboxZ[index]);
                node.Position = new Vector3(corner.Z, corner.Y, corner.X);
                //rotate so looks like diamond
            }
        }
    } 
}