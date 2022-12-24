using RayCasting.MathModels;
using ScaledBitmapPainter;
using System.Numerics;

namespace RayCasting;

internal class Scene
{
    private Vector3 _camera;
    private Vector3 _direction;
    private Polygon3[] _polygons;

    private readonly float _screenWidth;
    private readonly Vector3 _verticalAxe;
    private readonly Vector3 _stepLeftDirection;

    private const float ScreenDistance = 1;
    private const float RotationAngle = -(float)Math.PI / 1000;
    private const float Step = 0.5f;

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
        _camera += _direction * Step;
    }

    public void StepBackward()
    {
        _camera -= _direction * Step;
    }

    public void RotateX(int delta)
    {
        _direction = RotateAroundZ(_direction, RotationAngle * delta);
    }

    public void RotateY(int delta)
    {
        var xyAngle = (float)Math.Atan2(_direction.Y, _direction.X);
        _direction = RotateAroundZ(_direction, -xyAngle);
        _direction = RotateAroundY(_direction, RotationAngle * delta);
        _direction = RotateAroundZ(_direction, xyAngle);
    }

    public void StepUp()
    {
        _camera += _verticalAxe;
    }

    public void StepDown()
    {
        _camera -= _verticalAxe;
    }

    public void StepLeft()
    {
        var _leftDirection = -Vector3.Normalize(Vector3.Cross(_verticalAxe, _direction));
        _camera += _leftDirection * Step;
    }

    public void StepRight()
    {
        var _leftDirection = -Vector3.Normalize(Vector3.Cross(_verticalAxe, _direction));
        _camera -= _leftDirection * Step;
    }

    private Vector3 RotateAroundX(Vector3 vector, float angle)
    {
        var sin = (float)Math.Sin(angle);
        var cos = (float)Math.Cos(angle);
        return new Vector3(
            vector.X,
            vector.Y * cos - vector.Z * sin,
            vector.Y * sin + vector.Z * cos);
    }

    private Vector3 RotateAroundY(Vector3 vector, float angle)
    {
        var sin = (float)Math.Sin(angle);
        var cos = (float)Math.Cos(angle);
        return new Vector3(
            vector.X * cos + vector.Z * sin,
            vector.Y,
            vector.Z * cos - vector.X * sin);
    }

    private Vector3 RotateAroundZ(Vector3 vector, float angle)
    {
        var sin = (float)Math.Sin(angle);
        var cos = (float)Math.Cos(angle);
        return new Vector3(
            vector.X * cos - vector.Y * sin,
            vector.X * sin + vector.Y * cos,
            vector.Z);
    }

    public Bitmap Render(int width, int height)
    {
        var result = UnsafeBitmapController.Create(width, height);

        var leftDirection = Vector3.Normalize(Vector3.Cross(_verticalAxe, _direction));
        var topDirection = Vector3.Normalize(Vector3.Cross(leftDirection, _direction));
        var startPoint = _camera + Vector3.Normalize(_direction) * ScreenDistance - leftDirection * _screenWidth / 2 - topDirection * _screenWidth / 2;

        result.Lock();

        Parallel.For(0, width, x =>
        {
            for (int y = 0; y < height; y++)
            {
                var color = GetPixelColor(_camera, startPoint + leftDirection * x / width * _screenWidth + topDirection * y / height * _screenWidth);
                result.SetPixel(x, y, color);
            }
        });

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
        var intersectionResult = GetIntersectionResult(rayStart, rayEnd, triangle);
        if (!intersectionResult.Success)
        {
            return -1;
        }
        var intersectionPoint = new Vector3(intersectionResult.Values);

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

    private CramersMethod3x3Result GetIntersectionResult(Vector3 rayStart, Vector3 rayEnd, Polygon3 triangle)
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

        var matrix = new float[,]
        {
            { alpha, beta, gamma },
            { 0, -r.Z, r.Y },
            { r.Z, 0, -r.X }
        };
        var freeMembers = new float[]
        {
            a.X * alpha + a.Y * beta + a.Z * gamma,
            r.Y * rayStart.Z - r.Z * rayStart.Y,
            r.Z * rayStart.X - r.X * rayStart.Z
        };
        var model = new CramersMethod3x3(matrix, freeMembers);
        return model.Solve();
    }

    private bool IsPointInTriangle(Vector3 target, Vector3 a, Vector3 b, Vector3 c)
    {
        return
            Vector3.Dot(Vector3.Cross(c - a, target - a), Vector3.Cross(target - a, b - a)) > 0
            && Vector3.Dot(Vector3.Cross(a - b, target - b), Vector3.Cross(target - b, c - b)) > 0
            && Vector3.Dot(Vector3.Cross(a - c, target - c), Vector3.Cross(target - c, b - c)) > 0;
    }
}
