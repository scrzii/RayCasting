namespace RayCasting.Models;

internal class PrecalculatedAngle
{
    public float Angle { get; private set; }
    public float Sin { get; private set; }
    public float Cos { get; private set; }

    public PrecalculatedAngle(float angle)
    {
        Angle = angle;
        Sin = (float)Math.Sin(angle);
        Cos = (float)Math.Cos(angle);
    }

    public static PrecalculatedAngle operator-(PrecalculatedAngle target)
    {
        return new PrecalculatedAngle
        {
            Angle = -target.Angle,
            Sin = -target.Sin,
            Cos = target.Cos
        };
    }

    private PrecalculatedAngle() { }
}
