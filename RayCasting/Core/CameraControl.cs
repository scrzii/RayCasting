using RayCasting.Extenstions;
using System.Numerics;

namespace RayCasting.Core;

internal class CameraControl
{
    public Camera Camera { get; } 

    public CameraControl(Camera camera)
    {
        Camera = camera;
    }

    public void SetSize(int width, int height)
    {
        SetSize(new Size(width, height));
    }

    public void SetSize(Size newSize)
    {
        Camera.SetSize(newSize);
    }

    public void StepVertical(float delta)
    {
        Camera.SetPosition(Camera.Position + Vector3.UnitZ * delta);
    }

    public void StepHorizontal(float delta)
    {
        Camera.SetPosition(Camera.Position - GetFlatLeft() * delta);
    }

    public void StepByDirection(float delta)
    {
        Camera.SetPosition(Camera.Position + Camera.Direction * delta);
    }

    public void RotateX(float angle)
    {
        Camera.SetDirection(Camera.Direction.RotateAroundZ(angle));
    }

    public void RotateY(float angle)
    {
        var xyAngle = (float)Math.Atan2(Camera.Direction.Y, Camera.Direction.X);
        var buffer = Camera.Direction.RotateAroundZ(xyAngle);
        buffer = buffer.RotateAroundY(angle);
        buffer = buffer.RotateAroundZ(-xyAngle);
        if (Vector3.Dot(Vector3.Cross(Vector3.UnitZ, buffer), Vector3.Cross(Vector3.UnitZ, Camera.Direction)) < 0)
        {
            return;
        }

        Camera.SetDirection(buffer);
    }

    private Vector3 GetFlatLeft()
    {
        var projection = new Vector3(Camera.Direction.X, Camera.Direction.Y, 0);
        return Vector3.Cross(Vector3.UnitZ, projection).Normalize();
    }
}
