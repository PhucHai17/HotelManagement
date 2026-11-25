
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Drawing2D;
using Common.Constants;
using Common.Helpers;

namespace UI.Controls;

public class RoundedTextBox : Control
{
    private readonly TextBox _inner;
    private int _cornerRadius = 8;
    private Color _borderColor = AppConstants.MauVienInput;
    private Color _hoverBorderColor = Color.FromArgb(148, 163, 184);
    private Color _focusBorderColor = AppConstants.MauXanhNavy;
    private Color _fillColor = Color.White;
    private int _horizontalPadding = 12;
    private bool _isHovered;
    private bool _isFocused;

    public RoundedTextBox()
    {
        SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.SupportsTransparentBackColor,
            true);
        DoubleBuffered = true;
        base.TabStop = false;
        Size = new Size(300, 38);
        BackColor = Color.White;
        Cursor = Cursors.IBeam;

        _inner = new TextBox
        {
            BorderStyle = BorderStyle.None,
            BackColor = _fillColor,
            ForeColor = AppConstants.MauChuChinh,
            Font = new Font(AppConstants.FontChuChinh, 10.5f, FontStyle.Regular)
        };

        _inner.GotFocus += (_, _) =>
        {
            _isFocused = true;
            Invalidate();
        };
        _inner.LostFocus += (_, _) =>
        {
            _isFocused = false;
            Invalidate();
        };
        _inner.MouseEnter += (_, _) =>
        {
            _isHovered = true;
            Invalidate();
        };
        _inner.MouseLeave += (_, _) =>
        {
            _isHovered = false;
            Invalidate();
        };
        _inner.TextChanged += (_, e) => OnTextChanged(e);
        _inner.KeyDown += (_, e) => OnKeyDown(e);
        _inner.KeyPress += (_, e) => OnKeyPress(e);
        _inner.KeyUp += (_, e) => OnKeyUp(e);

        Controls.Add(_inner);
        Font = new Font(AppConstants.FontChuChinh, 10.5f, FontStyle.Regular);
        LayoutInnerBox();
    }

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [AllowNull]
    public override string Text
    {
        get => _inner.Text;
        set => _inner.Text = value ?? string.Empty;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string PlaceholderText
    {
        get => _inner.PlaceholderText;
        set => _inner.PlaceholderText = value ?? string.Empty;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public char PasswordChar
    {
        get => _inner.PasswordChar;
        set => _inner.PasswordChar = value;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool UseSystemPasswordChar
    {
        get => _inner.UseSystemPasswordChar;
        set => _inner.UseSystemPasswordChar = value;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool ReadOnly
    {
        get => _inner.ReadOnly;
        set
        {
            _inner.ReadOnly = value;
            SyncFillColor();
            Invalidate();
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int MaxLength
    {
        get => _inner.MaxLength;
        set => _inner.MaxLength = value;
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
    public Color FocusBorderColor
    {
        get => _focusBorderColor;
        set
        {
            _focusBorderColor = value;
            Invalidate();
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color FillColor
    {
        get => _fillColor;
        set
        {
            _fillColor = value;
            SyncFillColor();
            Invalidate();
        }
    }

    public TextBox InnerTextBox => _inner;

    public new bool Focus() => _inner.Focus();

    public void Clear() => _inner.Clear();

    public void SelectAll() => _inner.SelectAll();

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        LayoutInnerBox();
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        LayoutInnerBox();
    }

    protected override void OnFontChanged(EventArgs e)
    {
        base.OnFontChanged(e);
        if (_inner != null)
        {
            _inner.Font = Font;
            LayoutInnerBox();
        }
    }

    protected override void OnForeColorChanged(EventArgs e)
    {
        base.OnForeColorChanged(e);
        if (_inner != null)
            _inner.ForeColor = ForeColor;
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        base.OnMouseEnter(e);
        _isHovered = true;
        Invalidate();
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        _isHovered = false;
        Invalidate();
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        _inner.Focus();
    }

    protected override void OnGotFocus(EventArgs e)
    {
        base.OnGotFocus(e);
        _inner.Focus();
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        // Đọc nền thật từ parent để tránh lộ viền góc vuông khi vẽ bo góc
        var bg = UIHelper.LayMauNenThucTe(this);
        e.Graphics.Clear(bg);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        if (Width <= 2 || Height <= 2) return;

        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;

        var fill = EffectiveFillColor();
        var border = _isFocused
            ? _focusBorderColor
            : _isHovered
                ? _hoverBorderColor
                : _borderColor;
        var strokeWidth = _isFocused ? 1.6f : 1f;
        var inset = strokeWidth / 2f + 0.25f;

        var rect = new RectangleF(inset, inset, Width - inset * 2f, Height - inset * 2f);
        using var path = UIHelper.TaoDuongBoGoc(rect, _cornerRadius);
        using var fillBrush = new SolidBrush(fill);
        using var borderPen = new Pen(border, strokeWidth);

        g.FillPath(fillBrush, path);
        g.DrawPath(borderPen, path);
    }

    private Color EffectiveFillColor()
    {
        return !Enabled || _inner.ReadOnly ? Color.FromArgb(248, 250, 252) : _fillColor;
    }

    private void SyncFillColor()
    {
        if (_inner != null)
            _inner.BackColor = EffectiveFillColor();
    }

    private void LayoutInnerBox()
    {
        if (_inner == null) return;
        var textH = Math.Max(18, _inner.PreferredHeight);
        var top = Math.Max(4, (Height - textH) / 2);
        var width = Math.Max(20, Width - _horizontalPadding * 2);
        _inner.SetBounds(_horizontalPadding, top, width, textH);
    }
}
