using Godot;

namespace PinkDogMM_Gd.Core;

public class MeshPickingHelpers
{
    
    /*
     * shout out to https://tmptesting.godotforums.randommomentania.com/d/30491-how-to-translate-a-world-coordinate-to-uv-coordinate/11
     */
    static public Vector3[] GetTriangle(MeshDataTool dt, int p1i, int p2i, int p3i)
    {
        var p1 = dt.GetVertex(p1i);
        var p2 = dt.GetVertex(p2i);
        var p3 = dt.GetVertex(p3i);
        return new[] { p1, p2, p3 };
    }

    static public  bool EqualsWithEpsilon(Vector3 v1, Vector3 v2, float epsilon)
    {
        return v1.DistanceTo(v2) < epsilon;
    }

    static public  int? GetFace(MeshDataTool meshTool, Node3D meshInstance, Vector3 point, Vector3 normal, float epsilon = 2F)
    {
        int count = meshTool.GetFaceCount();

        for (int idx = 0; idx < count; idx++)
        {
            Vector3 faceNormal = meshTool.GetFaceNormal(idx);
            Vector3 worldNormal = meshInstance.GlobalTransform.Basis * faceNormal;

            if (!EqualsWithEpsilon(worldNormal, normal, epsilon))
                continue;

            Vector3 v1 = meshTool.GetVertex(meshTool.GetFaceVertex(idx, 0));
            Vector3 v2 = meshTool.GetVertex(meshTool.GetFaceVertex(idx, 1));
            Vector3 v3 = meshTool.GetVertex(meshTool.GetFaceVertex(idx, 2));

           
                v1 = meshInstance.GlobalTransform * v1;
                v2 = meshInstance.GlobalTransform * v2;
                v3 = meshInstance.GlobalTransform * v3;
            

            if (IsPointInTriangle(point, v1, v2, v3))
                return idx;
        }

        return null;
    }

    static public  Vector3 Barycentric(Vector3 P, Vector3 A, Vector3 B, Vector3 C)
    {
        Basis mat1 = new Basis(A, B, C);
        float det = mat1.Determinant();

        Basis mat2 = new Basis(P, B, C);
        float alpha = mat2.Determinant() / det;

        Basis mat3 = new Basis(P, C, A);
        float beta = mat3.Determinant() / det;

        float gamma = 1.0f - alpha - beta;
        return new Vector3(alpha, beta, gamma);
    }

    static public  Vector3 CartesianToBary(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 v0 = b - a;
        Vector3 v1 = c - a;
        Vector3 v2 = p - a;

        float d00 = v0.Dot(v0);
        float d01 = v0.Dot(v1);
        float d11 = v1.Dot(v1);
        float d20 = v2.Dot(v0);
        float d21 = v2.Dot(v1);

        float denom = d00 * d11 - d01 * d01;

        float v = (d11 * d20 - d01 * d21) / denom;
        float w = (d00 * d21 - d01 * d20) / denom;
        float u = 1.0f - v - w;

        return new Vector3(u, v, w);
    }

    static public  Vector3 TransferPoint(Basis from, Basis to, Vector3 point)
    {
        Basis transform = to * from.Inverse();
        return transform * point;
    }

    static public  Vector3 BaryToCartesian(Vector3 a, Vector3 b, Vector3 c, Vector3 bary)
    {
        return bary.X * a + bary.Y * b + bary.Z * c;
    }

    static public  bool IsPointInTriangle(Vector3 point, Vector3 v1, Vector3 v2, Vector3 v3)
    {
        Vector3 bc = Barycentric(point, v1, v2, v3);

        if (bc.X < 0 || bc.X > 1) return false;
        if (bc.Y < 0 || bc.Y > 1) return false;
        if (bc.Z < 0 || bc.Z > 1) return false;

        return true;
    }

    static public  Vector2? GetUvCoords(MeshDataTool meshTool, Node3D meshInstance, Vector3 point, Vector3 normal)
    {
        
        int? face = GetFace(meshTool, meshInstance, point, normal);
        if (face == null)
            return null;

        int f = face.Value;

        Vector3 v1 = meshTool.GetVertex(meshTool.GetFaceVertex(f, 0));
        Vector3 v2 = meshTool.GetVertex(meshTool.GetFaceVertex(f, 1));
        Vector3 v3 = meshTool.GetVertex(meshTool.GetFaceVertex(f, 2));

        
            v1 = meshInstance.GlobalTransform * v1;
            v2 = meshInstance.GlobalTransform * v2;
            v3 = meshInstance.GlobalTransform * v3;
        

        Vector3 bc = Barycentric(point, v1, v2, v3);

        Vector2 uv1 = meshTool.GetVertexUV(meshTool.GetFaceVertex(f, 0));
        Vector2 uv2 = meshTool.GetVertexUV(meshTool.GetFaceVertex(f, 1));
        Vector2 uv3 = meshTool.GetVertexUV(meshTool.GetFaceVertex(f, 2));

        return (uv1 * bc.X) + (uv2 * bc.Y) + (uv3 * bc.Z);
    }
}