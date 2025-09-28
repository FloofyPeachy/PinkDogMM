using System.Linq;
using Godot;
using PinkDogMM_Gd.Core;
using PinkDogMM_Gd.Core.Schema;

namespace PinkDogMM_Gd.Scenes;

public partial class ModelArea : HSplitContainer
{
	public override void _EnterTree()
	{
		base._EnterTree();
		var model = (GetNode("/root/AppState") as AppState).addingModel;
		if (model != null) 	this.SetMeta("model", model);
	}
}
