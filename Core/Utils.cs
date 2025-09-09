using System;
using System.Collections.Concurrent;
using System.IO;
using Godot;

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