using RayCasting.Extenstions;
using System.Numerics;

namespace RayCasting.MathModels;

internal class Geometry
{
    private const float Eps = 1e-5f;

    public static double GetIntersectionDistance(Vector3 rayStart, Vector3 rayEnd, Polygon3 triangle)
    {
        var intersectionResult = GetIntersectionResult(rayStart, rayEnd, triangle);
        if (intersectionResult is null)
        {
            return -1;
        }
        var intersectionPoint = (Vector3)intersectionResult;

        if (Vector3.Dot(rayEnd - rayStart, intersectionPoint - rayStart) < 0)
        {
            return -1;
        }

        if (!IsPointInTriangle(intersectionPoint, triangle.Vertices[0], triangle.Vertices[1], triangle.Vertices[2]))
        {
            return -1;
        }

        return (intersectionPoint - rayStart).Length();
    }

    private static Vector3? GetIntersectionResult(Vector3 rayStart, Vector3 rayEnd, Polygon3 triangle)
    {
        var vertices = triangle.Vertices;
        var normal = Vector3.Cross(vertices[1] - vertices[0], vertices[2] - vertices[0]);
        return GetIntersectionPointByYZPlane(rayStart, rayEnd, normal, vertices[0]);
    }

    private static Vector3? GetIntersectionPointByYZPlane(Vector3 rayStart, Vector3 rayEnd, Vector3 normal, Vector3 pointOnPlane)
    {
        var yAngle = (float)Math.Atan2(normal.Z, normal.X);
        var zAngle = (float)Math.Atan2(normal.Y, normal.X);

        rayStart = rayStart.RotateAroundY(-yAngle).RotateAroundZ(-zAngle);
        rayEnd = rayEnd.RotateAroundY(-yAngle).RotateAroundZ(-zAngle);
        var xCoord = pointOnPlane.RotateAroundY(-yAngle).RotateAroundZ(-zAngle).X;

        var result = IntersectYZPlane(rayStart, rayEnd, xCoord);
        if (result is null)
        { 
            return null; 
        }

        return result.Value.RotateAroundZ(zAngle).RotateAroundY(yAngle);
    }

    private static Vector3? IntersectYZPlane(Vector3 rayStart, Vector3 rayEnd, float x)
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

    private static bool IsPointInTriangle(Vector3 target, Vector3 a, Vector3 b, Vector3 c)
    {
        return
            Vector3.Dot(Vector3.Cross(c - a, target - a), Vector3.Cross(target - a, b - a)) > 0
            && Vector3.Dot(Vector3.Cross(a - b, target - b), Vector3.Cross(target - b, c - b)) > 0
            && Vector3.Dot(Vector3.Cross(a - c, target - c), Vector3.Cross(target - c, b - c)) > 0;
    }
}
