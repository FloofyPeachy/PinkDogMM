using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;
using Godot.Collections;
using PinkDogMM_Gd.Core.Commands;
using PinkDogMM_Gd.Core.Configuration;

namespace PinkDogMM_Gd.Core.Actions;

[Tool]
public partial class ActionRegistry : Node
{
    private readonly System.Collections.Generic.Dictionary<string, Type> actions = new System.Collections.Generic.Dictionary<string, Type>();

    private readonly System.Collections.Generic.Dictionary<int, int> keys = new System.Collections.Generic.Dictionary<int, int>();

    public static ActionRegistry Get(Node node)
    {
        return node.GetNode<ActionRegistry>("/root/ActionRegistry");
    }
    
    
    public ActionRegistry()
    {
        Assembly asm = AppDomain.CurrentDomain.GetAssemblies()
            .First(x => x.FullName != null && x.FullName.Contains("PinkDogMM_Gd"));

        var types = asm.GetTypes();
        for (var index = 0; index < types.Length; index++)
        {
            var type = types[index];
            if (type.FullName == null || (!type.FullName.StartsWith("PinkDogMM_Gd.Core.Actions.All"))) continue;
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

    public IAction GetAction(string name)
    {
        if (!actions.TryGetValue(name, out var actionType))
            throw new KeyNotFoundException($"No action registered for key: {name}");
        if (typeof(IAction).IsAssignableFrom(actionType))
        {
            return (IAction)Activator.CreateInstance(actionType, null)!;
        }
        throw new KeyNotFoundException($"No action registered for key: {name}");
    }
    
    public void Redo()
    {
        var appState = GetNode("/root/AppState") as AppState;
        appState!.ActiveEditorState.History.Redo();
    }

    public void Start(string key, Godot.Collections.Dictionary arguments)
    {
       var appState = GetNode("/root/AppState") as AppState;
        arguments.Add("appState", appState);
        if (key.Contains("Tool"))
        {
            arguments.Add("worldRoot", GetNode("/root/Node2D/Panel/VBoxContainer/AreaSwitcher/").GetChild(appState.ActiveModelIndex).GetNode("VBoxContainer/HSplitContainer/RenderArea/MarginContainer/SubViewportContainer/SubViewport/WorldEnvironment/WorldRoot"));
        }

        if (actions.TryGetValue(key, out var actionType))
        {
            if (!typeof(IAction).IsAssignableFrom(actionType)) return;
            var actInstance = (IStagedAction)Activator.CreateInstance(actionType, null)!;
            actInstance.SetArguments(arguments);
            appState!.ActiveEditorState.History.Start(key, actInstance);
            if (key.Contains("Tool"))
            {
                //technically not the best way to do things..
                
            }
        }
        else
        {
            throw new KeyNotFoundException($"No action registered for key: {key}");
        }
    }

    public void Tick(Godot.Collections.Dictionary arguments)
    {
        var appState = GetNode("/root/AppState") as AppState;
        appState.ActiveEditorState.History.Tick(arguments);
    }

    public void Finish()
    {
        var appState = GetNode("/root/AppState") as AppState;
        appState.ActiveEditorState.History.Finish();
    }
    public void Undo()
    {
        var appState = GetNode("/root/AppState") as AppState;
        appState!.ActiveEditorState.History.Undo();
    }

    public void Execute(string key, Godot.Collections.Dictionary arguments)
    {
        var appState = GetNode("/root/AppState") as AppState;
        arguments.Add("appState", appState);

        if (actions.TryGetValue(key, out var actionType))
        {
            if (typeof(IAction).IsAssignableFrom(actionType))
            {
                var actInstance = (IAction)Activator.CreateInstance(actionType, null)!;
                actInstance.SetArguments(arguments);
                appState!.ActiveEditorState.History.Execute(actInstance);
            }
            /*ParameterInfo[] parameters = [];
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
                   // trueArguments.Add(appState);
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
            }*/
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
        {
            if (action.HasMeta("Action") && action.GetMeta("Action").AsString() != "")
            {
                GD.Print("fgdfdg");
                //Execute(action.GetMeta("Action").AsString(), new Godot.Collections.Array());
                return;
            }

            if (action is { Keycode: Key.Z, CtrlPressed: true, Echo: false, ShiftPressed: false }) Undo();
            if (action is { Keycode: Key.Y, CtrlPressed: true, Echo: false }) Redo();
            if (action is { Keycode: Key.Z, CtrlPressed: true, Echo: false, ShiftPressed: true }) Redo();
        }


        if (keys.ContainsKey((int)action.Keycode))
        {
            var index = keys[(int)action.Keycode];
            var appState = GetNode("/root/AppState") as AppState;
            
            Execute(actions.Keys.ToList()[index], new Godot.Collections.Dictionary() {{"model", appState.ActiveModel}, {"byKey" , "true"}});
        }
    }
}