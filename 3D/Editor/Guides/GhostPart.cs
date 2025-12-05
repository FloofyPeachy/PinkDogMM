using Godot;
using System;
using System.Collections.Generic;
using PinkDogMM_Gd.Core;
using PinkDogMM_Gd.Core.Schema;

public partial class GhostPart : MeshInstance3D
{
    private Model model;
    public override void _Ready()
    {

        model = Model.Get(this);
        Mesh = new BoxMesh()
        {
            Size = new Vector3(1, 1, 1) / 1/16
        };
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
     
        //this.Position = new Vector3((float)Math.Round(model.State.GridMousePosition.X, 2), (float)Math.Round(model.State.GridMousePosition.Y, 2), (float)Math.Round(model.State.GridMousePosition.Z, 2)) / 1/16;
    }
}
