

using System.ComponentModel;
using System.Drawing.Drawing2D;
using Common.Constants;

namespace UI.Controls;

// Biểu đồ cột đơn giản dùng GDI+
public class SimpleBarChartControl : UserControl
{
    private readonly List<KeyValuePair<string, double>> _data = new();
    private Color _barColor = AppConstants.MauVangCam;

    public SimpleBarChartControl()
    {
        DoubleBuffered = true;
        BackColor = Color.White;
        Font = new Font(AppConstants.FontChuChinh, 10f, FontStyle.Regular);
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color BarColor
    {
        get => _barColor;
        set
        {
            _barColor = value;
            Invalidate();
        }
    }

    public void UpdateData(IEnumerable<KeyValuePair<string, double>> data)
    {
        _data.Clear();
        _data.AddRange(data);
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        if (_data.Count == 0) return;

        var paddingLeft = 35;
        var paddingRight = 25;
        var paddingTop = 25;
        var paddingBottom = 40;

        var chartWidth = Width - paddingLeft - paddingRight;
        var chartHeight = Height - paddingTop - paddingBottom;
        if (chartWidth <= 0 || chartHeight <= 0) return;

        double maxVal = 0;
        foreach (var item in _data)
            if (item.Value > maxVal)
                maxVal = item.Value;
        if (maxVal <= 0) maxVal = 100;

        var yBase = Height - paddingBottom;
        using var penAxis = new Pen(Color.FromArgb(226, 232, 240), 1.5f);
        g.DrawLine(penAxis, paddingLeft, yBase, Width - paddingRight, yBase);

        var barCount = _data.Count;
        var step = (float)chartWidth / barCount;
        var barWidth = Math.Min(38f, Math.Max(20f, step * 0.5f));

        using var brushBar = new SolidBrush(_barColor);
        using var brushText = new SolidBrush(Color.FromArgb(100, 116, 139));
        using var sf = new StringFormat { Alignment = StringAlignment.Center };

        for (var i = 0; i < barCount; i++)
        {
            var item = _data[i];
            var barHeight = (float)(item.Value / maxVal * (chartHeight * 0.85f));
            var x = paddingLeft + i * step + (step - barWidth) / 2;
            var y = yBase - barHeight;

            g.FillRectangle(brushBar, x, y, barWidth, barHeight);
            g.DrawString(item.Key, Font, brushText, x + barWidth / 2, yBase + 8, sf);
        }
    }
}
