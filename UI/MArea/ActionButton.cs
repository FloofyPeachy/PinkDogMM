using Godot;
using Godot.Collections;
using PinkDogMM_Gd.Core.Actions;
using PinkDogMM_Gd.Core.Actions.All.TheModel;
using PinkDogMM_Gd.Core.Commands;
using PinkDogMM_Gd.Core.Schema;

namespace PinkDogMM_Gd.Scenes;

[GlobalClass]
public partial class ActionButton: Button
{
    [Export] public string ActionPath = "";
    public IAction? ActionRef;
    private ActionRegistry registry = null!;
    private Model model;
    public override void _Ready()
    {
        registry = ActionRegistry.Get(this);
        CustomMinimumSize = new Vector2(35, 35);
        IconAlignment = HorizontalAlignment.Center;
        ExpandIcon = true;
        ActionRef ??= registry.GetAction(ActionPath);
        Icon = GD.Load<CompressedTexture2D>(
            $"res://Assets/placeholders/{ActionRef.Icon}.png");
        Text = "";
        Flat = true;
        model = Model.Get(this);
        model.State.ToolChanged += (sender, tuple) =>
        {
            UpdateTool();
        };
    }

    public void UpdateTool()
    {
        if (ActionPath.StartsWith("Tools/"))
        {
            ButtonPressed = model.State.CurrentTool == ActionPath;
        }
    }

    public override void _Pressed()
    {
        var dict = new Dictionary();
        dict.Add("model", Model.Get(this));
        if (ActionPath.StartsWith("Tools/"))
        {
            registry.Start(ActionPath, dict);
        }
        else
        {
            registry.Execute(ActionPath, dict);
        }
       
    }

    public override GodotObject _MakeCustomTooltip(string forText)
    {
        var makeCustomTooltip = GD.Load<PackedScene>("res://Scenes/Controls/ActionTooltip.tscn").Instantiate<ActionTooltip>();

        makeCustomTooltip.Setup("Action", "Description!!!\nYes we have one.", ActionRef.Icon);
        
        return makeCustomTooltip;
    }

    public override void _Process(double delta)
    {
      
    }
}