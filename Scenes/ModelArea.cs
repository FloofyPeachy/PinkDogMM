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
		VisibilityChanged += () =>
		{

		};
		var model = (GetNode("/root/AppState") as AppState).addingModel;
		if (model != null) 	this.SetMeta("model", model);
	}

	public override void _Ready()
	{
		VisibilityChanged += () =>
		{
			GetNode<SubViewport>("RenderArea/SubViewportContainer/SubViewport").SetUpdateMode(Visible ? SubViewport.UpdateMode.Always : SubViewport.UpdateMode.Disabled);
		};
	}
}
