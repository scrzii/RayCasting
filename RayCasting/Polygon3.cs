using System.Numerics;

namespace RayCasting;

internal class Polygon3
{
    public Vector3[] Vecrtices { get; set; }
    public Color PolygonColor { get; set; }

    private Polygon3()
    {
        Vecrtices = new Vector3[3];
    }

    public static Polygon3[] ParsePolygons(string data)
    {
        var lines = data.Split(Environment.NewLine, StringSplitOptions.None);
        var result = new List<Polygon3>();
        for (int i = 0; i < lines.Length; i += 4) 
        {
            result.Add(ParsePolygon(lines, i));
        }

        return result.ToArray();
    }

    public static Polygon3 ParsePolygon(string[] lines, int startIndex)
    {
        var result = new Polygon3();
        for (int i = 0; i < 3; i++)
        {
            result.Vecrtices[i] = ParseVector(lines[startIndex + i]);
        }
        result.PolygonColor = ParseColor(lines[startIndex + 3]);

        return result;
    }

    private static Vector3 ParseVector(string data)
    {
        var coords = data.Split()
            .Select(x => float.Parse(x))
            .ToArray();

        return new Vector3(coords);
    }

    private static Color ParseColor(string data)
    {
        var channels = data.Split()
            .Select(x => int.Parse(x))
            .ToArray();

        return Color.FromArgb(channels[0], channels[1], channels[2]);
    }
}
