using BLL;
using Common.Constants;
using Common.Helpers;
using Entity.Entities;
using Entity.Enums;

namespace UI.Controls;

public class CustomerMyBookingsTab : UserControl
{
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
            Text = "Đặt phòng",
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

        dgvDatPhongCuaToi.Columns["Ma"].MinimumWidth = 85;
        dgvDatPhongCuaToi.Columns["Ma"].FillWeight = 55;
        dgvDatPhongCuaToi.Columns["Phong"].MinimumWidth = 130;
        dgvDatPhongCuaToi.Columns["Phong"].FillWeight = 95;
        dgvDatPhongCuaToi.Columns["SoPhong"].MinimumWidth = 80;
        dgvDatPhongCuaToi.Columns["SoPhong"].FillWeight = 55;
        dgvDatPhongCuaToi.Columns["Nhan"].MinimumWidth = 110;
        dgvDatPhongCuaToi.Columns["Nhan"].FillWeight = 80;
        dgvDatPhongCuaToi.Columns["Tra"].MinimumWidth = 110;
        dgvDatPhongCuaToi.Columns["Tra"].FillWeight = 80;
        dgvDatPhongCuaToi.Columns["TongTien"].MinimumWidth = 120;
        dgvDatPhongCuaToi.Columns["TongTien"].FillWeight = 90;
        dgvDatPhongCuaToi.Columns["TrangThai"].MinimumWidth = 115;
        dgvDatPhongCuaToi.Columns["TrangThai"].FillWeight = 85;

        var colHuy = new DataGridViewButtonColumn
        {
            Name = "Huy",
            HeaderText = "Thao tác",
            Text = "Hủy đặt",
            UseColumnTextForButtonValue = true,
            MinimumWidth = 85,
            FillWeight = 60
        };
        dgvDatPhongCuaToi.Columns.Add(colHuy);
        dgvDatPhongCuaToi.CellClick += DgvDatPhongCuaToi_CellClick;

        tblRoot.Controls.Add(dgvDatPhongCuaToi, 0, 1);
        Controls.Add(tblRoot);
    }

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

                var idx = dgvDatPhongCuaToi.Rows.Add(
                    dp.DisplayCode,
                    dp.RoomTypeName,
                    soPhongHienThi,
                    FormatHelper.FormatDate(FormatHelper.ParseDate(dp.CheckInDate)),
                    FormatHelper.FormatDate(FormatHelper.ParseDate(dp.CheckOutDate)),
                    FormatHelper.FormatCurrency(dp.TotalAmount),
                    trangThaiChuoi);
                dgvDatPhongCuaToi.Rows[idx].Tag = dp;
            }
        }, "tải danh sách đặt phòng của bạn", this);
    }

    // Xử lý sự kiện bấm nút hủy đơn đặt phòng
    private void DgvDatPhongCuaToi_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0 && dgvDatPhongCuaToi.Columns["Huy"] != null &&
            e.ColumnIndex == dgvDatPhongCuaToi.Columns["Huy"]!.Index)
        {
            if (dgvDatPhongCuaToi.Rows[e.RowIndex].Tag is Booking dp)
            {
                if (dp.Status != BookingStatus.Pending)
                {
                    MessageBox.Show(this, "Chỉ có thể hủy đơn đặt phòng đang ở trạng thái Chờ duyệt!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var xacNhan = MessageBox.Show(this,
                    $"Bạn có chắc chắn muốn hủy đơn đặt phòng {dp.DisplayCode} ({dp.RoomTypeName})?",
                    "Xác nhận hủy đặt phòng", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (xacNhan == DialogResult.Yes)
                {
                    UIHelper.RunSafe(() =>
                    {
                        var kq = AppServices.Booking.CancelBooking(dp.BookingId, out var msg);
                        MessageBox.Show(this, msg, "Thông báo", MessageBoxButtons.OK,
                            kq ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
                        TaiDuLieuDatPhong();
                    }, "hủy đơn đặt phòng", this);
                }
            }
        }
    }
}
