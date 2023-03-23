using RayCasting.Extenstions;
using RayCasting.Models;
using System.Numerics;

namespace RayCasting.Core;

internal class Camera
{
    public Vector3 Position { get; private set; }
    public Vector3 Direction { get; private set; }
    public Size RenderSize { get; private set;  }
    public float ScreenWidth { get; private set; } = 1;
    public float ScreenHeight 
    {
        get => ScreenWidth * RenderSize.Height / RenderSize.Width;
    }
    public float ScreenDistance { get; private set; } = 1;

    private Vector3 _topLeftScreenPoint;
    private Vector3 _rightScreenDirection;
    private Vector3 _bottomScreenDirection;

    public Camera(Vector3 position, Vector3 direction, Size screenSize)
    {
        RenderSize = screenSize;

        Position = position;
        SetDirection(direction);
    }

    public void SetSize(Size newSize)
    {
        RenderSize = newSize;
    }

    public void SetPosition(Vector3 position)
    {
        Position = position;
        UpdateScreenVectors();
    }

    public void SetDirection(Vector3 direction)
    {
        Direction = direction.Normalize();
        UpdateScreenVectors();
    }

    public void SetScreenDistance(float distance)
    {
        ScreenDistance = distance;
        UpdateScreenVectors();
    }

    private void UpdateScreenVectors()
    {
        _rightScreenDirection = Vector3.Cross(Direction, Vector3.UnitZ).Normalize();
        _bottomScreenDirection = Vector3.Cross(Direction, _rightScreenDirection).Normalize();
        _topLeftScreenPoint = Position + Direction * ScreenDistance - _rightScreenDirection * ScreenWidth / 2 - _bottomScreenDirection * ScreenHeight / 2;
    }

    public Vector3 GetPointForPixel(int x, int y)
    {
        var horizontalRatio = (float)x / RenderSize.Width;
        var verticalRatio = (float)y / RenderSize.Height;
        return _topLeftScreenPoint + _rightScreenDirection * horizontalRatio * ScreenWidth + _bottomScreenDirection * verticalRatio * ScreenHeight;
    }
}
