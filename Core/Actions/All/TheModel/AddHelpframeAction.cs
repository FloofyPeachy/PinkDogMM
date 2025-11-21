using System;
using Godot;
using Godot.Collections;
using PinkDogMM_Gd.Core.Commands;
using PinkDogMM_Gd.Core.Schema;
using Texture = PinkDogMM_Gd.Core.Schema.Texture;

namespace PinkDogMM_Gd.Core.Actions.All.TheModel;

public class AddHelpframeAction : IAction
{
    public string Icon => "Added Helpframe";
    private Helpframe part;
    
    private string path;
    private Model model;
    
    public void Execute()
    {
        Image image = Utils.ImageFromFile(path);
         //   /home/peachy/Downloads/siemens_charger_venture_amtrak_cascades.jpg
         model.Helpers.Add(new Helpframe(new Texture(new Vector2(image.GetWidth(), image.GetHeight()), "helpframe", ImageTexture.CreateFromImage(image))));
        
    }



    public void SetArguments(Dictionary arguments)
    {
        path = arguments["path"].AsString() ?? throw new InvalidOperationException();
        model = arguments["model"].As<Model>() ?? throw new InvalidOperationException();
    }

    public void Undo()
    {
       
    }

    public bool AddToStack => true;
}