using System.ComponentModel;
using Godot;
using Godot.Collections;
using PinkDogMM_Gd.Core;
using PinkDogMM_Gd.Core.Schema;
using PinkDogMM_Gd.UltraBinder;

namespace PinkDogMM_Gd.Scenes.Elements;

public partial class ShapeboxPanel : EventMessenger
{
	
	private Shapebox? selected;

	private Dictionary<string, Array<ModelPropertyBinding>> listeners = [];
	
	public override void _Ready()
	{
		base._Ready();
		
		//Now create the stuff!
		AddChild(new HSeparator());
		for (var index = 0; index < 7; index++)
		{
			var container = new PanelContainer();
			var hbox = new HBoxContainer();
			hbox.AddChild(new Label()
			{
				Text = "P" + (index + 1)
			});
			hbox.GuiInput += @event =>
			{
				GD.Print(@event);
			};

			var rect = new ColorRect();
			var transparent = Consts.cornerToColor[index].Darkened(0.3f);
			transparent.A = 0;
			var material = new ShaderMaterial();
			material.Shader = new Shader()
			{
				Code = "shader_type canvas_item;\n\nuniform sampler2D tex;\n\nvoid fragment()\n{\n    COLOR = texture(tex, UV);\n}"
			};
			material.SetShaderParameter("tex", new GradientTexture2D()
			{
				Gradient = new Gradient()
				{
					Colors =
					[
						Consts.cornerToColor[index].Darkened(0.3f),
						transparent
					],
					
					Offsets = [
						0.0f,
						0.17f,
					]
				}
			});
			
			rect.Material = material;
			
			
			rect.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
			rect.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
			var xBox = new SpinBox();
			xBox.SetMeta("WatchedField", "ShapeboxX." + index);
			var yBox = new SpinBox();
			yBox.SetMeta("WatchedField", "ShapeboxY." + index);
			var zBox = new SpinBox();
			zBox.SetMeta("WatchedField", "ShapeboxZ." + index);
			
			hbox.AddChild(xBox);
			hbox.AddChild(yBox);
			hbox.AddChild(zBox);
			
			container.AddChild(rect);
			container.AddChild(hbox);
			AddChild(container);
			AddChild(new HSeparator());
		}
	}
	
	
}
