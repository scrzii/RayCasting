using RayCasting.Extenstions;
using System.Numerics;

namespace RayCasting.Models;

internal class BasePlaneIntersector
{
    private const float Eps = 1e-5f;

    public static Vector3? IntersectYZPlane(Vector3 rayStart, Vector3 rayEnd, float x)
    {
        var a = rayEnd - rayStart;
        if (Math.Abs(a.X) < Eps)
        {
            return null;
        }

        return new Vector3
        {
            X = x,
            Y = a.Y * (x - rayEnd.X) / a.X + rayEnd.Y,
            Z = a.Z * (x - rayEnd.X) / a.X + rayEnd.Z
        };
    }

    public static Vector3? IntersectXZPlane(Vector3 rayStart, Vector3 rayEnd, float y)
    {
        var a = rayEnd - rayStart;
        if (Math.Abs(a.Y) < Eps)
        {
            return null;
        }

        return new Vector3
        {
            X = a.X * (y - rayEnd.Y) / a.Y + rayEnd.X,
            Y = y,
            Z = a.Z * (y - rayEnd.Y) / a.Y + rayEnd.Z
        };
    }

    public static Vector3? IntersectXYPlane(Vector3 rayStart, Vector3 rayEnd, float z)
    {
        var a = rayEnd - rayStart;
        if (Math.Abs(a.Z) < Eps)
        {
            return null;
        }

        return new Vector3
        {
            X = a.X * (z - rayEnd.Z) / a.Z + rayEnd.X,
            Y = a.Y * (z - rayEnd.Z) / a.Z + rayEnd.Y,
            Z = z
        };
    }
}
