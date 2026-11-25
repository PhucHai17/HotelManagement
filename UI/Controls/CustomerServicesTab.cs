using System.Globalization;
using System.Text;
using BLL;
using Common.Constants;
using Common.Helpers;
using Entity.Entities;
using Entity.Enums;
using HotelManagementSystem.Models.Entities;
using UI.Helpers;

namespace UI.Controls;

public class CustomerServicesTab : UserControl
{
    private DataGridView dgvDichVu = null!;
    private DataGridView dgvLichSuDichVu = null!;
    private PictureBox picDichVu = null!;
    private Label lblPreviewTenDV = null!;
    private Label lblPreviewGiaDV = null!;
    private Label lblMoTaDV = null!;
    private Label lblQuyDinhDV = null!;
    private ComboBox cboDonDatPhong = null!;
    private Label lblThongBaoChuaCoDon = null!;
    private Label lblDichVuDangChon = null!;
    private NumericUpDown numSoLuong = null!;
    private Label lblTongTienDuKien = null!;
    private Button btnDatDichVu = null!;
    private Label lblDonPhongHienTai = null!;
    private Label lblTongTienLichSu = null!;

    private Service? _dichVuDangChon;
    private List<Service> _danhSachDichVu = new();

    public CustomerServicesTab()
    {
        Font = new Font("Times New Roman", 10f, FontStyle.Regular);
        KhoiTaoGiaoDien();
        TaiDuLieuDichVu();
    }

    // Khởi tạo bố cục giao diện tab dịch vụ khách hàng
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
            Text = "DỊCH VỤ KHÁCH SẠN (HOTEL SERVICES)",
            Font = new Font("Times New Roman", 16f, FontStyle.Bold),
            ForeColor = AppConstants.MauXanhNavy,
            Location = new Point(14, 6),
            AutoSize = true
        };

        var lblSubtitle = new Label
        {
            Text = "Khám phá các tiện ích cao cấp và đặt thêm dịch vụ trực tiếp vào phòng lưu trú của quý khách",
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
        btnLamMoi.Click += (s, e) => TaiDuLieuDichVu();

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
        tblLeft.RowStyles.Add(new RowStyle(SizeType.Percent, 58f));
        tblLeft.RowStyles.Add(new RowStyle(SizeType.Percent, 42f));

        var pnlCatalogBox = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(14),
            Margin = new Padding(0, 0, 0, 8)
        };

        var lblCatalogTitle = new Label
        {
            Text = "DANH MỤC DỊCH VỤ HIỆN CÓ",
            Font = new Font("Times New Roman", 12f, FontStyle.Bold),
            ForeColor = AppConstants.MauXanhNavy,
            Dock = DockStyle.Top,
            Height = 28
        };

        var tblCatalogSplit = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 1,
            ColumnCount = 2,
            Margin = new Padding(0)
        };
        tblCatalogSplit.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55f));
        tblCatalogSplit.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45f));

        dgvDichVu = new DataGridView
        {
            Dock = DockStyle.Fill,
            Font = new Font("Times New Roman", 10f, FontStyle.Regular)
        };
        UIHelper.DinhDangBangDuLieu(dgvDichVu);
        dgvDichVu.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 10f, FontStyle.Bold);

        dgvDichVu.Columns.Add("MaDV", "Mã");
        dgvDichVu.Columns.Add("TenDV", "Tên dịch vụ");
        dgvDichVu.Columns.Add("DonGia", "Đơn giá");
        dgvDichVu.Columns.Add("DonVi", "Đơn vị tính");

        dgvDichVu.Columns["MaDV"].MinimumWidth = 45;
        dgvDichVu.Columns["MaDV"].FillWeight = 40;
        dgvDichVu.Columns["TenDV"].MinimumWidth = 120;
        dgvDichVu.Columns["TenDV"].FillWeight = 115;
        dgvDichVu.Columns["DonGia"].MinimumWidth = 85;
        dgvDichVu.Columns["DonGia"].FillWeight = 80;
        dgvDichVu.Columns["DonVi"].MinimumWidth = 70;
        dgvDichVu.Columns["DonVi"].FillWeight = 65;

        dgvDichVu.SelectionChanged += (s, e) => XuLyChonDichVu();
        dgvDichVu.CellClick += (s, e) => XuLyChonDichVu();

        var pnlPreview = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Margin = new Padding(8, 0, 0, 0),
            AutoScroll = true
        };

        var pnlCardBody = new Panel
        {
            Location = new Point(0, 0),
            BackColor = Color.White
        };

        picDichVu = new PictureBox
        {
            BackColor = Color.FromArgb(245, 247, 250),
            SizeMode = PictureBoxSizeMode.StretchImage
        };

        lblPreviewTenDV = new Label
        {
            Font = new Font("Times New Roman", 13f, FontStyle.Bold),
            ForeColor = AppConstants.MauXanhNavy,
            TextAlign = ContentAlignment.MiddleLeft,
            Text = "Tên dịch vụ"
        };

        lblPreviewGiaDV = new Label
        {
            Font = new Font("Times New Roman", 12f, FontStyle.Bold),
            ForeColor = AppConstants.MauVangCamDam,
            TextAlign = ContentAlignment.MiddleLeft,
            Text = "0 đ / lượt"
        };

        var pnlPreviewDivider = new Panel
        {
            BackColor = AppConstants.MauVien
        };

        lblMoTaDV = new Label
        {
            Font = new Font("Times New Roman", 10f, FontStyle.Regular),
            ForeColor = AppConstants.MauChuChinh,
            TextAlign = ContentAlignment.TopLeft,
            Text = "Chọn dịch vụ để xem chi tiết"
        };

        lblQuyDinhDV = new Label
        {
            Font = new Font("Times New Roman", 9.5f, FontStyle.Regular),
            ForeColor = AppConstants.MauChuPhu,
            TextAlign = ContentAlignment.TopLeft,
            Text = "• Phục vụ hàng ngày: 06:30 - 22:00\n• Đặt trực tiếp và thanh toán khi trả phòng."
        };

        pnlCardBody.Controls.Add(picDichVu);
        pnlCardBody.Controls.Add(lblPreviewTenDV);
        pnlCardBody.Controls.Add(lblPreviewGiaDV);
        pnlCardBody.Controls.Add(pnlPreviewDivider);
        pnlCardBody.Controls.Add(lblMoTaDV);
        pnlCardBody.Controls.Add(lblQuyDinhDV);
        pnlPreview.Controls.Add(pnlCardBody);

        void CapNhatKichThuocKhungAnh()
        {
            var contentWidth = Math.Max(120, pnlPreview.ClientSize.Width - 24);
            var imgHeight = Math.Max(80, (int)(contentWidth * 768.0 / 1376.0));

            pnlCardBody.Width = pnlPreview.ClientSize.Width;

            picDichVu.SetBounds(12, 10, contentWidth, imgHeight);

            var top = picDichVu.Bottom + 10;
            lblPreviewTenDV.SetBounds(12, top, contentWidth, 28);

            top = lblPreviewTenDV.Bottom + 2;
            lblPreviewGiaDV.SetBounds(12, top, contentWidth, 24);

            top = lblPreviewGiaDV.Bottom + 6;
            pnlPreviewDivider.SetBounds(12, top, contentWidth, 1);

            top = pnlPreviewDivider.Bottom + 8;
            lblMoTaDV.SetBounds(12, top, contentWidth, 62);

            top = lblMoTaDV.Bottom + 6;
            lblQuyDinhDV.SetBounds(12, top, contentWidth, 56);

            pnlCardBody.Height = lblQuyDinhDV.Bottom + 12;
        }

        pnlPreview.ClientSizeChanged += (s, e) => CapNhatKichThuocKhungAnh();
        CapNhatKichThuocKhungAnh();

        tblCatalogSplit.Controls.Add(dgvDichVu, 0, 0);
        tblCatalogSplit.Controls.Add(pnlPreview, 1, 0);

        pnlCatalogBox.Controls.Add(tblCatalogSplit);
        pnlCatalogBox.Controls.Add(lblCatalogTitle);
        tblLeft.Controls.Add(pnlCatalogBox, 0, 0);

        var pnlOrderCard = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(16, 12, 16, 12),
            Margin = new Padding(0)
        };

        var lblOrderTitle = new Label
        {
            Text = "ĐẶT DỊCH VỤ VÀO PHÒNG LƯU TRÚ",
            Font = new Font("Times New Roman", 12f, FontStyle.Bold),
            ForeColor = AppConstants.MauXanhNavy,
            Dock = DockStyle.Top,
            Height = 26
        };

        var pnlDivider = new Panel
        {
            Dock = DockStyle.Top,
            Height = 1,
            BackColor = AppConstants.MauVien,
            Margin = new Padding(0, 0, 0, 8)
        };

        var pnlOrderBody = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(0, 6, 0, 0)
        };

        var lblChonDon = new Label
        {
            Text = "1. Chọn đơn phòng lưu trú của quý khách:",
            Font = new Font("Times New Roman", 10f, FontStyle.Bold),
            ForeColor = AppConstants.MauChuChinh,
            Location = new Point(0, 4),
            Size = new Size(480, 20)
        };

        cboDonDatPhong = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Times New Roman", 10f, FontStyle.Regular),
            Location = new Point(0, 26),
            Size = new Size(480, 28)
        };
        cboDonDatPhong.SelectedIndexChanged += (s, e) => XuLyChonDonDatPhong();

        lblThongBaoChuaCoDon = new Label
        {
            Text = "Quý khách cần có đơn đặt phòng ở trạng thái 'Đã xác nhận' hoặc 'Đang ở' để đặt dịch vụ.",
            Font = new Font("Times New Roman", 9f, FontStyle.Regular),
            ForeColor = AppConstants.MauDo,
            Location = new Point(0, 56),
            Size = new Size(480, 18),
            Visible = false
        };

        var lblChonDV = new Label
        {
            Text = "2. Dịch vụ đang chọn:",
            Font = new Font("Times New Roman", 10f, FontStyle.Bold),
            ForeColor = AppConstants.MauChuChinh,
            Location = new Point(0, 76),
            Size = new Size(480, 20)
        };

        lblDichVuDangChon = new Label
        {
            Text = "Chưa chọn dịch vụ",
            Font = new Font("Times New Roman", 10.5f, FontStyle.Bold),
            ForeColor = AppConstants.MauXanhNavyDam,
            Location = new Point(0, 98),
            Size = new Size(480, 22)
        };

        var lblSoLuongTitle = new Label
        {
            Text = "Số lượng:",
            Font = new Font("Times New Roman", 10f, FontStyle.Bold),
            ForeColor = AppConstants.MauChuChinh,
            Location = new Point(0, 128),
            Size = new Size(70, 24),
            TextAlign = ContentAlignment.MiddleLeft
        };

        numSoLuong = new NumericUpDown
        {
            Font = new Font("Times New Roman", 10f, FontStyle.Regular),
            Location = new Point(75, 126),
            Size = new Size(70, 28),
            Minimum = 1,
            Maximum = 50,
            Value = 1
        };
        numSoLuong.ValueChanged += (s, e) => CapNhatTongTienDuKien();

        var lblTongTienTitle = new Label
        {
            Text = "Tổng tiền tạm tính:",
            Font = new Font("Times New Roman", 10f, FontStyle.Bold),
            ForeColor = AppConstants.MauChuChinh,
            Location = new Point(170, 128),
            Size = new Size(130, 24),
            TextAlign = ContentAlignment.MiddleLeft
        };

        lblTongTienDuKien = new Label
        {
            Text = "0 đ",
            Font = new Font("Times New Roman", 12f, FontStyle.Bold),
            ForeColor = AppConstants.MauVangCamDam,
            Location = new Point(300, 128),
            Size = new Size(180, 24),
            TextAlign = ContentAlignment.MiddleLeft
        };

        btnDatDichVu = new Button
        {
            Text = "Xác nhận đặt dịch vụ",
            Font = new Font("Times New Roman", 10f, FontStyle.Bold),
            Location = new Point(0, 166),
            Size = new Size(200, 38),
            UseMnemonic = false,
            Cursor = Cursors.Hand
        };
        UIHelper.DinhDangNutBamNavy(btnDatDichVu);
        btnDatDichVu.Click += (s, e) => XuLyDatDichVu();

        pnlOrderBody.Controls.Add(lblChonDon);
        pnlOrderBody.Controls.Add(cboDonDatPhong);
        pnlOrderBody.Controls.Add(lblThongBaoChuaCoDon);
        pnlOrderBody.Controls.Add(lblChonDV);
        pnlOrderBody.Controls.Add(lblDichVuDangChon);
        pnlOrderBody.Controls.Add(lblSoLuongTitle);
        pnlOrderBody.Controls.Add(numSoLuong);
        pnlOrderBody.Controls.Add(lblTongTienTitle);
        pnlOrderBody.Controls.Add(lblTongTienDuKien);
        pnlOrderBody.Controls.Add(btnDatDichVu);

        pnlOrderCard.Controls.Add(pnlOrderBody);
        pnlOrderCard.Controls.Add(pnlDivider);
        pnlOrderCard.Controls.Add(lblOrderTitle);
        tblLeft.Controls.Add(pnlOrderCard, 0, 1);

        tblBody.Controls.Add(tblLeft, 0, 0);

        var pnlHistoryBox = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(16, 12, 16, 12),
            Margin = new Padding(0)
        };

        var pnlHistoryTop = new Panel
        {
            Dock = DockStyle.Top,
            Height = 52,
            BackColor = Color.White
        };

        var lblHistoryTitle = new Label
        {
            Text = "LỊCH SỬ DỊCH VỤ CỦA ĐƠN ĐANG CHỌN",
            Font = new Font("Times New Roman", 12f, FontStyle.Bold),
            ForeColor = AppConstants.MauXanhNavy,
            Location = new Point(0, 4),
            AutoSize = true
        };

        lblDonPhongHienTai = new Label
        {
            Text = "Đơn đặt phòng: Chưa chọn",
            Font = new Font("Times New Roman", 9.5f, FontStyle.Bold),
            ForeColor = AppConstants.MauChuPhu,
            Location = new Point(0, 28),
            Size = new Size(450, 20)
        };

        pnlHistoryTop.Controls.Add(lblHistoryTitle);
        pnlHistoryTop.Controls.Add(lblDonPhongHienTai);

        dgvLichSuDichVu = new DataGridView
        {
            Dock = DockStyle.Fill,
            Font = new Font("Times New Roman", 10f, FontStyle.Regular)
        };
        UIHelper.DinhDangBangDuLieu(dgvLichSuDichVu);
        dgvLichSuDichVu.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 10f, FontStyle.Bold);

        dgvLichSuDichVu.Columns.Add("TenDV", "Tên dịch vụ");
        dgvLichSuDichVu.Columns.Add("SoLuong", "SL");
        dgvLichSuDichVu.Columns.Add("DonGia", "Đơn giá");
        dgvLichSuDichVu.Columns.Add("ThanhTien", "Thành tiền");
        dgvLichSuDichVu.Columns.Add("NgayDat", "Thời gian đặt");

        dgvLichSuDichVu.Columns["TenDV"].MinimumWidth = 110;
        dgvLichSuDichVu.Columns["TenDV"].FillWeight = 110;
        dgvLichSuDichVu.Columns["SoLuong"].MinimumWidth = 45;
        dgvLichSuDichVu.Columns["SoLuong"].FillWeight = 40;
        dgvLichSuDichVu.Columns["DonGia"].MinimumWidth = 75;
        dgvLichSuDichVu.Columns["DonGia"].FillWeight = 75;
        dgvLichSuDichVu.Columns["ThanhTien"].MinimumWidth = 85;
        dgvLichSuDichVu.Columns["ThanhTien"].FillWeight = 85;
        dgvLichSuDichVu.Columns["NgayDat"].MinimumWidth = 95;
        dgvLichSuDichVu.Columns["NgayDat"].FillWeight = 95;

        var pnlHistoryBottom = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 62,
            BackColor = AppConstants.MauNenChung,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(12, 8, 12, 8)
        };

        var lblTongChiPhiTitle = new Label
        {
            Text = "TỔNG TIỀN DỊCH VỤ CỦA ĐƠN:",
            Font = new Font("Times New Roman", 10.5f, FontStyle.Bold),
            ForeColor = AppConstants.MauChuChinh,
            Location = new Point(10, 8),
            Size = new Size(240, 22),
            TextAlign = ContentAlignment.MiddleLeft
        };

        lblTongTienLichSu = new Label
        {
            Text = "0 đ",
            Font = new Font("Times New Roman", 14f, FontStyle.Bold),
            ForeColor = AppConstants.MauVangCamDam,
            Location = new Point(250, 6),
            Size = new Size(200, 24),
            TextAlign = ContentAlignment.MiddleRight,
            Anchor = AnchorStyles.Top | AnchorStyles.Right
        };

        var lblGhiChuThanhToan = new Label
        {
            Text = "Chi phí dịch vụ sẽ được kết chuyển vào hóa đơn thanh toán khi quý khách trả phòng.",
            Font = new Font("Times New Roman", 9f, FontStyle.Regular),
            ForeColor = AppConstants.MauChuPhu,
            Location = new Point(10, 34),
            Size = new Size(440, 20)
        };

        pnlHistoryBottom.Controls.Add(lblTongChiPhiTitle);
        pnlHistoryBottom.Controls.Add(lblTongTienLichSu);
        pnlHistoryBottom.Controls.Add(lblGhiChuThanhToan);

        pnlHistoryBox.Controls.Add(dgvLichSuDichVu);
        pnlHistoryBox.Controls.Add(pnlHistoryBottom);
        pnlHistoryBox.Controls.Add(pnlHistoryTop);
        tblBody.Controls.Add(pnlHistoryBox, 1, 0);

        tblRoot.Controls.Add(tblBody, 0, 1);
        Controls.Add(tblRoot);
    }

    // Tải danh mục dịch vụ và danh sách đơn phòng hợp lệ của khách
    public void TaiDuLieuDichVu()
    {
        UIHelper.RunSafe(() =>
        {
            _danhSachDichVu = AppServices.Hotel.GetAll();
            dgvDichVu.Rows.Clear();

            foreach (var dv in _danhSachDichVu)
            {
                var idx = dgvDichVu.Rows.Add(
                    $"DV{dv.ServiceId:D2}",
                    dv.Name,
                    FormatHelper.FormatCurrency(dv.UnitPrice),
                    dv.Unit);
                dgvDichVu.Rows[idx].Tag = dv;
            }

            if (dgvDichVu.Rows.Count > 0)
            {
                dgvDichVu.Rows[0].Selected = true;
                _dichVuDangChon = dgvDichVu.Rows[0].Tag as Service;
            }

            CapNhatThongTinDichVuDangChon();
            TaiDanhSachDonDatPhongHopLe();
        }, "tải dữ liệu dịch vụ", this);
    }

    // Tải danh sách đơn đặt phòng hợp lệ (Đã xác nhận hoặc Đang ở) của khách
    private void TaiDanhSachDonDatPhongHopLe()
    {
        var khach = SessionManager.CurrentCustomer;
        if (khach == null || khach.CustomerId <= 0)
        {
            HienThiTrangThaiChuaCoDon();
            return;
        }

        var dsDon = AppServices.Booking.GetByCustomerId(khach.CustomerId);
        var cacDonHopLe = dsDon.Where(d =>
            d.Status == BookingStatus.Confirmed ||
            d.Status == BookingStatus.CheckedIn ||
            d.Status.ToString().Equals("Confirmed", StringComparison.OrdinalIgnoreCase) ||
            d.Status.ToString().Equals("CheckedIn", StringComparison.OrdinalIgnoreCase)
        ).ToList();

        var dsChiTiet = AppServices.Hotel.GetCustomerBookingDetailsForService(khach.CustomerId);

        cboDonDatPhong.Items.Clear();
        foreach (var dp in cacDonHopLe)
        {
            var ct = dsChiTiet.FirstOrDefault(c => c.BookingId == dp.BookingId);
            if (ct != null)
            {
                var trangThaiText = (dp.Status == BookingStatus.CheckedIn || dp.Status.ToString().Equals("CheckedIn", StringComparison.OrdinalIgnoreCase))
                    ? "Đang ở"
                    : "Đã xác nhận";
                var roomText = string.IsNullOrWhiteSpace(ct.RoomNumber) ? dp.RoomNumber : ct.RoomNumber;
                var loaiText = string.IsNullOrWhiteSpace(ct.RoomTypeName) ? dp.RoomTypeName : ct.RoomTypeName;
                var itemText = $"[{dp.DisplayCode}] Phòng {roomText} - {loaiText} ({trangThaiText})";

                cboDonDatPhong.Items.Add(new BookingComboboxItem
                {
                    BookingDetailId = ct.BookingDetailId,
                    BookingId = dp.BookingId,
                    DisplayText = itemText,
                    RoomNumber = roomText,
                    DisplayCode = dp.DisplayCode
                });
            }
        }

        if (cboDonDatPhong.Items.Count > 0)
        {
            cboDonDatPhong.Enabled = true;
            lblThongBaoChuaCoDon.Visible = false;
            btnDatDichVu.Enabled = true;
            UIHelper.DinhDangNutBamNavy(btnDatDichVu);
            cboDonDatPhong.SelectedIndex = 0;
        }
        else
        {
            HienThiTrangThaiChuaCoDon();
        }
    }

    // Hiển thị trạng thái khi khách chưa có đơn phòng hợp lệ
    private void HienThiTrangThaiChuaCoDon()
    {
        cboDonDatPhong.Items.Clear();
        cboDonDatPhong.Items.Add("Chưa có đơn đặt phòng hợp lệ");
        cboDonDatPhong.SelectedIndex = 0;
        cboDonDatPhong.Enabled = false;

        lblThongBaoChuaCoDon.Visible = true;
        btnDatDichVu.Enabled = false;
        UIHelper.DinhDangNutBamVoHieu(btnDatDichVu);

        lblDonPhongHienTai.Text = "Đơn đặt phòng: Chưa có đơn hợp lệ (Cần đơn Đã xác nhận hoặc Đang ở)";
        dgvLichSuDichVu.Rows.Clear();
        lblTongTienLichSu.Text = "0 đ";
    }

    // Xử lý khi người dùng chọn dòng dịch vụ trong danh mục
    private void XuLyChonDichVu()
    {
        if (dgvDichVu.SelectedRows.Count > 0)
        {
            _dichVuDangChon = dgvDichVu.SelectedRows[0].Tag as Service;
        }
        else if (dgvDichVu.CurrentRow != null)
        {
            _dichVuDangChon = dgvDichVu.CurrentRow.Tag as Service;
        }
        else
        {
            _dichVuDangChon = null;
        }

        CapNhatThongTinDichVuDangChon();
    }

    // Cập nhật thông tin dịch vụ đang chọn trên thẻ xem trước chi tiết
    private void CapNhatThongTinDichVuDangChon()
    {
        if (_dichVuDangChon != null)
        {
            lblDichVuDangChon.Text = $"{_dichVuDangChon.Name} - {FormatHelper.FormatCurrency(_dichVuDangChon.UnitPrice)} / {_dichVuDangChon.Unit}";
            lblPreviewTenDV.Text = _dichVuDangChon.Name;
            lblPreviewGiaDV.Text = $"{FormatHelper.FormatCurrency(_dichVuDangChon.UnitPrice)} / {_dichVuDangChon.Unit}";
            lblMoTaDV.Text = LayMoTaChiTietDichVu(_dichVuDangChon);
            lblQuyDinhDV.Text = LayQuyDinhDichVu(_dichVuDangChon);
            picDichVu.Image = ImageHelper.LoadServiceImage(_dichVuDangChon.Name);
        }
        else
        {
            lblDichVuDangChon.Text = "Chưa chọn dịch vụ";
            lblPreviewTenDV.Text = "Chưa chọn dịch vụ";
            lblPreviewGiaDV.Text = "0 đ / lượt";
            lblMoTaDV.Text = "Vui lòng chọn một dịch vụ từ bảng danh mục bên trái để xem hình ảnh và thông tin chi tiết.";
            lblQuyDinhDV.Text = "• Phục vụ hàng ngày: 06:30 - 22:00\n• Đặt trực tiếp và thanh toán khi trả phòng";
            picDichVu.Image = ImageHelper.LoadServiceImage("breakfast");
        }

        picDichVu.Invalidate();
        CapNhatTongTienDuKien();
    }
    
    private static string LayMoTaChiTietDichVu(Service dv)
    {
        if (!string.IsNullOrWhiteSpace(dv.Description) && dv.Description.Length > 25 && !dv.Description.Equals(dv.Name, StringComparison.OrdinalIgnoreCase))
            return dv.Description;

        var ten = BoDauTiengViet(dv.Name);
        if (ten.Contains("spa") || ten.Contains("massage") || ten.Contains("tri lieu") || ten.Contains("xong hoi") || ten.Contains("thu gian"))
            return "Liệu trình massage và xông hơi thảo dược thư giãn chuyên sâu, giúp giải tỏa mệt mỏi và tái tạo năng lượng.";

        if (ten.Contains("giat") || ten.Contains("ui") || ten.Contains("la") || ten.Contains("laundry") || ten.Contains("quan ao"))
            return "Dịch vụ giặt sấy, ủi cao cấp và giao tận phòng trong ngày, bảo quản trang phục phẳng phiu và thơm mát.";

        if (ten.Contains("san bay") || ten.Contains("dua don") || ten.Contains("xe") || ten.Contains("airport") || ten.Contains("shuttle"))
            return "Dịch vụ xe sedan cao cấp đưa đón sân bay 2 chiều an toàn, tiện nghi với tài xế chuyên nghiệp và nước uống miễn phí.";

        if (ten.Contains("minibar") || ten.Contains("snack") || ten.Contains("tu lanh") || ten.Contains("nuoc ngot") || ten.Contains("bia"))
            return "Các loại nước giải khát, bia mát lạnh và đồ ăn vặt đóng gói cao cấp được bố trí tiện lợi tại phòng nghỉ.";

        if (ten.Contains("sang") || ten.Contains("buffet") || ten.Contains("breakfast") || ten.Contains("an sang") || ten.Contains("diem tam") || ten.Contains("am thuc"))
            return "Tiệc buffet sáng phong phú tiêu chuẩn 5 sao với hơn 40 món Âu - Á, bánh ngọt tươi và nước ép tự nhiên mỗi ngày.";

        return $"Dịch vụ {dv.Name} cao cấp tiêu chuẩn khách sạn, mang lại trải nghiệm nghỉ dưỡng trọn vẹn và tiện nghi cho quý khách.";
    }

    // Lấy thời gian phục vụ và quy định áp dụng theo dịch vụ
    private static string LayQuyDinhDichVu(Service dv)
    {
        var ten = BoDauTiengViet(dv.Name);
        if (ten.Contains("spa") || ten.Contains("massage") || ten.Contains("tri lieu") || ten.Contains("xong hoi") || ten.Contains("thu gian"))
            return "• Phục vụ hàng ngày: 09:00 - 22:00\n• Đặt lịch hẹn trước để được phục vụ chu đáo nhất\n• Đặt trực tiếp và thanh toán khi trả phòng";

        if (ten.Contains("giat") || ten.Contains("ui") || ten.Contains("la") || ten.Contains("laundry") || ten.Contains("quan ao"))
            return "• Tiếp nhận đồ trước 11:00, giao lại trước 19:00\n• Giao nhận trang phục trực tiếp tận phòng nghỉ\n• Đặt trực tiếp và thanh toán khi trả phòng";

        if (ten.Contains("san bay") || ten.Contains("dua don") || ten.Contains("xe") || ten.Contains("airport") || ten.Contains("shuttle"))
            return "• Phục vụ 24/7 theo lịch hẹn chuyến bay\n• Quý khách vui lòng đặt trước ít nhất 2 giờ\n• Đặt trực tiếp và thanh toán khi trả phòng";

        if (ten.Contains("minibar") || ten.Contains("snack") || ten.Contains("tu lanh") || ten.Contains("nuoc ngot") || ten.Contains("bia"))
            return "• Sẵn sàng phục vụ 24/7 tại tủ lạnh trong phòng\n• Đặt trực tiếp và thanh toán khi trả phòng";

        if (ten.Contains("sang") || ten.Contains("buffet") || ten.Contains("breakfast") || ten.Contains("an sang") || ten.Contains("diem tam") || ten.Contains("am thuc"))
            return "• Phục vụ hàng ngày: 06:00 - 10:00\n• Phục vụ tại nhà hàng tầng 1 hoặc đặt giao lên phòng\n• Đặt trực tiếp và thanh toán khi trả phòng";

        return "• Phục vụ hàng ngày: 06:30 - 22:00\n• Đặt trực tiếp và thanh toán khi trả phòng";
    }
    
    private static string BoDauTiengViet(string? input)
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

    // Cập nhật tổng tiền tạm tính theo số lượng
    private void CapNhatTongTienDuKien()
    {
        if (_dichVuDangChon != null)
        {
            var soLuong = (int)numSoLuong.Value;
            var tongTien = _dichVuDangChon.UnitPrice * soLuong;
            lblTongTienDuKien.Text = FormatHelper.FormatCurrency(tongTien);
        }
        else
        {
            lblTongTienDuKien.Text = "0 đ";
        }
    }
    
    private void XuLyChonDonDatPhong()
    {
        if (cboDonDatPhong.SelectedItem is BookingComboboxItem item)
        {
            lblDonPhongHienTai.Text = $"Đơn đặt phòng: [{item.DisplayCode}] Phòng {item.RoomNumber}";
            TaiLichSuDichVuCuaDon(item.BookingId);
        }
    }

    // Tải lịch sử dịch vụ đã sử dụng theo mã đặt phòng
    private void TaiLichSuDichVuCuaDon(int bookingId)
    {
        UIHelper.RunSafe(() =>
        {
            dgvLichSuDichVu.Rows.Clear();
            var dsUsages = AppServices.Hotel.GetServiceUsagesByBookingId(bookingId);
            double tongTien = 0;

            foreach (var u in dsUsages)
            {
                tongTien += u.LineTotal;
                var idx = dgvLichSuDichVu.Rows.Add(
                    u.ServiceName,
                    $"{u.Quantity} {u.Unit}",
                    FormatHelper.FormatCurrency(u.UnitPrice),
                    FormatHelper.FormatCurrency(u.LineTotal),
                    u.UsageDate);
                dgvLichSuDichVu.Rows[idx].Tag = u;
            }

            lblTongTienLichSu.Text = FormatHelper.FormatCurrency(tongTien);
        }, "tải lịch sử sử dụng dịch vụ", this);
    }

    // Xử lý xác nhận đặt dịch vụ vào phòng
    private void XuLyDatDichVu()
    {
        if (_dichVuDangChon == null)
        {
            MessageBox.Show(this, "Vui lòng chọn một dịch vụ trong danh mục!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (cboDonDatPhong.SelectedItem is not BookingComboboxItem item)
        {
            MessageBox.Show(this, "Vui lòng chọn đơn đặt phòng hợp lệ để gắn dịch vụ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var soLuong = (int)numSoLuong.Value;
        var tongTien = _dichVuDangChon.UnitPrice * soLuong;

        var hoi = MessageBox.Show(this,
            $"Quý khách có chắc chắn muốn đặt {soLuong} {_dichVuDangChon.Unit} '{_dichVuDangChon.Name}' với tổng tiền {FormatHelper.FormatCurrency(tongTien)} vào đơn phòng {item.DisplayCode} (Phòng {item.RoomNumber}) không?",
            "Xác nhận đặt dịch vụ",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (hoi != DialogResult.Yes) return;

        UIHelper.RunSafe(() =>
        {
            var thanhCong = AppServices.Hotel.RecordServiceUsageByBookingDetail(
                item.BookingDetailId,
                _dichVuDangChon.ServiceId,
                soLuong,
                out var thongBao);

            if (thanhCong)
            {
                MessageBox.Show(this, thongBao, "Đặt dịch vụ thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                numSoLuong.Value = 1;
                TaiLichSuDichVuCuaDon(item.BookingId);
            }
            else
            {
                MessageBox.Show(this, thongBao, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }, "đặt dịch vụ khách sạn", this);
    }

    private sealed class BookingComboboxItem
    {
        public int BookingDetailId { get; init; }
        public int BookingId { get; init; }
        public string DisplayText { get; init; } = string.Empty;
        public string RoomNumber { get; init; } = string.Empty;
        public string DisplayCode { get; init; } = string.Empty;

        public override string ToString() => DisplayText;
    }
}
