using System.ComponentModel;
using BLL;
using BLL.Interfaces;
using Common.Constants;
using Common.Helpers;
using Entity.DTOs;
using UI.Controls;

namespace UI.Forms;

// Giao diện chính của Lễ tân
[DesignerCategory("Code")]
public partial class ReceptionistMainForm : Form
{
    protected readonly IReportService _baoCaoBLL = AppServices.Report;
    private readonly IBookingService _datPhongBLL = AppServices.Booking;
    private readonly IHotelService _dichVuBLL = AppServices.Hotel;
    private readonly IInvoiceService _hoaDonBLL = AppServices.Invoice;
    private readonly ICustomerService _khachHangBLL = AppServices.Customer;
    private readonly IRoomService _phongBLL = AppServices.Room;

    protected readonly List<Button> _danhSachNutMenu = new();
    private readonly ToolTip _toolTip = new();
    private BookingTab _bookingTab = null!;
    protected Label lblTenNhanVien = null!;

    protected Panel pnlSidebar = null!;
    protected Panel pnlMainContent = null!;
    protected FlowLayoutPanel pnlMenuFlow = null!;

    private Panel pnlTongQuan = null!;
    private Panel pnlSoDoPhong = null!;
    private Panel pnlDatPhong = null!;
    private Panel pnlCheckInOut = null!;
    private Panel pnlKhachHang = null!;
    private Panel pnlDichVu = null!;
    private Panel pnlHoaDon = null!;
    protected Panel? pnlBaoCao;

    protected SimpleBarChartControl bieuDoThang = null!;
    protected StatCardControl cardBCTongDoanhThu = null!;
    protected StatCardControl cardBCCongSuat = null!;
    protected StatCardControl cardBCSoKhach = null!;
    private DateTimePicker dtpTuBaoCao = null!;
    private DateTimePicker dtpDenBaoCao = null!;
    private List<RevenueReportDto> _dsBaoCaoHienTai = new();

    public ReceptionistMainForm()
    {
        KhoiTaoKhungChinh();
        KhoiTaoCacTabNoiDung();
        UIHelper.ApDungFontTimesNewRoman(this);
        ChuyenTabMacDinh();
        Load += (s, e) => TaiDuLieuMacDinh();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) _toolTip.Dispose();
        base.Dispose(disposing);
    }
    
    protected virtual string LayTieuDeForm() => "Giao diện Lễ tân";
    protected virtual string LayNhanVaiTroSidebar() => "Lễ tân";
    protected virtual bool XuLyChuyenMenuMoRong(string tenChucNang) => false;
    
    protected virtual List<string> LayDanhSachChucNangMenu()
    {
        var cacChucNang = new List<string>
        {
            "Tổng quan", "Sơ đồ phòng", "Đặt phòng", "Check-in / Check-out", "Khách hàng", "Dịch vụ", "Hóa đơn"
        };
        if (SessionManager.HasPermission(AppConstants.QuyenXemBaoCao))
        {
            cacChucNang.Add("Báo cáo");
        }
        return cacChucNang;
    }
    
    protected virtual void ChuyenTabMacDinh()
    {
        ChuyenTab(pnlTongQuan, "Tổng quan");
    }
    
    protected virtual void TaiDuLieuMacDinh()
    {
        TaiDuLieuTongQuan();
    }
    
    protected Button TaoNutSidebar(string tenCN)
    {
        var btn = new Button
        {
            Text = tenCN,
            Size = new Size(240, 42),
            Margin = new Padding(0),
            Padding = new Padding(22, 0, 0, 0),
            FlatStyle = FlatStyle.Flat,
            ForeColor = Color.FromArgb(203, 213, 225),
            BackColor = AppConstants.MauXanhNavy,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font(AppConstants.FontChuChinh, 10.5f, FontStyle.Regular),
            Cursor = Cursors.Hand,
            UseMnemonic = false
        };
        btn.FlatAppearance.BorderSize = 0;
        btn.Click += (s, e) => XuLyChuyenMenu(tenCN);
        _danhSachNutMenu.Add(btn);
        pnlMenuFlow.Controls.Add(btn);
        return btn;
    }
    
    private void KhoiTaoKhungChinh()
    {
        Text = LayTieuDeForm();
        var vungLamViec = Screen.PrimaryScreen?.WorkingArea ?? new Rectangle(0, 0, 1280, 760);
        Size = new Size(
            Math.Min(1360, Math.Max(1100, vungLamViec.Width - 24)),
            Math.Min(820, Math.Max(700, vungLamViec.Height - 24)));
        MinimumSize = new Size(1100, 700);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.FromArgb(243, 244, 246);

        pnlSidebar = new Panel
        {
            Dock = DockStyle.Left,
            Width = 240,
            BackColor = AppConstants.MauXanhNavy
        };

        var tblSidebar = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 3,
            ColumnCount = 1,
            Margin = new Padding(0),
            Padding = new Padding(0)
        };
        tblSidebar.RowStyles.Add(new RowStyle(SizeType.Absolute, 68f));
        tblSidebar.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
        tblSidebar.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var pnlLogo = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppConstants.MauXanhNavy,
            Margin = new Padding(0),
            Padding = new Padding(22, 0, 12, 0)
        };
        var lblLogo = new HotelProLogo
        {
            Font = new Font(AppConstants.FontChuChinh, 16.5f, FontStyle.Bold),
            AutoSize = false,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };
        pnlLogo.Controls.Add(lblLogo);
        tblSidebar.Controls.Add(pnlLogo, 0, 0);

        pnlMenuFlow = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = true,
            BackColor = AppConstants.MauXanhNavy,
            Padding = new Padding(0, 4, 0, 0),
            Margin = new Padding(0)
        };

        var cacChucNang = LayDanhSachChucNangMenu();
        foreach (var tenCN in cacChucNang) TaoNutSidebar(tenCN);
        tblSidebar.Controls.Add(pnlMenuFlow, 0, 1);

        var pnlBottomUser = new Panel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            BackColor = AppConstants.MauXanhNavyDam,
            Margin = new Padding(0),
            Padding = new Padding(12, 10, 12, 14)
        };

        var tblBottomUser = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            RowCount = 3,
            ColumnCount = 1,
            Margin = new Padding(0),
            Padding = new Padding(0),
            BackColor = AppConstants.MauXanhNavyDam
        };
        tblBottomUser.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
        tblBottomUser.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        tblBottomUser.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        tblBottomUser.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var tenVaiTro = LayNhanVaiTroSidebar();
        var lblVaiTro = new Label
        {
            Text = tenVaiTro,
            ForeColor = AppConstants.MauVangCam,
            Font = new Font(AppConstants.FontChuChinh, 9.5f, FontStyle.Bold),
            AutoSize = true,
            UseMnemonic = false,
            Margin = new Padding(0, 0, 0, 2)
        };

        var tenHienThi = string.IsNullOrWhiteSpace(SessionManager.DisplayName)
            ? "Nhân viên"
            : SessionManager.DisplayName.Trim();
        lblTenNhanVien = new Label
        {
            Text = tenHienThi,
            ForeColor = Color.White,
            Font = new Font(AppConstants.FontChuChinh, 10f, FontStyle.Regular),
            AutoSize = true,
            MaximumSize = new Size(216, 0),
            AutoEllipsis = false,
            UseMnemonic = false,
            Margin = new Padding(0, 0, 0, 10)
        };
        _toolTip.SetToolTip(lblTenNhanVien, $"{tenVaiTro}: {tenHienThi}");

        var fontNutDangXuat = new Font(AppConstants.FontChuChinh, 10f, FontStyle.Bold);
        var caoNutDangXuat = Math.Max(36, TextRenderer.MeasureText("Đăng xuất", fontNutDangXuat).Height + 16);
        var btnDangXuat = new Button
        {
            Text = "Đăng xuất",
            Dock = DockStyle.Top,
            Height = caoNutDangXuat,
            MinimumSize = new Size(0, caoNutDangXuat),
            FlatStyle = FlatStyle.Flat,
            ForeColor = Color.White,
            BackColor = AppConstants.MauXanhNavy,
            Font = fontNutDangXuat,
            TextAlign = ContentAlignment.MiddleCenter,
            UseMnemonic = false,
            Margin = new Padding(0),
            Cursor = Cursors.Hand
        };
        btnDangXuat.FlatAppearance.BorderSize = 1;
        btnDangXuat.FlatAppearance.BorderColor = Color.FromArgb(148, 163, 184);
        btnDangXuat.Click += (s, e) =>
        {
            SessionManager.Logout();
            Close();
        };

        tblBottomUser.Controls.Add(lblVaiTro, 0, 0);
        tblBottomUser.Controls.Add(lblTenNhanVien, 0, 1);
        tblBottomUser.Controls.Add(btnDangXuat, 0, 2);
        pnlBottomUser.Controls.Add(tblBottomUser);
        tblSidebar.Controls.Add(pnlBottomUser, 0, 2);

        pnlSidebar.Controls.Add(tblSidebar);

        pnlMainContent = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(243, 244, 246),
            Padding = new Padding(16)
        };

        Controls.Add(pnlMainContent);
        Controls.Add(pnlSidebar);
    }
    
    protected virtual void XuLyChuyenMenu(string tenChucNang)
    {
        switch (tenChucNang)
        {
            case "Tổng quan":
                ChuyenTab(pnlTongQuan, tenChucNang);
                TaiDuLieuTongQuan();
                break;
            case "Sơ đồ phòng":
                ChuyenTab(pnlSoDoPhong, tenChucNang);
                TaiDuLieuSoDoPhong();
                break;
            case "Đặt phòng":
                ChuyenTab(pnlDatPhong, tenChucNang);
                _bookingTab.TaiDuLieuDatPhong();
                break;
            case "Check-in / Check-out":
                ChuyenTab(pnlCheckInOut, "Check-in / Check-out");
                TaiDuLieuCheckInOut();
                break;
            case "Khách hàng":
                ChuyenTab(pnlKhachHang, tenChucNang);
                TaiDuLieuKhachHang();
                break;
            case "Dịch vụ":
                ChuyenTab(pnlDichVu, tenChucNang);
                TaiDuLieuDichVu();
                break;
            case "Hóa đơn":
                ChuyenTab(pnlHoaDon, tenChucNang);
                TaiDuLieuHoaDon();
                break;
            case "Báo cáo":
                if (pnlBaoCao != null)
                {
                    ChuyenTab(pnlBaoCao, "Báo cáo");
                    TaiDuLieuBaoCao();
                }
                break;
            default:
                XuLyChuyenMenuMoRong(tenChucNang);
                break;
        }
    }
    
    protected void  ChuyenTab(Panel tabCanHien, string tenChucNang)
    {
        foreach (Control c in pnlMainContent.Controls) c.Visible = false;
        tabCanHien.Visible = true;
        tabCanHien.BringToFront();

        foreach (var btn in _danhSachNutMenu)
        {
            if (btn.Text.Trim() == tenChucNang)
            {
                btn.BackColor = AppConstants.MauVangCam;
                btn.ForeColor = Color.White;
                btn.Font = new Font(AppConstants.FontChuChinh, 10.5f, FontStyle.Bold);
            }
            else
            {
                btn.BackColor = AppConstants.MauXanhNavy;
                btn.ForeColor = Color.FromArgb(203, 213, 225);
                btn.Font = new Font(AppConstants.FontChuChinh, 10.5f, FontStyle.Regular);
            }
        }
    }
    
    protected virtual void KhoiTaoCacTabNoiDung()
    {
        pnlTongQuan = new Panel { Dock = DockStyle.Fill };
        KhoiTaoTabTongQuan();
        pnlMainContent.Controls.Add(pnlTongQuan);

        pnlSoDoPhong = new Panel { Dock = DockStyle.Fill };
        KhoiTaoTabSoDoPhong();
        pnlMainContent.Controls.Add(pnlSoDoPhong);

        pnlDatPhong = new Panel { Dock = DockStyle.Fill };
        _bookingTab = new BookingTab { Dock = DockStyle.Fill };
        pnlDatPhong.Controls.Add(_bookingTab);
        pnlMainContent.Controls.Add(pnlDatPhong);

        pnlCheckInOut = new Panel { Dock = DockStyle.Fill };
        KhoiTaoTabCheckInOut();
        pnlMainContent.Controls.Add(pnlCheckInOut);
        
        pnlKhachHang = new Panel { Dock = DockStyle.Fill };
        KhoiTaoTabKhachHang();
        pnlMainContent.Controls.Add(pnlKhachHang);
        
        pnlDichVu = new Panel { Dock = DockStyle.Fill };
        KhoiTaoTabDichVu();
        pnlMainContent.Controls.Add(pnlDichVu);
        
        pnlHoaDon = new Panel { Dock = DockStyle.Fill };
        KhoiTaoTabHoaDon();
        pnlMainContent.Controls.Add(pnlHoaDon);
        
        KhoiTaoTabBaoCao();
        if (pnlBaoCao != null)
        {
            pnlMainContent.Controls.Add(pnlBaoCao);
        }
    }

    // Khởi tạo giao diện tab Báo cáo
    protected void KhoiTaoTabBaoCao()
    {
        pnlBaoCao = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(243, 244, 246)
        };

        var tblRootBaoCao = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 1,
            Margin = new Padding(0)
        };
        tblRootBaoCao.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        tblRootBaoCao.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

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

        var flowFilterBC = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            Padding = new Padding(0),
            Margin = new Padding(0)
        };

        var lblLoai = new Label
        {
            Text = "Loại báo cáo:",
            AutoSize = true,
            Margin = new Padding(0, 8, 6, 6),
            ForeColor = Color.FromArgb(71, 85, 105),
            UseMnemonic = false
        };
        var cboLoai = new ComboBox
        {
            Width = 170,
            Height = 30,
            Margin = new Padding(0, 3, 12, 6),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cboLoai.Items.Add("Doanh thu theo tháng");
        cboLoai.SelectedIndex = 0;

        var lblTu = new Label
        {
            Text = "Từ ngày:",
            AutoSize = true,
            Margin = new Padding(0, 8, 6, 6),
            ForeColor = Color.FromArgb(71, 85, 105),
            UseMnemonic = false
        };
        dtpTuBaoCao = new DateTimePicker
        {
            Width = 138,
            Height = 30,
            Margin = new Padding(0, 3, 12, 6),
            Format = DateTimePickerFormat.Custom,
            CustomFormat = "dd/MM/yyyy",
            Value = new DateTime(DateTime.Today.Year, 1, 1)
        };
        dtpTuBaoCao.ValueChanged += (s, e) => TaiDuLieuBaoCao();

        var lblDen = new Label
        {
            Text = "Đến ngày:",
            AutoSize = true,
            Margin = new Padding(0, 8, 6, 6),
            ForeColor = Color.FromArgb(71, 85, 105),
            UseMnemonic = false
        };
        dtpDenBaoCao = new DateTimePicker
        {
            Width = 138,
            Height = 30,
            Margin = new Padding(0, 3, 12, 6),
            Format = DateTimePickerFormat.Custom,
            CustomFormat = "dd/MM/yyyy",
            Value = DateTime.Today
        };
        dtpDenBaoCao.ValueChanged += (s, e) => TaiDuLieuBaoCao();

        var btnXem = new Button
        {
            Text = "Xem",
            Width = 85,
            Height = 34,
            UseMnemonic = false,
            Margin = new Padding(0, 1, 8, 6)
        };
        UIHelper.DinhDangNutBamChinh(btnXem);
        btnXem.Click += (s, e) => TaiDuLieuBaoCao();

        var btnXuatExcel = new Button
        {
            Text = "Xuất Excel",
            Width = 118,
            Height = 34,
            UseMnemonic = false,
            Margin = new Padding(0, 1, 0, 6)
        };
        UIHelper.DinhDangNutBamPhu(btnXuatExcel);
        btnXuatExcel.Click += (s, e) => XuLyXuatExcel();

        flowFilterBC.Controls.AddRange(new Control[] { lblLoai, cboLoai, lblTu, dtpTuBaoCao, lblDen, dtpDenBaoCao, btnXem, btnXuatExcel });
        pnlTop.Controls.Add(flowFilterBC);
        tblRootBaoCao.Controls.Add(pnlTop, 0, 0);

        var tbl = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = new Padding(0)
        };
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 64f));
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 36f));

        var pnlLeftChart = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(18, 16, 18, 16),
            Margin = new Padding(0, 0, 8, 0),
            BorderStyle = BorderStyle.FixedSingle
        };
        var tblLeftChartLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 1,
            Margin = new Padding(0)
        };
        tblLeftChartLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35f));
        tblLeftChartLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

        var lblChartTitle = new Label
        {
            Text = "Doanh thu theo tháng",
            Font = new Font(AppConstants.FontChuChinh, 13f, FontStyle.Bold),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            UseMnemonic = false
        };
        bieuDoThang = new SimpleBarChartControl { Dock = DockStyle.Fill };
        tblLeftChartLayout.Controls.Add(lblChartTitle, 0, 0);
        tblLeftChartLayout.Controls.Add(bieuDoThang, 0, 1);
        pnlLeftChart.Controls.Add(tblLeftChartLayout);
        tbl.Controls.Add(pnlLeftChart, 0, 0);

        var pnlRightSummary = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(18, 16, 18, 16),
            Margin = new Padding(8, 0, 0, 0),
            BorderStyle = BorderStyle.FixedSingle
        };
        var lblSumTitle = new Label
        {
            Text = "Tổng hợp số liệu",
            Font = new Font(AppConstants.FontChuChinh, 13f, FontStyle.Bold),
            Dock = DockStyle.Top,
            Height = 35,
            UseMnemonic = false
        };

        cardBCTongDoanhThu = new StatCardControl
        {
            Title = "Tổng doanh thu",
            Value = "0 đ",
            TopAccentColor = Color.FromArgb(59, 130, 246),
            Dock = DockStyle.Top,
            Height = 85,
            Margin = new Padding(0, 0, 0, 10)
        };

        cardBCCongSuat = new StatCardControl
        {
            Title = "Công suất phòng TB",
            Value = "0%",
            TopAccentColor = Color.FromArgb(245, 158, 11),
            Dock = DockStyle.Top,
            Height = 85,
            Margin = new Padding(0, 0, 0, 10)
        };

        cardBCSoKhach = new StatCardControl
        {
            Title = "Số lượt khách lưu trú",
            Value = "0",
            TopAccentColor = Color.FromArgb(16, 185, 129),
            Dock = DockStyle.Top,
            Height = 85,
            Margin = new Padding(0, 0, 0, 10)
        };

        pnlRightSummary.Controls.Add(cardBCSoKhach);
        pnlRightSummary.Controls.Add(cardBCCongSuat);
        pnlRightSummary.Controls.Add(cardBCTongDoanhThu);
        pnlRightSummary.Controls.Add(lblSumTitle);
        lblSumTitle.SendToBack();
        tbl.Controls.Add(pnlRightSummary, 1, 0);

        tblRootBaoCao.Controls.Add(tbl, 0, 1);
        pnlBaoCao.Controls.Add(tblRootBaoCao);
    }

    // Tải dữ liệu biểu đồ và thống kê báo cáo theo khoảng thời gian
    public void TaiDuLieuBaoCao()
    {
        var tuNgay = dtpTuBaoCao != null ? dtpTuBaoCao.Value.Date : new DateTime(DateTime.Today.Year, 1, 1);
        var denNgay = dtpDenBaoCao != null ? dtpDenBaoCao.Value.Date : DateTime.Today;

        if (tuNgay > denNgay)
        {
            MessageBox.Show(this, "Từ ngày không được lớn hơn Đến ngày!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        UIHelper.RunSafe(() =>
        {
            var dsGoc = _baoCaoBLL.GetMonthlyRevenue(denNgay.Year);
            var thangBatDau = (tuNgay.Year == denNgay.Year) ? tuNgay.Month : 1;
            var thangKetThuc = denNgay.Month;

            _dsBaoCaoHienTai = dsGoc.Where(item =>
            {
                if (item.Period.StartsWith("T", StringComparison.OrdinalIgnoreCase)
                    && int.TryParse(item.Period.Substring(1), out var m))
                {
                    return m >= thangBatDau && m <= thangKetThuc;
                }
                return true;
            }).ToList();

            var duLieuVe = new List<KeyValuePair<string, double>>();
            double tongDoanhThu = 0;
            double tongCongSuat = 0;
            var tongKhach = 0;

            foreach (var item in _dsBaoCaoHienTai)
            {
                duLieuVe.Add(new KeyValuePair<string, double>(item.Period, item.Revenue));
                tongDoanhThu += item.Revenue;
                tongCongSuat += item.OccupancyRate;
                tongKhach += item.GuestCount;
            }

            bieuDoThang.UpdateData(duLieuVe);
            cardBCTongDoanhThu.Value = $"{tongDoanhThu:N0} đ";
            var congSuatTB = _dsBaoCaoHienTai.Count > 0 ? tongCongSuat / _dsBaoCaoHienTai.Count : 0;
            cardBCCongSuat.Value = $"{congSuatTB:N1}%";
            cardBCSoKhach.Value = $"{tongKhach:N0}";
        }, "tải báo cáo doanh thu", this);
    }

    // Xuất báo cáo doanh thu ra file Excel
    private void XuLyXuatExcel()
    {
        if (_dsBaoCaoHienTai.Count == 0)
        {
            TaiDuLieuBaoCao();
        }

        if (_dsBaoCaoHienTai.Count == 0)
        {
            MessageBox.Show(this, "Không có dữ liệu báo cáo trong khoảng thời gian đã chọn để xuất file!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        using var sfd = new SaveFileDialog
        {
            Filter = "File Excel CSV (*.csv)|*.csv",
            FileName = $"BaoCaoDoanhThu_{DateTime.Today:yyyyMMdd}.csv"
        };

        if (sfd.ShowDialog() == DialogResult.OK)
        {
            UIHelper.RunSafe(() =>
            {
                var thanhCong = _baoCaoBLL.ExportRevenueReportToExcel(sfd.FileName, _dsBaoCaoHienTai, out var msg);
                MessageBox.Show(this, msg, "Xuất file", MessageBoxButtons.OK,
                    thanhCong ? MessageBoxIcon.Information : MessageBoxIcon.Error);
            }, "xuất báo cáo doanh thu ra CSV", this);
        }
    }
}
