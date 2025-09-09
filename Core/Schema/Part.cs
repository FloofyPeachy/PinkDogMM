using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Godot;
using PinkDogMM_Gd.Core;

namespace PinkDogMM_Gd.Core.Schema;

public partial class Part : INotifyPropertyChanged
{
    
    private string _name;

    public int Id { get; set; }

    private PartTypes _partType;

    private Vector3L _position = new Vector3L();
    private Vector3L _size = new Vector3L(1,1,1);
    private Vector3L _rotation = new Vector3L();
    private Vector3L _offset = new Vector3L();

    private Vector2L _texture = new Vector2L(0,0);

    private bool _visible;

    private readonly string uuid = Guid.NewGuid().ToString();
    public string Name
    {
        get => _name;
        set => SetField(ref _name, value);
    }

    public PartTypes PartType
    {
        get => _partType;
        set => SetField(ref _partType, value);
    }

    public Vector3L Position
    {
        get => _position;
        set => SetField(ref _position, value);
    }
    
    public Vector3L Size
    {
        get => _size;
        set => SetField(ref _size, value);
    }
    
    public Vector3L Rotation
    {
        get => _rotation;
        set => SetField(ref _rotation, value);
    }
    
    public Vector3L Offset
    {
        get => _offset;
        set => SetField(ref _offset, value);
    }
    
    public Vector2L Texture
    {
        get => _texture;
        set => SetField(ref _texture, value);
    }
    
    public bool Visible
    {
        get => _visible;
        set => SetField(ref _visible, value);
    }
    
    
    public override string ToString() => "Part: " + Name + " (" + PartType + ")" + " (" + Position + ", " + Size + ", " + Rotation + ", " + Offset + ", " + Texture + ", " + Visible + ")";
    public event PropertyChangedEventHandler? PropertyChanged;

    public Part()
    {
        Position.PropertyChanged += (sender, args) => OnPropertyChanged(nameof(Position));
        Size.PropertyChanged += (sender, args) => OnPropertyChanged(nameof(Size));
        Rotation.PropertyChanged += (sender, args) => OnPropertyChanged(nameof(Rotation));
        Offset.PropertyChanged += (sender, args) => OnPropertyChanged(nameof(Offset));
        Texture.PropertyChanged += (sender, args) => OnPropertyChanged(nameof(Texture));
    }
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        if (value is INotifyPropertyChanged nfChanged)
        {
            nfChanged.PropertyChanged -= ((sender, args) => OnPropertyChanged(propertyName));
        }
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}

public partial class Shapebox : Part
{
    public BubblingObservableList<float> _shapeboxX = new BubblingObservableList<float>([0, 0, 0, 0, 0, 0, 0, 0]);
    public BubblingObservableList<float> _shapeboxY = new BubblingObservableList<float>([0, 0, 0, 0, 0, 0, 0, 0]);
    public BubblingObservableList<float> _shapeboxZ = new BubblingObservableList<float>([0, 0, 0, 0, 0, 0, 0, 0]);


    public Shapebox()
    {
        _shapeboxX.ItemChanged += (sender, args) => OnPropertyChanged(nameof(ShapeboxX) + "." + args.Index);
        _shapeboxY.ItemChanged += (sender, args) => OnPropertyChanged(nameof(ShapeboxY) + "." + args.Index);
        _shapeboxZ.ItemChanged += (sender, args) => OnPropertyChanged(nameof(ShapeboxZ) + "." + args.Index);
        
    }

    public BubblingObservableList<float> ShapeboxX
    {
        get => _shapeboxX;
        set => SetField(ref _shapeboxX, value);
    }
    
    public BubblingObservableList<float> ShapeboxY
    {
        get => _shapeboxY;
        set => SetField(ref _shapeboxY, value);
    }
    
    public BubblingObservableList<float> ShapeboxZ
    {
        get => _shapeboxZ;
        set => SetField(ref _shapeboxZ, value);
    }
    
    public Vector3 GetCornerPosition(int i)
    {
        var x = (i == 1 || i == 2 || i == 5 || i == 6) 
            ? Size.X + Offset.X + ShapeboxX[i] 
            : Offset.X - ShapeboxX[i];

        var y = (i < 4) 
            ? Offset.Y + ShapeboxY[i] 
            : -Size.Y - Offset.Y - ShapeboxY[i];

        var z = (i == 2 || i == 3 || i == 6 || i == 7) 
            ? Size.Z + Offset.Z + ShapeboxZ[i] 
            : Offset.Z - ShapeboxZ[i];

        return new Vector3(x, y, z);
    }

    public override string ToString()
    {
        return
            $"{base.ToString()}, {nameof(_shapeboxX)}: {_shapeboxX}, {nameof(_shapeboxY)}: {_shapeboxY}, {nameof(_shapeboxZ)}: {_shapeboxZ}, {nameof(ShapeboxX)}: {ShapeboxX}, {nameof(ShapeboxY)}: {ShapeboxY}, {nameof(ShapeboxZ)}: {ShapeboxZ}";
    }
}

public enum PartTypes
{
    Box,
    Shapebox
}