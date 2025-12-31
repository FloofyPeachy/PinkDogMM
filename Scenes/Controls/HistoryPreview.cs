using Godot;
using System;
using System.Linq;
using PinkDogMM_Gd.Core.Schema;
using PinkDogMM_Gd.Scenes;

public partial class HistoryPreview : PanelContainer
{
    private Model? model;

    public override void _Ready()
    {
        model = Model.Get(this);
        model.State.History.ActionExecuted += (sender, action) =>
        {
            /*GetNode("HBoxContainer").AddChild(new TextureRect()
            {
                StretchMode = TextureRect.StretchModeEnum.KeepAspectCovered,
                Texture = GD.Load<CompressedTexture2D>(
                    $"res://Assets/placeholders/{action.Icon}.png"),
            });*/
        };

        model.State.History.ActionUndone += (sender, action) =>
        {
            GetNode("HBoxContainer").GetChildren().Last().QueueFree();
        };
    }
}
