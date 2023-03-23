using RayCasting.Models;
using System.Numerics;

namespace RayCasting.Extenstions;

internal static class Vector3Extenstion
{
    public static Vector3 RotateAroundX(this Vector3 vector, float angle)
    {
        return vector.RotateAroundX((float)Math.Sin(angle), (float)Math.Cos(angle));
    }

    public static Vector3 RotateAroundX(this Vector3 vector, PrecalculatedAngle angle)
    {
        return vector.RotateAroundX(angle.Sin, angle.Cos);
    }

    public static Vector3 RotateAroundX(this Vector3 vector, float sin, float cos)
    {
        return new Vector3(
            vector.X,
            vector.Y * cos + vector.Z * sin,
            -vector.Y * sin + vector.Z * cos);
    }

    public static Vector3 RotateAroundY(this Vector3 vector, float angle)
    {
        return vector.RotateAroundY((float)Math.Sin(angle), (float)Math.Cos(angle));
    }

    public static Vector3 RotateAroundY(this Vector3 vector, PrecalculatedAngle angle)
    {
        return vector.RotateAroundY(angle.Sin, angle.Cos);
    }

    public static Vector3 RotateAroundY(this Vector3 vector, float sin, float cos)
    {
        return new Vector3(
            vector.X * cos - vector.Z * sin,
            vector.Y,
            vector.X * sin + vector.Z * cos);
    }

    public static Vector3 RotateAroundZ(this Vector3 vector, float angle)
    {
        return vector.RotateAroundZ((float)Math.Sin(angle), (float)Math.Cos(angle));
    }

    public static Vector3 RotateAroundZ(this Vector3 vector, PrecalculatedAngle angle)
    {
        return vector.RotateAroundZ(angle.Sin, angle.Cos);
    }

    public static Vector3 RotateAroundZ(this Vector3 vector, float sin, float cos)
    {
        return new Vector3(
            vector.X * cos + vector.Y * sin,
            -vector.X * sin + vector.Y * cos,
            vector.Z);
    }

    public static Vector3 Normalize(this Vector3 vector)
    {
        return Vector3.Normalize(vector);
    }
}
