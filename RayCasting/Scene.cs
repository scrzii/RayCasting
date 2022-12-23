using MatrixAlgo;
using MatrixAlgo.Exceptions;
using MatrixAlgo.Models;
using ScaledBitmapPainter;
using System.Numerics;
using System.Xml.Serialization;

namespace RayCasting;

internal class Scene
{
    private Vector3 _camera;
    private Vector3 _direction;
    private Polygon3[] _polygons;

    private readonly float _screenWidth;
    private readonly Vector3 _verticalAxe;
    private readonly Vector3 _stepLeftDirection;

    private const float _screenDistance = 1;

    public Scene()
    {
        _camera = new Vector3(-5, 0, 0);
        _direction = new Vector3(1, 0, 0);
        _polygons = Polygon3.ParsePolygons(EmbeddedResourceManager.GetFileData(EmbeddedResourceManager.SceneObjects));
        _screenWidth = 1;
        _verticalAxe = new Vector3(0, 0, 1);
        _stepLeftDirection = Vector3.Normalize(Vector3.Cross(_direction, _verticalAxe));
    }

    public void StepForward()
    {
        _camera += _direction;
    }

    public void StepBackward()
    {
        _camera -= _direction;
    }

    public void StepLeft()
    {
        _camera += _stepLeftDirection;
    }

    public void StepRight()
    {
        _camera -= _stepLeftDirection;
    }

    public void StepUp()
    {
        _camera += _verticalAxe;
    }

    public void StepDown()
    {
        _camera -= _verticalAxe;
    }

    public Bitmap Render(int width, int height)
    {
        var result = UnsafeBitmapController.Create(width, height);

        var leftDirection = Vector3.Normalize(Vector3.Cross(_verticalAxe, _direction));
        var topDirection = Vector3.Normalize(Vector3.Cross(leftDirection, _direction));
        var startPoint = _camera + Vector3.Normalize(_direction) * _screenDistance - leftDirection * _screenWidth / 2 - topDirection * _screenWidth / 2;
        
        result.Lock();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var color = GetPixelColor(_camera, startPoint + leftDirection * x / width * _screenWidth + topDirection * y / height * _screenWidth);
                result.SetPixel(x, y, color);
            }
        }

        result.Unlock();

        return result.GetBitmap();
    }

    private Color GetPixelColor(Vector3 rayStart, Vector3 rayEnd)
    {
        var distances = _polygons.Select(x => (x, GetIntersectionDistance(rayStart, rayEnd, x))).Where(x => x.Item2 > 0);
        if (!distances.Any())
        {
            return Color.Black;
        }
        return distances.MinBy(x => x.Item2).x.PolygonColor;
    }

    private double GetIntersectionDistance(Vector3 rayStart, Vector3 rayEnd, Polygon3 triangle)
    {
        Vector3 intersectionPoint;
        try
        {
            intersectionPoint = GetIntersectionPoint(rayStart, rayEnd, triangle);
        }
        catch (InvalidSolutionException)
        {
            return -1;
        }

        if (Vector3.Dot(rayEnd - rayStart, intersectionPoint - rayStart) < 0)
        {
            return -1;
        }

        if (!IsPointInTriangle(intersectionPoint, triangle.Vecrtices[0], triangle.Vecrtices[1], triangle.Vecrtices[2]))
        {
            return -1;
        }

        return (intersectionPoint - rayStart).Length();
    }

    private Vector3 GetIntersectionPoint(Vector3 rayStart, Vector3 rayEnd, Polygon3 triangle)
    {
        var a = triangle.Vecrtices[0];
        var b = triangle.Vecrtices[1];
        var c = triangle.Vecrtices[2];

        var p = b - a;
        var q = c - a;
        var r = rayEnd - rayStart;

        var alpha = p.Y * q.Z - p.Z * q.Y;
        var beta = p.Z * q.X - p.X * q.Z;
        var gamma = p.X * q.Y - p.Y * q.X;

        var matrix = new SuplementedMatrix(3, 3);
        matrix.SetRow(new SuplementedMatrixRow(new double[] { alpha, beta, gamma }, a.X * alpha + a.Y * beta + a.Z * gamma), 0);
        matrix.SetRow(new SuplementedMatrixRow(new double[] { 0, -r.Z, r.Y }, r.Y * rayStart.Z - r.Z * rayStart.Y), 1);
        matrix.SetRow(new SuplementedMatrixRow(new double[] { r.Z, 0, -r.X }, r.Z * rayStart.X - r.X * rayStart.Z), 2);

        return new Vector3(GaussMethod.Solve(matrix).Select(x => (float)x).ToArray());
    }

    private bool IsPointInTriangle(Vector3 target, Vector3 a, Vector3 b, Vector3 c)
    {
        return
            Vector3.Dot(Vector3.Cross(c - a, target - a), Vector3.Cross(target - a, b - a)) > 0
            && Vector3.Dot(Vector3.Cross(a - b, target - b), Vector3.Cross(target - b, c - b)) > 0
            && Vector3.Dot(Vector3.Cross(a - c, target - c), Vector3.Cross(target - c, b - c)) > 0;
    }
}
