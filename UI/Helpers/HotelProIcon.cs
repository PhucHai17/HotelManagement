using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace UI.Helpers;

/// <summary>Creates a small HotelPro navy-and-gold hotel mark for WinForms title bars and taskbar buttons.</summary>
public static class HotelProIcon
{
    private static readonly Lazy<Icon> CachedIcon = new(CreateIcon);
    public static Icon Instance => CachedIcon.Value;

    private static Icon CreateIcon()
    {
        using var bitmap = new Bitmap(64, 64);
        using (var graphics = Graphics.FromImage(bitmap))
        using (var navy = new SolidBrush(Color.FromArgb(15, 35, 75)))
        using (var gold = new SolidBrush(Color.FromArgb(250, 198, 69)))
        using (var goldPen = new Pen(Color.FromArgb(250, 198, 69), 3.2f))
        using (var white = new SolidBrush(Color.FromArgb(255, 242, 194)))
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.Clear(Color.Transparent);
            graphics.FillRectangle(navy, 2, 2, 60, 60);
            graphics.DrawRectangle(goldPen, 3.5f, 3.5f, 57, 57);
            graphics.FillPolygon(gold, new[] { new Point(11, 24), new Point(21, 17), new Point(21, 49), new Point(11, 52) });
            graphics.FillPolygon(gold, new[] { new Point(53, 24), new Point(43, 17), new Point(43, 49), new Point(53, 52) });
            graphics.FillPolygon(gold, new[] { new Point(18, 34), new Point(32, 25), new Point(46, 34), new Point(46, 39), new Point(32, 31), new Point(18, 39) });
            graphics.FillRectangle(white, 27, 38, 4, 4);
            graphics.FillRectangle(white, 33, 38, 4, 4);
            graphics.FillRectangle(white, 27, 44, 4, 4);
            graphics.FillRectangle(white, 33, 44, 4, 4);
        }
        var handle = bitmap.GetHicon();
        try { return (Icon)Icon.FromHandle(handle).Clone(); }
        finally { DestroyIcon(handle); }
    }

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DestroyIcon(IntPtr hIcon);
}
