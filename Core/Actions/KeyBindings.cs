using System.Collections.Generic;
using Godot;
using PinkDogMM_Gd.Core.Configuration;

namespace PinkDogMM_Gd.Core.Actions;

public class KeyBindings
{

    public  KeyBindings()
    {
        List<IKeybind> keybinds = new List<IKeybind>();
        FileAccess.GetFileAsString("user://keybinds.json");
        
        /*foreach (var partKeybind in Settings.GetThem().Keybinds.Parts)
        {
            var kevent = new InputEventKey()
            {
                Keycode = Key.A,
                AltPressed = partKeybind.GetModifiers().HasFlag(Modifiers.Alt),
                CtrlPressed = partKeybind.GetModifiers().HasFlag(Modifiers.Ctrl),
                ShiftPressed = partKeybind.GetModifiers().HasFlag(Modifiers.Shift),
            };
            kevent.SetMeta("Action", partKeybind.Action);
            InputMap.Singleton.ActionAddEvent(partKeybind.Action, kevent);
            GD.Print("Added binding: " + partKeybind.Action);
        }*/
    }

    public void DefaultBindings()
    {
        //Part
        
    }
}

public class KeyCombo
{
    public int Key
    {
        get => _key;
        set => _key = value;
    }

    public Modifiers Modifiers
    {
        get => _modifiers;
        set => _modifiers = value;
    }

    public int _key;
    Modifiers _modifiers;

    public KeyCombo(int combined)
    {
        this.Key = combined & 0xFFFF;
        this.Modifiers = (Modifiers)(combined & ~0xFFFF);
    }
    public static int KeyAndModifiers(int keyCode, Modifiers modifiers)
    {
        return keyCode | (int)modifiers;
    }
    
    int GetKey() => Key & 0xFFFF;

    public Modifiers GetModifiers() => (Modifiers)(Key & ~0xFFFF);
    
    
}