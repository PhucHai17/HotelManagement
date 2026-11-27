using BLL;
using Common.Constants;
using Common.Helpers;
using DAL.Repositories;
using Entity.Entities;
using Entity.Enums;
using UI.Helpers;

namespace UI.Controls;

public class CustomerAccountTab : UserControl
{
    public event Action<string>? OnProfileUpdated;

    private TextBox txtMaKH = null!;
    private TextBox txtTenDangNhap = null!;
    private TextBox txtHoTen = null!;
    private TextBox txtSoDienThoai = null!;
    private TextBox txtEmail = null!;
    private TextBox txtCCCD = null!;
    private Button btnLuuThayDoi = null!;
    private Button btnKhoiPhuc = null!;

    private Label lblTierBadge = null!;
    private Label lblDiemTichLuy = null!;
    private Label lblLuotLuuTru = null!;
    private Label lblTongChiTieu = null!;
    private Label lblThongKeDon = null!;

    private TextBox txtMatKhauCu = null!;
    private TextBox txtMatKhauMoi = null!;
    private TextBox txtXacNhanMatKhau = null!;
    private CheckBox chkHienThiMatKhau = null!;
    private Button btnDoiMatKhau = null!;

    public CustomerAccountTab()
    {
        Font = new Font("Times New Roman", 10f, FontStyle.Regular);
        KhoiTaoGiaoDien();
        TaiDuLieuTaiKhoan();
    }

    // Khởi tạo bố cục giao diện tab tài khoản khách hàng
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
            Text = "TÀI KHOẢN & HỒ SƠ KHÁCH HÀNG",
            Font = new Font("Times New Roman", 16f, FontStyle.Bold),
            ForeColor = AppConstants.MauXanhNavy,
            Location = new Point(14, 6),
            AutoSize = true,
            UseMnemonic = false
        };

        var lblSubtitle = new Label
        {
            Text = "Quản lý thông tin cá nhân, theo dõi quyền lợi hạng thành viên và bảo mật tài khoản",
            Font = new Font("Times New Roman", 9.5f, FontStyle.Regular),
            ForeColor = AppConstants.MauChuPhu,
            Location = new Point(16, 32),
            AutoSize = true,
            UseMnemonic = false
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
        btnLamMoi.Click += (s, e) => TaiDuLieuTaiKhoan();

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
        tblBody.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 48f));
        tblBody.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 52f));

        var pnlLeftCard = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(18, 12, 18, 12),
            Margin = new Padding(0, 0, 10, 0),
            AutoScroll = true
        };

        var lblLeftTitle = new Label
        {
            Text = "THÔNG TIN CÁ NHÂN & LIÊN HỆ",
            Font = new Font("Times New Roman", 12f, FontStyle.Bold),
            ForeColor = AppConstants.MauXanhNavy,
            Dock = DockStyle.Top,
            Height = 24,
            UseMnemonic = false
        };

        var lblLeftSub = new Label
        {
            Text = "Cập nhật thông tin để khách sạn liên hệ và phục vụ chu đáo nhất",
            Font = new Font("Times New Roman", 9.5f, FontStyle.Regular),
            ForeColor = AppConstants.MauChuPhu,
            Dock = DockStyle.Top,
            Height = 20,
            UseMnemonic = false
        };

        var pnlDividerLeft = new Panel
        {
            Dock = DockStyle.Top,
            Height = 1,
            BackColor = AppConstants.MauVien,
            Margin = new Padding(0, 4, 0, 8)
        };

        var pnlFormContent = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(0, 4, 0, 0),
            AutoScroll = true
        };

        var y = 4;
        ThemDongTruong(pnlFormContent, "Mã khách hàng:", y, out txtMaKH, isReadOnly: true);
        y += 50;
        ThemDongTruong(pnlFormContent, "Tên đăng nhập (Tài khoản):", y, out txtTenDangNhap, isReadOnly: true);
        y += 50;
        ThemDongTruong(pnlFormContent, "Họ và tên (*):", y, out txtHoTen);
        y += 50;
        ThemDongTruong(pnlFormContent, "Số điện thoại (*):", y, out txtSoDienThoai);
        y += 50;
        ThemDongTruong(pnlFormContent, "Địa chỉ Email:", y, out txtEmail);
        y += 50;
        ThemDongTruong(pnlFormContent, "Số CCCD / Hộ chiếu:", y, out txtCCCD);
        y += 58;

        btnLuuThayDoi = new Button
        {
            Text = "Lưu thay đổi",
            Font = new Font("Times New Roman", 10f, FontStyle.Bold),
            Location = new Point(0, y),
            Size = new Size(130, 36),
            UseMnemonic = false,
            Cursor = Cursors.Hand
        };
        UIHelper.DinhDangNutBamNavy(btnLuuThayDoi);
        btnLuuThayDoi.Click += (s, e) => XuLyLuuThongTin();

        btnKhoiPhuc = new Button
        {
            Text = "Khôi phục",
            Font = new Font("Times New Roman", 10f, FontStyle.Regular),
            Location = new Point(140, y),
            Size = new Size(100, 36),
            UseMnemonic = false,
            Cursor = Cursors.Hand
        };
        UIHelper.DinhDangNutBamPhu(btnKhoiPhuc);
        btnKhoiPhuc.Click += (s, e) => TaiDuLieuTaiKhoan();

        EventHandler onProfileFieldChanged = (s, e) =>
        {
            if (SessionManager.CurrentCustomer != null && SessionManager.CurrentCustomer.CustomerId > 0)
            {
                btnKhoiPhuc.Enabled = true;
                UIHelper.DinhDangNutBamPhu(btnKhoiPhuc);
            }
        };
        txtHoTen.TextChanged += onProfileFieldChanged;
        txtSoDienThoai.TextChanged += onProfileFieldChanged;
        txtEmail.TextChanged += onProfileFieldChanged;
        txtCCCD.TextChanged += onProfileFieldChanged;

        pnlFormContent.Controls.Add(btnLuuThayDoi);
        pnlFormContent.Controls.Add(btnKhoiPhuc);

        pnlLeftCard.Controls.Add(pnlFormContent);
        pnlLeftCard.Controls.Add(pnlDividerLeft);
        pnlLeftCard.Controls.Add(lblLeftSub);
        pnlLeftCard.Controls.Add(lblLeftTitle);
        tblBody.Controls.Add(pnlLeftCard, 0, 0);

        var tblRight = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 1,
            Margin = new Padding(0)
        };
        tblRight.RowStyles.Add(new RowStyle(SizeType.Percent, 46f));
        tblRight.RowStyles.Add(new RowStyle(SizeType.Percent, 54f));

        var pnlMembershipCard = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(16, 10, 16, 8),
            Margin = new Padding(0, 0, 0, 8)
        };

        var lblMemTitle = new Label
        {
            Text = "HẠNG THÀNH VIÊN & TÍCH LŨY",
            Font = new Font("Times New Roman", 12f, FontStyle.Bold),
            ForeColor = AppConstants.MauXanhNavy,
            Dock = DockStyle.Top,
            Height = 22,
            UseMnemonic = false
        };

        var lblMemSub = new Label
        {
            Text = "Quyền lợi tích lũy và lịch sử đặt phòng của quý khách tại HotelPro",
            Font = new Font("Times New Roman", 9f, FontStyle.Regular),
            ForeColor = AppConstants.MauChuPhu,
            Dock = DockStyle.Top,
            Height = 18,
            UseMnemonic = false
        };

        var pnlDividerMem = new Panel
        {
            Dock = DockStyle.Top,
            Height = 1,
            BackColor = AppConstants.MauVien,
            Margin = new Padding(0, 3, 0, 6)
        };

        var pnlMemBody = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(0, 4, 0, 0)
        };

        lblTierBadge = new Label
        {
            Text = "HẠNG THÀNH VIÊN: THƯỜNG",
            Font = new Font("Times New Roman", 10.5f, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = AppConstants.MauXanhNavy,
            Dock = DockStyle.Top,
            Height = 28,
            TextAlign = ContentAlignment.MiddleCenter,
            UseMnemonic = false
        };

        var tblStats = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 116,
            RowCount = 2,
            ColumnCount = 2,
            Margin = new Padding(0, 6, 0, 4),
            Padding = new Padding(0)
        };
        tblStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
        tblStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
        tblStats.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
        tblStats.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));

        var pnlO1 = TaoOThongKe("Điểm tích lũy:", "0 điểm", AppConstants.MauVangCamDam, out lblDiemTichLuy);
        var pnlO2 = TaoOThongKe("Số lượt lưu trú:", "0 lượt", AppConstants.MauXanhNavy, out lblLuotLuuTru);
        var pnlO3 = TaoOThongKe("Tổng chi tiêu tích lũy:", "0 đ", AppConstants.MauVangCamDam, out lblTongChiTieu);
        var pnlO4 = TaoOThongKe("Đơn đặt phòng:", "0 đơn", AppConstants.MauChuChinh, out lblThongKeDon);

        tblStats.Controls.Add(pnlO1, 0, 0);
        tblStats.Controls.Add(pnlO2, 1, 0);
        tblStats.Controls.Add(pnlO3, 0, 1);
        tblStats.Controls.Add(pnlO4, 1, 1);

        var lblQuyenLoi = new Label
        {
            Text = "• Tích 1 điểm thưởng cho mỗi 100.000 đ chi tiêu tiền phòng và dịch vụ tại khách sạn.\n• Điểm thưởng được tự động xét nâng hạng VIP và hưởng ưu đãi giá phòng.",
            Font = new Font("Times New Roman", 8.5f, FontStyle.Regular),
            ForeColor = AppConstants.MauChuPhu,
            Dock = DockStyle.Top,
            Height = 30,
            UseMnemonic = false
        };

        pnlMemBody.Controls.Add(lblQuyenLoi);
        pnlMemBody.Controls.Add(tblStats);
        pnlMemBody.Controls.Add(lblTierBadge);

        pnlMembershipCard.Controls.Add(pnlMemBody);
        pnlMembershipCard.Controls.Add(pnlDividerMem);
        pnlMembershipCard.Controls.Add(lblMemSub);
        pnlMembershipCard.Controls.Add(lblMemTitle);
        tblRight.Controls.Add(pnlMembershipCard, 0, 0);

        var pnlSecurityCard = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(16, 10, 16, 10),
            Margin = new Padding(0)
        };

        var lblSecTitle = new Label
        {
            Text = "ĐỔI MẬT KHẨU TÀI KHOẢN",
            Font = new Font("Times New Roman", 12f, FontStyle.Bold),
            ForeColor = AppConstants.MauXanhNavy,
            Dock = DockStyle.Top,
            Height = 22,
            UseMnemonic = false
        };

        var lblSecSub = new Label
        {
            Text = "Mật khẩu mới phải từ 6 ký tự trở lên để đảm bảo an toàn",
            Font = new Font("Times New Roman", 9f, FontStyle.Regular),
            ForeColor = AppConstants.MauChuPhu,
            Dock = DockStyle.Top,
            Height = 18,
            UseMnemonic = false
        };

        var pnlDividerSec = new Panel
        {
            Dock = DockStyle.Top,
            Height = 1,
            BackColor = AppConstants.MauVien,
            Margin = new Padding(0, 3, 0, 6)
        };

        var pnlSecBody = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(0, 2, 0, 0)
        };

        var ySec = 2;
        ThemDongTruong(pnlSecBody, "Mật khẩu hiện tại (*):", ySec, out txtMatKhauCu, isPassword: true);
        ySec += 48;
        ThemDongTruong(pnlSecBody, "Mật khẩu mới (*):", ySec, out txtMatKhauMoi, isPassword: true);
        ySec += 48;
        ThemDongTruong(pnlSecBody, "Xác nhận mật khẩu mới (*):", ySec, out txtXacNhanMatKhau, isPassword: true);
        ySec += 48;

        chkHienThiMatKhau = new CheckBox
        {
            Text = "Hiển thị mật khẩu",
            Font = new Font("Times New Roman", 9f, FontStyle.Regular),
            ForeColor = AppConstants.MauChuPhu,
            Location = new Point(0, ySec),
            AutoSize = true,
            Cursor = Cursors.Hand,
            UseMnemonic = false
        };
        chkHienThiMatKhau.CheckedChanged += (s, e) =>
        {
            var an = !chkHienThiMatKhau.Checked;
            txtMatKhauCu.UseSystemPasswordChar = an;
            txtMatKhauMoi.UseSystemPasswordChar = an;
            txtXacNhanMatKhau.UseSystemPasswordChar = an;
        };

        ySec += 24;
        btnDoiMatKhau = new Button
        {
            Text = "Cập nhật mật khẩu",
            Font = new Font("Times New Roman", 10f, FontStyle.Bold),
            Location = new Point(0, ySec),
            Size = new Size(160, 34),
            UseMnemonic = false,
            Cursor = Cursors.Hand
        };
        UIHelper.DinhDangNutBamNavy(btnDoiMatKhau);
        btnDoiMatKhau.Click += (s, e) => XuLyDoiMatKhau();

        pnlSecBody.Controls.Add(chkHienThiMatKhau);
        pnlSecBody.Controls.Add(btnDoiMatKhau);

        pnlSecurityCard.Controls.Add(pnlSecBody);
        pnlSecurityCard.Controls.Add(pnlDividerSec);
        pnlSecurityCard.Controls.Add(lblSecSub);
        pnlSecurityCard.Controls.Add(lblSecTitle);
        tblRight.Controls.Add(pnlSecurityCard, 0, 1);

        tblBody.Controls.Add(tblRight, 1, 0);
        tblRoot.Controls.Add(tblBody, 0, 1);
        Controls.Add(tblRoot);
    }
    
    private static void ThemDongTruong(Panel parent, string title, int y, out TextBox textBox, bool isReadOnly = false, bool isPassword = false)
    {
        var lbl = new Label
        {
            Text = title,
            Font = new Font("Times New Roman", 9.5f, FontStyle.Bold),
            ForeColor = AppConstants.MauChuChinh,
            Location = new Point(0, y),
            Size = new Size(340, 16),
            UseMnemonic = false
        };

        textBox = new TextBox
        {
            Font = new Font("Times New Roman", 10f, FontStyle.Regular),
            Location = new Point(0, y + 18),
            Size = new Size(420, 26),
            ReadOnly = isReadOnly,
            BackColor = isReadOnly ? AppConstants.MauNenChung : Color.White,
            UseSystemPasswordChar = isPassword
        };

        parent.Controls.Add(lbl);
        parent.Controls.Add(textBox);
    }
    
    private static Panel TaoOThongKe(string tieuDe, string giaTriKhoiTao, Color mauGiaTri, out Label lblGiaTri)
    {
        var pnl = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppConstants.MauNenChung,
            BorderStyle = BorderStyle.FixedSingle,
            Margin = new Padding(0, 0, 6, 6),
            Padding = new Padding(8, 4, 8, 4)
        };

        var lblTieuDe = new Label
        {
            Text = tieuDe,
            Font = new Font("Times New Roman", 8.5f, FontStyle.Regular),
            ForeColor = AppConstants.MauChuPhu,
            Dock = DockStyle.Top,
            Height = 15,
            UseMnemonic = false
        };

        lblGiaTri = new Label
        {
            Text = giaTriKhoiTao,
            Font = new Font("Times New Roman", 11.5f, FontStyle.Bold),
            ForeColor = mauGiaTri,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            UseMnemonic = false
        };

        pnl.Controls.Add(lblGiaTri);
        pnl.Controls.Add(lblTieuDe);
        return pnl;
    }

    // Tải dữ liệu tài khoản, hồ sơ khách và các chỉ số thống kê
    public void TaiDuLieuTaiKhoan()
    {
        UIHelper.RunSafe(() =>
        {
            var khach = SessionManager.CurrentCustomer;
            var acc = SessionManager.CurrentUser;

            if (acc != null && acc.AccountId > 0)
            {
                try
                {
                    var khachDb = AppServices.Customer.GetByAccountId(acc.AccountId);
                    if (khachDb != null && khachDb.CustomerId > 0)
                    {
                        SessionManager.CurrentCustomer = khachDb;
                        khach = khachDb;
                    }
                }
                catch (Exception ex)
                {
                    AppLogger.Error(ex, "Đồng bộ thông tin khách hàng từ CSDL");
                }
            }

            NapThongTinKhachHang(khach);
            TaiThongKeThanhVienVaDonPhong(khach);
        }, "tải dữ liệu tài khoản", this);
    }
    
    private void NapThongTinKhachHang(Customer? khach)
    {
        if (khach != null && khach.CustomerId > 0)
        {
            txtMaKH.Text = $"KH{khach.CustomerId:D3}";
            txtTenDangNhap.Text = SessionManager.CurrentUser?.Username ?? khach.Email;
            txtHoTen.Text = khach.FullName;
            txtSoDienThoai.Text = !string.IsNullOrWhiteSpace(khach.PhoneNumber) ? khach.PhoneNumber : khach.Phone;
            txtEmail.Text = khach.Email;
            txtCCCD.Text = !string.IsNullOrWhiteSpace(khach.NationalId) ? khach.NationalId : khach.IdentityCard;

            btnLuuThayDoi.Enabled = true;
            btnKhoiPhuc.Enabled = true;
            UIHelper.DinhDangNutBamNavy(btnLuuThayDoi);
        }
        else
        {
            txtMaKH.Text = "Chưa đăng nhập";
            txtTenDangNhap.Text = string.Empty;
            txtHoTen.Text = string.Empty;
            txtSoDienThoai.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtCCCD.Text = string.Empty;

            btnLuuThayDoi.Enabled = false;
            btnKhoiPhuc.Enabled = false;
            UIHelper.DinhDangNutBamVoHieu(btnLuuThayDoi);
        }
    }

    // Tải thống kê hạng thành viên, điểm thưởng và chi tiêu đơn phòng
    private void TaiThongKeThanhVienVaDonPhong(Customer? khach)
    {
        if (khach == null || khach.CustomerId <= 0)
        {
            lblTierBadge.Text = "HẠNG THÀNH VIÊN: CHƯA ĐĂNG NHẬP";
            lblTierBadge.BackColor = AppConstants.MauXam;
            lblDiemTichLuy.Text = "0 điểm";
            lblLuotLuuTru.Text = "0 lượt";
            lblTongChiTieu.Text = "0 đ";
            lblThongKeDon.Text = "0 đơn";
            return;
        }

        var hang = string.IsNullOrWhiteSpace(khach.TierName) ? "Thường" : khach.TierName;
        lblTierBadge.Text = $"HẠNG THÀNH VIÊN: {hang.ToUpperInvariant()}";

        if (hang.Contains("Kim Cương", StringComparison.OrdinalIgnoreCase) || khach.LoyaltyPoints >= 500)
        {
            lblTierBadge.BackColor = AppConstants.MauXanhNavyDam;
        }
        else if (hang.Contains("Vàng", StringComparison.OrdinalIgnoreCase) || khach.LoyaltyPoints >= 200)
        {
            lblTierBadge.BackColor = AppConstants.MauVangCamDam;
        }
        else if (hang.Contains("Bạc", StringComparison.OrdinalIgnoreCase) || khach.LoyaltyPoints >= 50)
        {
            lblTierBadge.BackColor = Color.FromArgb(71, 85, 105);
        }
        else
        {
            lblTierBadge.BackColor = AppConstants.MauXanhNavy;
        }

        lblDiemTichLuy.Text = $"{khach.LoyaltyPoints:N0} điểm";
        lblLuotLuuTru.Text = $"{khach.StayCount:N0} lượt";

        var dsDon = new List<Booking>();
        try
        {
            dsDon = new BookingRepository().GetByCustomerId(khach.CustomerId);
        }
        catch (Exception ex)
        {
            AppLogger.Error(ex, "Tải danh sách đơn đặt phòng cho thống kê khách hàng");
        }

        var tongDon = dsDon.Count;
        var donHoanTat = dsDon.Count(b => b.Status == BookingStatus.CheckedOut || b.Status.ToString().Equals("CheckedOut", StringComparison.OrdinalIgnoreCase));
        var donDangO = dsDon.Count(b =>
            b.Status == BookingStatus.Confirmed ||
            b.Status == BookingStatus.CheckedIn ||
            b.Status.ToString().Equals("Confirmed", StringComparison.OrdinalIgnoreCase) ||
            b.Status.ToString().Equals("CheckedIn", StringComparison.OrdinalIgnoreCase));

        var tongChiTieu = dsDon
            .Where(b => b.Status != BookingStatus.Cancelled && !b.Status.ToString().Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
            .Sum(b => b.TotalAmount);

        lblTongChiTieu.Text = FormatHelper.FormatCurrency(tongChiTieu);
        lblThongKeDon.Text = $"{tongDon} đơn ({donHoanTat} xong, {donDangO} đang ở/sắp tới)";
    }

    // Xử lý lưu thông tin cá nhân khách hàng
    private void XuLyLuuThongTin()
    {
        var khach = SessionManager.CurrentCustomer;
        if (khach == null || khach.CustomerId <= 0)
        {
            MessageBox.Show("Vui lòng đăng nhập để cập nhật hồ sơ cá nhân!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var hoTen = txtHoTen.Text.Trim();
        var sdt = txtSoDienThoai.Text.Trim();
        var email = txtEmail.Text.Trim();
        var cccd = txtCCCD.Text.Trim();

        if (string.IsNullOrWhiteSpace(hoTen))
        {
            MessageBox.Show("Họ và tên khách hàng không được để trống!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtHoTen.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(sdt) || sdt.Length < 9)
        {
            MessageBox.Show("Số điện thoại không hợp lệ (tối thiểu 9 chữ số)!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtSoDienThoai.Focus();
            return;
        }

        if (!string.IsNullOrWhiteSpace(cccd) && cccd.Length < 9)
        {
            MessageBox.Show("Số CCCD / Hộ chiếu không hợp lệ (tối thiểu 9 chữ số)!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtCCCD.Focus();
            return;
        }

        khach.FullName = hoTen;
        khach.PhoneNumber = sdt;
        khach.Phone = sdt;
        khach.Email = email;
        khach.NationalId = cccd;
        khach.IdentityCard = cccd;

        var thanhCong = false;
        var message = string.Empty;

        try
        {
            thanhCong = AppServices.Customer.SaveCustomer(khach, out message);
        }
        catch (Exception ex)
        {
            AppLogger.Error(ex, "Cập nhật hồ sơ khách hàng qua CustomerService");
            message = "Đã xảy ra lỗi khi lưu thông tin hồ sơ!";
        }

        if (thanhCong)
        {
            SessionManager.CurrentCustomer = khach;
            NapThongTinKhachHang(khach);
            OnProfileUpdated?.Invoke(hoTen);
            MessageBox.Show("Cập nhật thông tin hồ sơ thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        else
        {
            MessageBox.Show(message, "Không thể cập nhật", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // Xử lý đổi mật khẩu tài khoản đăng nhập
    private void XuLyDoiMatKhau()
    {
        var matKhauCu = txtMatKhauCu.Text;
        var matKhauMoi = txtMatKhauMoi.Text;
        var xacNhanMatKhau = txtXacNhanMatKhau.Text;

        if (string.IsNullOrWhiteSpace(matKhauCu) || string.IsNullOrWhiteSpace(matKhauMoi))
        {
            MessageBox.Show("Vui lòng nhập đầy đủ mật khẩu hiện tại và mật khẩu mới!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (matKhauMoi.Length < 6)
        {
            MessageBox.Show("Mật khẩu mới phải từ 6 ký tự trở lên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtMatKhauMoi.Focus();
            return;
        }

        if (matKhauMoi != xacNhanMatKhau)
        {
            MessageBox.Show("Mật khẩu mới và mật khẩu xác nhận không khớp!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtXacNhanMatKhau.Focus();
            return;
        }

        var accountId = SessionManager.CurrentUser?.AccountId ?? SessionManager.CurrentCustomer?.AccountId ?? 0;
        if (accountId <= 0)
        {
            MessageBox.Show("Không tìm thấy thông tin tài khoản đăng nhập!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        var ketQua = AppServices.Auth.ChangePassword(accountId, matKhauCu, matKhauMoi, xacNhanMatKhau, out var message);
        if (ketQua)
        {
            txtMatKhauCu.Text = string.Empty;
            txtMatKhauMoi.Text = string.Empty;
            txtXacNhanMatKhau.Text = string.Empty;
            MessageBox.Show("Đổi mật khẩu tài khoản thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        else
        {
            MessageBox.Show(message, "Không thể đổi mật khẩu", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
