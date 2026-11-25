using BLL;
using Common.Constants;
using Common.Helpers;
using Entity.Entities;

namespace UI.Controls;

public class CustomerTab : UserControl
{
    private TextBox txtTimKhachHang = null!;
    private DataGridView dgvKhachHang = null!;
    private TextBox txtHoSoTen = null!;
    private TextBox txtHoSoSDT = null!;
    private TextBox txtHoSoCCCD = null!;
    private TextBox txtHoSoGhiChu = null!;
    private Customer? _khachHangDangChon;

    public CustomerTab()
    {
        KhoiTaoGiaoDien();
        TaiDuLieuKhachHang();
    }

    private void KhoiTaoGiaoDien()
    {
        var tblRootKhachHang = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 1,
            Margin = new Padding(0)
        };
        tblRootKhachHang.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        tblRootKhachHang.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

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

        var flowFilterKH = new FlowLayoutPanel
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
            Text = "Tìm khách hàng:",
            AutoSize = true,
            Margin = new Padding(0, 8, 8, 6),
            ForeColor = Color.FromArgb(71, 85, 105)
        };
        txtTimKhachHang = new TextBox
        {
            Width = 280,
            Height = 30,
            Margin = new Padding(0, 3, 20, 6),
            PlaceholderText = "Tên, SĐT hoặc số CCCD..."
        };
        txtTimKhachHang.TextChanged += (s, e) => TaiDuLieuKhachHang();

        var btnThemKhach = new Button
        {
            Text = "+ Thêm khách",
            Width = 150,
            Height = 34,
            UseMnemonic = false,
            Margin = new Padding(0, 1, 0, 6)
        };
        UIHelper.DinhDangNutBamChinh(btnThemKhach);
        btnThemKhach.Click += (s, e) =>
        {
            _khachHangDangChon = new Customer();
            txtHoSoTen.Text = string.Empty;
            txtHoSoSDT.Text = string.Empty;
            txtHoSoCCCD.Text = string.Empty;
            txtHoSoGhiChu.Text = string.Empty;
            txtHoSoTen.Focus();
        };

        flowFilterKH.Controls.AddRange(new Control[] { lblTim, txtTimKhachHang, btnThemKhach });
        pnlTop.Controls.Add(flowFilterKH);
        tblRootKhachHang.Controls.Add(pnlTop, 0, 0);

        var tbl = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = new Padding(0)
        };
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55f));
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45f));

        dgvKhachHang = new DataGridView { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 8, 0) };
        UIHelper.DinhDangBangDuLieu(dgvKhachHang);
        dgvKhachHang.Columns.Add("MaKH", "Mã KH");
        dgvKhachHang.Columns.Add("HoTen", "Họ tên");
        dgvKhachHang.Columns.Add("SDT", "SĐT");
        dgvKhachHang.Columns.Add("SoLanO", "Số lần ở");
        dgvKhachHang.Columns["MaKH"].MinimumWidth = 95;
        dgvKhachHang.Columns["MaKH"].FillWeight = 65;
        dgvKhachHang.Columns["HoTen"].MinimumWidth = 140;
        dgvKhachHang.Columns["HoTen"].FillWeight = 120;
        dgvKhachHang.Columns["SDT"].MinimumWidth = 110;
        dgvKhachHang.Columns["SDT"].FillWeight = 90;
        dgvKhachHang.Columns["SoLanO"].MinimumWidth = 90;
        dgvKhachHang.Columns["SoLanO"].FillWeight = 60;
        dgvKhachHang.SelectionChanged += DgvKhachHang_SelectionChanged;
        tbl.Controls.Add(dgvKhachHang, 0, 0);

        var pnlHoSo = new Panel
        {
            Dock = DockStyle.Fill,
            Width = 340,
            AutoScroll = true,
            BackColor = Color.White,
            Padding = new Padding(20, 16, 20, 16),
            Margin = new Padding(8, 0, 0, 0),
            BorderStyle = BorderStyle.FixedSingle
        };
        var lblTieuDeHoSo = new Label
        {
            Text = "Hồ sơ khách",
            Font = new Font(AppConstants.FontChuChinh, 13f, FontStyle.Bold),
            Dock = DockStyle.Top,
            Height = 35
        };
        pnlHoSo.Controls.Add(lblTieuDeHoSo);
        lblTieuDeHoSo.SendToBack();

        var y = 65;
        ThemNhanVaInput(pnlHoSo, "Họ tên", y, out txtHoSoTen);
        ThemNhanVaInput(pnlHoSo, "SĐT", y + 66, out txtHoSoSDT);
        ThemNhanVaInput(pnlHoSo, "CCCD / Hộ chiếu", y + 132, out txtHoSoCCCD);
        ThemNhanVaInput(pnlHoSo, "Ghi chú", y + 198, out txtHoSoGhiChu);
        txtHoSoGhiChu.ReadOnly = true;
        txtHoSoGhiChu.PlaceholderText = "VIP, yêu cầu đặc biệt";

        var btnLuuHoSo = new Button
        {
            Text = "Lưu",
            Location = new Point(16, y + 266),
            Width = 96,
            Height = 36,
            UseMnemonic = false
        };
        UIHelper.DinhDangNutBamChinh(btnLuuHoSo);
        btnLuuHoSo.Click += (s, e) => XuLyLuuHoSoKhach();

        pnlHoSo.Controls.Add(btnLuuHoSo);
        tbl.Controls.Add(pnlHoSo, 1, 0);

        tblRootKhachHang.Controls.Add(tbl, 0, 1);
        Controls.Add(tblRootKhachHang);
    }

    private static void ThemNhanVaInput(Panel p, string nhan, int y, out TextBox txt)
    {
        var lbl = new Label
        {
            Text = nhan,
            Location = new Point(16, y),
            AutoSize = true,
            ForeColor = Color.FromArgb(100, 116, 139),
            UseMnemonic = false
        };
        var caoNhan = Math.Max(lbl.PreferredHeight, TextRenderer.MeasureText(nhan, lbl.Font).Height);
        var chieuRongO = Math.Max(220, p.Width - 36);
        txt = new TextBox
        {
            Location = new Point(16, y + caoNhan + 4),
            Width = chieuRongO,
            Height = 30,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };
        p.Controls.Add(lbl);
        p.Controls.Add(txt);
    }

    private void TaiDuLieuKhachHang()
    {
        UIHelper.RunSafe(() =>
        {
            dgvKhachHang.Rows.Clear();
            var tuKhoa = txtTimKhachHang.Text.Trim();
            var ds = AppServices.Customer.Search(tuKhoa);

            foreach (var kh in ds)
            {
                var idx = dgvKhachHang.Rows.Add($"KH{kh.CustomerId:D3}", kh.FullName, kh.PhoneNumber, kh.StayCount);
                dgvKhachHang.Rows[idx].Tag = kh;
            }

            if (dgvKhachHang.Rows.Count > 0)
                dgvKhachHang.Rows[0].Selected = true;
        }, "tải danh sách khách hàng", this);
    }

    private void DgvKhachHang_SelectionChanged(object? sender, EventArgs e)
    {
        if (dgvKhachHang.CurrentRow?.Tag is Customer kh)
        {
            _khachHangDangChon = kh;
            txtHoSoTen.Text = kh.FullName;
            txtHoSoSDT.Text = kh.PhoneNumber;
            txtHoSoCCCD.Text = kh.NationalId;
            txtHoSoGhiChu.Text = $"Hạng {kh.TierName} - Điểm tích lũy: {kh.LoyaltyPoints}";
        }
    }

    // Lưu hồ sơ khách hàng
    private void XuLyLuuHoSoKhach()
    {
        _khachHangDangChon ??= new Customer();

        UIHelper.RunSafe(() =>
        {
            _khachHangDangChon.FullName = txtHoSoTen.Text.Trim();
            _khachHangDangChon.PhoneNumber = txtHoSoSDT.Text.Trim();
            _khachHangDangChon.NationalId = txtHoSoCCCD.Text.Trim();

            var thanhCong = AppServices.Customer.SaveCustomer(_khachHangDangChon, out var msg);
            MessageBox.Show(this, msg, "Thông báo", MessageBoxButtons.OK,
                thanhCong ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            if (thanhCong)
                TaiDuLieuKhachHang();
        }, "lưu hồ sơ khách hàng", this);
    }
}
