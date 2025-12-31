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
    
    private PartTypes _partType;
    
    private Vector3L _offset = new Vector3L();
    
    private readonly string uuid = Guid.NewGuid().ToString();

    private bool _cull = false;

    public bool Cull
    {
        get => _cull;
        set => SetField(ref _cull, value);
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
    private BubblingObservableList<Vector3L> _corners = [];

    public BubblingObservableList<Vector3L> Corners
    {
        get => _corners;
        set => SetField(ref _corners, value);
    }


    protected Shape()
    {
        _corners.ItemChanged  += (sender, args) => OnPropertyChanged(nameof(Corners) + "." + args.Index);
    }
}


public class Flexbox : Shapebox
{
    private Direction _direction = Direction.Top;
    private float _minusXDistance = 0.0f;
    private float _plusXDistance = 0.0f;
    
    private float _minusZDistance = 0.0f;
    private float _plusZDistance = 0.0f;


    public float PlusZDistance
    {
        get => _plusZDistance;
        set
        {
            if (value.Equals(_plusZDistance)) return;
            _plusZDistance = value;
            UpdateFlex();
            OnPropertyChanged();
        }
    }

    public float MinusZDistance
    {
        get => _minusZDistance;
        set
        {
            if (value.Equals(_minusZDistance)) return;
            _minusZDistance = value;
            OnPropertyChanged();
        }
    }

    public float PlusXDistance
    {
        get => _plusXDistance;
        set
        {
            if (value.Equals(_plusXDistance)) return;
            _plusXDistance = value;
            UpdateFlex();
            OnPropertyChanged();
        }
    }

    public float MinusXDistance
    {
        get => _minusXDistance;
        set
        {
            if (value.Equals(_minusXDistance)) return;
            _minusXDistance = value;
            UpdateFlex();
            OnPropertyChanged();
        }
    }

    public Direction Direction
    {
        get => _direction;
        set
        {
            if (value == _direction) return;
            _direction = value;
            UpdateFlex();
            OnPropertyChanged();
        }
    }
    record CornerMap(int Index, float X, float Y, float Z);
    
    private void UpdateFlex()
    {
       
        var faceMap = new Dictionary<Direction, CornerMap[]>
        {
            {
                Direction.Top,
                [
                    new CornerMap(0, _minusXDistance, 0, _minusZDistance),
                    new CornerMap(1, _plusXDistance, 0, _minusZDistance),
                    new CornerMap(2, _plusXDistance, 0, _plusZDistance),
                    new CornerMap(3, _minusXDistance, 0, _plusZDistance)
                ]
            },

            {
                Direction.Bottom,
                [
                    new CornerMap(4, _minusXDistance, 0, _minusZDistance),
                    new CornerMap(5, _plusXDistance, 0, _minusZDistance),
                    new CornerMap(6, _plusXDistance, 0, _plusZDistance),
                    new CornerMap(7, _minusXDistance, 0, _plusZDistance)
                ]
            },

            {
                Direction.Left,
                [
                    new CornerMap(1, 0, _minusXDistance, _minusZDistance),
                    new CornerMap(2, 0, _minusXDistance, _plusZDistance),
                    new CornerMap(5, 0, _plusXDistance, _minusZDistance),
                    new CornerMap(6, 0, _plusXDistance, _plusZDistance)
                ]
            },

            {
                Direction.Right,
                [
                    new CornerMap(0, 0, _minusXDistance, _minusZDistance),
                    new CornerMap(3, 0, _minusXDistance, _plusZDistance),
                    new CornerMap(4, 0, _plusXDistance, _minusZDistance),
                    new CornerMap(7, 0, _plusXDistance, _plusZDistance)
                ]
            },
            
            {
                Direction.Front,
                [
                    new CornerMap(0, _plusZDistance, _minusXDistance, 0),
                    new CornerMap(1, _minusZDistance, _minusXDistance, 0),
                    new CornerMap(4, _plusZDistance, _plusXDistance, 0),
                    new CornerMap(5, _minusZDistance, _plusXDistance, 0)
                ]
            },

            {
                Direction.Back,
                [
                    new CornerMap(2, _minusZDistance, _minusXDistance, 0),
                    new CornerMap(3, _plusZDistance, _minusXDistance, 0),
                    new CornerMap(6, _minusZDistance, _plusXDistance, 0),
                    new CornerMap(7, _plusZDistance, _plusXDistance, 0)
                ]
            }
            
            
        };

        if (!faceMap.TryGetValue(_direction, out var corners)) return;
        foreach (var vector3L in Corners)
        {
            vector3L.X = 0;
            vector3L.Y = 0;
            vector3L.Z = 0;
        }
        foreach (var c in corners)
        {
            Corners[c.Index].X = c.X;
            Corners[c.Index].Y = c.Y;
            Corners[c.Index].Z = c.Z;
        }
    }
}

public class Trapezoid : Shapebox
{
    private Direction _direction = Direction.Top;
    private float _distance = 0.0f;
    private bool _updatingZoid = false;
    private Dictionary<Direction, int[]> _trapMap = new()
    {
        { Direction.Top, [0, 1, 2, 3] },
        { Direction.Bottom, [4, 5, 6, 7] },
        { Direction.Left, [1, 2, 5, 6] },
        { Direction.Right, [0, 3, 4, 7] },
        { Direction.Front, [0, 1, 4, 5] },
        { Direction.Back, [2, 3, 6, 7] }
    };
    
    public Direction Direction
    {
        get => _direction;
        set
        {
            if (value == _direction) return;
            _direction = value;
            UpdateZoid();
            OnPropertyChanged();
        }
    }

    public float Distance
    {
        get => _distance;
        set
        {
            if (value.Equals(_distance)) return;
            _distance = value;
            UpdateZoid();
            OnPropertyChanged();
        }
    }

    public void UpdateZoid()
    {
        _updatingZoid = true;
        if (!_trapMap.TryGetValue(_direction, out var indices)) return;
        
        //reset the corners...
        foreach (var vector3L in Corners)
        {
            vector3L.X = 0;
            vector3L.Y = 0;
            vector3L.Z = 0;
        }
        foreach (var i in indices)
        {
            Corners[i].X = _distance;
            Corners[i].Y = _distance;
            Corners[i].Z = _distance;
        }
        _updatingZoid = false;
    }

    
}


public class Shapebox : Shape
{
    
    private Direction _direction = Direction.None;
    
    public Shapebox()
    {
        for (int i = 0; i < 8; i++)
        {
            Corners.Add(new Vector3L());
        }
        Corners.ItemChanged  += (sender, args) =>
        {
          
            OnPropertyChanged(nameof(Corners) + "." + args.Index);
        };
      
        /*_shapeboxX.ItemChanged += (sender, args) =>
        {
            _sections[args.Index].X = (int)_shapeboxX[args.Index];
            Dictionary<Direction, int[]> faceMap = new()
            {
                { Direction.Top, [0, 1, 2, 3] },
                { Direction.Bottom, [4, 5, 6, 7] },
                { Direction.Left, [1, 2, 5, 6] },
                { Direction.Right, [0, 3, 4, 7] },
                { Direction.Front, [0, 1, 4, 5] },
                { Direction.Back, [2, 3, 6, 7] }
            };
            if (_trapezoid)
            {
                if (!faceMap.TryGetValue(_direction, out var indices)) return;
                foreach (var i in indices)
                {
                    _shapeboxX[i] = _shapeboxX[args.Index];
                    _shapeboxY[i] = _shapeboxX[args.Index];
                    _shapeboxZ[i] = _shapeboxX[args.Index];
                }
                return;
            }
            OnPropertyChanged("ShapeboxX"+ "." + args.Index);
        };
        _shapeboxY.ItemChanged += (sender, args) =>
        {
            _sections[args.Index].Y = (int)_shapeboxX[args.Index];
            OnPropertyChanged("ShapeboxY"+ "." + args.Index);
        };
        _shapeboxZ.ItemChanged += (sender, args) =>
        {
            _sections[args.Index].Z = (int)_shapeboxZ[args.Index];
            OnPropertyChanged("ShapeboxZ"+ "." + args.Index);
        };*/
    }

    public void SetCorner(Axis axis, int corner, float value)
    {
       
        
        /*switch (axis)
        {
            case Axis.X:
               _shapeboxX[corner] = value;
                break;
            case Axis.Y:
                _shapeboxY[corner] = value;
                break;
            case Axis.Z:
                _shapeboxZ[corner] = value;
                break;
            case Axis.All:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
        }*/
    }
    
    public Vector3 GetCornerPosition(int i)
    {
        var x = i is 1 or 2 or 5 or 6 
            ? Size.X + Offset.X + Corners[i].X 
            : Offset.X - Corners[i].X ;

        var y = (i < 4) 
            ? Offset.Y + Corners[i].Y  
            : -Size.Y - Offset.Y - Corners[i].Y ;

        var z = (i == 2 || i == 3 || i == 6 || i == 7) 
            ? Size.Z + Offset.Z + Corners[i].Z  
            : Offset.Z - Corners[i].Z ;

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

public enum Direction
{
    None,
    Top,
    Bottom,
    Left,
    Right,
    Front,
    Back
}