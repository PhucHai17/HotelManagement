using Models;

namespace UI;

public partial class FormLeTan : Form
{
    private readonly TaiKhoan _taiKhoan;
    private readonly Panel pnlMenu = new Panel { Dock = DockStyle.Left, Width = 210, BackColor = Color.FromArgb(32, 51, 92) };
    private readonly Panel pnlNoiDung = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(243, 245, 249) };
    private readonly List<Button> _dsNut = new List<Button>();

    // true nếu người dùng bấm Đăng xuất (để Program mở lại form đăng nhập)
    public bool DangXuat { get; private set; }

    public FormLeTan(TaiKhoan taiKhoan)
    {
        InitializeComponent();
        _taiKhoan = taiKhoan;

        Text = "HotelPro - Giao diện Lễ tân";
        Size = new Size(1200, 720);
        StartPosition = FormStartPosition.CenterScreen;

        Controls.Add(pnlNoiDung);
        Controls.Add(pnlMenu);

        TaoMenu();
        MoTrang(new UcSoDoPhong(), _dsNut[1]);   // mở sẵn trang Sơ đồ phòng
    }

    private void TaoMenu()
    {
        // Phần dưới của menu: tên lễ tân + nút đăng xuất
        var pnlDuoi = new Panel { Dock = DockStyle.Bottom, Height = 90 };
        var btnDangXuat = new Button
        {
            Text = "Đăng xuất", Dock = DockStyle.Fill, FlatStyle = FlatStyle.Flat,
            ForeColor = Color.White
        };
        btnDangXuat.FlatAppearance.BorderSize = 0;
        btnDangXuat.Click += (s, e) => { DangXuat = true; Close(); };
        var lblLeTan = new Label
        {
            Text = "Lễ tân: " + _taiKhoan.TenDangNhap, Dock = DockStyle.Top,
            Height = 44, ForeColor = Color.White, TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(16, 0, 0, 0)
        };
        pnlDuoi.Controls.Add(btnDangXuat);
        pnlDuoi.Controls.Add(lblLeTan);
        pnlMenu.Controls.Add(pnlDuoi);

        // Logo
        pnlMenu.Controls.Add(new Label
        {
            Text = "HOTELPRO", Font = new Font("Times New Roman", 16, FontStyle.Bold),
            ForeColor = Color.White, Location = new Point(16, 20), AutoSize = true
        });

        // Các mục menu: tên + cách tạo trang tương ứng
        var cacTrang = new (string Ten, Func<UserControl> Tao)[]
        {
            ("Tổng quan",           () => new UcTamThoi("Tổng quan")),
            ("Sơ đồ phòng",         () => new UcSoDoPhong()),
            ("Đặt phòng",           () => new UcTamThoi("Đặt phòng")),
            ("Check-in / Check-out", () => new UcTamThoi("Check-in / Check-out")),
            ("Khách hàng",          () => new UcTamThoi("Khách hàng")),
            ("Dịch vụ",             () => new UcTamThoi("Dịch vụ")),
            ("Hóa đơn",             () => new UcTamThoi("Hóa đơn")),
            ("Báo cáo",             () => new UcTamThoi("Báo cáo"))
        };

        int y = 80;
        foreach (var trang in cacTrang)
        {
            var nut = new Button
            {
                Text = trang.Ten, Width = 210, Height = 48, Location = new Point(0, y),
                FlatStyle = FlatStyle.Flat, ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(16, 0, 0, 0),
                BackColor = pnlMenu.BackColor
            };
            nut.FlatAppearance.BorderSize = 0;
            nut.Click += (s, e) => MoTrang(trang.Tao(), nut);
            pnlMenu.Controls.Add(nut);
            _dsNut.Add(nut);
            y += 52;
        }
    }

    // Đổi trang bên phải và tô vàng nút đang chọn
    private void MoTrang(UserControl trang, Button nutChon)
    {
        pnlNoiDung.Controls.Clear();
        trang.Dock = DockStyle.Fill;
        pnlNoiDung.Controls.Add(trang);

        foreach (var nut in _dsNut)
        {
            nut.BackColor = pnlMenu.BackColor;
            nut.ForeColor = Color.White;
        }
        nutChon.BackColor = Color.FromArgb(201, 162, 77);
        nutChon.ForeColor = Color.FromArgb(25, 35, 60);
    }
}