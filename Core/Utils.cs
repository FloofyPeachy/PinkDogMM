using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;
using PinkDogMM_Gd.Core.Schema;
using FileAccess = Godot.FileAccess;

namespace PinkDogMM_Gd.Core;

public class Utils
{
    public static byte[] ReadAllBytes(Stream inStream)
    {
        if (inStream is MemoryStream inMemoryStream)
            return inMemoryStream.ToArray();

        using (var outStream = new MemoryStream())
        {
            inStream.CopyTo(outStream);
            return outStream.ToArray();
        }
    }

    /*public static Vector3 AvgPos(List<Renderable> objects, bool CenterOnAnchor, int FocusedCorner)
    {
        if (objects.Count == 0)
            return Vector3.Zero;

        var sum = objects.Aggregate(Vector3.Zero, (current, obj) => current + PositionOfCorner(obj, CenterOnAnchor, FocusedCorner));

        var avg = sum / objects.Count;

        return new Vector3(avg.Z, avg.Y, avg.X);
    }
    
    public static Vector3 PositionOfCorner(Renderable renderable, bool CenterOnAnchor, int FocusedCorner)
    {
        var extra = Vector3.Zero;
        if (!CenterOnAnchor || renderable is not Part part)
        {
            return ((renderable.Position.AsVector3() + renderable.Size.AsVector3()) / 2);
        }
        int index = FocusedCorner;

        double GetComponent(double pos, double dim, double offset, float[]? shapeArray, bool addDim)
        {
            double shapeValue = 0;
            if (shapeArray != null && index >= 0 && index < shapeArray.Length)
                shapeValue = shapeArray[index];

            return addDim ? pos + dim + shapeValue + offset
                : pos - shapeValue + offset;
        }

        bool addDimX =  FocusedCorner is 1 or 2 or 5 or 6;

        bool addDimY =  FocusedCorner is >= 4 and <= 7; 

        bool addDimZ =  FocusedCorner is 2 or 3 or 6 or 7;
        Vector3 result = new Vector3(
            (float)GetComponent(part.Position.X, part.Size.X, part.Offset.X, (part is Shapebox shapebox ? shapebox.ShapeboxX.ToArray() : null), addDimX),
            (float)GetComponent(part.Position.Y, part.Size.Y, part.Offset.Y, (part is Shapebox shapebox1 ? shapebox1.ShapeboxY.ToArray() : null), addDimY),
            (float)GetComponent(part.Position.Z, part.Size.Z, part.Offset.Z, (part is Shapebox shapebox2 ? shapebox2.ShapeboxZ.ToArray(): null), addDimZ)
        );

        return result;
    }*/
    public static Image ImageFromJpgData(Stream data)
    {
        using var reader = new StreamReader(data);
        var imageBytes = ReadAllBytes(reader.BaseStream);
                
        var image = new Image();

        image.LoadJpgFromBuffer(imageBytes);

        return image;
    }
    
    public static Image ImageFromPngData(Stream data)
    {
        using var reader = new StreamReader(data);
        var imageBytes = ReadAllBytes(reader.BaseStream);
                
        var image = new Image();

        image.LoadPngFromBuffer(imageBytes);

        return image;
    }

    public static Image ImageFromFile(String path)
    {
        if (path.EndsWith(".jpg"))
        {
            return ImageFromJpgData(new FileAccessStream(path, FileAccess.ModeFlags.Read));
        }
        else
        {
            return ImageFromPngData(new FileAccessStream(path, FileAccess.ModeFlags.Read));
        }
        
    }

    public static bool IsEqualTo(double a, double b)
    {
        return Math.Abs(a - b) < double.Epsilon;
    }

    
    private static ConcurrentDictionary<Tuple<Type,Type>, bool> cache = new ConcurrentDictionary<Tuple<Type,Type>, bool>();

    public static bool IsSubclassOfRawGeneric(Type toCheck, Type generic) {
        var input = Tuple.Create(toCheck, generic);
        bool isSubclass = cache.GetOrAdd(input, key => IsSubclassOfRawGenericInternal(toCheck, generic));
        return isSubclass;
    }

    private static bool IsSubclassOfRawGenericInternal(Type toCheck, Type generic) {
        while (toCheck != null && toCheck != typeof(object)) {
            var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
            if (generic == cur) {
                return true;
            }
            toCheck = toCheck.BaseType;
        }
        return false;
    }

    public static SurfaceTool MakeSurfaceTool(ArrayMesh mesh)
    {
        var surfaceTool = new SurfaceTool();
        surfaceTool.CreateFrom(mesh, 0); // use surface 0
        return surfaceTool;
    }
}