using RayCasting.Models;
using ScaledBitmapPainter;
using System.Numerics;

namespace RayCasting.Core;

internal class Projector
{
    private List<Polygon3> _polygons;
    private Camera _camera;

    public Size RenderSize { get; private set; }

    public Projector(Camera camera, Size renderSize)
    {
        _polygons = new List<Polygon3>();
        _camera = camera;
        RenderSize = renderSize;
    }

    public void AddPolygon(Polygon3 polygon)
    {
        _polygons.Add(polygon);
    }

    public void AddPolygons(IEnumerable<Polygon3> polygons)
    {
        _polygons.AddRange(polygons);
    }

    public void SetRenderSize(Size renderSize)
    {
        RenderSize = renderSize;
    }

    public Bitmap Render(int width, int height)
    {
        var result = UnsafeBitmap.Create(width, height);
        result.Lock();

#if DEBUG
        FillSync(result);
#else
        FillParallel(result);
#endif

        result.Unlock();
        return result.GetBitmap();
    }

    private void FillSync(UnsafeBitmap bitmap)
    {
        for (int x = 0; x < RenderSize.Width; x++)
        {
            for (int y = 0; y < RenderSize.Height; y++)
            {
                var point = _camera.GetPointForPixel(x, y);
                var color = GetPixelColor(point);
                bitmap.SetPixel(x, y, color);
            }
        }
    }

    private void FillParallel(UnsafeBitmap bitmap)
    {
        Parallel.For(0, RenderSize.Width, x =>
        {
            for (int y = 0; y < RenderSize.Height; y++)
            {
                var point = _camera.GetPointForPixel(x, y);
                var color = GetPixelColor(point);
                bitmap.SetPixel(x, y, color);
            }
        });
    }

    private Color GetPixelColor(Vector3 rayEnd)
    {
        var distances = _polygons.Select(x => (x, x.GetIntersectionDistance(_camera.Position, rayEnd))).Where(x => x.Item2 > 0).ToArray();
        if (!distances.Any())
        {
            return Color.Black;
        }
        return distances.MinBy(x => x.Item2).x.PolygonColor;
    }
}
