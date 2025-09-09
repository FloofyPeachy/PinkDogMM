using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.JavaScript;
using Godot;
using PinkDogMM_Gd.Core;

namespace PinkDogMM_Gd.Core.Schema;

public partial class Model
{
    public delegate void ChangedEventHandler();

    public delegate void PartChangedEventHandler();

    public readonly BubblingObservableDictionary<string, BubblingObservableList<Part>> PartGroups = new();
    public readonly BubblingObservableDictionary<string, Texture> Textures = new();

    public string Name { get; set; } = "Untitled Model";

    public ModelEditorState State = new();

    
    public (string, Part)? GetPartById(int id)
    {
        foreach (var group in PartGroups)
        {
            var part = group.Value.FirstOrDefault(part => part.Id == id);
            if (part != null) return (group.Key, part);
        }
        return (string.Empty, null);
        
    }

    public bool RemovePart(int id)
    {
        var part = GetPartById(id);
        if (part == null) return false;
        PartGroups[part.Value.Item1].Remove(part.Value.Item2);
        return true;
    }
    public bool HasTextures => Textures.Count != 0;

    public int TotalPartCount => PartGroups.Aggregate(0, (current, keyValuePair) => current + keyValuePair.Value.Count);
    
    public List<Part> AllParts => PartGroups.SelectMany(group => group.Value).ToList();
   
}

enum ModelType
{
    SMPToolbox,
    PinkDog
}