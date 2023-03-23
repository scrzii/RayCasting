using System.Numerics;

namespace RayCasting.Models;

public class Ray3
{
    public Vector3 Start { get; set; }
    public Vector3 End { get; set; }

    public Ray3(Vector3 start, Vector3 end)
    {
        Start = start;
        End = end;
    }
}
