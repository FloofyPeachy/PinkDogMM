using System;
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
	private ModelEditorState _state;
	private Dictionary<string, Array<ModelPropertyBinding>> listeners = [];

	private FoldableContainer _trapezoidFoldable;
	private FoldableContainer _flexboxFoldable;
	private FoldableContainer _shapeboxFoldable;
	public FoldableContainer MakeFlexboxFoldable()
	{
		var foldable = new FoldableContainer()
		{
			Title = "Flexbox"
		};
		var vbox = new VBoxContainer();
		
		var dirHbox = new HBoxContainer();
		dirHbox.AddChild(new Label()
		{
			Text = "Direction:" 
		});
		var directionOptions = new OptionButton();
		foreach (var name in Enum.GetNames(typeof(Direction)))
		{
			directionOptions.AddItem(name);
		}
		directionOptions.SetMeta("WatchedField", "Direction");
		dirHbox.AddChild(directionOptions);

		//messy but im tired
		var minXHbox = new HBoxContainer();
		minXHbox.AddChild(new Label()
		{
			Text = "X-:" 
		});
		var spinBox = new SpinBox();
		spinBox.SetMeta("WatchedField", "MinusXDistance");

		minXHbox.AddChild(spinBox);
			
		var plusXHbox = new HBoxContainer();
		plusXHbox.AddChild(new Label()
		{
			Text = "X+:" 
		});
		var spinBox1 = new SpinBox();
		spinBox1.SetMeta("WatchedField", "PlusXDistance");

		plusXHbox.AddChild(spinBox1);
		
		vbox.AddChild(dirHbox);
		vbox.AddChild(minXHbox);
		vbox.AddChild(plusXHbox);
		
		foldable.AddChild(vbox);
		foldable.Visible = false;	
		
		return foldable;
	}
	public FoldableContainer MakeTrapezoidFoldable()
	{
		var foldable = new FoldableContainer()
		{
			Title = "Trapezoid"
		};
		var vbox = new VBoxContainer();
		
		var dirHbox = new HBoxContainer();
		dirHbox.AddChild(new Label()
		{
			Text = "Direction:" 
		});
		var directionOptions = new OptionButton();
		foreach (var name in Enum.GetNames(typeof(Direction)))
		{
			directionOptions.AddItem(name);
		}
		directionOptions.SetMeta("WatchedField", "Direction");
		dirHbox.AddChild(directionOptions);
		
		var disHbox = new HBoxContainer();
		disHbox.AddChild(new Label()
		{
			Text = "Distance:" 
		});
		var spinBox = new SpinBox();
		spinBox.SetMeta("WatchedField", "Distance");
		
		disHbox.AddChild(spinBox);
		
		vbox.AddChild(dirHbox);
		vbox.AddChild(disHbox);
		
		foldable.AddChild(vbox);
		foldable.Visible = false;	
		
		return foldable;
	}
	public FoldableContainer MakeShapeboxFoldable()
	{
		var foldable = new FoldableContainer()
		{
			Title = "Shapebox"
		};
		var vbox = new VBoxContainer();
		for (var index = 0; index < 7; index++)
		{
			var container = new PanelContainer();
			var hbox = new HBoxContainer();
			hbox.AddChild(new Label()
			{
				Text = "P" + (index + 1)
			});
			

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
			xBox.SetMeta("WatchedField", "Corners." + index + ".X");
			var yBox = new SpinBox();
			yBox.SetMeta("WatchedField", "Corners." + index + ".Y");
			var zBox = new SpinBox();
			zBox.SetMeta("WatchedField", "Corners." + index + ".Z");
			
			hbox.AddChild(xBox);
			hbox.AddChild(yBox);
			hbox.AddChild(zBox);
			
			container.AddChild(rect);
			container.AddChild(hbox);
			vbox.AddChild(container);
			
		}

		foldable.AddChild(vbox);
		return foldable;
	}
	public override void _Ready()
	{
		base._Ready();
		
		//Now create the stuff!
		var vbox = new VBoxContainer();
		_trapezoidFoldable = MakeTrapezoidFoldable();
		_shapeboxFoldable = MakeShapeboxFoldable();
		_flexboxFoldable = MakeFlexboxFoldable();
		
		vbox.AddChild(_trapezoidFoldable);
		vbox.AddChild(_flexboxFoldable);
		vbox.AddChild(_shapeboxFoldable);
		
		_state = Model.Get(this).State;
		_state.ObjectSelected += (sender, renderable) =>
		{
			_trapezoidFoldable.Visible = renderable is Trapezoid;
			_flexboxFoldable.Visible = renderable is Flexbox;
			_shapeboxFoldable.Visible = true;
		};
		
		AddChild(vbox);
		
	
		
		
	}
	
	
}
