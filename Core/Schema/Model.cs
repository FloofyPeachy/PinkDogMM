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

        BigParent.Indicies.Add(BigParent.Indicies.Count, childNode);
        Children.Add(childNode);

        CollectionChanged?.Invoke(this, (true, childNode));
        return childNode;
    }

    public TreeNode<T> Add(TreeNode<T> childNode)
    {
        if (childNode.Value == null)
        {
            foreach (var childNodeChild in childNode.Children)
            {
                if (!BigParent.Indicies.ContainsValue(childNodeChild))
                {
                    BigParent.Indicies.Add(BigParent.Indicies.Count, childNodeChild);
                }
            }
        }

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
    private readonly List<Renderable> Items = new List<Renderable>();
    private readonly Dictionary<int, int> Index = []; //internal id / index
    private readonly Dictionary<int, string> GroupIndex = []; //internal id / group name 
    public readonly Dictionary<string, List<int>> Groups = new Dictionary<string, List<int>>();

    public readonly BubblingObservableList<Renderable> Helpers = new();
    public readonly List<Texture> Textures = new();
    public event EventHandler<(bool, Renderable)> CollectionChanged;
    
    public string Name { get; set; } = "Untitled Model";
    static int NextId = 1;
    static int NextIdGp = -1;
    [Export] public ModelEditorState State;

    public Renderable? GetItemById(int id) => Index.TryGetValue(id, out int value) ? Items[value] : null;

    public Model()
    {
        State = new(this);
    }

    public bool RemovePart(int id)
    {
        return true;
    }

    public bool HasTextures => Textures.Count != 0;

    public int TotalPartCount => Items.Count;
    
    public void Add(Renderable item, string? group = null)
    {
        var id = NextId++;
        
        item.Id = id;
        Items.Add(item);
        AddToGroup(id, group);
        RebuildIndex();
        CollectionChanged?.Invoke(this, (true, item));
    }

    public List<Renderable> GetObjects(string? group = null)
    {
        if (group == null) return Items;
        List<Renderable> results = [];
        for (var index = 0; index < Groups[group].Count; index++)
        {
            var c = Groups[group][index];
            results.Add(GetItemById(c));
        }

        return results;
    }

    public void RebuildIndex()
    {
        Index.Clear();
        for (var i = 0; i < Items.Count; i++)
        {
            var item = Items[i];
            Index.Add(item.Id, i);
        }
        
    }

    public void AddGroup(string name)
    {
        var id = NextIdGp--;
        Groups.Add(name, []);
    }
    
    public void AddToGroup(int id, string? group)
    {
        if (group == null) return;
        if (!Groups.ContainsKey(group)) Groups.Add(group, []);
        Groups[group].Add(id);
    }

    public void RemoveFromGroup(int id, string? group)
    {
        if (group == null) return;
        if (!Groups.TryGetValue(group, out var group1)) return;
        group1.Add(id);
    }
    
    public void Remove(Renderable item)
    {
        Items.Remove(item);
        RebuildIndex();
    }

    public void Remove(int id)
    {
        var item = Items[Index[id]];
        Items.RemoveAt(Index[id]);
        CollectionChanged?.Invoke(this, (true, item));
    }

    public List<Renderable> AllObjects => Items;
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