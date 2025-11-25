using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;
using PinkDogMM_Gd.Core.Commands;
using PinkDogMM_Gd.Core.Configuration;
using PinkDogMM_Gd.Core.Schema;

namespace PinkDogMM_Gd.Core.Actions.All.TheModel;

public class ClonePartAction : IAction
{
    private List<Part> parts;
    private Model model;
    
    public void Execute()
    {
        parts = [];
        foreach (var selected in model.State.SelectedObjects)
        {
            if (selected is not Part selPart) continue;

            int id = model.TotalPartCount + 1;
            string name = "Copy of " + selPart.Name;
            PartTypes type = selPart.PartType;
            Vector3 position = selPart.Position.AsVector3();
            Vector3 offset = selPart.Offset.AsVector3();
            Vector3 size = selPart.Size.AsVector3();
            Vector3 rotation = selPart.Rotation.AsVector3();
            Vector2 textureSize = selPart.TextureSize.AsVector2();
            
            var extra = selPart.Extra;
            
            if (selPart is Shapebox)
            {
                var newshp = new Shapebox();
                newshp.Name = name;
                newshp.PartType = type;
                newshp.Position = new Vector3L(position);
                newshp.Offset = new Vector3L(offset);
                newshp.Size = new Vector3L(size);
                newshp.Rotation = new Vector3L(rotation);
                newshp.TextureSize = new Vector2L(textureSize);
                newshp.Extra = extra;
                parts.Add(newshp);
                model.Add(newshp);
         
                return;
            }

            var newpart = new Part();
            newpart.Name = name;
            newpart.PartType = type;
            newpart.Position = new Vector3L(position);
            newpart.Offset = new Vector3L(offset);
            newpart.Size = new Vector3L(size);
            newpart.Rotation = new Vector3L(rotation);
            newpart.TextureSize = new Vector2L(textureSize);
            newpart.Extra = extra;
            parts.Add(newpart);
            model.Add(newpart);
           




        }
        
    }

    public void SetArguments(Dictionary arguments)
    {
        model = arguments["model"].As<Model>()?? throw new InvalidOperationException();
    }

    public void Undo()
    {
        foreach (var part in parts)
        {
            model.Remove(part);
        }
    }

    public string Icon => "icon_clone_new";
    public bool AddToStack => true;
    
    public static int DefaultKeys => KeyCombo.KeyAndModifiers((int)Key.D, KeyModifiers.Ctrl);
}