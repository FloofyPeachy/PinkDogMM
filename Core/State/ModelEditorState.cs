using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using Godot;
using PinkDogMM_Gd.Core.Actions.All.Tools;
using PinkDogMM_Gd.Core.Commands;
using PinkDogMM_Gd.Core.Schema;
using PinkDogMM_Gd.Render;
using Projection = PinkDogMM_Gd.Render.Projection;
using Texture = PinkDogMM_Gd.Core.Schema.Texture;

namespace PinkDogMM_Gd.Core;

public enum EditorMode
{
    Normal,
    Move,
    Resize,
    Rotate,
    ShapeEdit,
    Paint
}

public enum Axis
{
    X = 0,
    Y = 1,
    Z = 2,
    All = 3
}

public partial class PaintState : Resource
{
    private Color _primaryColor = Colors.White;
    private Color _secondaryColor = Colors.Black;

    public Color SecondaryColor
    {
        get => _secondaryColor;
        set => _secondaryColor = value;
    }

    public Color PrimaryColor
    {
        get => _primaryColor;
        set => _primaryColor = value;
    }
}
[GlobalClass]
public partial class ModelEditorState : Resource
{
    public ObservableCollection<Renderable> SelectedObjects = new();
    public HistoryStack History = new();
    public Model model;
    public Camera Camera; //RotationX, RotationY, Zoom
    public Renderable? Hovering = new();
    public Vector3 WorldMousePosition = Vector3.Zero;
    public Vector3 GridMousePosition = Vector3.Zero;
    public int CurrentTexture = 0;
    public Vector3 HoveredSide = Vector3.Zero;
    public Axis ActiveAxis = Axis.All;
    public Axis HoveredAxis = Axis.All;
    public bool IsPeeking = false;
    public bool IsMoving = false;
    public string BottomText = "Welcome to PDMM!";
    public int _focusedCorner = -1;
    public string CurrentTool = "";
    public PaintState PaintState = new PaintState();

    public int FocusedCorner
    {
        get => _focusedCorner;
        set
        {
            _focusedCorner = value;
            ;
            OnFocusedCornerChanged(_focusedCorner);
        }
    }

    public EditorMode Mode = EditorMode.Normal;

    [Signal]
    public delegate void EditorEventHappenedEventHandler(string eventText);

    [Signal]
    public delegate void MovingObjectEventHandler(bool moving);

    public event EventHandler<(Renderable, bool)>? ObjectSelectionChanged;
    public event EventHandler<string>? BottomTextChanged;
    public event EventHandler<Renderable>? ObjectSelected;
    public event EventHandler? AllPartsUnselected;
    public event EventHandler<Renderable?>? ObjectHoveringChanged;
    public event EventHandler<bool>? IsPeekingChanged;
    public event EventHandler<int>? FocusedCornerChanged;
    public event EventHandler<int> TextureChanged;
    public event EventHandler<bool> ModelReloaded;
    public event EventHandler<EditorMode>? ModeChanged;

    public event EventHandler<string>? ToolChanged;

    public ModelEditorState(Model model)
    {
        Camera = new(this);
        this.model = model;
        History.ActionStarted += (sender, action) =>
        {
            if (action.Item2 is Tool)
            {
                ChangeTool(action.Item1);
            }
        };
    }

    public void SwitchTexture(int idx)
    {
        CurrentTexture = idx;
        OnTextureChanged(idx);
    }

    public void SelectPart(Part part)
    {
        SelectedObjects.Add(part);

        OnObjectSelectionChanged((part, true));
        OnObjectSelected(part);
    }
    public bool IsPartSelected(int id)
    {
        var item = model.GetItemById(id);
        return item != null && SelectedObjects.Contains(item);
    }
    public void SetMoving(bool moving)
    {
        if (this.IsMoving == moving) return;
        this.IsMoving = moving;
        EmitSignalMovingObject(moving);
    }

    public void SelectObject(int id)
    {
        var objec = model!.GetItemById(id)!;
        if (!SelectedObjects.Remove(objec) && !SelectedObjects.Contains(objec))
        {
            SelectedObjects.Add(objec);
        }

        objec.PropertyChanged += (sender, args) => { ; };
        ;

        OnObjectSelectionChanged((objec, true));
    }


    public void ShowEventText(string text)
    {
        OnEditorEventHappened(text);
    }

    public void SwitchCameraMode()
    {
        Camera.SetMode(Camera.Mode == Render.Mode.Orbit ? Render.Mode.Free : Render.Mode.Orbit, Camera.Projection);
        ShowEventText(Camera.Mode.ToString());
        ;
    }

    public void SwitchCameraProjection()
    {
        Camera.SetMode(Camera.Mode,
            Camera.Projection == Projection.Perspective
                ? Projection.Orthogonal
                : Projection.Perspective);
        ShowEventText(Camera.Projection.ToString());
        ;
    }

    public void UnselectObject(Renderable part)
    {
        SelectedObjects.Remove(part);

        OnObjectSelectionChanged((part, false));
    }

    public void HoverOverObject(Renderable? obj)
    {
     
            Hovering = obj;
            OnObjectHoveringChanged(obj);
        
    }


    public void UnselectAll()
    {
        foreach (var selectedPart in SelectedObjects.ToList())
        {
            UnselectObject(selectedPart);
        }

        ;
    }

    public void ChangeMode(EditorMode mode)
    {
        /*Mode = mode;
        OnModeChanged(Mode);
        FocusedCorner = 0;
        if (mode == EditorMode.Normal) History.Finish();*/
    }

    public void ChangeTool(string tool = "Tools/PointerTool")
    {
        OnToolChanged(tool);
        CurrentTool = tool;
    }

    public void ReloadModel(bool full)
    {
        OnModelReloaded(full);
    }

    public void SetPeek(bool peeking)
    {
        IsPeeking = peeking;
        OnIsPeekingChanged(IsPeeking);
    }

    protected virtual void OnAllPartsUnselected()
    {
        AllPartsUnselected?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnIsPeekingChanged(bool e)
    {
        IsPeekingChanged?.Invoke(this, e);
    }

    protected virtual void OnModeChanged(EditorMode e)
    {
        ModeChanged?.Invoke(this, e);
    }

    protected virtual void OnFocusedCornerChanged(int e)
    {
        FocusedCornerChanged?.Invoke(this, e);
    }

    protected virtual void OnObjectSelected(Renderable e)
    {
        ObjectSelected?.Invoke(this, e);
    }


    public void SetBottomText(String text)
    {
        this.BottomText = text;
        OnBottomTextChanged(text);
    }

    protected virtual void OnObjectSelectionChanged((Renderable, bool) e)
    {
        ObjectSelectionChanged?.Invoke(this, e);
    }

    protected virtual void OnObjectHoveringChanged(Renderable? e)
    {
        ObjectHoveringChanged?.Invoke(this, e);
    }

    protected virtual void OnEditorEventHappened(string e)
    {
        EmitSignalEditorEventHappened(e);
    }

    protected virtual void OnBottomTextChanged(string e)
    {
        BottomTextChanged?.Invoke(this, e);
    }

    protected virtual void OnTextureChanged(int e)
    {
        TextureChanged?.Invoke(this, e);
    }

    protected virtual void OnModelReloaded(bool e)
    {
        ModelReloaded?.Invoke(this, e);
    }


    protected virtual void OnToolChanged(string e)
    {
        ToolChanged?.Invoke(this, e);
    }
}