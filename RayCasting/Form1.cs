namespace RayCasting;

public partial class Form1 : Form
{
    private Scene _scene;
    private Point? _center = null;

    public Form1()
    {
        InitializeComponent();
        _scene = new Scene();
    }

    private void panel1_Paint(object sender, PaintEventArgs e)
    {
        var start = DateTime.UtcNow;
        e.Graphics.DrawImage(_scene.Render(panel1.Width, panel1.Height), 0, 0);
        Text = (DateTime.UtcNow - start).TotalMilliseconds.ToString();
    }

    private void panel1_Click(object sender, EventArgs e)
    {
        panel1.Invalidate();
    }

    private void Form1_KeyPress(object sender, KeyPressEventArgs e)
    {

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
        }
        panel1.Invalidate();
    }

    private void panel1_MouseMove(object sender, MouseEventArgs e)
    {
        Cursor.Position = new Point(Left + panel1.Left + panel1.Width / 2, Top + panel1.Top + panel1.Height / 2);

        if (_center is null)
        {
            return;
        }

        var deltaX = _center.Value.X - e.X;
        var deltaY = _center.Value.Y - e.Y;
        _scene.RotateX(deltaX);
        _scene.RotateY(deltaY);
        panel1.Invalidate();
    }

    private void panel1_MouseClick(object sender, MouseEventArgs e)
    {
        _center = e.Location;
    }
}
