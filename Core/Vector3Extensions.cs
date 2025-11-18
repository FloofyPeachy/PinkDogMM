

using Godot;

namespace PinkDogMM_Gd.Core;

public static class Vector3Extensions
{
    public static Vector3 MCS(this Vector3 vec)
    {
        return vec * 1 / 16;
    }
    public static Vector3 LH(this Vector3 vec)
    {
        (vec.X, vec.Y, vec.Z) = (vec.Z, -vec.Y, vec.X);
        return vec;
        
        return vec;
    }
    public static Vector3 LHS(this Vector3 vec)
    {
        
        (vec.X, vec.Y, vec.Z) = (vec.Z * 1/16, -vec.Y * 1/16, vec.X * 1/16) ;
        return vec;
    }
    
    public static Vector3 FXZ(this Vector3 vec)
    {
        (vec.X, vec.Y, vec.Z) = (vec.Z, vec.Y, vec.X);
        return vec;
    }
    
    public static Vector3 FY(this Vector3 vec)
    {
      
        return vec;
    }
}