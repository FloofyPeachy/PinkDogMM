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
            PL.I.Info("Added binding: " + partKeybind.Action);
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

    public KeyModifiers KeyModifiers
    {
        get => _keyModifiers;
        set => _keyModifiers = value;
    }

    public int _key;
    KeyModifiers _keyModifiers;

    public KeyCombo()
    {
        
    }
    public KeyCombo(int combined)
    {
        this.Key = combined & 0xFFFF;
        this.KeyModifiers = (KeyModifiers)(combined & ~0xFFFF);
    }
    public static int KeyAndModifiers(int keyCode, KeyModifiers keyModifiers)
    {
        return keyCode | (int)keyModifiers;
    }

    public static KeyCombo FromInputKeyEvent(InputEventKey keyEvent)
    {
        return new KeyCombo()
        {
            Key = (int)keyEvent.Keycode,
            KeyModifiers = keyEvent.AltPressed ? KeyModifiers.Alt : KeyModifiers.None,
            
        };
    }
    int GetKey() => Key & 0xFFFF;

    public KeyModifiers GetModifiers() => (KeyModifiers)(Key & ~0xFFFF);
    
    
}