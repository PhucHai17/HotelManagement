using BLL;
using Common.Constants;
using Common.Helpers;

namespace UI.Controls;

// Lịch còn trống theo tháng của một hạng phòng (đỏ: hết phòng, vàng: ngày đang chọn)
public class AvailabilityCalendarControl : UserControl
{
    private readonly int _roomTypeId;
    private readonly DateTime? _checkIn;
    private readonly DateTime? _checkOut;
    private DateTime _thang;

    private readonly Label lblThang = new();
    private readonly TableLayoutPanel tblNgay = new();

    public AvailabilityCalendarControl(int roomTypeId, DateTime? checkIn = null, DateTime? checkOut = null)
    {
        _roomTypeId = roomTypeId;
        _checkIn = checkIn?.Date;
        _checkOut = checkOut?.Date;
        var goc = _checkIn ?? DateTime.Today;
        _thang = new DateTime(goc.Year, goc.Month, 1);

        Font = new Font("Times New Roman", 10f, FontStyle.Regular);
        KhoiTaoGiaoDien();
        VeLich();
    }

    private void KhoiTaoGiaoDien()
    {
        BackColor = Color.White;
        Padding = new Padding(14);

        var pnlDau = new Panel { Dock = DockStyle.Top, Height = 40 };

        var btnSau = new Button { Text = "▶", Dock = DockStyle.Right, Width = 34, Cursor = Cursors.Hand };
        var btnTruoc = new Button { Text = "◀", Dock = DockStyle.Right, Width = 34, Cursor = Cursors.Hand };
        UIHelper.DinhDangNutBamPhu(btnSau);
        UIHelper.DinhDangNutBamPhu(btnTruoc);
        btnTruoc.Click += (_, _) => { _thang = _thang.AddMonths(-1); VeLich(); };
        btnSau.Click += (_, _) => { _thang = _thang.AddMonths(1); VeLich(); };

        lblThang.Dock = DockStyle.Fill;
        lblThang.TextAlign = ContentAlignment.MiddleLeft;
        lblThang.Font = new Font("Times New Roman", 12f, FontStyle.Bold);
        lblThang.ForeColor = AppConstants.MauChuChinh;

        pnlDau.Controls.Add(lblThang);
        pnlDau.Controls.Add(btnTruoc);
        pnlDau.Controls.Add(btnSau);

        var tblThu = new TableLayoutPanel { Dock = DockStyle.Top, Height = 26, ColumnCount = 7, RowCount = 1 };
        for (var c = 0; c < 7; c++) tblThu.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / 7));
        var thu = new[] { "T2", "T3", "T4", "T5", "T6", "T7", "CN" };
        for (var c = 0; c < 7; c++)
        {
            tblThu.Controls.Add(new Label
            {
                Text = thu[c],
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = AppConstants.MauChuPhu,
                Font = new Font("Times New Roman", 9.5f, FontStyle.Bold)
            }, c, 0);
        }

        tblNgay.Dock = DockStyle.Fill;
        tblNgay.ColumnCount = 7;
        tblNgay.RowCount = 6;
        for (var c = 0; c < 7; c++) tblNgay.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / 7));
        for (var r = 0; r < 6; r++) tblNgay.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / 6));

        var thongTinChon = _checkIn.HasValue && _checkOut.HasValue
            ? $"Nhận - Trả: {_checkIn:dd/MM} - {_checkOut:dd/MM}"
            : string.Empty;

        var lblChuThich = new Label
        {
            Dock = DockStyle.Bottom,
            Height = 52,
            ForeColor = AppConstants.MauChuPhu,
            Text = "Vàng: đang chọn. Đỏ: đã hết phòng." +
                   (thongTinChon.Length > 0 ? Environment.NewLine + thongTinChon : string.Empty)
        };

        Controls.Add(tblNgay);
        Controls.Add(lblChuThich);
        Controls.Add(tblThu);
        Controls.Add(pnlDau);
    }

    // Vẽ lại lưới ngày của tháng đang xem
    private void VeLich()
    {
        lblThang.Text = $"Lịch còn trống - Tháng {_thang.Month}/{_thang.Year}";

        var hetPhong = new HashSet<DateTime>();
        try
        {
            foreach (var d in AppServices.Booking.GetFullyBookedDates(_roomTypeId, _thang.Year, _thang.Month))
                hetPhong.Add(d.Date);
        }
        catch (Exception ex)
        {
            AppLogger.Error(ex, "Tải lịch còn trống của hạng phòng");
        }

        tblNgay.SuspendLayout();
        foreach (Control c in tblNgay.Controls.OfType<Control>().ToList())
        {
            tblNgay.Controls.Remove(c);
            c.Dispose();
        }

        var soNgay = DateTime.DaysInMonth(_thang.Year, _thang.Month);
        var lech = ((int)_thang.DayOfWeek + 6) % 7; // Thứ hai là cột đầu tiên

        for (var ngay = 1; ngay <= soNgay; ngay++)
        {
            var date = new DateTime(_thang.Year, _thang.Month, ngay);
            var viTri = lech + ngay - 1;

            var daChon = _checkIn.HasValue && _checkOut.HasValue && date >= _checkIn && date <= _checkOut;
            var daHet = hetPhong.Contains(date);
            var quaKhu = date < DateTime.Today;

            var lbl = new Label
            {
                Text = ngay.ToString(),
                Dock = DockStyle.Fill,
                Margin = new Padding(2),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = AppConstants.MauNenChung,
                ForeColor = quaKhu ? AppConstants.MauVienInput : AppConstants.MauChuChinh
            };

            if (daHet)
            {
                lbl.BackColor = AppConstants.MauPhongCoKhach;
                lbl.ForeColor = AppConstants.MauChuPhongCoKhach;
            }
            else if (daChon)
            {
                lbl.BackColor = AppConstants.MauVangCam;
                lbl.ForeColor = AppConstants.MauChuTrenVang;
                lbl.Font = new Font(Font, FontStyle.Bold);
            }

            tblNgay.Controls.Add(lbl, viTri % 7, viTri / 7);
        }

        tblNgay.ResumeLayout(true);
    }
}
