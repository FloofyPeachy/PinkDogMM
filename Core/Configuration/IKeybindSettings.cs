using System.Collections.Generic;

namespace PinkDogMM_Gd.Core.Configuration;

public interface IKeybindSettings
{
    IEnumerable<IKeybind> Parts { get; }
}

public enum Modifiers
{
    None  = 0,
    Shift = 1 << 16,
    Ctrl  = 1 << 17,
    Alt   = 1 << 18,
    Meta  = 1 << 19
}

public interface IKeybind
{
    int Key { get; set; }
    string Action { get; set; }
    
    int KeyAndModifiers(int keyCode, Modifiers modifiers)
    {
        return keyCode | (int)modifiers;
    }
    
    int GetKey() => Key & 0xFFFF;
    
    Modifiers GetModifiers() => (Modifiers)(Key & ~0xFFFF);
    
}