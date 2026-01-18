using Godot;
using System;
using PinkDogMM_Gd.Core.Schema;
using PinkDogMM_Gd.Render;
using PinkDogMM_Gd.Scenes;


public partial class Editor3dHost : SubViewport
{
	private PivotXY2 pivotXy2;

	private Camera? _camera;
	private Model _model;
	public Camera? Camera
	{
		get => _camera;
		set => _camera = value ?? throw new ArgumentNullException(nameof(value));
	}

	public override void _EnterTree()
	{
		_model = Model.Get(this);
		SetMeta("model", _model);
	}

	public override async void _Ready()
	{
		
		pivotXy2 = GetNode<PivotXY2>("WorldEnvironment/WorldRoot/PivotXY2");
		_camera ??= _model.State.Camera;
		
		await ToSignal(RenderingServer.Singleton, RenderingServerInstance.SignalName.FramePostDraw); 
		
		var takeScreenshot = GetTexture().GetImage();
		takeScreenshot.SavePng("result.png");
	}
	
	
}
