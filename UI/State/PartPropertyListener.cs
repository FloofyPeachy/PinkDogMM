using System.Reflection;
using Godot;
using PinkDogMM_Gd.Core;
using PinkDogMM_Gd.Core.Schema;

namespace PinkDogMM_Gd.UI.State;

public partial class PartPropertyListener : Node3D
{
    [Export] private string Field;
    [Export] private string Property;

    private PropertyInfo propertyInfoPart;
    private PropertyInfo propertyInfoControl;
    
    
    private AppState state;
    
    public override void _Ready()
    {
        base._Ready();
        state = GetNode("/root/AppState") as AppState;
        
        propertyInfoPart = typeof(Part).GetProperty(Field);
        propertyInfoControl = typeof(Control).GetProperty(Property);
        
    }
    
    
}