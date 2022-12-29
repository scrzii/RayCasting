using RayCasting.Extenstions;
using RayCasting.Models;
using ScaledBitmapPainter;
using System.Numerics;

namespace RayCasting.Services;

internal class Scene
{
    private Vector3 _camera;
    private Vector3 _direction;
    private Polygon3[] _polygons;

    private readonly float _screenWidth;
    private float _lastScreenHeight;
    private readonly Vector3 _verticalAxe;
    private readonly Vector3 _stepLeftDirection;

    private const float ScreenDistance = 1;
    private const float RotationAngle = -(float)Math.PI / 1000;
    private const float Step = 0.1f;

    public Scene()
    {
        _camera = new Vector3(-5, 0, 0);
        _direction = new Vector3(1, 0, 0);
        _polygons = TriangleParser.ParsePolygons(EmbeddedResourceManager.SceneObjects);
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
        _direction = _direction.RotateAroundZ(RotationAngle * -delta);
    }

    public void RotateY(int delta)
    {
        var xyAngle = (float)Math.Atan2(_direction.Y, _direction.X);
        _direction = _direction.RotateAroundZ(xyAngle);
        _direction = _direction.RotateAroundY(RotationAngle * -delta);
        _direction = _direction.RotateAroundZ(-xyAngle);
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

    public Bitmap Render(int width, int height)
    {
        var result = UnsafeBitmapController.Create(width, height);

        var leftDirection = Vector3.Normalize(Vector3.Cross(_verticalAxe, _direction));
        var topDirection = Vector3.Normalize(Vector3.Cross(leftDirection, _direction));
        _lastScreenHeight = (float)height / width * _screenWidth;
        var startPoint = _camera + Vector3.Normalize(_direction) * ScreenDistance - leftDirection * _screenWidth / 2 - topDirection * _lastScreenHeight / 2;

        result.Lock();

#if DEBUG
        FillSync(result, width, height, startPoint, leftDirection, topDirection);
#else
        FillParallel(result, width, height, startPoint, leftDirection, topDirection);
#endif

        result.Unlock();

        return result.GetBitmap();
    }

    private void FillSync(UnsafeBitmapController bitmap, int width, int height, Vector3 startPoint, Vector3 leftDirection, Vector3 topDirection)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var pixelPosition = startPoint + leftDirection * x / width * _screenWidth + topDirection * y / height * _lastScreenHeight;
                var color = GetPixelColor(pixelPosition);
                bitmap.SetPixel(x, y, color);
            }
        }
    }

    private void FillParallel(UnsafeBitmapController bitmap, int width, int height, Vector3 startPoint, Vector3 leftDirection, Vector3 topDirection)
    {
        Parallel.For(0, width, x =>
        {
            for (int y = 0; y < height; y++)
            {
                var pixelPosition = startPoint + leftDirection * x / width * _screenWidth + topDirection * y / height * _lastScreenHeight;
                var color = GetPixelColor(pixelPosition);
                bitmap.SetPixel(x, y, color);
            }
        });
    }

    private Color GetPixelColor(Vector3 rayEnd)
    {
        var distances = _polygons.Select(x => (x, x.GetIntersectionDistance(_camera, rayEnd))).Where(x => x.Item2 > 0).ToArray();
        if (!distances.Any())
        {
            return Color.Black;
        }
        return distances.MinBy(x => x.Item2).x.PolygonColor;
    }
}
