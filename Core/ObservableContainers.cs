using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Godot;

namespace PinkDogMM_Gd.Core;
interface IBubblingObservable
{
    public event EventHandler<CollectionItemUpdatedArgs> ItemChanged;
    public event EventHandler<NestedItemChanged> NestedItemChanged;
    public event EventHandler<(string, CollectionItemsChanged)> NestedCollectionChanged;
    public event EventHandler<CollectionItemsChanged> ItemsChanged;
}

public class CollectionItemUpdatedArgs : EventArgs
{
    public int Index { get; set; }
    public PropertyChangedEventArgs Args { get; set; }
}

public class CollectionItemsChanged(bool added, object item) : EventArgs
{
    public bool Added { get; } = added;
    public object Item { get; } = item;
}


public class NestedItemChanged : EventArgs
{
    public string Key { get; set; }
    public CollectionItemUpdatedArgs Args;
}

public class BubblingObservableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IBubblingObservable where TKey : notnull
{

    public new void Add(TKey key, TValue value)
    {
        
        base.Add(key, value);
     
        
        if (this[key] is not IBubblingObservable observable) return;
        
        observable.ItemChanged += (sender, args) =>
        {
            OnNestedItemChanged(new NestedItemChanged()
            {
                Key = key.ToString(),
                Args = args
            });
        };

        observable.ItemsChanged += (sender, args) =>
        {
            OnNestedCollectionChanged(key!.ToString(), args);
        };
        
        OnItemsChanged();
    }

    public new bool Remove(TKey key)
    {
        bool removed = base.Remove(key);
        if (removed)
            ItemsChanged?.Invoke(this, null);
        return removed;
    }

    // Add other dictionary modification methods as needed
    protected virtual void OnItemChanged(CollectionItemUpdatedArgs e)  
    {  
        ItemChanged?.Invoke(this, e);  
    }  
    
    
    protected virtual void OnItemsChanged()  
    {  
        ItemsChanged?.Invoke(this, null);  
    }

    protected virtual void OnNestedItemChanged(NestedItemChanged e)
    {
        NestedItemChanged?.Invoke(this, e);
    }
    protected virtual void OnNestedCollectionChanged(string key, CollectionItemsChanged args)
    {
        NestedCollectionChanged?.Invoke(this, (key,args));
    }

    public event EventHandler<CollectionItemUpdatedArgs>? ItemChanged;
    public event EventHandler<NestedItemChanged>? NestedItemChanged;
    public event EventHandler<(string, CollectionItemsChanged)>? NestedCollectionChanged;
    public event EventHandler<CollectionItemsChanged>? ItemsChanged;
}

/*
public class BubblingObservableDictionary<TKey, TValue> : ObservableDictionary<TKey, TValue> where TKey : notnull
{
    public event EventHandler<NotifyCollectionChangedEventArgs> CollectionInsideUpdated;
    public event EventHandler<ItemUpdatedEventArgs>? ItemChanged;
    
    
    public new void Add(TKey key, TValue value) {
        base.Add(key, value);
        if (value is IBubblingObservable observable)
        {
            observable.ItemChanged += (sender, args) =>
            {
                PL.I.Info("Listening to inner 2!!");
                OnItemChanged(sender, args);
            };
            
            observable.
        }

    }
    
    private void OnItemChanged(object? sender, ItemUpdatedEventArgs args)
    {
        ItemChanged?.Invoke(this, args);
    }
    
    void InvokeCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
        // This is the tricky part. Nothing actually changed in our collection, but we
        // have to signify that something did.
        CollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }
}
*/

/*public class BubblingObservableList<T>() : ObservableList<T>, IBubblingObservable
{
    
    public event EventHandler<ItemUpdatedEventArgs>? ItemChanged;
    
    public new void Add(T item) {
       base.Add(item);
        if (item is INotifyPropertyChanged changable)
        {
            changable.PropertyChanged += (sender, args) =>
            {
                PL.I.Info("Listening to inner!!");
                OnItemChanged(sender, 0, args);
            };
        }
        
    }

    private void OnItemChanged(object? sender, int index, PropertyChangedEventArgs e)
    {
        ItemChanged?.Invoke(this, new ItemUpdatedEventArgs()
        {
            Index = index,
            Args = e
        });
    }

    public new bool Remove(T item) {
        if (!base.Remove(item)) return false;
        if (item is INotifyPropertyChanged changable)
        {
            changable.PropertyChanged -= (sender, args) =>
            {
                
                OnItemChanged(sender, 0, args);
            };
        }
        
        
        return true;
    }

 
}*/

[Serializable]
public sealed class BubblingObservableList<T> : ObservableCollection<T>, IBubblingObservable
{
    public string Descriptor => "deez";

    public BubblingObservableList(List<T> items)
    {
        foreach (var item in items)
        {
            Add(item);
        }
    }
    
    public new T this[int index]
    {
        get => base[index];
        set
        {
            if (base[index].Equals(value)) return;
            base[index] = value;
            OnItemChanged(new CollectionItemUpdatedArgs()
            {
                Index = index,
                Args = null
            });
        }
    }
    public BubblingObservableList()
    {
        this.CollectionChanged += MyCollectionChanged;
    }

    public void Subscribe(EventHandler<CollectionItemUpdatedArgs> callback)
    {
        ItemChanged += callback;
    }

    void MyCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
    {

        if (args.Action == NotifyCollectionChangedAction.Add)
        {
            OnItemsChanged(new CollectionItemsChanged(true, args.NewItems[0]));
        }

        if (args.Action == NotifyCollectionChangedAction.Remove)
        {
            OnItemsChanged(new CollectionItemsChanged(false, args.OldItems[0]));
        }

        if (args.NewItems == null) return;
        
        for (var index = 0; index < args.NewItems.Cast<T>().ToArray().Length; index++)
        {
            var newItem = args.NewItems.Cast<T>().ToArray()[index];
            if (newItem is not INotifyPropertyChanged changable) continue;
            var index2 = index;
            changable.PropertyChanged += (sender, args) =>
            {
                //PL.I.Info("Inner updated!!");
                InvokeItemChanged(new CollectionItemUpdatedArgs()
                {
                    Index = index2,
                    Args = args
                });
                
                var index1 = index2;
                /*ItemChanged += (_, itemUpdatedEventArgs) =>
                {
                    OnItemChanged(new ItemUpdatedEventArgs()
                    {
                        Index = index1,
                        Args = itemUpdatedEventArgs.Args
                    });
                  
                };*/
            };
        }
    }

    void InvokeCollectionChanged()
    {
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    

    void InvokeItemChanged(CollectionItemUpdatedArgs e)
    {
        OnItemChanged(e);
    }

    private void OnItemChanged(CollectionItemUpdatedArgs e)  
    {  
        ItemChanged?.Invoke(this, e);  
    }


    private void OnItemsChanged(CollectionItemsChanged e)  
    {  
        ItemsChanged?.Invoke(this, e);  
    }

    public event EventHandler<CollectionItemUpdatedArgs>? ItemChanged;
    public event EventHandler<NestedItemChanged>? NestedItemChanged;
    public event EventHandler<(string, CollectionItemsChanged)>? NestedCollectionChanged;
    public event EventHandler<CollectionItemsChanged>? ItemsChanged;


}

public class Vector3L(Vector3 initialVector) : INotifyPropertyChanged
{
    private Vector3 _vector = initialVector;

    public event PropertyChangedEventHandler? PropertyChanged;

    public Vector3L() : this(Vector3.Zero)
    {
    }

    
    public Vector3L(float x, float y, float z) : this(new Vector3(x, y, z))
    {
    }

    public override string ToString()
    {
        return "(" + X + ", " + Y + ", " + Z + ")";
           
    }

    public float X
    {
        get => _vector.X;
        set
        {
            if (Utils.IsEqualTo(_vector.X, value)) return;
            _vector.X = value;
            OnPropertyChanged(nameof(X));
        }
    }

    public float Y
    {
        get => _vector.Y;
        set
        {
            if (Utils.IsEqualTo(_vector.Y, value)) return;
            _vector.Y = value;
            OnPropertyChanged(nameof(Y));
        }
    }

    public float Z
    {
        get => _vector.Z;
        set
        {
            if (Utils.IsEqualTo(_vector.Z, value)) return;
            _vector.Z = value;
            OnPropertyChanged(nameof(Z));
        }
    }


    public int XI
    {
        get => (int)_vector.X;
        set
        {
            if (Utils.IsEqualTo(_vector.X, value)) return;
            _vector.X = value;
            OnPropertyChanged(nameof(X));
        }
    }
    
    public int YI
    {
        get => (int)_vector.Y;
        set
        {
            if (Utils.IsEqualTo(_vector.Y, value)) return;
            _vector.Y = value;
            OnPropertyChanged(nameof(Y));
        }
    }
    
    public int ZI
    {
        get => (int)_vector.Z;
        set
        {
            if (Utils.IsEqualTo(_vector.Z, value)) return;
            _vector.Z = value;
            OnPropertyChanged(nameof(Z));
        }
    }
    
    public static Vector3L operator +(Vector3L a, Vector3L b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    public static Vector3L operator -(Vector3L a, Vector3L b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    public static Vector3L operator *(Vector3L a, Vector3L b) => new(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
    public static Vector3L operator /(Vector3L a, Vector3L b) => new(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
    public Vector3 Value
    {
        get => _vector;
        set
        {
            if (_vector == value) return;
            _vector = value;
            OnPropertyChanged(nameof(X)); 
            OnPropertyChanged(nameof(Y));
            OnPropertyChanged(nameof(Z));
            OnPropertyChanged(nameof(Value)); 
        }
    }

    
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public Vector3 AsVector3() => _vector;
}


public class Vector2L(Vector2 initialVector) : INotifyPropertyChanged
{
    private Vector2 _vector = initialVector;

    public event PropertyChangedEventHandler? PropertyChanged;

    public Vector2L() : this(Vector2.Zero)
    {
    }

    
    public Vector2L(float x, float y) : this(new Vector2(x, y))
    {
    }
    
    public float X
    {
        get => _vector.X;
        set
        {
            if (Utils.IsEqualTo(_vector.X, value)) return;
            _vector.X = value;
            OnPropertyChanged(nameof(X));
        }
    }

    public float Y
    {
        get => _vector.Y;
        set
        {
            if (Utils.IsEqualTo(_vector.Y, value)) return;
            _vector.Y = value;
            OnPropertyChanged(nameof(Y));
        }
    }
    
    public int XI 
    {
        get => (int)_vector.X;
        set
        {
            if (Utils.IsEqualTo(_vector.X, value)) return;
            _vector.X = value;
            OnPropertyChanged(nameof(X));
        }
    }
    
    public int YI 
    {
        get => (int)_vector.Y;
        set
        {
            if (Utils.IsEqualTo(_vector.Y, value)) return;
            _vector.Y = value;
            OnPropertyChanged(nameof(Y));
        }
    }
    
    public Vector2 Value
    {
        get => _vector;
        set
        {
            if (_vector == value) return;
            _vector = value;
            OnPropertyChanged(nameof(X)); 
            OnPropertyChanged(nameof(Y));
            OnPropertyChanged(nameof(Value)); 
        }
    }

    
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public override string ToString()
    {
        return $"{nameof(X)}: {X}, {nameof(Y)}: {Y}";
    }
}


/*
using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using Godot;

namespace PinkDogMM_Gd.Core;

using Godot;
using System.Collections.Generic;

using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public interface IObservable
{

}

public partial class ObservableArray<T> : INotifyPropertyChanged, IList<T>
{

private readonly List<T> _items = new();

public ObservableArray() { }

public ObservableArray(List<T> items)
{
    _items = items ?? new List<T>();
}

// IList<T> implementation
public T this[int index]
{
    get => _items[index];
    set
    {
        _items[index] = value;
        EmitSignal(Observable.SignalName.Changed);
    }
}

void ListenIfObservable(object item)
{
    switch (item)
    {
        case Observable observable:
            observable.Changed += () =>
            {
                EmitSignal(Observable.SignalName.Changed);
            };
            break;
        case ObservableResource resource:
            resource.Changed += field =>
            {
                EmitSignal(Observable.SignalName.ItemChanged, field);
            };

            resource.ItemChanged += field =>
            {
                EmitSignal(Observable.SignalName.Changed, field);
            };
            break;
    }
}


public int Count => _items.Count;

public bool IsReadOnly => false;

public void Add(T item)
{
    _items.Add(item);
    EmitSignal(Observable.SignalName.Changed);
    ListenIfObservable(item);
}

public void Clear()
{
    if (_items.Count > 0)
    {
        _items.Clear();
        EmitSignal(Observable.SignalName.Changed);
    }
}

public bool Contains(T item) => _items.Contains(item);

public void CopyTo(T[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);

public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

public int IndexOf(T item) => _items.IndexOf(item);

public void Insert(int index, T item)
{
    _items.Insert(index, item);
    EmitSignal(Observable.SignalName.Changed);
}

public bool Remove(T item)
{
    if (_items.Remove(item))
    {
        EmitSignal(Observable.SignalName.Changed);
        return true;
    }
    return false;
}

public void RemoveAt(int index)
{
    _items.RemoveAt(index);
    EmitSignal(Observable.SignalName.Changed);
}

// Optional: expose underlying list if needed
public List<T> Items => _items;
}

public partial class ObservableMap<K, V> : ObservableResource, IEnumerable<KeyValuePair<K, V>>
{
public Dictionary<K, V> Items = new Dictionary<K, V>();


public void Add(K key, V value)
{
    Items.Add(key, value);
    if (value is Observable observable)
    {
        observable.Changed += () =>
        {
            EmitSignal(Observable.SignalName.Changed);
        };
        observable.ItemChanged += (index) =>
        {
            EmitSignal(Observable.SignalName.ItemChanged, index);
        };
    }
    EmitSignal(Observable.SignalName.Changed);
}

public void Remove(K key)
{
    if (Items.Remove(key))
        EmitSignal(Observable.SignalName.Changed);
}

public V this[K key]
{
    get => Items[key];
    set
    {
        Items[key] = value;
        EmitSignal(Observable.SignalName.Changed);
    }
}

public List<K> Keys => Items.Keys.ToList();
public int Count => Items.Count;

// Implement IEnumerable
public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
{
    return Items.GetEnumerator();
}

IEnumerator IEnumerable.GetEnumerator()
{
    return GetEnumerator();
}
}

public partial class ObvsVector3 : ObservableArray<float>
{
public ObvsVector3(float x, float y, float z) : base([x, y, z])
{

}


public ObvsVector3(Vector3 vector) : base(new List<float> { vector.X, vector.Y, vector.Z })
{
}

public ObvsVector3() : base([0, 0, 0])
{

}

public float X
{
    get => this[0];
    set => this[0] = value;
}
public float Y
{
    get => this[1];
    set => this[1] = value;
}
public float Z
{
    get => this[2];
    set => this[2] = value;
}

public Vector3 AsVector3()
{
    return new Vector3(X, Y, Z);
}
}

public partial class ObvsVector3I : ObservableArray<int>
{

public ObvsVector3I(int x, int y, int z) : base(new List<int> { x, y, z })
{
}

public ObvsVector3I(): base([1,1,1])
{
}

public int X
{
    get => this[0];
    set => this[0] = value;
}
public int Y
{
    get => this[1];
    set => this[1] = value;
}
public int Z
{
    get => this[2];
    set => this[2] = value;
}
}

public partial class ObvsVector2I : ObservableArray<int>
{

public ObvsVector2I(int x, int y) : base(new List<int> { x, y })
{
}

public ObvsVector2I() : base([0,0])
{
}

public int X
{
    get => this[0];
    set => this[0] = value;
}
public int Y
{
    get => this[1];
    set => this[1] = value;
}

public Vector2 AsVector2()
{
    return new Vector2(X, Y);
}
}
*/

