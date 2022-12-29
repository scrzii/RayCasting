using RayCasting.Services;
using System.Runtime.CompilerServices;

namespace RayCasting;

public partial class Form1 : Form
{
    private Scene _scene;
    private Point? _center = null;
    private bool _cliped = false;

    public Form1()
    {
        InitializeComponent();
        _scene = new Scene();
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
                _scene.StepForward(); break;
            case Keys.S:
                _scene.StepBackward(); break;
            case Keys.A:
                _scene.StepLeft(); break;
            case Keys.D:
                _scene.StepRight(); break;
            case Keys.Space:
                _scene.StepUp(); break;
            case Keys.ShiftKey:
                _scene.StepDown(); break;
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
        if (_center is null)
        {
            _center = e.Location;
        }

        Cursor.Position = new Point(Left + panel1.Left + panel1.Width / 2, Top + panel1.Top + panel1.Height / 2);

        var deltaX = _center.Value.X - e.X;
        var deltaY = _center.Value.Y - e.Y;
        _scene.RotateX(deltaX);
        _scene.RotateY(deltaY);
        panel1.Invalidate();
    }

    private void panel1_MouseClick(object sender, MouseEventArgs e)
    {
        Clip();
    }

    private void Form1_Resize(object sender, EventArgs e)
    {
        panel1.Width = Width;
        panel1.Height = Height;
    }
}
