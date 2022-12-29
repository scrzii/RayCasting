using RayCasting.Extenstions;
using System.Numerics;

namespace RayCasting.Models;

internal class Polygon3
{
    public Vector3[] Vertices { get; set; }
    public Color PolygonColor { get; set; }

    private PrecalculatedAngle _aroundYAngle;
    private PrecalculatedAngle _aroundYAngleNegative;
    private PrecalculatedAngle _aroundZAngle;
    private PrecalculatedAngle _aroundZAngleNegative;
    private float _xCoord;

    public Polygon3(Vector3[] vertices)
    {
        Vertices = vertices.ToArray();
        InitTrigonometry();
    }

    private void InitTrigonometry()
    {
        var normal = Vector3.Cross(Vertices[1] - Vertices[0], Vertices[2] - Vertices[0]);

        var yAngle = (float)Math.Atan2(normal.Z, normal.X);
        _aroundYAngle = new PrecalculatedAngle(yAngle);
        _aroundYAngleNegative = -_aroundYAngle;

        normal = normal.RotateAroundY(_aroundYAngleNegative);

        var zAngle = -(float)Math.Atan2(normal.Y, normal.X);
        _aroundZAngle = new PrecalculatedAngle(zAngle);
        _aroundZAngleNegative = -_aroundZAngle;

        var rotatedPoint = RotateToXPlane(Vertices[0]);
        _xCoord = rotatedPoint.X;
    }

    public float GetIntersectionDistance(Vector3 rayStart, Vector3 rayEnd)
    {
        var intersectionResult = IntersectTrianglePlane(rayStart, rayEnd);
        if (
            intersectionResult is null 
            || Vector3.Dot(rayEnd - rayStart, intersectionResult.Value - rayStart) < 0
            || !IsPointInTriangle(intersectionResult.Value))
        {
            return -1;
        }

        return (intersectionResult.Value - rayStart).Length();
    }

    private Vector3? IntersectTrianglePlane(Vector3 rayStart, Vector3 rayEnd)
    {
        rayStart = RotateToXPlane(rayStart);
        rayEnd = RotateToXPlane(rayEnd);
        var result = BasePlaneIntersector.IntersectYZPlane(rayStart, rayEnd, _xCoord);
        if (result is null)
        {
            return null;
        }
        return RotateFromXPlane(result.Value);
    }

    private Vector3 RotateToXPlane(Vector3 vector)
    {
        return vector.RotateAroundY(_aroundYAngleNegative).RotateAroundZ(_aroundZAngleNegative);
    }

    private Vector3 RotateFromXPlane(Vector3 vector)
    {
        return vector.RotateAroundZ(_aroundZAngle).RotateAroundY(_aroundYAngle);
    }

    private bool IsPointInTriangle(Vector3 point)
    {
        return
            Vector3.Dot(Vector3.Cross(Vertices[2] - Vertices[0], point - Vertices[0]), Vector3.Cross(point - Vertices[0], Vertices[1] - Vertices[0])) > 0
            && Vector3.Dot(Vector3.Cross(Vertices[0] - Vertices[1], point - Vertices[1]), Vector3.Cross(point - Vertices[1], Vertices[2] - Vertices[1])) > 0
            && Vector3.Dot(Vector3.Cross(Vertices[0] - Vertices[2], point - Vertices[2]), Vector3.Cross(point - Vertices[2], Vertices[1] - Vertices[2])) > 0;
    }
}
