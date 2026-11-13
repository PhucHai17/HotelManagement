
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.Reflection;
using System.Text;
using Common.Constants;
using Common.Helpers;

namespace UI.Helpers;

public static class ImageHelper
{
    private static readonly object SyncLock = new();
    private static readonly Dictionary<string, Image?> Cache = new(StringComparer.OrdinalIgnoreCase);
    private static bool _exitHooked;

    public static Image? LoadRoomImage(string? roomTypeName)
    {
        var key = NormalizeText(roomTypeName);
        var fileName = key.Contains("suite")
            ? "suite.jpg"
            : key.Contains("deluxe")
                ? "deluxe.jpg"
                : "standard.jpg";

        return LoadEmbeddedImage(fileName) ?? LoadEmbeddedImage("standard.jpg");
    }
    
    public static Image? LoadServiceImage(string? serviceName)
    {
        var key = NormalizeText(serviceName);
        var fileName = key.Contains("an sang") || key.Contains("buffet") || key.Contains("breakfast")
            ? "breakfast.jpg"
            : key.Contains("san bay") || key.Contains("dua don") || key.Contains("airport")
                ? "airport.jpg"
                : key.Contains("spa") || key.Contains("massage")
                    ? "spa.jpg"
                    : key.Contains("giat") || key.Contains("laundry")
                        ? "laundry.jpg"
                        : "default-service.jpg";

        return LoadEmbeddedImage(fileName) ?? LoadEmbeddedImage("default-service.jpg");
    }
    
    public static Image? LoadHero()
    {
        return LoadEmbeddedImage("hero.jpg");
    }

    public static void DrawCoverImage(
        Graphics g,
        Image? image,
        Rectangle bounds,
        int cornerRadius = 0,
        bool topCornersOnly = false,
        Color? overlayColor = null)
    {
        if (g == null || bounds.Width <= 0 || bounds.Height <= 0) return;

        var state = g.Save();
        try
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            GraphicsPath? clipPath = null;
            if (cornerRadius > 0)
            {
                clipPath = UIHelper.TaoDuongBoGoc(bounds, cornerRadius, topCornersOnly);
                g.SetClip(clipPath);
            }
            else
            {
                g.SetClip(bounds);
            }

            try
            {
                if (image != null && image.Width > 0 && image.Height > 0)
                {
                    var srcRect = ComputeCoverSourceRect(image.Size, bounds.Size);
                    // Dùng TileFlipXY để nội suy bicubic không tạo viền trắng/trong suốt ở cạnh trên và trái
                    using var attrs = new ImageAttributes();
                    attrs.SetWrapMode(WrapMode.TileFlipXY);
                    g.DrawImage(
                        image,
                        bounds,
                        srcRect.X,
                        srcRect.Y,
                        srcRect.Width,
                        srcRect.Height,
                        GraphicsUnit.Pixel,
                        attrs);
                }
                else
                {
                    using var fallbackBrush = new SolidBrush(AppConstants.MauXanhNavyDam);
                    g.FillRectangle(fallbackBrush, bounds);
                }

                if (overlayColor.HasValue && overlayColor.Value.A > 0)
                {
                    using var overlayBrush = new SolidBrush(overlayColor.Value);
                    g.FillRectangle(overlayBrush, bounds);
                }
            }
            finally
            {
                clipPath?.Dispose();
            }
        }
        finally
        {
            g.Restore(state);
        }
    }
    
    public static Panel CreateCoverImagePanel(
        Func<Image?> imageProvider,
        int cornerRadius = 0,
        bool topCornersOnly = false,
        Color? overlayColor = null)
    {
        var pnl = new DoubleBufferedCoverPanel(imageProvider, cornerRadius, topCornersOnly, overlayColor);
        return pnl;
    }
    public static void DisposeCache()
    {
        lock (SyncLock)
        {
            foreach (var img in Cache.Values)
            {
                img?.Dispose();
            }
            Cache.Clear();
        }
    }

    private static Image? LoadEmbeddedImage(string fileName)
    {
        lock (SyncLock)
        {
            if (!_exitHooked)
            {
                Application.ApplicationExit += (_, _) => DisposeCache();
                _exitHooked = true;
            }

            if (Cache.TryGetValue(fileName, out var cached))
                return cached;

            var asm = Assembly.GetExecutingAssembly();
            var suffix = $".Resources.Images.{fileName}";
            var resName = asm.GetManifestResourceNames()
                .FirstOrDefault(n => n.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
                ?? asm.GetManifestResourceNames()
                    .FirstOrDefault(n => n.EndsWith($".{fileName}", StringComparison.OrdinalIgnoreCase));

            Image? loaded = null;
            if (!string.IsNullOrEmpty(resName))
            {
                using var stream = asm.GetManifestResourceStream(resName);
                if (stream != null)
                {
                    // Tạo bản Bitmap độc lập để đóng Stream ngay, tránh rò rỉ hoặc lỗi GDI+
                    using var temp = Image.FromStream(stream);
                    loaded = new Bitmap(temp);
                }
            }

            Cache[fileName] = loaded;
            return loaded;
        }
    }

    private static Rectangle ComputeCoverSourceRect(Size imgSize, Size targetSize)
    {
        var srcRatio = (float)imgSize.Width / imgSize.Height;
        var dstRatio = (float)targetSize.Width / targetSize.Height;

        if (srcRatio > dstRatio)
        {
            var cropW = (int)Math.Round(imgSize.Height * dstRatio);
            var x = Math.Max(0, (imgSize.Width - cropW) / 2);
            return new Rectangle(x, 0, Math.Min(cropW, imgSize.Width), imgSize.Height);
        }

        var cropH = (int)Math.Round(imgSize.Width / dstRatio);
        var y = Math.Max(0, (imgSize.Height - cropH) / 2);
        return new Rectangle(0, y, imgSize.Width, Math.Min(cropH, imgSize.Height));
    }

    private static string NormalizeText(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;

        var formD = input.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(formD.Length);
        foreach (var ch in formD)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(ch == 'đ' ? 'd' : ch == 'Đ' ? 'D' : ch);
            }
        }
        return sb.ToString().Normalize(NormalizationForm.FormC).Trim().ToLowerInvariant();
    }

    private sealed class DoubleBufferedCoverPanel : Panel
    {
        private readonly Func<Image?> _imageProvider;
        private readonly int _cornerRadius;
        private readonly bool _topCornersOnly;
        private readonly Color? _overlayColor;

        public DoubleBufferedCoverPanel(
            Func<Image?> imageProvider,
            int cornerRadius,
            bool topCornersOnly,
            Color? overlayColor)
        {
            _imageProvider = imageProvider;
            _cornerRadius = cornerRadius;
            _topCornersOnly = topCornersOnly;
            _overlayColor = overlayColor;
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true);
            DoubleBuffered = true;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Bỏ qua vẽ nền mặc định để tránh nháy hoặc ló viền sáng trước khi OnPaint vẽ ảnh phủ kín
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            DrawCoverImage(e.Graphics, _imageProvider(), ClientRectangle, _cornerRadius, _topCornersOnly, _overlayColor);
            base.OnPaint(e);
        }
    }
}
