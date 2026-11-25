using System.Drawing.Drawing2D;
using BLL;
using BLL.Interfaces;
using Common.Constants;
using Common.Helpers;
using Entity.Entities;
using Entity.Enums;
using UI.Forms;

namespace UI.Controls;

public class BookingTab : UserControl
{
    private readonly IBookingService _datPhongBLL = AppServices.Booking;

    private TextBox txtTimDatPhong = null!;
    private ComboBox cboLocTrangThai = null!;
    private DateTimePicker dtpLocNgay = null!;
    private DataGridView dgvDatPhong = null!;

    public BookingTab()
    {
        Font = new Font("Times New Roman", 10f, FontStyle.Regular);
        BackColor = AppConstants.MauNenChung;
        KhoiTaoGiaoDien();
    }

    // Khởi tạo giao diện tab quản lý đặt phòng cho lễ tân
    private void KhoiTaoGiaoDien()
    {
        var fontNoiDung = new Font("Times New Roman", 10f, FontStyle.Regular);
        var fontNutChinh = new Font("Times New Roman", 10f, FontStyle.Bold);

        var tblRoot = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 3,
            ColumnCount = 1,
            Margin = new Padding(0)
        };
        tblRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 52f));
        tblRoot.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        tblRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

        var pnlHeader = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(16, 0, 16, 0),
            Margin = new Padding(0, 0, 0, 8)
        };
        var lblTitle = new Label
        {
            Text = "Quản lý đặt phòng",
            Font = new Font("Times New Roman", 16f, FontStyle.Bold),
            ForeColor = AppConstants.MauXanhNavy,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };
        pnlHeader.Controls.Add(lblTitle);
        tblRoot.Controls.Add(pnlHeader, 0, 0);

        var pnlTop = new Panel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(14, 10, 14, 6),
            Margin = new Padding(0, 0, 0, 8)
        };

        var flowFilter = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            Padding = new Padding(0),
            Margin = new Padding(0)
        };

        var lblTim = new Label
        {
            Text = "Tìm kiếm:",
            Font = fontNoiDung,
            AutoSize = true,
            Margin = new Padding(0, 7, 6, 6),
            ForeColor = AppConstants.MauChuPhu
        };
        txtTimDatPhong = new TextBox
        {
            Width = 175,
            Height = 30,
            Font = fontNoiDung,
            Margin = new Padding(0, 3, 12, 6),
            PlaceholderText = "Mã, tên, SĐT, phòng..."
        };
        txtTimDatPhong.TextChanged += (s, e) => TaiDuLieuDatPhong();

        var lblTT = new Label
        {
            Text = "Trạng thái:",
            Font = fontNoiDung,
            AutoSize = true,
            Margin = new Padding(0, 7, 6, 6),
            ForeColor = AppConstants.MauChuPhu
        };
        cboLocTrangThai = new ComboBox
        {
            Width = 135,
            Height = 30,
            Font = fontNoiDung,
            Margin = new Padding(0, 3, 12, 6),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cboLocTrangThai.Items.AddRange(new object[]
        {
            "Tất cả",
            AppConstants.DPChoDuyet,
            AppConstants.DPDaXacNhan,
            AppConstants.DPDangO,
            AppConstants.DPDaHoanTat,
            AppConstants.DPDaHuy
        });
        cboLocTrangThai.SelectedIndex = 0;
        cboLocTrangThai.SelectedIndexChanged += (s, e) => TaiDuLieuDatPhong();

        var lblTuNgay = new Label
        {
            Text = "Từ ngày:",
            Font = fontNoiDung,
            AutoSize = true,
            Margin = new Padding(0, 7, 6, 6),
            ForeColor = AppConstants.MauChuPhu
        };
        dtpLocNgay = new DateTimePicker
        {
            Width = 155,
            Height = 30,
            Font = fontNoiDung,
            Margin = new Padding(0, 3, 14, 6),
            Format = DateTimePickerFormat.Custom,
            CustomFormat = AppConstants.DateFormat,
            ShowCheckBox = true,
            Checked = false
        };
        dtpLocNgay.ValueChanged += (s, e) => TaiDuLieuDatPhong();

        var btnThemDP = new Button
        {
            Text = "+ Thêm đặt phòng",
            Width = 180,
            Height = 34,
            UseMnemonic = false,
            Margin = new Padding(0, 1, 0, 6)
        };
        UIHelper.DinhDangNutBamChinh(btnThemDP);
        btnThemDP.Font = fontNutChinh;
        btnThemDP.Click += (s, e) =>
        {
            using var frm = new AddBookingForm();
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                TaiDuLieuDatPhong();
            }
        };

        flowFilter.Controls.AddRange(new Control[]
        {
            lblTim, txtTimDatPhong,
            lblTT, cboLocTrangThai,
            lblTuNgay, dtpLocNgay,
            btnThemDP
        });
        pnlTop.Controls.Add(flowFilter);
        tblRoot.Controls.Add(pnlTop, 0, 1);

        var pnlGridContainer = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(0)
        };

        dgvDatPhong = new DataGridView
        {
            Dock = DockStyle.Fill,
            Font = fontNoiDung
        };
        UIHelper.DinhDangBangDuLieu(dgvDatPhong);
        dgvDatPhong.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 10f, FontStyle.Bold);
        dgvDatPhong.DefaultCellStyle.Font = fontNoiDung;

        dgvDatPhong.Columns.Add("Ma", "Mã");
        dgvDatPhong.Columns.Add("Khach", "Khách hàng");
        dgvDatPhong.Columns.Add("Phong", "Phòng");
        dgvDatPhong.Columns.Add("Nhan", "Ngày nhận");
        dgvDatPhong.Columns.Add("Tra", "Ngày trả");
        dgvDatPhong.Columns.Add("TrangThai", "Trạng thái");

        dgvDatPhong.Columns["Ma"]!.MinimumWidth = 80;
        dgvDatPhong.Columns["Ma"]!.FillWeight = 55;

        dgvDatPhong.Columns["Khach"]!.MinimumWidth = 150;
        dgvDatPhong.Columns["Khach"]!.FillWeight = 115;

        dgvDatPhong.Columns["Phong"]!.MinimumWidth = 75;
        dgvDatPhong.Columns["Phong"]!.FillWeight = 50;

        dgvDatPhong.Columns["Nhan"]!.MinimumWidth = 110;
        dgvDatPhong.Columns["Nhan"]!.FillWeight = 85;

        dgvDatPhong.Columns["Tra"]!.MinimumWidth = 110;
        dgvDatPhong.Columns["Tra"]!.FillWeight = 85;

        dgvDatPhong.Columns["TrangThai"]!.MinimumWidth = 130;
        dgvDatPhong.Columns["TrangThai"]!.FillWeight = 95;

        var colDuyet = new DataGridViewButtonColumn
        {
            Name = "Duyet",
            HeaderText = "Duyệt",
            Text = "Duyệt",
            UseColumnTextForButtonValue = true,
            MinimumWidth = 80,
            FillWeight = 55
        };
        dgvDatPhong.Columns.Add(colDuyet);

        var colHuy = new DataGridViewButtonColumn
        {
            Name = "Huy",
            HeaderText = "Hủy",
            Text = "Hủy",
            UseColumnTextForButtonValue = true,
            MinimumWidth = 80,
            FillWeight = 55
        };
        dgvDatPhong.Columns.Add(colHuy);

        dgvDatPhong.CellClick += DgvDatPhong_CellClick;
        dgvDatPhong.CellPainting += DgvDatPhong_CellPainting;

        pnlGridContainer.Controls.Add(dgvDatPhong);
        tblRoot.Controls.Add(pnlGridContainer, 0, 2);
        Controls.Add(tblRoot);
    }

    // Tải danh sách đơn đặt phòng theo điều kiện tìm kiếm và lọc
    public void TaiDuLieuDatPhong()
    {
        DamBaoFontTimesNewRoman(this);
        UIHelper.RunSafe(() =>
        {
            dgvDatPhong.Rows.Clear();
            var tuKhoa = txtTimDatPhong.Text.Trim();
            var trangThai = cboLocTrangThai.SelectedItem?.ToString() ?? "Tất cả";
            var tuNgay = dtpLocNgay.Checked ? dtpLocNgay.Value.ToString(AppConstants.DateFormat) : string.Empty;

            var ds = _datPhongBLL.Search(tuKhoa, trangThai, tuNgay);
            foreach (var dp in ds)
            {
                var nhanStr = FormatHelper.FormatDate(FormatHelper.ParseDate(dp.CheckInDate));
                var traStr = FormatHelper.FormatDate(FormatHelper.ParseDate(dp.CheckOutDate));
                var soPhong = string.IsNullOrWhiteSpace(dp.RoomNumber) ? "Chưa xếp" : dp.RoomNumber;
                var trangThaiHienThi = ChuyenTrangThaiHienThi(dp.Status);

                var index = dgvDatPhong.Rows.Add(
                    dp.DisplayCode,
                    dp.CustomerName,
                    soPhong,
                    nhanStr,
                    traStr,
                    trangThaiHienThi);
                dgvDatPhong.Rows[index].Tag = dp;
            }
        }, "tải danh sách đặt phòng", this);
    }

    // Xử lý bấm nút duyệt hoặc hủy đơn trên bảng đặt phòng
    private void DgvDatPhong_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;
        if (dgvDatPhong.Rows[e.RowIndex].Tag is not Booking dp) return;

        if (dgvDatPhong.Columns["Duyet"] != null && e.ColumnIndex == dgvDatPhong.Columns["Duyet"]!.Index)
        {
            if (dp.Status != BookingStatus.Pending)
            {
                MessageBox.Show(this, "Chỉ có thể duyệt đơn đặt phòng đang ở trạng thái Chờ duyệt!",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show(this,
                    $"Xác nhận duyệt đơn đặt phòng {dp.DisplayCode} của khách {dp.CustomerName}?",
                    "Xác nhận duyệt đơn", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                UIHelper.RunSafe(() =>
                {
                    var thanhCong = _datPhongBLL.ConfirmBooking(dp.BookingId, out var msg);
                    MessageBox.Show(this, msg, "Thông báo", MessageBoxButtons.OK,
                        thanhCong ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
                    TaiDuLieuDatPhong();
                }, "duyệt đơn đặt phòng", this);
            }

            return;
        }

        if (dgvDatPhong.Columns["Huy"] != null && e.ColumnIndex == dgvDatPhong.Columns["Huy"]!.Index)
        {
            if (dp.Status == BookingStatus.Cancelled)
            {
                MessageBox.Show(this, "Đơn đặt phòng này đã bị hủy trước đó!",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dp.Status == BookingStatus.CheckedIn || dp.Status == BookingStatus.CheckedOut)
            {
                MessageBox.Show(this, "Phòng đã nhận hoặc đã hoàn tất, không thể hủy!",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show(this,
                    $"Bạn có chắc chắn muốn hủy đơn đặt phòng {dp.DisplayCode} của khách {dp.CustomerName}?",
                    "Xác nhận hủy đơn", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                UIHelper.RunSafe(() =>
                {
                    var thanhCong = _datPhongBLL.CancelBooking(dp.BookingId, out var msg);
                    MessageBox.Show(this, msg, "Thông báo", MessageBoxButtons.OK,
                        thanhCong ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
                    TaiDuLieuDatPhong();
                }, "hủy đơn đặt phòng", this);
            }
        }
    }

    
    private void DgvDatPhong_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
    {
        if (e.RowIndex < 0 || dgvDatPhong.Columns["TrangThai"] == null ||
            e.ColumnIndex != dgvDatPhong.Columns["TrangThai"]!.Index || e.Value == null)
            return;

        var tt = e.Value.ToString() ?? string.Empty;
        Color mauNen;
        Color mauChu;

        if (tt == AppConstants.DPDaXacNhan || tt == AppConstants.DPDaHoanTat)
        {
            mauNen = Color.FromArgb(220, 252, 231);
            mauChu = Color.FromArgb(21, 128, 61);
        }
        else if (tt == AppConstants.DPChoDuyet)
        {
            mauNen = Color.FromArgb(254, 243, 199);
            mauChu = Color.FromArgb(180, 83, 9);
        }
        else if (tt == AppConstants.DPDaHuy)
        {
            mauNen = Color.FromArgb(254, 226, 226);
            mauChu = Color.FromArgb(185, 28, 28);
        }
        else
        {
            mauNen = Color.FromArgb(224, 242, 254);
            mauChu = Color.FromArgb(3, 105, 161);
        }

        e.PaintBackground(e.CellBounds, true);
        var g = e.Graphics;
        if (g == null) return;

        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var font = new Font("Times New Roman", 9f, FontStyle.Bold);
        var size = g.MeasureString(tt, font);
        var badgeW = Math.Max(68, (int)size.Width + 18);
        var badgeH = 24;
        var badgeX = e.CellBounds.X + 8;
        var badgeY = e.CellBounds.Y + (e.CellBounds.Height - badgeH) / 2;
        var rect = new RectangleF(badgeX, badgeY, badgeW, badgeH);

        using var brushNen = new SolidBrush(mauNen);
        using var path = UIHelper.TaoDuongBoGoc(rect, 11f);
        g.FillPath(brushNen, path);

        using var brushChu = new SolidBrush(mauChu);
        using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        g.DrawString(tt, font, brushChu, rect, sf);

        e.Handled = true;
    }

    private void DamBaoFontTimesNewRoman(Control parent)
    {
        if (parent.Font.FontFamily.Name != "Times New Roman")
            parent.Font = new Font("Times New Roman", parent.Font.Size, parent.Font.Style);

        foreach (Control child in parent.Controls)
            DamBaoFontTimesNewRoman(child);
    }

    private static string ChuyenTrangThaiHienThi(BookingStatus status) => status switch
    {
        BookingStatus.Pending => AppConstants.DPChoDuyet,
        BookingStatus.Confirmed => AppConstants.DPDaXacNhan,
        BookingStatus.CheckedIn => AppConstants.DPDangO,
        BookingStatus.CheckedOut => AppConstants.DPDaHoanTat,
        BookingStatus.Cancelled => AppConstants.DPDaHuy,
        _ => AppConstants.DPChoDuyet
    };
}
