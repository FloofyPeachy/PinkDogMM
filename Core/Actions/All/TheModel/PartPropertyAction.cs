using PinkDogMM_Gd.Core.Schema;

namespace PinkDogMM_Gd.Core.Actions.All.TheModel;

public class PartPropertyAction(object target, string propertyName, Part oldValue, Part newValue) : PropertyAction<Part>(target, propertyName, oldValue, newValue)
{
    
}