using System;
using System.Reflection;
using PinkDogMM_Gd.Core.Commands;

namespace PinkDogMM_Gd.Core.Actions;

public class PropertyAction<T>(object target, string propertyName, T oldValue, T newValue)
    : IAction
{
    private readonly PropertyInfo propertyInfo = target.GetType().GetProperty(propertyName)
                                                 ?? throw new ArgumentException($"Property {propertyName} not found");

    public string TextPrefix => $"Set {propertyInfo.Name}";
    public int DefaultKeys { get; }

    public void Execute()
    {
        propertyInfo.SetValue(target, newValue);
    }

    public void Undo()
    {
        propertyInfo.SetValue(target, oldValue);
    }

    public bool AddToStack => true;
    public string Id => target.GetType() + "/" + propertyInfo.Name;

    public void SetArguments(params object[] args)
    {
        throw new NotImplementedException();
    }
}
