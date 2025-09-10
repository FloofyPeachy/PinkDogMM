using Godot;
using PinkDogMM_Gd.Core.Commands;
using PinkDogMM_Gd.Core.Schema;
using Texture = PinkDogMM_Gd.Core.Schema.Texture;

namespace PinkDogMM_Gd.Core.Actions.All.TheModel;

public class AddHelpframeAction(string path, AppState state) : IAction
{
    public string TextPrefix => "Added Helpframe";
    private Helpframe part;
    public void Execute()
    {
        Image image = Utils.ImageFromFile(path);
         //   /home/peachy/Downloads/siemens_charger_venture_amtrak_cascades.jpg
        state.ActiveModel.Helpers.Add(new Helpframe(new Texture(new Vector2(image.GetWidth(), image.GetHeight()), ImageTexture.CreateFromImage(image))));
        
    }

    public void Undo()
    {
       
    }

    public bool AddToStack => true;
}