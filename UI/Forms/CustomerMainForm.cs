using Common.Constants;
using Common.Helpers;
using UI.Controls;

namespace UI.Forms;

public class CustomerMainForm : Form
{
    private Panel _pnlTimPhong = null!;
    private Panel _pnlDatPhong = null!;
    private Panel _pnlDichVu = null!;
    private Panel _pnlTaiKhoan = null!;
    private CustomerSearchRoomTab _searchRoomTab = null!;
    private CustomerMyBookingsTab _myBookingsTab = null!;
    private readonly List<Button> _danhSachNutNav = new();
    private readonly Dictionary<string, Panel> _tabPanels = new();

    public CustomerMainForm()
    {
        KhoiTaoGiaoDien();
        XuLyChuyenTab("Tìm phòng");
    }

    // Khởi tạo khung giao diện chính cho khách hàng
    private void KhoiTaoGiaoDien()
    {
        Text = "HotelPro - Đặt phòng trực tuyến";
        Size = new Size(1180, 720);
        MinimumSize = new Size(1100, 700);
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Times New Roman", 10f, FontStyle.Regular);
        BackColor = AppConstants.MauNenChung;

        var pnlTopBar = new Panel
        {
            Dock = DockStyle.Top,
            Height = 64,
            BackColor = AppConstants.MauXanhNavy
        };

        var lblLogo = new HotelProLogo
        {
            Font = new Font("Times New Roman", 16f, FontStyle.Bold),
            AutoSize = false,
            Dock = DockStyle.Left,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(18, 0, 14, 0),
            Width = 180
        };
        var kichThuocLogo = lblLogo.GetPreferredSize(Size.Empty);
        if (kichThuocLogo.Width > 0)
        {
            lblLogo.Width = kichThuocLogo.Width;
        }

        var pnlRightUser = new FlowLayoutPanel
        {
            Dock = DockStyle.Right,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            BackColor = AppConstants.MauXanhNavy,
            Padding = new Padding(8, 14, 16, 10)
        };

        var tenHienThi = string.IsNullOrWhiteSpace(SessionManager.DisplayName)
            ? "Khách hàng"
            : SessionManager.DisplayName.Trim();
        var tenRutGon = tenHienThi.Length > 30
            ? tenHienThi[..27].TrimEnd() + "..."
            : tenHienThi;

        var lblXinChao = new Label
        {
            Text = $"Xin chào, {tenRutGon}",
            ForeColor = Color.White,
            Font = new Font("Times New Roman", 10f, FontStyle.Regular),
            AutoSize = true,
            TextAlign = ContentAlignment.MiddleRight,
            UseMnemonic = false,
            Margin = new Padding(0, 10, 12, 0)
        };

        var toolTip = new ToolTip();
        toolTip.SetToolTip(lblXinChao, $"Xin chào, {tenHienThi}");

        var fontNutDangXuat = new Font("Times New Roman", 10f, FontStyle.Bold);
        var rongNutDangXuat = Math.Max(108, TextRenderer.MeasureText("Đăng xuất", fontNutDangXuat).Width + 28);
        var btnDangXuat = new Button
        {
            Text = "Đăng xuất",
            AutoSize = false,
            Size = new Size(rongNutDangXuat, 36),
            Margin = new Padding(0),
            UseMnemonic = false,
            Font = fontNutDangXuat,
            FlatStyle = FlatStyle.Flat,
            BackColor = AppConstants.MauXanhNavyDam,
            ForeColor = Color.White,
            Cursor = Cursors.Hand
        };
        btnDangXuat.FlatAppearance.BorderSize = 1;
        btnDangXuat.FlatAppearance.BorderColor = Color.FromArgb(148, 163, 184);
        btnDangXuat.Click += (s, e) =>
        {
            SessionManager.Logout();
            Close();
        };

        pnlRightUser.Controls.Add(lblXinChao);
        pnlRightUser.Controls.Add(btnDangXuat);

        var flowNav = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Padding = new Padding(4, 12, 8, 0)
        };

        string[] cacTab = { "Tìm phòng", "Đặt phòng", "Dịch vụ", "Tài khoản" };
        foreach (var tab in cacTab)
        {
            var btn = new Button
            {
                Text = tab,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Height = 40,
                MinimumSize = new Size(0, 40),
                Padding = new Padding(10, 0, 10, 0),
                Margin = new Padding(0, 0, 4, 0),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = AppConstants.MauXanhNavy,
                Font = new Font("Times New Roman", 10.5f, FontStyle.Bold),
                UseMnemonic = false,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += (s, e) => XuLyChuyenTab(tab);
            _danhSachNutNav.Add(btn);
            flowNav.Controls.Add(btn);
        }

        pnlTopBar.Controls.Add(flowNav);
        pnlTopBar.Controls.Add(pnlRightUser);
        pnlTopBar.Controls.Add(lblLogo);
        flowNav.BringToFront();

        var pnlMainContent = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppConstants.MauNenChung,
            Padding = new Padding(20)
        };

        _pnlTimPhong = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
        _searchRoomTab = new CustomerSearchRoomTab { Dock = DockStyle.Fill };
        _searchRoomTab.OnBookingCreated += () => XuLyChuyenTab("Đặt phòng");
        _pnlTimPhong.Controls.Add(_searchRoomTab);

        _pnlDatPhong = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
        _myBookingsTab = new CustomerMyBookingsTab { Dock = DockStyle.Fill };
        _pnlDatPhong.Controls.Add(_myBookingsTab);

        _pnlDichVu = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
        _pnlTaiKhoan = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };

        _tabPanels["Tìm phòng"] = _pnlTimPhong;
        _tabPanels["Đặt phòng"] = _pnlDatPhong;
        _tabPanels["Dịch vụ"] = _pnlDichVu;
        _tabPanels["Tài khoản"] = _pnlTaiKhoan;

        pnlMainContent.Controls.Add(_pnlTimPhong);
        pnlMainContent.Controls.Add(_pnlDatPhong);
        pnlMainContent.Controls.Add(_pnlDichVu);
        pnlMainContent.Controls.Add(_pnlTaiKhoan);

        Controls.Add(pnlMainContent);
        Controls.Add(pnlTopBar);
        pnlTopBar.SendToBack();
        pnlMainContent.BringToFront();
    }

    // Chuyển đổi tab hiển thị trên giao diện
    private void XuLyChuyenTab(string tenTab)
    {
        foreach (var (tab, panel) in _tabPanels)
        {
            var hienThi = (tab == tenTab);
            panel.Visible = hienThi;
            if (hienThi)
            {
                panel.BringToFront();
            }
        }

        foreach (var btn in _danhSachNutNav)
        {
            btn.ForeColor = (btn.Text == tenTab) ? AppConstants.MauVangCam : Color.White;
        }

        if (tenTab == "Đặt phòng")
        {
            _myBookingsTab.TaiDuLieuDatPhong();
        }
        else if (tenTab == "Tìm phòng")
        {
            _searchRoomTab.TaiDanhSachPhongTrong();
        }
    }
}
