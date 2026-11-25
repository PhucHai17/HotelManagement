using System.ComponentModel;
using System.Drawing.Drawing2D;
using Common.Constants;
using Common.Helpers;

namespace UI.Controls;

public class RoundedPanel : Panel
{
    private int _cornerRadius = 10;
    private Color _cardColor = Color.White;
    private Color _borderColor = AppConstants.MauVien;
    private float _borderThickness = 1f;
    private Color _topAccentColor = Color.Transparent;
    private int _topAccentHeight = 4;

    public RoundedPanel()
    {
        SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.SupportsTransparentBackColor,
            true);
        DoubleBuffered = true;
        BorderStyle = BorderStyle.None;
        // Để BackColor thật khớp màu thẻ để control con nền trong suốt ăn theo mượt mà
        base.BackColor = Color.White;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int CornerRadius
    {
        get => _cornerRadius;
        set
        {
            _cornerRadius = Math.Max(0, value);
            Invalidate();
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color CardColor
    {
        get => _cardColor;
        set
        {
            _cardColor = value;
            base.BackColor = value;
            Invalidate();
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color BorderColor
    {
        get => _borderColor;
        set
        {
            _borderColor = value;
            Invalidate();
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public float BorderThickness
    {
        get => _borderThickness;
        set
        {
            _borderThickness = Math.Max(0f, value);
            Invalidate();
        }
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

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int TopAccentHeight
    {
        get => _topAccentHeight;
        set
        {
            _topAccentHeight = Math.Max(0, value);
            Invalidate();
        }
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        // Tô nền ngoài theo control cha trước khi vẽ hình bo góc
        var bg = UIHelper.LayMauNenThucTe(this);
        e.Graphics.Clear(bg);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        if (Width <= 2 || Height <= 2) return;

        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;

        var inset = _borderThickness > 0 ? _borderThickness / 2f : 0f;
        var rect = new RectangleF(inset, inset, Width - _borderThickness, Height - _borderThickness);

        using var path = UIHelper.TaoDuongBoGoc(rect, _cornerRadius);
        using var fillBrush = new SolidBrush(_cardColor);
        g.FillPath(fillBrush, path);

        // Vẽ dải nhấn màu ở cạnh trên nếu là KPI Stat Card
        if (_topAccentColor != Color.Transparent && _topAccentColor.A > 0 && _topAccentHeight > 0)
        {
            var state = g.Save();
            g.SetClip(path);
            using var accentBrush = new SolidBrush(_topAccentColor);
            g.FillRectangle(accentBrush, 0, 0, Width, _topAccentHeight);
            g.Restore(state);
        }

        if (_borderThickness > 0f && _borderColor != Color.Transparent)
        {
            using var pen = new Pen(_borderColor, _borderThickness);
            g.DrawPath(pen, path);
        }
    }
}
