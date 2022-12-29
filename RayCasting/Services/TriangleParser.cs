using RayCasting.Extenstions;
using RayCasting.Models;
using System.Numerics;

namespace RayCasting.Services;

internal class TriangleParser
{
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
        var vertices = new Vector3[3];
        for (int i = 0; i < 3; i++)
        {
            vertices[i] = ParseVector(lines[startIndex + i]);
            vertices[i] = vertices[i].RotateAroundZ((float)Math.PI / 18);
        }
        var result = new Polygon3(vertices);
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
