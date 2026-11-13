
using System.ComponentModel;
using System.Drawing.Drawing2D;
using Common.Constants;

namespace UI.Controls;

// Control thẻ thống kê Dashboard
public class StatCardControl : UserControl
{
    private readonly Label _lblValue;
    private readonly Label _lblTitle;
    private Color _topAccentColor = Color.FromArgb(16, 185, 129);

    public StatCardControl()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer,
            true);
        BackColor = Color.White;
        Size = new Size(220, 85);
        Margin = new Padding(6);
        Padding = new Padding(14, 14, 14, 10);

        _lblTitle = new Label
        {
            Dock = DockStyle.Top,
            Height = 26,
            Font = new Font(AppConstants.FontChuChinh, 10f, FontStyle.Regular),
            ForeColor = Color.FromArgb(100, 116, 139),
            BackColor = Color.Transparent,
            Text = "Tiêu đề"
        };

        _lblValue = new Label
        {
            Dock = DockStyle.Fill,
            Font = new Font(AppConstants.FontChuChinh, 16f, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42),
            BackColor = Color.Transparent,
            Text = "[ ]",
            TextAlign = ContentAlignment.MiddleLeft
        };

        Controls.Add(_lblValue);
        Controls.Add(_lblTitle);
        _lblTitle.SendToBack();
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Title
    {
        get => _lblTitle.Text;
        set => _lblTitle.Text = value;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Value
    {
        get => _lblValue.Text;
        set => _lblValue.Text = value;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color TopAccentColor
    {
        get => _topAccentColor;
        set
        {
            _topAccentColor = value;
            Invalidate();
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        using var pen = new Pen(Color.FromArgb(226, 232, 240), 1.2f);
        g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);

        using var brushTop = new SolidBrush(_topAccentColor);
        g.FillRectangle(brushTop, 1, 1, Width - 2, 5);
    }
}
