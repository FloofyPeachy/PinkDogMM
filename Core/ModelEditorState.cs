using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using Godot;
using PinkDogMM_Gd.Core.Commands;
using PinkDogMM_Gd.Core.Schema;
using PinkDogMM_Gd.Render;
using Texture = PinkDogMM_Gd.Core.Schema.Texture;

namespace PinkDogMM_Gd.Core;

public enum EditorMode
{
    Normal,
    Resize,
    Rotate,
    ShapeEdit
}
public partial class ModelEditorState : Resource
{
    public ObservableCollection<Part> SelectedParts = new();
    public ObservableCollection<Renderable> SelectedObjects = new();
    
    public HistoryStack History = new();
    public Camera Camera = new(); //RotationX, RotationY, Zoom
    public List<(int, Part)> Hovering = new();
    public string CurrentTexture;
    public Vector3 HoveredSide = Vector3.Zero;
    public bool IsPeeking = false;

    public int _focusedCorner = 0;

    public int FocusedCorner
    {
        get => _focusedCorner;
        set
        {
            _focusedCorner = value;
            UpdateCamera();
            OnFocusedCornerChanged(_focusedCorner);
        }
    }

    public EditorMode Mode = EditorMode.Normal;
    public event EventHandler<Part>? PartSelected;
    public event EventHandler<Renderable>? ObjectSelected;
    public event EventHandler? AllPartsUnselected;
    public event EventHandler<Part>? PartUnselected;
    public event EventHandler<bool>? IsPeekingChanged;
    public event EventHandler<int>? FocusedCornerChanged;
    
    public event EventHandler<EditorMode>? ModeChanged;
    
    public void SelectPart(Part part)
    {
        SelectedParts.Add(part);
        SelectedObjects.Add(part);
        
        part.PropertyChanged += (sender, args) =>
        {
            UpdateCamera();
        };
        UpdateCamera();
        OnPartSelected(part);
    }

    public void SelectObject(Renderable objec)
    {
        SelectedObjects.Add(objec);
        objec.PropertyChanged += (sender, args) =>
        {
            UpdateCamera();
        };
        UpdateCamera();
        OnObjectSelected(objec);
    }

    public void UpdateCamera()
    {
        if (SelectedParts.Count != 0)
        {
            var pos = PositionOfCorner(SelectedParts.First() as Part);
            
            Camera.Position.X = pos.X;
            Camera.Position.Y = -pos.Y;
            Camera.Position.Z = -pos.Z;
        }
    }

    public Vector3 PositionOfCorner(Part part)
    {
        var extra = Vector3.Zero;
        
        int index = _focusedCorner;

        double GetComponent(double pos, double dim, double offset, float[]? shapeArray, bool addDim)
        {
            double shapeValue = 0;
            if (shapeArray != null && index >= 0 && index < shapeArray.Length)
                shapeValue = shapeArray[index];

            return addDim ? pos + dim + shapeValue + offset
                : pos - shapeValue + offset;
        }

        bool addDimX = _focusedCorner is 1 or 2 or 5 or 6;

        bool addDimY = _focusedCorner is >= 4 and <= 7; 

        bool addDimZ = _focusedCorner is 2 or 3 or 6 or 7;

        Vector3 result = new Vector3(
            (float)GetComponent(part.Position.X, part.Size.X, part.Offset.X, (part is Shapebox shapebox ? shapebox.ShapeboxX.ToArray() : null), addDimX),
            (float)GetComponent(part.Position.Y, part.Size.Y, part.Offset.Y, (part is Shapebox shapebox1 ? shapebox1.ShapeboxY.ToArray() : null), addDimY),
            (float)GetComponent(part.Position.Z, part.Size.Z, part.Offset.Z, (part is Shapebox shapebox2 ? shapebox2.ShapeboxZ.ToArray(): null), addDimZ)
        );

        return result;
    }
    
    public void UnselectPart(Part part)
    {
        SelectedParts.Remove(part);
        UpdateCamera();
        
        OnPartUnselected(part);
    }
    
    public void UnselectAllParts()
    {
        SelectedParts.Clear();
        OnAllPartsUnselected();
        Camera.Position.X = 0;
        Camera.Position.Y = 0;
        Camera.Position.Z = 0;
    }

    public void ToggleShapeEditMode()
    {
        if (Mode != EditorMode.ShapeEdit)
        {
            Mode = EditorMode.ShapeEdit;
           
        }
        else
        {
            Mode =  EditorMode.Normal;
            FocusedCorner = 0; 
        }
     
        OnModeChanged(Mode);
    }
    public void TogglePeek()
    {
        IsPeeking = !IsPeeking;
        OnIsPeekingChanged(IsPeeking);
    }
    protected virtual void OnPartSelected(Part e)
    {
        PartSelected?.Invoke(this, e);
    }

    protected virtual void OnPartUnselected(Part e)
    {
        PartUnselected?.Invoke(this, e);
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
}