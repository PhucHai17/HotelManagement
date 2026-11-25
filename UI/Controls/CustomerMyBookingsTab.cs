using BLL;
using Common.Constants;
using Common.Helpers;
using Entity.Entities;
using Entity.Enums;
using UI.Forms;

namespace UI.Controls;

public class CustomerMyBookingsTab : UserControl
{
    private const string CotChiTiet = "ChiTiet";
    private const string CotHuy = "Huy";
    private const string CotDanhGia = "DanhGia";

    private DataGridView dgvDatPhongCuaToi = null!;

    public CustomerMyBookingsTab()
    {
        Font = new Font("Times New Roman", 10f, FontStyle.Regular);
        KhoiTaoGiaoDien();
        TaiDuLieuDatPhong();
    }

    // Khởi tạo bố cục danh sách đặt phòng của khách hàng
    private void KhoiTaoGiaoDien()
    {
        var tblRoot = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 1,
            Margin = new Padding(0)
        };
        tblRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 54f));
        tblRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

        var pnlTop = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(18, 0, 0, 0),
            Margin = new Padding(0, 0, 0, 10)
        };

        var lblTitle = new Label
        {
            Text = "Đặt phòng của tôi",
            Font = new Font("Times New Roman", 16f, FontStyle.Bold),
            ForeColor = AppConstants.MauXanhNavy,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };
        pnlTop.Controls.Add(lblTitle);
        tblRoot.Controls.Add(pnlTop, 0, 0);

        dgvDatPhongCuaToi = new DataGridView
        {
            Dock = DockStyle.Fill,
            Font = new Font("Times New Roman", 10f, FontStyle.Regular)
        };
        UIHelper.DinhDangBangDuLieu(dgvDatPhongCuaToi);
        dgvDatPhongCuaToi.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 10f, FontStyle.Bold);

        dgvDatPhongCuaToi.Columns.Add("Ma", "Mã đơn");
        dgvDatPhongCuaToi.Columns.Add("Phong", "Loại phòng");
        dgvDatPhongCuaToi.Columns.Add("SoPhong", "Phòng");
        dgvDatPhongCuaToi.Columns.Add("Nhan", "Ngày nhận");
        dgvDatPhongCuaToi.Columns.Add("Tra", "Ngày trả");
        dgvDatPhongCuaToi.Columns.Add("TongTien", "Tổng tiền");
        dgvDatPhongCuaToi.Columns.Add("TrangThai", "Trạng thái");

        dgvDatPhongCuaToi.Columns["Ma"]!.MinimumWidth = 85;
        dgvDatPhongCuaToi.Columns["Ma"]!.FillWeight = 55;
        dgvDatPhongCuaToi.Columns["Phong"]!.MinimumWidth = 120;
        dgvDatPhongCuaToi.Columns["Phong"]!.FillWeight = 90;
        dgvDatPhongCuaToi.Columns["SoPhong"]!.MinimumWidth = 80;
        dgvDatPhongCuaToi.Columns["SoPhong"]!.FillWeight = 55;
        dgvDatPhongCuaToi.Columns["Nhan"]!.MinimumWidth = 105;
        dgvDatPhongCuaToi.Columns["Nhan"]!.FillWeight = 75;
        dgvDatPhongCuaToi.Columns["Tra"]!.MinimumWidth = 105;
        dgvDatPhongCuaToi.Columns["Tra"]!.FillWeight = 75;
        dgvDatPhongCuaToi.Columns["TongTien"]!.MinimumWidth = 115;
        dgvDatPhongCuaToi.Columns["TongTien"]!.FillWeight = 85;
        dgvDatPhongCuaToi.Columns["TrangThai"]!.MinimumWidth = 115;
        dgvDatPhongCuaToi.Columns["TrangThai"]!.FillWeight = 85;

        // Cột thao tác theo wireframe: Chi tiết / Hủy / Đánh giá (nút nào không áp dụng sẽ được ẩn theo từng dòng)
        dgvDatPhongCuaToi.Columns.Add(TaoCotNut(CotChiTiet, "Thao tác", 90));
        dgvDatPhongCuaToi.Columns.Add(TaoCotNut(CotHuy, string.Empty, 75));
        dgvDatPhongCuaToi.Columns.Add(TaoCotNut(CotDanhGia, string.Empty, 110));

        dgvDatPhongCuaToi.CellClick += DgvDatPhongCuaToi_CellClick;
        dgvDatPhongCuaToi.CellPainting += DgvDatPhongCuaToi_CellPainting;

        tblRoot.Controls.Add(dgvDatPhongCuaToi, 0, 1);
        Controls.Add(tblRoot);
    }

    private static DataGridViewButtonColumn TaoCotNut(string name, string header, int minWidth)
    {
        return new DataGridViewButtonColumn
        {
            Name = name,
            HeaderText = header,
            UseColumnTextForButtonValue = false,
            FlatStyle = FlatStyle.Flat,
            MinimumWidth = minWidth,
            FillWeight = 60
        };
    }

    // Nút nào được hiển thị với đơn đặt phòng này
    private static bool HienNut(string tenCot, Booking dp) => tenCot switch
    {
        CotChiTiet => true,
        CotHuy => dp.Status == BookingStatus.Pending,
        CotDanhGia => dp.Status == BookingStatus.CheckedOut,
        _ => true
    };

    // Tải danh sách đơn đặt phòng của khách hàng đang đăng nhập
    public void TaiDuLieuDatPhong()
    {
        UIHelper.RunSafe(() =>
        {
            dgvDatPhongCuaToi.Rows.Clear();
            var khach = SessionManager.CurrentCustomer;
            if (khach == null || khach.CustomerId <= 0) return;

            var ds = AppServices.Booking.GetByCustomerId(khach.CustomerId);

            foreach (var dp in ds)
            {
                var trangThaiChuoi = dp.Status switch
                {
                    BookingStatus.Pending => AppConstants.DPChoDuyet,
                    BookingStatus.Confirmed => AppConstants.DPDaXacNhan,
                    BookingStatus.CheckedIn => AppConstants.DPDangO,
                    BookingStatus.CheckedOut => AppConstants.DPDaHoanTat,
                    BookingStatus.Cancelled => AppConstants.DPDaHuy,
                    _ => AppConstants.DPChoDuyet
                };

                var soPhongHienThi = string.IsNullOrWhiteSpace(dp.RoomNumber) ? "Chưa xếp" : dp.RoomNumber;

                var nhanDanhGia = "Đánh giá";
                if (dp.Status == BookingStatus.CheckedOut && DaDanhGia(dp.BookingId))
                    nhanDanhGia = "Xem đánh giá";

                var idx = dgvDatPhongCuaToi.Rows.Add(
                    dp.DisplayCode,
                    dp.RoomTypeName,
                    soPhongHienThi,
                    FormatHelper.FormatDate(FormatHelper.ParseDate(dp.CheckInDate)),
                    FormatHelper.FormatDate(FormatHelper.ParseDate(dp.CheckOutDate)),
                    FormatHelper.FormatCurrency(dp.TotalAmount),
                    trangThaiChuoi,
                    "Chi tiết",
                    "Hủy",
                    nhanDanhGia);
                dgvDatPhongCuaToi.Rows[idx].Tag = dp;
            }
        }, "tải danh sách đặt phòng của bạn", this);
    }

    private static bool DaDanhGia(int bookingId)
    {
        try
        {
            return AppServices.Review.GetByBookingId(bookingId) != null;
        }
        catch (Exception ex)
        {
            AppLogger.Error(ex, "Kiểm tra đánh giá của đơn đặt phòng");
            return false;
        }
    }

    // Ẩn nút không áp dụng và tô màu trạng thái
    private void DgvDatPhongCuaToi_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

        var tenCot = dgvDatPhongCuaToi.Columns[e.ColumnIndex].Name;

        if ((tenCot == CotChiTiet || tenCot == CotHuy || tenCot == CotDanhGia) &&
            dgvDatPhongCuaToi.Rows[e.RowIndex].Tag is Booking dp && !HienNut(tenCot, dp))
        {
            e.Paint(e.CellBounds, DataGridViewPaintParts.Background | DataGridViewPaintParts.Border);
            e.Handled = true;
            return;
        }

        if (tenCot == "TrangThai" && e.Value != null)
        {
            var tt = e.Value.ToString() ?? string.Empty;
            if (tt == AppConstants.DPDaXacNhan || tt == AppConstants.DPDaHoanTat)
                UIHelper.VeBadge(e, tt, Color.FromArgb(220, 252, 231), Color.FromArgb(21, 128, 61));
            else if (tt == AppConstants.DPChoDuyet)
                UIHelper.VeBadge(e, tt, Color.FromArgb(254, 243, 199), Color.FromArgb(180, 83, 9));
            else if (tt == AppConstants.DPDaHuy)
                UIHelper.VeBadge(e, tt, Color.FromArgb(254, 226, 226), Color.FromArgb(185, 28, 28));
            else
                UIHelper.VeBadge(e, tt, Color.FromArgb(224, 242, 254), Color.FromArgb(3, 105, 161));
            e.Handled = true;
        }
    }

    // Xử lý bấm nút Chi tiết / Hủy / Đánh giá
    private void DgvDatPhongCuaToi_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
        if (dgvDatPhongCuaToi.Rows[e.RowIndex].Tag is not Booking dp) return;

        var tenCot = dgvDatPhongCuaToi.Columns[e.ColumnIndex].Name;
        if (!HienNut(tenCot, dp)) return;

        switch (tenCot)
        {
            case CotChiTiet:
                HienChiTiet(dp);
                break;
            case CotHuy:
                HuyDatPhong(dp);
                break;
            case CotDanhGia:
                DanhGiaDatPhong(dp);
                break;
        }
    }

    private void HienChiTiet(Booking dp)
    {
        var nhan = FormatHelper.ParseDate(dp.CheckInDate);
        var tra = FormatHelper.ParseDate(dp.CheckOutDate);
        var soDem = FormatHelper.CalculateNights(nhan, tra);
        var trangThai = dgvDatPhongCuaToi.CurrentRow?.Cells["TrangThai"].Value?.ToString() ?? string.Empty;

        var noiDung =
            $"Mã đơn: {dp.DisplayCode}{Environment.NewLine}" +
            $"Loại phòng: {dp.RoomTypeName}{Environment.NewLine}" +
            $"Phòng: {(string.IsNullOrWhiteSpace(dp.RoomNumber) ? "Chưa xếp" : dp.RoomNumber)}{Environment.NewLine}" +
            $"Nhận - Trả: {FormatHelper.FormatDate(nhan)} - {FormatHelper.FormatDate(tra)} ({soDem} đêm){Environment.NewLine}" +
            $"Số khách: {dp.GuestCount}{Environment.NewLine}" +
            $"Tiền cọc: {FormatHelper.FormatCurrency(dp.Deposit)}{Environment.NewLine}" +
            $"Tổng tiền: {FormatHelper.FormatCurrency(dp.TotalAmount)}{Environment.NewLine}" +
            $"Trạng thái: {trangThai}";

        MessageBox.Show(this, noiDung, $"Chi tiết đặt phòng {dp.DisplayCode}", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void HuyDatPhong(Booking dp)
    {
        var xacNhan = MessageBox.Show(this,
            $"Bạn có chắc chắn muốn hủy đơn đặt phòng {dp.DisplayCode} ({dp.RoomTypeName})?",
            "Xác nhận hủy đặt phòng", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (xacNhan != DialogResult.Yes) return;

        UIHelper.RunSafe(() =>
        {
            var kq = AppServices.Booking.CancelBooking(dp.BookingId, out var msg);
            MessageBox.Show(this, msg, "Thông báo", MessageBoxButtons.OK,
                kq ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            TaiDuLieuDatPhong();
        }, "hủy đơn đặt phòng", this);
    }

    private void DanhGiaDatPhong(Booking dp)
    {
        UIHelper.RunSafe(() =>
        {
            var daCo = AppServices.Review.GetByBookingId(dp.BookingId);
            if (daCo != null)
            {
                using var xem = new ReviewDialog(dp, daCo);
                xem.ShowDialog(this);
                return;
            }

            using var frm = new ReviewDialog(dp);
            if (frm.ShowDialog(this) != DialogResult.OK) return;

            var ok = AppServices.Review.SubmitReview(dp.BookingId, frm.SoSao, frm.NoiDung, out var msg);
            MessageBox.Show(this, msg, "Thông báo", MessageBoxButtons.OK,
                ok ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (ok) TaiDuLieuDatPhong();
        }, "gửi đánh giá", this);
    }
}
