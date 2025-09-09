using System;
using System.Reflection;
using Godot;

namespace PinkDogMM_Gd.Scenes.Controls;

[Tool]
public partial class LinkedControl : Control
{
    [Export] public Node Target { get; set; }
    [Export] public String PropertyName2 { get; set; }
    
    
    public void Update(object? value)
    {
        var propertyInfo =  Target.GetType().GetProperty(PropertyName2)
                            ?? throw new ArgumentException($"Property {PropertyName2} not found");
        
        propertyInfo.SetValue(Target, value);
    }


}