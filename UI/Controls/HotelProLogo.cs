using System.ComponentModel;
using Common.Constants;

namespace UI.Controls;

public class HotelProLogo : Control
{
    private const string LeftText = "HOTEL";
    private const string RightText = "PRO";
    private ContentAlignment _textAlign = ContentAlignment.MiddleLeft;
    private Color? _accentColor;

    public HotelProLogo()
    {
        SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.SupportsTransparentBackColor,
            true);
        DoubleBuffered = true;
        AutoSize = true;
        TabStop = false;
        BackColor = Color.Transparent;
        ForeColor = Color.White;
        Font = new Font(AppConstants.FontChuChinh, 16f, FontStyle.Bold);
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ContentAlignment TextAlign
    {
        get => _textAlign;
        set
        {
            if (_textAlign == value) return;
            _textAlign = value;
            Invalidate();
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color AccentColor
    {
        get => _accentColor ?? AppConstants.MauVangCam;
        set
        {
            _accentColor = value;
            Invalidate();
        }
    }

    public override Size GetPreferredSize(Size proposedSize)
    {
        using var g = CreateGraphics();
        var (w, h) = MeasureLogo(g);
        return new Size(w + Padding.Horizontal, h + Padding.Vertical);
    }

    protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
    {
        if (AutoSize)
        {
            var pref = GetPreferredSize(Size.Empty);
            width = pref.Width;
            height = pref.Height;
        }
        base.SetBoundsCore(x, y, width, height, specified);
    }

    protected override void OnFontChanged(EventArgs e)
    {
        base.OnFontChanged(e);
        if (AutoSize) Size = GetPreferredSize(Size.Empty);
        Invalidate();
    }

    protected override void OnPaddingChanged(EventArgs e)
    {
        base.OnPaddingChanged(e);
        if (AutoSize) Size = GetPreferredSize(Size.Empty);
        Invalidate();
    }

    private (int Width, int Height) MeasureLogo(IDeviceContext dc)
    {
        const TextFormatFlags flags = TextFormatFlags.NoPadding | TextFormatFlags.SingleLine;
        var leftSize = TextRenderer.MeasureText(dc, LeftText, Font, Size.Empty, flags);
        var rightSize = TextRenderer.MeasureText(dc, RightText, Font, Size.Empty, flags);
        // Cộng khoảng dự phòng nhỏ để ký tự cuối không bị cắt ở DPI cao
        return (leftSize.Width + rightSize.Width + 6, Math.Max(leftSize.Height, rightSize.Height) + 2);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        const TextFormatFlags flags = TextFormatFlags.NoPadding | TextFormatFlags.SingleLine;
        var leftSize = TextRenderer.MeasureText(e.Graphics, LeftText, Font, Size.Empty, flags);
        var rightSize = TextRenderer.MeasureText(e.Graphics, RightText, Font, Size.Empty, flags);
        var totalW = leftSize.Width + rightSize.Width;
        var totalH = Math.Max(leftSize.Height, rightSize.Height);

        var area = new Rectangle(
            Padding.Left,
            Padding.Top,
            Math.Max(0, Width - Padding.Horizontal),
            Math.Max(0, Height - Padding.Vertical));

        var x = _textAlign is ContentAlignment.TopCenter or ContentAlignment.MiddleCenter or ContentAlignment.BottomCenter
            ? area.X + Math.Max(0, (area.Width - totalW) / 2)
            : _textAlign is ContentAlignment.TopRight or ContentAlignment.MiddleRight or ContentAlignment.BottomRight
                ? area.Right - totalW
                : area.X;

        var y = _textAlign is ContentAlignment.MiddleLeft or ContentAlignment.MiddleCenter or ContentAlignment.MiddleRight
            ? area.Y + Math.Max(0, (area.Height - totalH) / 2)
            : _textAlign is ContentAlignment.BottomLeft or ContentAlignment.BottomCenter or ContentAlignment.BottomRight
                ? area.Bottom - totalH
                : area.Y;

        TextRenderer.DrawText(e.Graphics, LeftText, Font, new Point(x, y), ForeColor, flags);
        TextRenderer.DrawText(e.Graphics, RightText, Font, new Point(x + leftSize.Width, y), AccentColor, flags);
    }
}
