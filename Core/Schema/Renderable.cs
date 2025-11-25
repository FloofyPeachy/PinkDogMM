using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Godot;

namespace PinkDogMM_Gd.Core.Schema;

public class Renderable : INotifyPropertyChanged
{
    /*Renderables are back, bitches!!!!*/
    private int _id = 0;
    private string _name;
    
    private Vector3L _position = new Vector3L();
    private Vector3L _size = new Vector3L(1,1,1);
    private Vector3L _rotation = new Vector3L();
    private Aabb? _bounding = null; //This is a little scary. It's null until its rendered. Be careful!!

    public Aabb? Bounding
    {
        get => _bounding;
        set => _bounding = value;
    }
    
    public string Name
    {
        get => _name;
        set => SetField(ref _name, value);
    }
    
    private bool _visible;
    
    private Vector2L textureSizeSize = new Vector2L(0,0);
    public Dictionary<string, object> Extra { get; set; } = new();
    
    
    public Renderable()
    {
        Position.PropertyChanged += (sender, args) => OnPropertyChanged(nameof(Position) + "." + args.PropertyName);
        Size.PropertyChanged += (sender, args) =>
        {
            switch (args.PropertyName)
            {
                case "X":
                    if (Size.X < 0)
                    {
                        
                    }
                    
                    break;
                case "Y":
                    break;
                case "Z":
                    break;
            }
            OnPropertyChanged(nameof(Size) + "." + args.PropertyName);
        };
        
        Rotation.PropertyChanged += (sender, args) => OnPropertyChanged(nameof(Rotation) + "." + args.PropertyName);
        TextureSize.PropertyChanged += (sender, args) => OnPropertyChanged(nameof(TextureSize) + "." + args.PropertyName);
    }

    
    public int Id
    {
        get => _id;
        set => SetField(ref _id, value);
    }
    
    public Vector2L TextureSize
    {
        get => textureSizeSize;
        set => SetField(ref textureSizeSize, value);
    }

    public Vector3L Rotation
    {
        get => _rotation;
        set => SetField(ref _rotation, value);
    }
    public bool Visible
    {
        get => _visible;
        set => SetField(ref _visible, value);
    }
    public Vector3L Size
    {
        get => _size;
        set => SetField(ref _size, value);
    }

    public Vector3L Position
    {
        get => _position;
        set => SetField(ref _position, value);
    }

    
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}