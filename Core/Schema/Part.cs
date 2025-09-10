using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Godot;
using PinkDogMM_Gd.Core;

namespace PinkDogMM_Gd.Core.Schema;

public class Part : Renderable
{
    
    private string _name;
    
    private PartTypes _partType;
    
    private Vector3L _offset = new Vector3L();
    
    

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
    

    public Vector3L Offset
    {
        get => _offset;
        set => SetField(ref _offset, value);
    }
    
    
    public override string ToString() => "Part: " + Name + " (" + PartType + ")" + " (" + Position + ", " + Size + ", " + Rotation + ", " + Offset + ", " + TextureSize + ", " + Visible + ")";

    public Part()
    {
        Offset.PropertyChanged += (sender, args) => OnPropertyChanged(nameof(Offset));
    }
   
}

public class Shape : Part
{
    public BubblingObservableList<Vector3L> _sections = [];

    public BubblingObservableList<Vector3L> Sections
    {
        get => _sections;
        set => SetField(ref _sections, value);
    }


    public Shape()
    {
        _sections.ItemChanged  += (sender, args) => OnPropertyChanged(nameof(_sections) + "." + args.Index);
    }
}


public class Shapebox : Shape
{
    private new BubblingObservableList<Vector3L> _sections = new(Enumerable.Repeat(new Vector3L(), 8).ToList());
    public new BubblingObservableList<Vector3L> Sections
    {
        get => _sections;
        set => SetField(ref _sections, value);
    }
    
    public BubblingObservableList<float> _shapeboxX = new BubblingObservableList<float>([0, 0, 0, 0, 0, 0, 0, 0]);
    public BubblingObservableList<float> _shapeboxY = new BubblingObservableList<float>([0, 0, 0, 0, 0, 0, 0, 0]);
    public BubblingObservableList<float> _shapeboxZ = new BubblingObservableList<float>([0, 0, 0, 0, 0, 0, 0, 0]);

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
    
    
    public Shapebox()
    {
        _shapeboxX.ItemChanged += (sender, args) =>
        {
            _sections[args.Index].X = (int)_shapeboxX[args.Index];
        };
        _shapeboxY.ItemChanged += (sender, args) =>
        {
            _sections[args.Index].Y = (int)_shapeboxX[args.Index];
        };
        _shapeboxX.ItemChanged += (sender, args) =>
        {
            _sections[args.Index].Z = (int)_shapeboxX[args.Index];
        };
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
}

/*public partial class Shapebox1 : Part
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
}*/

public enum PartTypes
{
    Box,
    Shapebox
}