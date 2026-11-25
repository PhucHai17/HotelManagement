using BLL;
using Common.Constants;
using Common.Helpers;
using Entity.Entities;
using Entity.Enums;

namespace UI.Controls;

public class CustomerKeyLockerTab : UserControl
{
    private DataGridView dgvDonPhong = null!;
    private ComboBox cboTang = null!;
    private TableLayoutPanel tblLockerGrid = null!;
    private Label lblChiTietPhong = null!;
    private Label lblChiTietViTriTu = null!;
    private Label lblChiTietLuuTru = null!;
    private Label lblChiTietTrangThai = null!;
    private Panel pnlPinContainer = null!;
    private Label lblPinTieuDe = null!;
    private Label lblPinGiaTri = null!;
    private Label lblPinGhiChu = null!;
    private Booking? _donDangChon;
    private readonly List<Panel> _danhSachOThuNgan = new();

    public CustomerKeyLockerTab()
    {
        Font = new Font("Times New Roman", 10f, FontStyle.Regular);
        KhoiTaoGiaoDien();
        TaiDuLieuTuKhoa();
    }

    // Khởi tạo toàn bộ giao diện tab tủ chìa khóa
    private void KhoiTaoGiaoDien()
    {
        var tblRoot = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 1,
            Margin = new Padding(0)
        };
        tblRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 56f));
        tblRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

        var pnlHeader = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(16, 8, 16, 8),
            Margin = new Padding(0, 0, 0, 10)
        };

        var lblTitle = new Label
        {
            Text = "TỦ CHÌA KHÓA TỰ ĐỘNG (SMART KEY LOCKER)",
            Font = new Font("Times New Roman", 16f, FontStyle.Bold),
            ForeColor = AppConstants.MauXanhNavy,
            Location = new Point(14, 6),
            AutoSize = true
        };

        var lblSubtitle = new Label
        {
            Text = "Tra cứu vị trí ngăn tủ thông minh và mã PIN nhận chìa khóa tự phục vụ 24/7",
            Font = new Font("Times New Roman", 9.5f, FontStyle.Regular),
            ForeColor = AppConstants.MauChuPhu,
            Location = new Point(16, 32),
            AutoSize = true
        };

        var btnLamMoi = new Button
        {
            Text = "Làm mới",
            Font = new Font("Times New Roman", 10f, FontStyle.Regular),
            Size = new Size(95, 34),
            Location = new Point(pnlHeader.Width - 110, 10),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            UseMnemonic = false,
            Cursor = Cursors.Hand
        };
        UIHelper.DinhDangNutBamPhu(btnLamMoi);
        btnLamMoi.Click += (s, e) => TaiDuLieuTuKhoa();

        pnlHeader.Controls.Add(lblTitle);
        pnlHeader.Controls.Add(lblSubtitle);
        pnlHeader.Controls.Add(btnLamMoi);
        tblRoot.Controls.Add(pnlHeader, 0, 0);

        var tblBody = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 1,
            ColumnCount = 2,
            Margin = new Padding(0)
        };
        tblBody.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 56f));
        tblBody.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 44f));

        var tblLeft = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 1,
            Margin = new Padding(0, 0, 10, 0)
        };
        tblLeft.RowStyles.Add(new RowStyle(SizeType.Percent, 48f));
        tblLeft.RowStyles.Add(new RowStyle(SizeType.Percent, 52f));

        var pnlGridBox = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(14),
            Margin = new Padding(0, 0, 0, 8)
        };

        var lblGridTitle = new Label
        {
            Text = "Danh sách đơn đặt phòng của quý khách",
            Font = new Font("Times New Roman", 12f, FontStyle.Bold),
            ForeColor = AppConstants.MauXanhNavy,
            Dock = DockStyle.Top,
            Height = 28
        };

        dgvDonPhong = new DataGridView
        {
            Dock = DockStyle.Fill,
            Font = new Font("Times New Roman", 10f, FontStyle.Regular)
        };
        UIHelper.DinhDangBangDuLieu(dgvDonPhong);
        dgvDonPhong.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 10f, FontStyle.Bold);

        dgvDonPhong.Columns.Add("Ma", "Mã đơn");
        dgvDonPhong.Columns.Add("Phong", "Số phòng");
        dgvDonPhong.Columns.Add("LoaiPhong", "Hạng phòng");
        dgvDonPhong.Columns.Add("ViTriTu", "Vị trí ngăn tủ");
        dgvDonPhong.Columns.Add("TrangThaiDon", "Trạng thái đơn");
        dgvDonPhong.Columns.Add("TrangThaiChia", "Trạng thái chìa");

        dgvDonPhong.Columns["Ma"].MinimumWidth = 65;
        dgvDonPhong.Columns["Ma"].FillWeight = 50;
        dgvDonPhong.Columns["Phong"].MinimumWidth = 65;
        dgvDonPhong.Columns["Phong"].FillWeight = 48;
        dgvDonPhong.Columns["LoaiPhong"].MinimumWidth = 100;
        dgvDonPhong.Columns["LoaiPhong"].FillWeight = 80;
        dgvDonPhong.Columns["ViTriTu"].MinimumWidth = 125;
        dgvDonPhong.Columns["ViTriTu"].FillWeight = 95;
        dgvDonPhong.Columns["TrangThaiDon"].MinimumWidth = 95;
        dgvDonPhong.Columns["TrangThaiDon"].FillWeight = 72;
        dgvDonPhong.Columns["TrangThaiChia"].MinimumWidth = 110;
        dgvDonPhong.Columns["TrangThaiChia"].FillWeight = 85;

        dgvDonPhong.SelectionChanged += (s, e) => XuLyChonDongDonPhong();

        pnlGridBox.Controls.Add(dgvDonPhong);
        pnlGridBox.Controls.Add(lblGridTitle);
        tblLeft.Controls.Add(pnlGridBox, 0, 0);

        var pnlDetailCard = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(16, 12, 16, 12),
            Margin = new Padding(0)
        };

        var lblDetailHeader = new Label
        {
            Text = "THÔNG TIN CHI TIẾT NHẬN CHÌA KHÓA",
            Font = new Font("Times New Roman", 12f, FontStyle.Bold),
            ForeColor = AppConstants.MauXanhNavy,
            Dock = DockStyle.Top,
            Height = 26
        };

        var pnlDividerDetail = new Panel
        {
            Dock = DockStyle.Top,
            Height = 1,
            BackColor = AppConstants.MauVien,
            Margin = new Padding(0, 0, 0, 8)
        };

        var pnlInfoFields = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(0, 8, 0, 0)
        };

        lblChiTietPhong = new Label
        {
            Text = "Phòng: Chưa chọn đơn",
            Font = new Font("Times New Roman", 10.5f, FontStyle.Bold),
            ForeColor = AppConstants.MauChuChinh,
            Location = new Point(0, 6),
            Size = new Size(500, 22)
        };

        lblChiTietViTriTu = new Label
        {
            Text = "Vị trí ngăn tủ: ---",
            Font = new Font("Times New Roman", 10.5f, FontStyle.Bold),
            ForeColor = AppConstants.MauXanhNavy,
            Location = new Point(0, 30),
            Size = new Size(500, 22)
        };

        lblChiTietLuuTru = new Label
        {
            Text = "Thời gian lưu trú: ---",
            Font = new Font("Times New Roman", 9.5f, FontStyle.Regular),
            ForeColor = AppConstants.MauChuPhu,
            Location = new Point(0, 54),
            Size = new Size(500, 20)
        };

        lblChiTietTrangThai = new Label
        {
            Text = "Trạng thái chìa: ---",
            Font = new Font("Times New Roman", 9.5f, FontStyle.Bold),
            ForeColor = AppConstants.MauChuChinh,
            Location = new Point(0, 76),
            Size = new Size(500, 20)
        };

        pnlPinContainer = new Panel
        {
            Location = new Point(0, 102),
            Size = new Size(480, 72),
            BackColor = Color.FromArgb(254, 249, 195),
            BorderStyle = BorderStyle.FixedSingle
        };

        lblPinTieuDe = new Label
        {
            Text = "MÃ PIN MỞ TỦ (6 CHỮ SỐ):",
            Font = new Font("Times New Roman", 9.5f, FontStyle.Bold),
            ForeColor = AppConstants.MauChuChinh,
            Location = new Point(12, 8),
            AutoSize = true
        };

        lblPinGiaTri = new Label
        {
            Text = "------",
            Font = new Font("Times New Roman", 16f, FontStyle.Bold),
            ForeColor = AppConstants.MauXanhNavyDam,
            Location = new Point(12, 30),
            AutoSize = true
        };

        lblPinGhiChu = new Label
        {
            Text = "Nhập mã PIN trên màn hình cảm ứng của ngăn tủ để mở khóa.",
            Font = new Font("Times New Roman", 9f, FontStyle.Regular),
            ForeColor = AppConstants.MauChuPhu,
            Location = new Point(170, 36),
            Size = new Size(300, 28)
        };

        pnlPinContainer.Controls.Add(lblPinTieuDe);
        pnlPinContainer.Controls.Add(lblPinGiaTri);
        pnlPinContainer.Controls.Add(lblPinGhiChu);

        var lblHuongDan = new Label
        {
            Text = "• Bước 1: Di chuyển đến khu tủ thông minh tại sảnh tầng tương ứng.\n" +
                   "• Bước 2: Nhập đúng mã PIN 6 số trên bàn phím cảm ứng ngăn tủ.\n" +
                   "• Bước 3: Cửa tủ tự động mở để quý khách nhận chìa khóa phòng (hoàn trả chìa khi trả phòng).",
            Font = new Font("Times New Roman", 9f, FontStyle.Regular),
            ForeColor = AppConstants.MauChuPhu,
            Location = new Point(0, 182),
            Size = new Size(490, 52)
        };

        pnlInfoFields.Controls.Add(lblChiTietPhong);
        pnlInfoFields.Controls.Add(lblChiTietViTriTu);
        pnlInfoFields.Controls.Add(lblChiTietLuuTru);
        pnlInfoFields.Controls.Add(lblChiTietTrangThai);
        pnlInfoFields.Controls.Add(pnlPinContainer);
        pnlInfoFields.Controls.Add(lblHuongDan);

        pnlDetailCard.Controls.Add(pnlInfoFields);
        pnlDetailCard.Controls.Add(pnlDividerDetail);
        pnlDetailCard.Controls.Add(lblDetailHeader);
        tblLeft.Controls.Add(pnlDetailCard, 0, 1);

        tblBody.Controls.Add(tblLeft, 0, 0);

        var pnlRight = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(16, 12, 16, 12),
            Margin = new Padding(0)
        };

        var pnlRightTop = new Panel
        {
            Dock = DockStyle.Top,
            Height = 64,
            BackColor = Color.White
        };

        var lblRightTitle = new Label
        {
            Text = "SƠ ĐỒ TRỰC QUAN TỦ CHÌA KHÓA",
            Font = new Font("Times New Roman", 12f, FontStyle.Bold),
            ForeColor = AppConstants.MauXanhNavy,
            Location = new Point(0, 4),
            AutoSize = true
        };

        var lblChonTang = new Label
        {
            Text = "Khu vực tầng:",
            Font = new Font("Times New Roman", 10f, FontStyle.Bold),
            ForeColor = AppConstants.MauChuChinh,
            Location = new Point(0, 34),
            AutoSize = true
        };

        cboTang = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Times New Roman", 10f, FontStyle.Regular),
            Location = new Point(100, 31),
            Width = 180
        };
        cboTang.Items.AddRange(new object[]
        {
            "Tầng 1 (Khu Tủ A)",
            "Tầng 2 (Khu Tủ B)",
            "Tầng 3 (Khu Tủ C)"
        });
        cboTang.SelectedIndex = 0;
        cboTang.SelectedIndexChanged += (s, e) => VeSoDoNganTu();

        pnlRightTop.Controls.Add(lblRightTitle);
        pnlRightTop.Controls.Add(lblChonTang);
        pnlRightTop.Controls.Add(cboTang);

        var pnlLegend = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 36,
            BackColor = Color.White
        };

        var lblLegend = new Label
        {
            Text = "[■] Vàng đồng: Ngăn tủ của bạn     [□] Trắng viền xám: Ngăn tủ khác / đang khóa",
            Font = new Font("Times New Roman", 9.5f, FontStyle.Regular),
            ForeColor = AppConstants.MauChuChinh,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };
        pnlLegend.Controls.Add(lblLegend);

        tblLockerGrid = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 3,
            Margin = new Padding(0, 8, 0, 8),
            BackColor = AppConstants.MauNenChung,
            Padding = new Padding(6)
        };
        for (var c = 0; c < 4; c++)
        {
            tblLockerGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
        }
        for (var r = 0; r < 3; r++)
        {
            tblLockerGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33f));
        }

        KhoiTao12ONganTu();

        pnlRight.Controls.Add(tblLockerGrid);
        pnlRight.Controls.Add(pnlLegend);
        pnlRight.Controls.Add(pnlRightTop);
        tblBody.Controls.Add(pnlRight, 1, 0);

        tblRoot.Controls.Add(tblBody, 0, 1);
        Controls.Add(tblRoot);
    }

    // Tạo 12 ô ngăn tủ
    private void KhoiTao12ONganTu()
    {
        _danhSachOThuNgan.Clear();
        tblLockerGrid.Controls.Clear();

        for (var i = 0; i < 12; i++)
        {
            var pnlO = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(4),
                Padding = new Padding(6)
            };

            var lblTenNgan = new Label
            {
                Name = "lblTenNgan",
                Text = "Ngăn ---",
                Font = new Font("Times New Roman", 10f, FontStyle.Bold),
                ForeColor = AppConstants.MauChuChinh,
                Dock = DockStyle.Top,
                Height = 20,
                TextAlign = ContentAlignment.MiddleCenter
            };

            var lblTrangThaiO = new Label
            {
                Name = "lblTrangThaiO",
                Text = "[ĐÃ KHÓA]",
                Font = new Font("Times New Roman", 9f, FontStyle.Regular),
                ForeColor = AppConstants.MauChuPhu,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            var lblPhongO = new Label
            {
                Name = "lblPhongO",
                Text = "---",
                Font = new Font("Times New Roman", 8.5f, FontStyle.Regular),
                ForeColor = AppConstants.MauChuPhu,
                Dock = DockStyle.Bottom,
                Height = 18,
                TextAlign = ContentAlignment.MiddleCenter
            };

            pnlO.Controls.Add(lblTrangThaiO);
            pnlO.Controls.Add(lblPhongO);
            pnlO.Controls.Add(lblTenNgan);

            var col = i % 4;
            var row = i / 4;
            tblLockerGrid.Controls.Add(pnlO, col, row);
            _danhSachOThuNgan.Add(pnlO);
        }
    }

    // Tải danh sách đơn đặt phòng và cập nhật tủ chìa khóa
    public void TaiDuLieuTuKhoa()
    {
        UIHelper.RunSafe(() =>
        {
            dgvDonPhong.Rows.Clear();
            _donDangChon = null;

            var khach = SessionManager.CurrentCustomer;
            if (khach == null || khach.CustomerId <= 0)
            {
                CapNhatChiTietTrong();
                VeSoDoNganTu();
                return;
            }

            var ds = AppServices.Booking.GetByCustomerId(khach.CustomerId);
            foreach (var dp in ds)
            {
                var (_, _, _, maNganTu, viTriTu) = LayThongTinNganTu(dp.RoomNumber);
                var (chuoiTrangThaiDon, chuoiTrangThaiChia, _) = LayTrangThaiNghiepVu(dp);

                var soPhongHienThi = string.IsNullOrWhiteSpace(dp.RoomNumber) ? "Chưa xếp" : dp.RoomNumber;
                var viTriHienThi = string.IsNullOrWhiteSpace(dp.RoomNumber) ? "Chưa cấp tủ" : viTriTu;

                var idx = dgvDonPhong.Rows.Add(
                    dp.DisplayCode,
                    soPhongHienThi,
                    dp.RoomTypeName,
                    viTriHienThi,
                    chuoiTrangThaiDon,
                    chuoiTrangThaiChia);
                dgvDonPhong.Rows[idx].Tag = dp;
            }

            if (dgvDonPhong.Rows.Count > 0)
            {
                dgvDonPhong.Rows[0].Selected = true;
                _donDangChon = dgvDonPhong.Rows[0].Tag as Booking;
            }

            CapNhatChiTietDonDangChon();
            VeSoDoNganTu();
        }, "tải dữ liệu tủ chìa khóa", this);
    }

    // Xử lý lựa chọn đơn đặt phòng từ danh sách
    private void XuLyChonDongDonPhong()
    {
        if (dgvDonPhong.SelectedRows.Count > 0)
        {
            _donDangChon = dgvDonPhong.SelectedRows[0].Tag as Booking;
        }
        else
        {
            _donDangChon = null;
        }

        CapNhatChiTietDonDangChon();
        VeSoDoNganTu();
    }

    // Cập nhật thông tin chi tiết nhận chìa khóa theo đơn đang chọn
    private void CapNhatChiTietDonDangChon()
    {
        if (_donDangChon == null)
        {
            CapNhatChiTietTrong();
            return;
        }

        var (tang, _, _, maNganTu, viTriTu) = LayThongTinNganTu(_donDangChon.RoomNumber);
        var (chuoiTrangThaiDon, chuoiTrangThaiChia, coTheNhanChia) = LayTrangThaiNghiepVu(_donDangChon);

        var soPhong = string.IsNullOrWhiteSpace(_donDangChon.RoomNumber) ? "Chưa xếp" : _donDangChon.RoomNumber;
        lblChiTietPhong.Text = $"Phòng: {soPhong} - Hạng phòng: {_donDangChon.RoomTypeName}";
        lblChiTietViTriTu.Text = $"Vị trí ngăn tủ: {viTriTu}";
        lblChiTietLuuTru.Text = $"Thời gian lưu trú: {_donDangChon.CheckInDate} đến {_donDangChon.CheckOutDate}";
        lblChiTietTrangThai.Text = $"Trạng thái đơn: {chuoiTrangThaiDon} | Chìa khóa: {chuoiTrangThaiChia}";

        if (coTheNhanChia && !string.IsNullOrWhiteSpace(_donDangChon.RoomNumber))
        {
            var maPin = TinhMaPinCoDinh(_donDangChon.BookingId, _donDangChon.RoomNumber);
            pnlPinContainer.BackColor = Color.FromArgb(254, 249, 195);
            lblPinTieuDe.Text = "MÃ PIN MỞ TỦ (6 CHỮ SỐ):";
            lblPinGiaTri.Text = $"[ {maPin[0]} {maPin[1]} {maPin[2]}  {maPin[3]} {maPin[4]} {maPin[5]} ]";
            lblPinGiaTri.ForeColor = AppConstants.MauXanhNavyDam;

            if (_donDangChon.Status == BookingStatus.CheckedIn)
            {
                lblPinGhiChu.Text = "Quý khách đang lưu trú. Có thể dùng mã PIN trên để mở lại tủ hoặc hoàn trả chìa.";
            }
            else
            {
                lblPinGhiChu.Text = "Nhập mã PIN trên màn hình cảm ứng của ngăn tủ để mở khóa lấy chìa.";
            }

            var indexTang = Math.Clamp(tang - 1, 0, 2);
            if (cboTang.SelectedIndex != indexTang)
            {
                cboTang.SelectedIndex = indexTang;
            }
        }
        else
        {
            pnlPinContainer.BackColor = AppConstants.MauNenChung;
            lblPinTieuDe.Text = "MÃ PIN MỞ TỦ:";
            lblPinGiaTri.Text = "[ CHƯA CẤP ]";
            lblPinGiaTri.ForeColor = AppConstants.MauChuPhu;

            if (_donDangChon.Status == BookingStatus.Pending)
            {
                lblPinGhiChu.Text = "Đơn đang chờ lễ tân xác nhận và xếp số phòng. Mã mở tủ sẽ hiển thị ngay khi được duyệt.";
            }
            else if (_donDangChon.Status == BookingStatus.CheckedOut)
            {
                lblPinGhiChu.Text = "Kỳ nghỉ đã hoàn tất. Chìa khóa phòng đã được bàn giao về quầy lễ tân.";
            }
            else if (_donDangChon.Status == BookingStatus.Cancelled)
            {
                lblPinGhiChu.Text = "Đơn đặt phòng đã hủy. Hệ thống không cấp ngăn tủ chìa khóa cho đơn này.";
            }
            else
            {
                lblPinGhiChu.Text = "Phòng chưa được xếp cụ thể. Vui lòng liên hệ lễ tân để được hỗ trợ.";
            }
        }
    }

    // Làm khung hiển thị khi không có đơn nào
    private void CapNhatChiTietTrong()
    {
        lblChiTietPhong.Text = "Phòng: Quý khách chưa chọn đơn phòng nào";
        lblChiTietViTriTu.Text = "Vị trí ngăn tủ: ---";
        lblChiTietLuuTru.Text = "Thời gian lưu trú: ---";
        lblChiTietTrangThai.Text = "Trạng thái chìa: ---";
        pnlPinContainer.BackColor = AppConstants.MauNenChung;
        lblPinTieuDe.Text = "MÃ PIN MỞ TỦ:";
        lblPinGiaTri.Text = "[ ------ ]";
        lblPinGiaTri.ForeColor = AppConstants.MauChuPhu;
        lblPinGhiChu.Text = "Vui lòng chọn một đơn đặt phòng ở bảng bên trên để xem vị trí tủ và mã nhận chìa.";
    }

    // Hiển thị trạng thái 12 ngăn tủ theo tầng đang chọn
    private void VeSoDoNganTu()
    {
        var tangHienTai = cboTang.SelectedIndex + 1;
        var khuHienTai = (char)('A' + cboTang.SelectedIndex);

        string? maNganKhach = null;
        var khachCoTheNhanChia = false;

        if (_donDangChon != null && !string.IsNullOrWhiteSpace(_donDangChon.RoomNumber))
        {
            var (tangDon, _, _, maNgan, _) = LayThongTinNganTu(_donDangChon.RoomNumber);
            var (_, _, hopLe) = LayTrangThaiNghiepVu(_donDangChon);
            if (hopLe && tangDon == tangHienTai)
            {
                maNganKhach = maNgan;
                khachCoTheNhanChia = true;
            }
        }

        for (var i = 0; i < 12 && i < _danhSachOThuNgan.Count; i++)
        {
            var oPanel = _danhSachOThuNgan[i];
            var soThuTu = $"{i + 1:D2}";
            var maNgan = $"{khuHienTai}{soThuTu}";
            var phongTuongUng = $"{tangHienTai}{soThuTu}";

            var lblTenNgan = oPanel.Controls["lblTenNgan"] as Label;
            var lblTrangThaiO = oPanel.Controls["lblTrangThaiO"] as Label;
            var lblPhongO = oPanel.Controls["lblPhongO"] as Label;

            var laNganCuaKhach = (khachCoTheNhanChia && maNgan == maNganKhach);

            if (laNganCuaKhach)
            {
                oPanel.BackColor = AppConstants.MauVangCam;
                if (lblTenNgan != null)
                {
                    lblTenNgan.Text = $"Ngăn {maNgan}";
                    lblTenNgan.ForeColor = AppConstants.MauChuTrenVang;
                    lblTenNgan.Font = new Font("Times New Roman", 10.5f, FontStyle.Bold);
                }
                if (lblTrangThaiO != null)
                {
                    lblTrangThaiO.Text = "[CHÌA CỦA BẠN]";
                    lblTrangThaiO.ForeColor = AppConstants.MauChuTrenVang;
                    lblTrangThaiO.Font = new Font("Times New Roman", 10f, FontStyle.Bold);
                }
                if (lblPhongO != null)
                {
                    lblPhongO.Text = $"Phòng {_donDangChon?.RoomNumber}";
                    lblPhongO.ForeColor = AppConstants.MauChuTrenVang;
                    lblPhongO.Font = new Font("Times New Roman", 9.5f, FontStyle.Bold);
                }
            }
            else
            {
                oPanel.BackColor = Color.White;
                if (lblTenNgan != null)
                {
                    lblTenNgan.Text = $"Ngăn {maNgan}";
                    lblTenNgan.ForeColor = AppConstants.MauChuChinh;
                    lblTenNgan.Font = new Font("Times New Roman", 10f, FontStyle.Bold);
                }
                if (lblTrangThaiO != null)
                {
                    lblTrangThaiO.Text = "[ĐÃ KHÓA]";
                    lblTrangThaiO.ForeColor = AppConstants.MauChuPhu;
                    lblTrangThaiO.Font = new Font("Times New Roman", 9f, FontStyle.Regular);
                }
                if (lblPhongO != null)
                {
                    lblPhongO.Text = $"P.{phongTuongUng}";
                    lblPhongO.ForeColor = AppConstants.MauChuPhu;
                    lblPhongO.Font = new Font("Times New Roman", 8.5f, FontStyle.Regular);
                }
            }
        }
    }
    
    private static (int tang, char khu, string soThuTu, string maNganTu, string viTriTu) LayThongTinNganTu(string? roomNumber)
    {
        var raw = (roomNumber ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(raw))
        {
            return (1, 'A', "00", "Chưa xếp", "Chưa xếp phòng");
        }

        var tang = 1;
        var digitsOnly = new string(raw.Where(char.IsDigit).ToArray());
        if (int.TryParse(digitsOnly, out var num) && num >= 100)
        {
            tang = Math.Max(1, num / 100);
        }
        else if (digitsOnly.Length > 0)
        {
            tang = Math.Max(1, digitsOnly[0] - '0');
        }

        var khu = (char)('A' + Math.Min(25, Math.Max(0, tang - 1)));
        var soThuTu = "01";
        if (digitsOnly.Length >= 2)
        {
            soThuTu = digitsOnly[^2..];
        }
        else if (digitsOnly.Length == 1)
        {
            soThuTu = "0" + digitsOnly;
        }

        var maNganTu = $"{khu}{soThuTu}";
        var viTriTu = $"Khu Tủ Tầng {tang} - Ngăn {maNganTu}";
        return (tang, khu, soThuTu, maNganTu, viTriTu);
    }

    // Tính mã PIN 6 số cố định từ BookingId và tổng mã ASCII của RoomNumber
    private static string TinhMaPinCoDinh(int bookingId, string? roomNumber)
    {
        var charSum = 0;
        var rawRoom = (roomNumber ?? string.Empty).Trim();
        foreach (var c in rawRoom)
        {
            charSum = (charSum * 31) + c;
        }

        var seed = Math.Abs((bookingId * 7919) + charSum);
        var pinNumber = 100000 + (seed % 900000);
        return $"{pinNumber:D6}";
    }

    // Phân tích trạng thái đơn và trạng thái cấp chìa khóa
    private static (string trangThaiDon, string trangThaiChia, bool hopLe) LayTrangThaiNghiepVu(Booking dp)
    {
        return dp.Status switch
        {
            BookingStatus.Confirmed => (AppConstants.DPDaXacNhan, "Sẵn sàng nhận chìa", true),
            BookingStatus.CheckedIn => (AppConstants.DPDangO, "Đang giữ chìa khóa", true),
            BookingStatus.Pending => (AppConstants.DPChoDuyet, "Chờ lễ tân duyệt", false),
            BookingStatus.CheckedOut => (AppConstants.DPDaHoanTat, "Đã hoàn trả chìa", false),
            BookingStatus.Cancelled => (AppConstants.DPDaHuy, "Đơn đã hủy", false),
            _ => (AppConstants.DPChoDuyet, "Chờ duyệt", false)
        };
    }
}
