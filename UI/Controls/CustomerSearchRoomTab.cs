using BLL;
using Common.Constants;
using Common.Helpers;
using Entity.Entities;
using Entity.Enums;
using UI.Forms;
using UI.Helpers;

namespace UI.Controls;

public class CustomerSearchRoomTab : UserControl
{
    private DateTimePicker dtpTimNgayNhan = null!;
    private DateTimePicker dtpTimNgayTra = null!;
    private ComboBox cboTimSoKhach = null!;
    private ComboBox cboTimLoaiPhong = null!;
    private FlowLayoutPanel flowDanhSachPhong = null!;

    private Label lblDatPhongTenLoai = null!;
    private Label lblDatPhongNhanTra = null!;
    private Label lblDatPhongSoDem = null!;
    private Label lblDatPhongTienPhong = null!;
    private Panel pnlDongAnSang = null!;
    private Label lblDatPhongTienAnSang = null!;
    private Panel pnlDongDuaDon = null!;
    private Label lblDatPhongTienDuaDon = null!;
    private Label lblDatPhongTongTien = null!;
    private Button btnXacNhanDatPhong = null!;
    private CheckBox chkDatPhongAnSang = null!;
    private CheckBox chkDatPhongDuaDon = null!;
    private RoomType? _loaiPhongDangChon;

    public event Action? OnBookingCreated;

    public CustomerSearchRoomTab()
    {
        Font = new Font("Times New Roman", 10f, FontStyle.Regular);
        KhoiTaoGiaoDien();
        TaiDanhSachPhongTrong();
    }

    // Khởi tạo giao diện tab tìm phòng
    private void KhoiTaoGiaoDien()
    {
        var tblTimPhongRoot = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 1,
            Margin = new Padding(0)
        };
        tblTimPhongRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 104f));
        tblTimPhongRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

        var pnlSearch = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(16, 12, 16, 12),
            Margin = new Padding(0, 0, 0, 12)
        };

        var tblSearchInputs = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 5,
            RowCount = 2,
            Margin = new Padding(0),
            Padding = new Padding(0)
        };
        tblSearchInputs.RowStyles.Add(new RowStyle(SizeType.Absolute, 24f));
        tblSearchInputs.RowStyles.Add(new RowStyle(SizeType.Absolute, 42f));
        tblSearchInputs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22f));
        tblSearchInputs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22f));
        tblSearchInputs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20f));
        tblSearchInputs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20f));
        tblSearchInputs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16f));

        var lblNhan = new Label
        {
            Text = "Ngày nhận phòng",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.BottomLeft,
            Font = new Font("Times New Roman", 9.5f, FontStyle.Regular),
            ForeColor = Color.FromArgb(71, 85, 105),
            Margin = new Padding(0, 0, 12, 4)
        };
        dtpTimNgayNhan = new DateTimePicker
        {
            Dock = DockStyle.Top,
            Height = 30,
            Font = new Font("Times New Roman", 10f, FontStyle.Regular),
            Format = DateTimePickerFormat.Custom,
            CustomFormat = "dd/MM/yyyy",
            Margin = new Padding(0, 2, 12, 0)
        };
        dtpTimNgayNhan.ValueChanged += (s, e) => TinhTongTienDatPhong();

        var lblTra = new Label
        {
            Text = "Ngày trả phòng",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.BottomLeft,
            Font = new Font("Times New Roman", 9.5f, FontStyle.Regular),
            ForeColor = Color.FromArgb(71, 85, 105),
            Margin = new Padding(0, 0, 12, 4)
        };
        dtpTimNgayTra = new DateTimePicker
        {
            Dock = DockStyle.Top,
            Height = 30,
            Font = new Font("Times New Roman", 10f, FontStyle.Regular),
            Format = DateTimePickerFormat.Custom,
            CustomFormat = "dd/MM/yyyy",
            Value = DateTime.Today.AddDays(1),
            Margin = new Padding(0, 2, 12, 0)
        };
        dtpTimNgayTra.ValueChanged += (s, e) => TinhTongTienDatPhong();

        var lblKhach = new Label
        {
            Text = "Số khách",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.BottomLeft,
            Font = new Font("Times New Roman", 9.5f, FontStyle.Regular),
            ForeColor = Color.FromArgb(71, 85, 105),
            Margin = new Padding(0, 0, 12, 4)
        };
        cboTimSoKhach = new ComboBox
        {
            Dock = DockStyle.Top,
            Height = 30,
            Font = new Font("Times New Roman", 10f, FontStyle.Regular),
            DropDownStyle = ComboBoxStyle.DropDownList,
            Margin = new Padding(0, 2, 12, 0)
        };
        cboTimSoKhach.Items.AddRange(new object[] { "1 người lớn", "2 người lớn", "3 người lớn", "4 người lớn" });
        cboTimSoKhach.SelectedIndex = 1;

        var lblLoai = new Label
        {
            Text = "Loại phòng",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.BottomLeft,
            Font = new Font("Times New Roman", 9.5f, FontStyle.Regular),
            ForeColor = Color.FromArgb(71, 85, 105),
            Margin = new Padding(0, 0, 12, 4)
        };
        cboTimLoaiPhong = new ComboBox
        {
            Dock = DockStyle.Top,
            Height = 30,
            Font = new Font("Times New Roman", 10f, FontStyle.Regular),
            DropDownStyle = ComboBoxStyle.DropDownList,
            Margin = new Padding(0, 2, 12, 0)
        };
        cboTimLoaiPhong.Items.AddRange(new object[] { "Tất cả", "Standard", "Deluxe", "Suite" });
        cboTimLoaiPhong.SelectedIndex = 0;

        var btnTim = new Button
        {
            Text = "Tìm phòng",
            Dock = DockStyle.Top,
            Height = 34,
            Font = new Font("Times New Roman", 10f, FontStyle.Bold),
            Margin = new Padding(0, 1, 0, 0)
        };
        UIHelper.DinhDangNutBamChinh(btnTim);
        btnTim.Click += (s, e) => TaiDanhSachPhongTrong();

        tblSearchInputs.Controls.Add(lblNhan, 0, 0);
        tblSearchInputs.Controls.Add(lblTra, 1, 0);
        tblSearchInputs.Controls.Add(lblKhach, 2, 0);
        tblSearchInputs.Controls.Add(lblLoai, 3, 0);
        tblSearchInputs.Controls.Add(new Panel { Dock = DockStyle.Fill }, 4, 0);

        tblSearchInputs.Controls.Add(dtpTimNgayNhan, 0, 1);
        tblSearchInputs.Controls.Add(dtpTimNgayTra, 1, 1);
        tblSearchInputs.Controls.Add(cboTimSoKhach, 2, 1);
        tblSearchInputs.Controls.Add(cboTimLoaiPhong, 3, 1);
        tblSearchInputs.Controls.Add(btnTim, 4, 1);

        pnlSearch.Controls.Add(tblSearchInputs);
        tblTimPhongRoot.Controls.Add(pnlSearch, 0, 0);

        var tblTimPhongBody = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = new Padding(0)
        };
        tblTimPhongBody.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65f));
        tblTimPhongBody.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35f));

        var pnlLeftRooms = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppConstants.MauNenChung,
            Padding = new Padding(0),
            Margin = new Padding(0, 0, 10, 0)
        };
        var tblLeftLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 1,
            Margin = new Padding(0)
        };
        tblLeftLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35f));
        tblLeftLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

        var lblTitleLeft = new Label
        {
            Text = "Phòng còn trống",
            Font = new Font("Times New Roman", 12f, FontStyle.Bold),
            ForeColor = AppConstants.MauXanhNavy,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };
        flowDanhSachPhong = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(0, 4, 8, 28)
        };

        tblLeftLayout.Controls.Add(lblTitleLeft, 0, 0);
        tblLeftLayout.Controls.Add(flowDanhSachPhong, 0, 1);
        pnlLeftRooms.Controls.Add(tblLeftLayout);
        tblTimPhongBody.Controls.Add(pnlLeftRooms, 0, 0);

        var pnlRightBooking = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(18),
            Margin = new Padding(10, 0, 0, 0)
        };

        var lblTitleTT = new Label
        {
            Text = "Thông tin đặt phòng",
            Font = new Font("Times New Roman", 12f, FontStyle.Bold),
            ForeColor = AppConstants.MauXanhNavy,
            Dock = DockStyle.Top,
            Height = 34
        };

        var pnlTomTatBody = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = true,
            BackColor = Color.White,
            Padding = new Padding(0, 6, 0, 10)
        };

        TaoDongTomTatDat(pnlTomTatBody, "Phòng:", "Chưa chọn", out lblDatPhongTenLoai);
        TaoDongTomTatDat(pnlTomTatBody, "Nhận - Trả:", "-", out lblDatPhongNhanTra);
        TaoDongTomTatDat(pnlTomTatBody, "Số đêm:", "1 đêm", out lblDatPhongSoDem);
        TaoDongTomTatDat(pnlTomTatBody, "Giá phòng:", "0 đ", out lblDatPhongTienPhong);
        pnlDongAnSang = TaoDongTomTatDat(pnlTomTatBody, "Ăn sáng buffet:", "0 đ", out lblDatPhongTienAnSang);
        pnlDongAnSang.Visible = false;
        pnlDongDuaDon = TaoDongTomTatDat(pnlTomTatBody, "Đưa đón sân bay:", "200.000 đ", out lblDatPhongTienDuaDon);
        pnlDongDuaDon.Visible = false;

        var pnlKeNgang = new Panel
        {
            Height = 1,
            BackColor = AppConstants.MauVien,
            Margin = new Padding(0, 6, 0, 8)
        };
        pnlTomTatBody.Controls.Add(pnlKeNgang);

        TaoDongTomTatDat(pnlTomTatBody, "Tổng cộng:", "0 đ", out lblDatPhongTongTien, true);
        lblDatPhongTongTien.Font = new Font("Times New Roman", 12f, FontStyle.Bold);
        lblDatPhongTongTien.ForeColor = AppConstants.MauVangCamDam;

        var lblDVThem = new Label
        {
            Text = "Dịch vụ thêm:",
            AutoSize = true,
            Font = new Font("Times New Roman", 10f, FontStyle.Bold),
            ForeColor = AppConstants.MauChuChinh,
            Margin = new Padding(0, 12, 0, 4)
        };
        chkDatPhongAnSang = new CheckBox
        {
            Text = "Ăn sáng buffet (+50.000 đ/đêm)",
            AutoSize = true,
            Font = new Font("Times New Roman", 10f, FontStyle.Regular),
            ForeColor = AppConstants.MauChuChinh,
            Margin = new Padding(0, 2, 0, 4)
        };
        chkDatPhongDuaDon = new CheckBox
        {
            Text = "Đưa đón sân bay (+200.000 đ)",
            AutoSize = true,
            Font = new Font("Times New Roman", 10f, FontStyle.Regular),
            ForeColor = AppConstants.MauChuChinh,
            Margin = new Padding(0, 2, 0, 4)
        };
        chkDatPhongAnSang.CheckedChanged += (s, e) => TinhTongTienDatPhong();
        chkDatPhongDuaDon.CheckedChanged += (s, e) => TinhTongTienDatPhong();

        pnlTomTatBody.Controls.Add(lblDVThem);
        pnlTomTatBody.Controls.Add(chkDatPhongAnSang);
        pnlTomTatBody.Controls.Add(chkDatPhongDuaDon);

        void CapNhatDoRongDongTomTat()
        {
            var rowWidth = Math.Max(220, pnlTomTatBody.ClientSize.Width - 6);
            foreach (Control ctrl in pnlTomTatBody.Controls)
                if (ctrl is Panel)
                    ctrl.Width = rowWidth;
        }

        pnlTomTatBody.ClientSizeChanged += (s, e) => CapNhatDoRongDongTomTat();

        btnXacNhanDatPhong = new Button
        {
            Text = "Xác nhận đặt phòng",
            Dock = DockStyle.Bottom,
            Height = 40,
            Font = new Font("Times New Roman", 10.5f, FontStyle.Bold)
        };
        UIHelper.DinhDangNutBamVoHieu(btnXacNhanDatPhong);
        btnXacNhanDatPhong.Click += (s, e) => XuLyXacNhanDatPhong();

        pnlRightBooking.Controls.Add(pnlTomTatBody);
        pnlRightBooking.Controls.Add(btnXacNhanDatPhong);
        pnlRightBooking.Controls.Add(lblTitleTT);
        CapNhatDoRongDongTomTat();
        TinhTongTienDatPhong();

        tblTimPhongBody.Controls.Add(pnlRightBooking, 1, 0);
        tblTimPhongRoot.Controls.Add(tblTimPhongBody, 0, 1);
        Controls.Add(tblTimPhongRoot);
    }

    private static Panel TaoDongTomTatDat(
        FlowLayoutPanel parent,
        string tenKhoan,
        string giaTriMacDinh,
        out Label lblGiaTri,
        bool laTongCong = false)
    {
        var row = new Panel
        {
            Height = laTongCong ? 32 : 28,
            BackColor = Color.White,
            Margin = new Padding(0, 0, 0, 4)
        };

        var lblTen = new Label
        {
            Text = tenKhoan,
            Dock = DockStyle.Left,
            AutoSize = true,
            TextAlign = ContentAlignment.MiddleLeft,
            ForeColor = laTongCong ? AppConstants.MauChuChinh : AppConstants.MauChuPhu,
            Font = new Font("Times New Roman", laTongCong ? 11f : 9.5f,
                laTongCong ? FontStyle.Bold : FontStyle.Regular)
        };

        lblGiaTri = new Label
        {
            Text = giaTriMacDinh,
            Dock = DockStyle.Right,
            AutoSize = true,
            TextAlign = ContentAlignment.MiddleRight,
            ForeColor = AppConstants.MauChuChinh,
            Font = new Font("Times New Roman", laTongCong ? 12f : 9.5f, FontStyle.Bold)
        };

        row.Controls.Add(lblGiaTri);
        row.Controls.Add(lblTen);
        parent.Controls.Add(row);
        return row;
    }

    // Tải danh sách các loại phòng trống hiển thị lên thẻ
    public void TaiDanhSachPhongTrong()
    {
        UIHelper.RunSafe(() =>
        {
            flowDanhSachPhong.Controls.Clear();
            var dsLoai = AppServices.Room.GetAllRoomTypes();

            var filterLoai = cboTimLoaiPhong.SelectedItem?.ToString() ?? "Tất cả";
            var dsHienThi = dsLoai
                .Where(lp => filterLoai == "Tất cả" || lp.Name == filterLoai)
                .ToList();

            foreach (var lp in dsHienThi)
            {
                var card = TaoTheLoaiPhong(lp);
                flowDanhSachPhong.Controls.Add(card);
            }

            if (dsHienThi.Count > 0)
            {
                ChonLoaiPhong(dsHienThi[0]);
            }
            else
            {
                _loaiPhongDangChon = null;
                TinhTongTienDatPhong();
            }
        }, "tải danh sách phòng trống", this);
    }

    private Panel TaoTheLoaiPhong(RoomType lp)
    {
        var pnl = new Panel
        {
            Size = new Size(255, 304),
            BackColor = Color.White,
            Margin = new Padding(0, 0, 16, 16),
            BorderStyle = BorderStyle.FixedSingle
        };

        var pnlAnh = ImageHelper.CreateCoverImagePanel(() => ImageHelper.LoadRoomImage(lp.Name));
        pnlAnh.Dock = DockStyle.Top;
        pnlAnh.Height = 125;
        pnlAnh.BackColor = Color.FromArgb(241, 245, 249);

        var lblTen = new Label
        {
            Text = lp.Name,
            Font = new Font("Times New Roman", 12f, FontStyle.Bold),
            ForeColor = AppConstants.MauChuChinh,
            Location = new Point(14, 135),
            AutoSize = true
        };

        var lblMoTa = new Label
        {
            Text = lp.Description,
            Font = new Font("Times New Roman", 9.5f, FontStyle.Regular),
            ForeColor = AppConstants.MauChuPhu,
            Location = new Point(14, 162),
            Size = new Size(225, 44),
            AutoEllipsis = true
        };

        var lblGia = new Label
        {
            Text = $"{FormatHelper.FormatCurrency(lp.BasePrice)}/đêm",
            Font = new Font("Times New Roman", 10.5f, FontStyle.Bold),
            ForeColor = AppConstants.MauVangCamDam,
            Location = new Point(14, 212),
            AutoSize = true
        };

        var btnChiTiet = new Button
        {
            Text = "Xem chi tiết",
            Font = new Font("Times New Roman", 10f, FontStyle.Regular),
            Location = new Point(14, 248),
            Width = 108,
            Height = 38,
            UseMnemonic = false,
            Cursor = Cursors.Hand
        };
        UIHelper.DinhDangNutBamPhu(btnChiTiet);
        btnChiTiet.Click += (s, e) =>
        {
            using var frm = new RoomDetailForm(lp);
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                ChonLoaiPhong(lp);
            }
        };

        var btnChon = new Button
        {
            Text = "Chọn phòng",
            Font = new Font("Times New Roman", 10f, FontStyle.Bold),
            Location = new Point(128, 248),
            Width = 111,
            Height = 38,
            UseMnemonic = false,
            Cursor = Cursors.Hand
        };
        UIHelper.DinhDangNutBamNavy(btnChon);
        btnChon.Click += (s, e) => ChonLoaiPhong(lp);

        pnl.Controls.AddRange(new Control[] { pnlAnh, lblTen, lblMoTa, lblGia, btnChiTiet, btnChon });
        return pnl;
    }

    private void ChonLoaiPhong(RoomType lp)
    {
        _loaiPhongDangChon = lp;
        TinhTongTienDatPhong();
    }

    // Tính toán tổng tiền dự kiến theo số đêm và dịch vụ phụ trợ
    private void TinhTongTienDatPhong()
    {
        var soDem = FormatHelper.CalculateNights(dtpTimNgayNhan.Value, dtpTimNgayTra.Value);
        lblDatPhongNhanTra.Text = $"{dtpTimNgayNhan.Value:dd/MM} - {dtpTimNgayTra.Value:dd/MM}";
        lblDatPhongSoDem.Text = $"{soDem} đêm";

        if (_loaiPhongDangChon == null)
        {
            lblDatPhongTenLoai.Text = "Chưa chọn";
            lblDatPhongTienPhong.Text = "0 đ";
            pnlDongAnSang.Visible = false;
            pnlDongDuaDon.Visible = false;
            lblDatPhongTongTien.Text = "0 đ";
            UIHelper.DinhDangNutBamVoHieu(btnXacNhanDatPhong);
            return;
        }

        lblDatPhongTenLoai.Text = _loaiPhongDangChon.Name;

        var tienPhong = _loaiPhongDangChon.BasePrice * soDem;
        lblDatPhongTienPhong.Text = $"{FormatHelper.FormatCurrency(_loaiPhongDangChon.BasePrice)} x {soDem} đêm";

        var tien = tienPhong;

        pnlDongAnSang.Visible = chkDatPhongAnSang.Checked;
        if (chkDatPhongAnSang.Checked)
        {
            var tienAnSang = 50000d * soDem;
            tien += tienAnSang;
            lblDatPhongTienAnSang.Text = $"{FormatHelper.FormatCurrency(50000d)} x {soDem} đêm";
        }

        pnlDongDuaDon.Visible = chkDatPhongDuaDon.Checked;
        if (chkDatPhongDuaDon.Checked)
        {
            tien += 200000d;
            lblDatPhongTienDuaDon.Text = FormatHelper.FormatCurrency(200000d);
        }

        lblDatPhongTongTien.Text = FormatHelper.FormatCurrency(tien);
        UIHelper.DinhDangNutBamNavy(btnXacNhanDatPhong);
    }

    // Xử lý xác nhận đặt phòng trực tuyến
    private void XuLyXacNhanDatPhong()
    {
        if (_loaiPhongDangChon == null)
        {
            MessageBox.Show(this, "Vui lòng chọn loại phòng muốn đặt!", "Thông báo", MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        if (dtpTimNgayTra.Value.Date <= dtpTimNgayNhan.Value.Date)
        {
            MessageBox.Show(this, "Ngày trả phòng phải sau ngày nhận phòng!", "Thông báo", MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        var khach = SessionManager.CurrentCustomer;
        if (khach == null || khach.CustomerId <= 0)
        {
            MessageBox.Show(this, "Phiên đăng nhập khách hàng không hợp lệ. Vui lòng đăng nhập lại!", "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        UIHelper.RunSafe(() =>
        {
            var dsPhongTrong = AppServices.Room.GetAvailableRooms(_loaiPhongDangChon.RoomTypeId);
            if (dsPhongTrong.Count == 0)
            {
                MessageBox.Show(this, $"Rất tiếc, hiện tại loại phòng {_loaiPhongDangChon.Name} đã hết phòng trống!",
                    "Hết phòng", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var dp = new Booking
            {
                CustomerId = khach.CustomerId,
                CheckInDate = dtpTimNgayNhan.Value.ToString("dd/MM/yyyy"),
                CheckOutDate = dtpTimNgayTra.Value.ToString("dd/MM/yyyy"),
                GuestCount = cboTimSoKhach.SelectedIndex + 1,
                Deposit = 0,
                Channel = "Trực tuyến",
                Status = BookingStatus.Pending
            };

            var maPhong = dsPhongTrong[0].RoomId;
            var thanhCong = AppServices.Booking.CreateBooking(dp, maPhong, _loaiPhongDangChon.BasePrice, out var msg);

            if (thanhCong)
            {
                MessageBox.Show(this, "Đặt phòng thành công! Đơn đặt của bạn đã được gửi tới lễ tân xác nhận.",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                TaiDanhSachPhongTrong();
                OnBookingCreated?.Invoke();
            }
            else
            {
                MessageBox.Show(this, msg, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }, "xác nhận đặt phòng trực tuyến", this);
    }
}
