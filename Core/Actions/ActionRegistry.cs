using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;
using PinkDogMM_Gd.Core.Commands;
using PinkDogMM_Gd.Core.Configuration;

namespace PinkDogMM_Gd.Core.Actions;

[Tool]
public partial class ActionRegistry : Node
{
    private readonly Dictionary<string, Type> actions = new Dictionary<string, Type>();
    
    private readonly Dictionary<int, int> keys = new Dictionary<int, int>();

    public ActionRegistry()
    {
        Assembly asm = AppDomain.CurrentDomain.GetAssemblies()
            .First(x => x.FullName != null && x.FullName.Contains("PinkDogMM_Gd"));

        var types = asm.GetTypes();
        for (var index = 0; index < types.Length; index++)
        {
            var type = types[index];
            if (type.FullName == null || !type.FullName.StartsWith("PinkDogMM_Gd.Core.Actions.All")) continue;
            var newName = type.FullName.Substring(30).Replace(".", "/").Replace("Action", "");
            actions.Add(newName, type);
            PL.I.Info($"Added action {newName}");

            var keyInt =
                (int)((type.GetProperty("DefaultKeys", BindingFlags.Static | BindingFlags.Public)?.GetValue(null)) ??
                      -1);
            if (keyInt == -1) continue;

            var key = new KeyCombo(keyInt);

            keys.Add(keyInt, actions.Values.ToList().IndexOf(type));
            
            var kevent = new InputEventKey()
            {
                Keycode = (Key)key.Key,
                AltPressed = key.GetModifiers().HasFlag(KeyModifiers.Alt),
                CtrlPressed = key.GetModifiers().HasFlag(KeyModifiers.Ctrl),
                ShiftPressed = key.GetModifiers().HasFlag(KeyModifiers.Shift),
            };

            kevent.SetMeta("Action", newName);
            //TODO: Use Godot system, doesn't seem to work though...
            /*InputMap.Singleton.AddAction(newName);
            InputMap.Singleton.ActionAddEvent(newName, kevent);*/

            PL.I.Info("Added key binding for action: " + kevent);
        }
    }

    public void RunFromKey()
    {
    }

    public void Execute(string key, Godot.Collections.Array arguments)
    {
        var appState = GetNode("/root/AppState") as AppState;
        if (appState == null) return;
        arguments.Add(appState);


        if (actions.TryGetValue(key, out var actionType))
        {
            ParameterInfo[] parameters = [];
            ConstructorInfo? theConstructor;
            // Ensure the type implements IAction
            if (typeof(IAction).IsAssignableFrom(actionType))
            {
                ConstructorInfo[] constructors = actionType.GetConstructors();
                //Check if the action takes the right arguments
                bool match = false;
                List<object> trueArguments = [];

                if (arguments.Count == 1)
                {
                    trueArguments.Add(appState);
                }
                else
                {
                    foreach (var constructor in constructors)
                    {
                        parameters = constructor.GetParameters();
                        theConstructor = constructor;
                        if (parameters.Length != arguments.Count)
                        {
                            match = false;
                            break;
                        }

                        ;
                        for (var i = 0; i < parameters.Length; i++)
                        {
                            if (parameters[i].ParameterType != arguments[i].GetType())
                            {
                                match = false;
                            }

                            ;
                            match = true;
                        }

                        if (!match) throw new ArgumentException("Invalid arguments for action");
                    }

              
                    for (var i = 0; i < arguments.Count; i++)
                    {
                        Godot.Variant variant = arguments[i];
                        trueArguments.Add(typeof(Variant).GetMethod("As").MakeGenericMethod(parameters[i].ParameterType)
                            .Invoke(variant, null));
                    }
                }
                
              
                // Create an instance at runtime
                var actionInstance = (IAction)Activator.CreateInstance(actionType, trueArguments.ToArray())!;
                appState!.ActiveEditorState.History.Execute(actionInstance);
            }
            else
            {
                throw new InvalidOperationException($"{actionType.Name} does not implement IAction.");
            }
        }
        else
        {
            throw new KeyNotFoundException($"No action registered for key: {key}");
        }
    }

    public string GetActionName(IAction action)
    {
        return action.GetType().Name;
    }
    
    public override void _UnhandledKeyInput(InputEvent @event)
    {
        base._UnhandledKeyInput(@event);
        if (@event is not InputEventKey action) return;
        if (action is { Echo: false, Pressed: true })
        if (action.HasMeta("Action") && action.GetMeta("Action").AsString() != "")
        {
           
                Execute(action.GetMeta("Action").AsString(), new Godot.Collections.Array());
            
         
        }

        if (keys.ContainsKey((int)action.Keycode))
        {
            var index = keys[(int)action.Keycode];
            Execute(actions.Keys.ToList()[index], new Godot.Collections.Array());
        }

    } 
}