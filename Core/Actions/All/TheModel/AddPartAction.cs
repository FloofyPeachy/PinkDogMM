using System;
using Godot;
using PinkDogMM_Gd.Core.Commands;
using PinkDogMM_Gd.Core.Configuration;
using PinkDogMM_Gd.Core.Schema;

namespace PinkDogMM_Gd.Core.Actions.All.TheModel;

public class AddPartAction(String group, AppState state) : IAction
{
    
    
    private Part part;
    public void Execute()
    {
        part = new Shapebox();
        part.Id = state.ActiveModel!.TotalPartCount + 1;
        part.Name = "Part " + (state.ActiveModel!.TotalPartCount + 1);
        
        state.ActiveModel!.PartGroups[group].Add(part);
        state.ActiveEditorState?.SelectPart(part);
    }

    public void Undo()
    {
        throw new System.NotImplementedException();
    }

    public string TextPrefix => "Added Part";
    public bool AddToStack => true;
    
    public int DefaultKeys => KeyCombo.KeyAndModifiers((int)Key.A, Modifiers.Shift);
}