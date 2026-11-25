using System.Drawing.Drawing2D;
using Common.Constants;

namespace Common.Helpers;

public static class UIHelper
{
    // Áp dụng font chính cho toàn bộ control
    public static void ApDungFontChinh(Control parent, float baseSize = 10f)
    {
        if (parent == null) return;

        if (parent.Font.FontFamily.Name != AppConstants.FontChuChinh)
            parent.Font = new Font(AppConstants.FontChuChinh, parent.Font.Size, parent.Font.Style);

        foreach (Control child in parent.Controls) ApDungFontChinh(child, baseSize);
    }

    // Giữ tên hàm tương thích cho các form chưa cập nhật
    public static void ApDungFontTimesNewRoman(Control parent, float baseSize = 10f)
    {
        ApDungFontChinh(parent, baseSize);
    }

    // Định dạng bảng dữ liệu DataGridView
    public static void DinhDangBangDuLieu(DataGridView dgv)
    {
        if (dgv == null) return;

        dgv.BackgroundColor = Color.White;
        dgv.BorderStyle = BorderStyle.None;
        dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        dgv.GridColor = Color.FromArgb(241, 245, 249);
        dgv.RowHeadersVisible = false;
        dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgv.MultiSelect = false;
        dgv.ReadOnly = true;
        dgv.AllowUserToAddRows = false;
        dgv.AllowUserToDeleteRows = false;
        dgv.AllowUserToResizeRows = false;
        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgv.RowTemplate.Height = 40;

        // Định dạng tiêu đề cột bảng
        dgv.EnableHeadersVisualStyles = false;
        dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
        dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        dgv.ColumnHeadersHeight = 42;
        dgv.ColumnHeadersDefaultCellStyle.BackColor = AppConstants.MauNenChung;
        dgv.ColumnHeadersDefaultCellStyle.ForeColor = AppConstants.MauChuChinh;
        dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = AppConstants.MauNenChung;
        dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = AppConstants.MauChuChinh;
        dgv.ColumnHeadersDefaultCellStyle.Font = new Font(AppConstants.FontChuChinh, 9.5f, FontStyle.Bold);
        dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(10, 0, 6, 0);

        // Định dạng dòng dữ liệu
        dgv.DefaultCellStyle.Font = new Font(AppConstants.FontChuChinh, 9.5f, FontStyle.Regular);
        dgv.DefaultCellStyle.ForeColor = AppConstants.MauChuChinh;
        dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(241, 245, 249);
        dgv.DefaultCellStyle.SelectionForeColor = AppConstants.MauChuChinh;
        dgv.DefaultCellStyle.Padding = new Padding(10, 0, 6, 0);
    }

    // Vẽ nhãn trạng thái bo tròn trong bảng
    public static void VeBadge(DataGridViewCellPaintingEventArgs e, string text, Color mauNen, Color mauChu)
    {
        e.PaintBackground(e.CellBounds, true);

        var g = e.Graphics;
        if (g == null) return;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        // Đo kích thước nhãn trạng thái
        using var font = new Font(AppConstants.FontChuChinh, 9f, FontStyle.Bold);
        var size = g.MeasureString(text, font);
        var badgeW = Math.Max(60, (int)size.Width + 18);
        var badgeH = 24;
        var badgeX = e.CellBounds.X + 8;
        var badgeY = e.CellBounds.Y + (e.CellBounds.Height - badgeH) / 2;

        var rect = new RectangleF(badgeX, badgeY, badgeW, badgeH);

        // Tô nền bo góc cho nhãn
        using var brushNen = new SolidBrush(mauNen);
        using var path = TaoDuongBoGoc(rect, 11f);
        g.FillPath(brushNen, path);

        // Vẽ chữ lên nhãn
        using var brushChu = new SolidBrush(mauChu);
        using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        g.DrawString(text, font, brushChu, rect, sf);

        e.Handled = true;
    }

    // Tạo GraphicsPath bo góc an toàn cho control/card/ảnh
    public static GraphicsPath TaoDuongBoGoc(Rectangle r, int radius, bool topCornersOnly = false)
    {
        return TaoDuongBoGoc(new RectangleF(r.X, r.Y, r.Width, r.Height), radius, topCornersOnly);
    }

    // Tạo GraphicsPath bo góc chính xác theo tọa độ thực (chống răng cưa)
    public static GraphicsPath TaoDuongBoGoc(RectangleF r, float radius, bool topCornersOnly = false)
    {
        var path = new GraphicsPath();
        if (r.Width <= 0 || r.Height <= 0)
            return path;

        var maxRadius = Math.Min(r.Width / 2f, r.Height / 2f);
        var rad = Math.Min(radius, maxRadius);
        if (rad <= 0.1f)
        {
            path.AddRectangle(r);
            return path;
        }

        var d = rad * 2f;
        path.AddArc(r.X, r.Y, d, d, 180, 90);
        path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
        if (topCornersOnly)
        {
            path.AddLine(r.Right, r.Bottom, r.X, r.Bottom);
        }
        else
        {
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
        }

        path.CloseFigure();
        return path;
    }

    // Lấy màu nền không trong suốt gần nhất từ control cha để bo góc mượt không lộ viền
    public static Color LayMauNenThucTe(Control? control)
    {
        var current = control?.Parent;
        while (current != null)
        {
            if (current.BackColor != Color.Transparent && current.BackColor.A == 255)
                return current.BackColor;
            current = current.Parent;
        }

        return AppConstants.MauNenChung;
    }

    // Định dạng nút bấm chính
    public static void DinhDangNutBamChinh(Button btn)
    {
        if (btn == null) return;
        btn.FlatStyle = FlatStyle.Flat;
        btn.FlatAppearance.BorderSize = 0;
        btn.BackColor = AppConstants.MauVangCam;
        btn.ForeColor = AppConstants.MauXanhNavyDam;
        btn.Font = new Font(AppConstants.FontChuChinh, 10f, FontStyle.Bold);
        btn.Cursor = Cursors.Hand;
    }

    // Định dạng nút bấm xanh Navy
    public static void DinhDangNutBamNavy(Button btn)
    {
        if (btn == null) return;
        btn.FlatStyle = FlatStyle.Flat;
        btn.FlatAppearance.BorderSize = 0;
        btn.BackColor = AppConstants.MauXanhNavy;
        btn.ForeColor = Color.White;
        btn.Font = new Font(AppConstants.FontChuChinh, 10f, FontStyle.Bold);
        btn.Cursor = Cursors.Hand;
    }

    // Định dạng nút bấm phụ
    public static void DinhDangNutBamPhu(Button btn)
    {
        if (btn == null) return;
        btn.FlatStyle = FlatStyle.Flat;
        btn.FlatAppearance.BorderSize = 1;
        btn.FlatAppearance.BorderColor = AppConstants.MauVienInput;
        btn.BackColor = Color.White;
        btn.ForeColor = AppConstants.MauXanhNavyDam;
        btn.Font = new Font(AppConstants.FontChuChinh, 10f, FontStyle.Regular);
        btn.Cursor = Cursors.Hand;
    }

    // Định dạng nút khi chưa đủ điều kiện thao tác (nền xám, chữ trắng dễ đọc)
    public static void DinhDangNutBamVoHieu(Button btn)
    {
        if (btn == null) return;
        btn.FlatStyle = FlatStyle.Flat;
        btn.FlatAppearance.BorderSize = 0;
        btn.BackColor = AppConstants.MauXam;
        btn.ForeColor = Color.White;
        btn.Font = new Font(AppConstants.FontChuChinh, 10f, FontStyle.Bold);
        btn.Cursor = Cursors.Default;
    }

    // Định dạng nút cảnh báo / hủy
    public static void DinhDangNutBamNguyHiem(Button btn)
    {
        if (btn == null) return;
        btn.FlatStyle = FlatStyle.Flat;
        btn.FlatAppearance.BorderSize = 1;
        btn.FlatAppearance.BorderColor = AppConstants.MauDo;
        btn.BackColor = Color.White;
        btn.ForeColor = AppConstants.MauDo;
        btn.Font = new Font(AppConstants.FontChuChinh, 10f, FontStyle.Bold);
        btn.Cursor = Cursors.Hand;
    }

    // Thực thi an toàn thao tác UI gọi BLL/DAL: bắt riêng lỗi thiếu quyền và lỗi hệ thống
    public static bool RunSafe(Action action, string operationDescription = "thực hiện thao tác", IWin32Window? owner = null)
    {
        return RunSafe(() =>
        {
            action();
            return true;
        }, false, operationDescription, owner);
    }

    // Thực thi an toàn thao tác truy vấn UI gọi BLL/DAL, trả về fallback nếu gặp ngoại lệ
    public static T RunSafe<T>(Func<T> func, T fallback, string operationDescription = "tải dữ liệu", IWin32Window? owner = null)
    {
        try
        {
            return func();
        }
        catch (UnauthorizedAccessException ex)
        {
            AppLogger.Error(ex, operationDescription);
            ShowMessageSafe(owner, ex.Message, "Không đủ quyền", MessageBoxIcon.Warning);
            return fallback;
        }
        catch (Exception ex)
        {
            AppLogger.Error(ex, operationDescription);
            ShowMessageSafe(
                owner,
                $"Đã xảy ra lỗi khi {operationDescription}. Vui lòng thử lại!\nChi tiết: {ex.Message}",
                "Lỗi hệ thống",
                MessageBoxIcon.Error);
            return fallback;
        }
    }

    private static void ShowMessageSafe(IWin32Window? owner, string message, string caption, MessageBoxIcon icon)
    {
        if (owner != null)
            MessageBox.Show(owner, message, caption, MessageBoxButtons.OK, icon);
        else
            MessageBox.Show(message, caption, MessageBoxButtons.OK, icon);
    }
}
