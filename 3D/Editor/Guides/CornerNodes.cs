using System.Collections.Generic;
using System.Linq;
using Godot;
using PinkDogMM_Gd.Core;
using PinkDogMM_Gd.Core.Schema;

namespace PinkDogMM_Gd.UI.Viewport;

public partial class CornerNodes : Node3D
{
    private List<CornerNode> corners;
    private Model model;
    private Part part;

    public override void _Ready()
    {
        model = Model.Get(this);
        model.State.ModeChanged += (sender, mode) =>
        {
            if (mode != EditorMode.ShapeEdit)
            {
           
                foreach (var cornerNode in corners)
                {
                    cornerNode.Free();
                }
                corners.Clear();
                return;
            };
            
            for (var i = 0; i < 8; i++)
            {
                var corner = new CornerNode(i);
                corner.SetMeta("corner", i);
                corners.Add(corner);
                AddChild(corner);
            }
            
            part = model.State.SelectedObjects.First() as Part;
            part.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName.Contains("Shapebox"))
                {
                    PositionCorners();
                }
            };
         
            this.Scale = new Vector3(0.01f, 0.01f, 0.01f);
            PositionCorners();
        };
        //Assuming 8 corners for a shapebox box. Other shapes not here yet.
        corners = [];
        


        /*part.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName.Contains("Shapebox"))
            {
                PositionCorners();
            }
        };*/
        //GetNode<AppState>("/root/AppState").FocusedCornerChanged += (sender, i) => { SetFocusedAll(i); };
        
        this.Scale = new Vector3(0.01f, 0.01f, 0.01f);
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
        PositionCorners();
        this.Scale = this.Scale.Lerp(model.State.Mode == EditorMode.ShapeEdit ? Vector3.One : new Vector3(0.01f, 0.01f, 0.01f), (float)delta * 24.0f);

    }


    public void PositionCorners()
    {
        if (part == null) return;
        this.Position = part.Position.AsVector3().LHS();
        if (part is Shapebox shapebox)
        {
            for (var index = 0; index < corners.Count; index++)
            {
                var node = corners[index];
                var corner = shapebox.GetCornerPosition(index);
                node.Position = new Vector3(corner.Z, corner.Y, corner.X) / 2/* ((new Vector3(corner.Z, corner.Y, corner.X) / 1 / 20) /
                                 new Vector3(model.State.Camera.Zoom, model.State.Camera.Zoom, model.State.Camera.Zoom))*/;
                node.SetFocused(false);
                
                
            }
        }
    }
}