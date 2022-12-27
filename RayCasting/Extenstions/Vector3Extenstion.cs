using System.Numerics;

namespace RayCasting.Extenstions;

internal static class Vector3Extenstion
{
    public static Vector3 RotateAroundX(this Vector3 vector, float angle)
    {
        var sin = (float)Math.Sin(angle);
        var cos = (float)Math.Cos(angle);
        return new Vector3(
            vector.X,
            vector.Y * cos - vector.Z * sin,
            vector.Y * sin + vector.Z * cos);
    }

    public static Vector3 RotateAroundY(this Vector3 vector, float angle)
    {
        var sin = (float)Math.Sin(angle);
        var cos = (float)Math.Cos(angle);
        return new Vector3(
            vector.X * cos + vector.Z * sin,
            vector.Y,
            vector.Z * cos - vector.X * sin);
    }

    public static Vector3 RotateAroundZ(this Vector3 vector, float angle)
    {
        var sin = (float)Math.Sin(angle);
        var cos = (float)Math.Cos(angle);
        return new Vector3(
            vector.X * cos - vector.Y * sin,
            vector.X * sin + vector.Y * cos,
            vector.Z);
    }
}
