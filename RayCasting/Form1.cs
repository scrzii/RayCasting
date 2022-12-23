namespace RayCasting;

public partial class Form1 : Form
{
    private Scene _scene;

    public Form1()
    {
        InitializeComponent();
        _scene = new Scene();
    }

    private void panel1_Paint(object sender, PaintEventArgs e)
    {
        e.Graphics.DrawImage(_scene.Render(panel1.Width, panel1.Height), 0, 0);
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
}
