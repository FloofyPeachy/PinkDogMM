using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.JavaScript;
using Godot;
using PinkDogMM_Gd.Core;

namespace PinkDogMM_Gd.Core.Schema;


public class TreeNode<T> 
{
    public string Name { get; set; }
    public int Id { get; set; } = -1;
    public T? Value { get; set; }
    public TreeNode<T> Parent { get; set; }
    public TreeNode<T> BigParent { get; set; }
    
    public List<TreeNode<T>> Children { get; set; }
    public Dictionary<int, TreeNode<T>> Indicies = new();
    
    public TreeNode(T value)
    {
        Value = value;
        Children = new List<TreeNode<T>>();
    }

    public TreeNode()
    {
        Id = 0;
        Children = new List<TreeNode<T>>();
        BigParent = this;
    }
    public TreeNode(string name)
    {
        Name = name;
        Id = 0;
        BigParent = this;
        Children = new List<TreeNode<T>>();
    }

    public TreeNode<T> Add(T childValue, String name)
    {
        var childNode = new TreeNode<T>(childValue)
        {
            Parent = this,
            Name = name,
            BigParent = BigParent
        };
        
        BigParent.Indicies.Add(BigParent.Children.Count, childNode);
        Children.Add(childNode);
        
        CollectionChanged?.Invoke(this, (true, childNode));
        return childNode;
    }
    
    public bool Remove(T childValue)
    {
        var childNode = Children.FirstOrDefault(node => node.Value.Equals(childValue));
        if (childNode == null) return false;
        BigParent.Indicies.Remove(BigParent.Indicies.FirstOrDefault(pair => pair.Value == childNode).Key);
        Children.Remove(childNode);
        
        CollectionChanged?.Invoke(this, (false, childNode));
        return true;
    }
    
    public TreeNode<T>? Traverse(TreeNode<T> node)
    {
        foreach (var child in node.Children)
        {
            Traverse(child);
            return child;
        }

        return null;
    }
  
    
    public TreeNode<T>? GetItemById(int id)
    {
        return Indicies.GetValueOrDefault(id);
    }

    // Other methods like RemoveChild, Traverse, etc.
    public event EventHandler<(bool, TreeNode<T>)> CollectionChanged;
}


[GlobalClass]
public partial class Model : Resource
{
    public delegate void ChangedEventHandler();

    public delegate void PartChangedEventHandler();

    public readonly TreeNode<Renderable> Items = new("Root");
    
    public readonly BubblingObservableList<Renderable> Helpers = new();
    public readonly List<Texture> Textures = new();

    public string Name { get; set; } = "Untitled Model";

    [Export] public ModelEditorState State;
    
    public TreeNode<Renderable>? GetItemById(int id) => Items.GetItemById(id);

    public Model()
    {
        State = new(this);
    }
    
    public bool RemovePart(int id)
    {
        return true;
    }
    public bool HasTextures => Textures.Count != 0;

    public int TotalPartCount => Items.Indicies.Count;
    
    public List<TreeNode<Renderable>> AllParts => Items.Indicies.Values.ToList();

    public static Model Get(Node it)
    {
        return it.Owner != null ? it.Owner.GetMeta("model").As<Model>() : it.GetParent().GetMeta("model").As<Model>();
    }
}

enum ModelType
{
    SMPToolbox,
    PinkDog
}