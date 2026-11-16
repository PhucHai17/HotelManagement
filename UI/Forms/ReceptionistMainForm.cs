using BLL;
using BLL.Interfaces;
using Common.Constants;
using Common.Helpers;
using UI.Controls;

namespace UI.Forms;

// Giao diện chính của Lễ tân 
[System.ComponentModel.DesignerCategory("Code")]
public partial class ReceptionistMainForm : Form
{
    private readonly IRoomService _phongBLL = AppServices.Room;
    private readonly ICustomerService _khachHangBLL = AppServices.Customer;
    private readonly IHotelService _dichVuBLL = AppServices.Hotel;
    private readonly IInvoiceService _hoaDonBLL = AppServices.Invoice;
    private readonly IReportService _baoCaoBLL = AppServices.Report;
    // Thanh điều hướng
    protected Panel pnlSidebar = null!;
    protected Panel pnlMainContent = null!;
    protected FlowLayoutPanel pnlMenuFlow = null!;
    protected Label lblTenNhanVien = null!;
    protected readonly List<Button> _danhSachNutMenu = new List<Button>();
    private readonly ToolTip _toolTip = new ToolTip();

    // Các tab chức năng chính
    private Panel pnlTongQuan = null!;
    private Panel pnlSoDoPhong = null!;
    private Panel pnlDatPhong = null!;
    private Panel pnlCheckInOut = null!;
    private Panel pnlKhachHang = null!;
    private Panel pnlDichVu = null!;
    private Panel pnlHoaDon = null!;
    private Panel? pnlBaoCao;

    public ReceptionistMainForm()
    {
        KhoiTaoKhungChinh();
        KhoiTaoCacTabNoiDung();
        ChuyenTab(pnlTongQuan, "Tổng quan");
        UIHelper.ApDungFontTimesNewRoman(this);
        Load += (s, e) => TaiDuLieuTongQuan();
    }
    protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _toolTip.Dispose();
            }
            base.Dispose(disposing);
        }

        // Cho phép lớp con (ManagerMainForm) tùy biến tiêu đề form
        protected virtual string LayTieuDeForm() => "Giao diện Lễ tân";

        // Cho phép lớp con tùy biến nhãn vai trò ở góc dưới sidebar
        protected virtual string LayNhanVaiTroSidebar() => "Lễ tân";
        
        protected virtual bool XuLyChuyenMenuMoRong(string tenChucNang) => false;

    
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

            // 1. Menu dọc bên trái (rộng 240px)
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

            // Hàng 0: Logo HOTELPRO hai màu trên 1 dòng, căn thẳng lề với menu
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

            // Hàng 1: Danh sách menu theo quyền
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

            var cacChucNang = new List<string>
            {
                "Tổng quan", "Sơ đồ phòng", "Đặt phòng", "Check-in / Check-out", "Khách hàng", "Dịch vụ", "Hóa đơn"
            };
            // Chỉ hiện menu Báo cáo khi tài khoản có quyền XemBaoCao
            if (SessionManager.HasPermission(AppConstants.QuyenXemBaoCao))
            {
                cacChucNang.Add("Báo cáo");
            }

            foreach (var tenCN in cacChucNang)
            {
                TaoNutSidebar(tenCN);
            }
            tblSidebar.Controls.Add(pnlMenuFlow, 0, 1);

            // Hàng 2: Vai trò (trên), họ tên đầy đủ tự xuống dòng (giữa) và nút Đăng xuất cao >= 36px (dưới)
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

            // 2. Vùng nội dung chính bên phải
            pnlMainContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(243, 244, 246),
                Padding = new Padding(16)
            };

            Controls.Add(pnlMainContent);
            Controls.Add(pnlSidebar);
        }

        private void XuLyChuyenMenu(string tenChucNang)
        {
            switch (tenChucNang)
            {
                case "Tổng quan":
                    ChuyenTab(pnlTongQuan, tenChucNang);
                    TaiDuLieuTongQuan();
                    break;
                default:
                    XuLyChuyenMenuMoRong(tenChucNang);
                    break;
            }
        }

        protected void ChuyenTab(Panel tabCanHien, string tenChucNang)
        {
            foreach (Control c in pnlMainContent.Controls)
            {
                c.Visible = false;
            }
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

        private void KhoiTaoCacTabNoiDung()
        {
            pnlTongQuan = new Panel { Dock = DockStyle.Fill };
            KhoiTaoTabTongQuan();
            pnlMainContent.Controls.Add(pnlTongQuan);
            
        }
}

