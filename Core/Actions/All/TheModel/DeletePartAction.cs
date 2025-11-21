using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using PinkDogMM_Gd.Core.Commands;
using PinkDogMM_Gd.Core.Configuration;
using PinkDogMM_Gd.Core.Schema;

namespace PinkDogMM_Gd.Core.Actions.All.TheModel;

public class DeletePartAction : IAction
{
    private int id = -1;
    private Model model;
    List<TreeNode<Renderable>> parts = [];
    public void Execute()
    {
        
        if (id != -1)
        {
            var part = model.Items.GetItemById(id);
            if (part != null) parts.Add(part);
        }
        else
        {
            parts.AddRange(model.State.SelectedObjects.ToList().Select(selectedPart => model.Items.GetItemById(selectedPart.Id)).OfType<TreeNode<Renderable>>());
        }
      
        foreach (var treeNode in parts)
        {
            model.Items.Remove(treeNode.Value);
            model.State.UnselectObject(treeNode.Value);
        }
    }

    public void SetArguments(Dictionary arguments)
    {
        model = arguments["model"].As<Model>()?? throw new InvalidOperationException();
    }

    public void SetArguments(System.Collections.Generic.Dictionary<string, Variant> arguments)
    {
        throw new System.NotImplementedException();
    }

    public void Undo()
    {
        foreach (var treeNode in parts)
        {
            model.Items.Add(treeNode.Value, treeNode.Name);
            model.State.SelectObject(treeNode.Value.Id);
        }
    }

    public bool AddToStack => parts.Count != 0;
    public string Icon => "icon_delete";
    
    public static int DefaultKeys => KeyCombo.KeyAndModifiers((int)Key.Delete, KeyModifiers.None);

 
  
}