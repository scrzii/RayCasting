using RayCasting.Core;
using RayCasting.Services;
using ScaledBitmapPainter;
using System.Numerics;

namespace RayCasting;

public partial class Form1 : Form
{
    private Projector _scene;
    private CameraControl _control;
    private Camera _camera;
    private ImageChartTransformer _transformer;
    private Point? _center = null;
    private bool _cliped = false;

    private const float VerticalStep = 1;
    private const float HorizontalStep = 0.1f;
    private const float RotationAngle = (float)Math.PI / 1000;

    public Form1()
    {
        InitializeComponent();
        _camera = new Camera(new Vector3(-5, 0, 0), Vector3.UnitX, panel1.Size);
        _control = new CameraControl(_camera);
        _scene = new Projector(_camera, panel1.Size);
        _scene.AddPolygons(TriangleParser.ParsePolygons(EmbeddedResourceManager.SceneObjects));
    }

    private void panel1_Paint(object sender, PaintEventArgs e)
    {
        var start = DateTime.UtcNow;
        e.Graphics.DrawImage(_scene.Render(panel1.Width, panel1.Height), 0, 0);
        Text = $"{panel1.Width * panel1.Height} pixels: {(DateTime.UtcNow - start).TotalMilliseconds}ms";
    }

    private void Form1_KeyDown(object sender, KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.W:
                _control.StepByDirection(HorizontalStep); break;
            case Keys.S:
                _control.StepByDirection(-HorizontalStep); break;
            case Keys.A:
                _control.StepHorizontal(-HorizontalStep); break;
            case Keys.D:
                _control.StepHorizontal(HorizontalStep); break;
            case Keys.Space:
                _control.StepVertical(VerticalStep); break;
            case Keys.ShiftKey:
                _control.StepVertical(-VerticalStep); break;
            case Keys.Escape:
                Unclip();
                break;
        }
        panel1.Invalidate();
    }

    private void Clip()
    {
        Cursor.Position = new Point(Left + panel1.Left + panel1.Width / 2, Top + panel1.Top + panel1.Height / 2);
        Cursor.Hide();
        _cliped = true;
    }

    private void Unclip()
    {
        _cliped = false;
        _center = null;
        Cursor.Show();
    }

    private void panel1_MouseMove(object sender, MouseEventArgs e)
    {
        if (!_cliped)
        {
            return;
        }

        _center ??= e.Location;

        Cursor.Position = new Point(Left + panel1.Left + panel1.Width / 2, Top + panel1.Top + panel1.Height / 2);

        var deltaX = _center.Value.X - e.X;
        var deltaY = _center.Value.Y - e.Y;
        _control.RotateX(-RotationAngle * deltaX);
        _control.RotateY(RotationAngle * deltaY);
        panel1.Invalidate();
    }

    private void panel1_MouseClick(object sender, MouseEventArgs e)
    {
        Clip();
    }

    private void Form1_Resize(object sender, EventArgs e)
    {
        panel1.Size = Size;
        _scene.SetRenderSize(panel1.Size);
        _camera.SetSize(panel1.Size);
    }
}
