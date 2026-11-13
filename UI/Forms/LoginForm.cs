using BLL;
using BLL.Interfaces;
using Common.Constants;
using Common.Helpers;
using UI.Controls;
using UI.Helpers;

namespace UI.Forms;

public class LoginForm : Form
{
    private readonly IAuthService _taiKhoanBLL = AppServices.Auth;
    private Button btnDangNhap = null!;
    private Button btnVaiTroKhach = null!;
    private Button btnVaiTroLeTan = null!;
    private Button btnVaiTroQuanLy = null!;
    private CheckBox chkGhiNho = null!;
    private LinkLabel lnkDangKy = null!;
    private RoundedTextBox txtMatKhau = null!;
    private RoundedTextBox txtTenDangNhap = null!;

    public LoginForm()
    {
        KhoiTaoGiaoDien();
        UIHelper.ApDungFontChinh(this);
        ChonVaiTro("Lễ tân");
    }

    private void KhoiTaoGiaoDien()
    {
        Text = "HotelPro - Đăng nhập";
        Size = new Size(980, 610);
        MinimumSize = new Size(900, 550);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        BackColor = AppConstants.MauNen;

        const int formWidth = 400;
        var (_, _, flowForm) = AuthLayoutHelper.BuildAuthSplitLayout(
            this,
            "Khách sạn tiện nghi - Dịch vụ tận tâm",
            formWidth);

        // Tiêu đề form (giãn cách thoáng trước trường nhập đầu tiên)
        var lblHeader = AuthLayoutHelper.CreateHeaderLabel("Đăng nhập", bottomMargin: 22);
        flowForm.Controls.Add(lblHeader);

        // Ô nhập tên đăng nhập / Email (38px bo góc)
        var pnlTen = AuthLayoutHelper.CreateLabeledInput(
            "Tên đăng nhập / Email",
            formWidth,
            out txtTenDangNhap,
            placeholder: "Nhập tên tài khoản hoặc email...",
            bottomMargin: 16);
        flowForm.Controls.Add(pnlTen);

        // Ô nhập mật khẩu (38px bo góc)
        var pnlMK = AuthLayoutHelper.CreateLabeledInput(
            "Mật khẩu",
            formWidth,
            out txtMatKhau,
            placeholder: "Nhập mật khẩu...",
            isPassword: true,
            bottomMargin: 12);
        flowForm.Controls.Add(pnlMK);

        // Dòng Ghi nhớ đăng nhập & Quên mật khẩu
        var pnlRememberForgot = new Panel
        {
            Size = new Size(formWidth, 28),
            BackColor = AppConstants.MauNen,
            Margin = new Padding(0, 0, 0, 18)
        };

        chkGhiNho = new CheckBox
        {
            Text = "Ghi nhớ đăng nhập",
            Dock = DockStyle.Left,
            AutoSize = true,
            Checked = true,
            ForeColor = AppConstants.MauChuPhu,
            Font = new Font(AppConstants.FontChuChinh, 9.5f),
            Cursor = Cursors.Hand
        };

        var lnkQuenMK = new LinkLabel
        {
            Text = "Quên mật khẩu?",
            Dock = DockStyle.Right,
            AutoSize = true,
            Padding = new Padding(0, 3, 0, 0),
            LinkColor = AppConstants.MauXanhNavy,
            ActiveLinkColor = AppConstants.MauVangCamDam,
            VisitedLinkColor = AppConstants.MauXanhNavy,
            LinkBehavior = LinkBehavior.HoverUnderline,
            UseMnemonic = false,
            Font = new Font(AppConstants.FontChuChinh, 9.5f)
        };
        lnkQuenMK.LinkClicked += (_, _) =>
            MessageBox.Show(
                this,
                "Chức năng đang phát triển. Vui lòng liên hệ ban quản trị để được hỗ trợ cấp lại mật khẩu!",
                "Quên mật khẩu",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

        pnlRememberForgot.Controls.Add(chkGhiNho);
        pnlRememberForgot.Controls.Add(lnkQuenMK);
        flowForm.Controls.Add(pnlRememberForgot);

        // Nút Đăng nhập chính (màu vàng đồng bo góc có hiệu ứng hover)
        btnDangNhap = new Button
        {
            Text = "Đăng nhập",
            Size = new Size(formWidth, 44),
            Margin = new Padding(0, 0, 0, 16)
        };
        UIHelper.DinhDangNutBamChinh(btnDangNhap);
        btnDangNhap.Click += BtnDangNhap_Click;
        flowForm.Controls.Add(btnDangNhap);

        // Liên kết chuyển sang đăng ký tài khoản
        lnkDangKy = AuthLayoutHelper.CreateCenteredSwitchLink(
            "Chưa có tài khoản? ",
            "Đăng ký",
            formWidth,
            bottomMargin: 14);
        lnkDangKy.LinkClicked += (_, _) =>
        {
            using var frmDK = new RegisterForm();
            frmDK.ShowDialog(this);
        };
        flowForm.Controls.Add(lnkDangKy);

        // Hàng chọn nhanh vai trò: Khách hàng | Lễ tân | Quản lý
        var pnlVaiTroGroup = new Panel
        {
            Size = new Size(formWidth, 40),
            AutoSize = true,
            MinimumSize = new Size(formWidth, 40),
            BackColor = AppConstants.MauNen,
            Margin = Padding.Empty
        };

        var flowVT = new FlowLayoutPanel
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            BackColor = AppConstants.MauNen,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };

        var lblVaiTro = new Label
        {
            Text = "Vai trò:",
            AutoSize = true,
            ForeColor = AppConstants.MauChuPhu,
            Font = new Font(AppConstants.FontChuChinh, 9.5f),
            Margin = new Padding(0, 8, 8, 0)
        };

        btnVaiTroKhach = TaoNutVaiTro("Khách hàng", rightMargin: 6);
        btnVaiTroKhach.Click += (_, _) => ChonVaiTro("Khách hàng");

        btnVaiTroLeTan = TaoNutVaiTro("Lễ tân", rightMargin: 6);
        btnVaiTroLeTan.Click += (_, _) => ChonVaiTro("Lễ tân");

        btnVaiTroQuanLy = TaoNutVaiTro("Quản lý", rightMargin: 0);
        btnVaiTroQuanLy.Click += (_, _) => ChonVaiTro("Quản lý");

        flowVT.Controls.AddRange(lblVaiTro, btnVaiTroKhach, btnVaiTroLeTan, btnVaiTroQuanLy);
        pnlVaiTroGroup.Controls.Add(flowVT);

        void CanGiuaVaiTro()
        {
            flowVT.PerformLayout();
            pnlVaiTroGroup.Width = Math.Max(formWidth, flowVT.Width);
            flowVT.Location = new Point(
                Math.Max(0, (pnlVaiTroGroup.Width - flowVT.Width) / 2),
                Math.Max(0, (pnlVaiTroGroup.Height - flowVT.Height) / 2));
        }

        pnlVaiTroGroup.Resize += (_, _) => CanGiuaVaiTro();
        flowVT.SizeChanged += (_, _) => CanGiuaVaiTro();
        CanGiuaVaiTro();

        flowForm.Controls.Add(pnlVaiTroGroup);
        AcceptButton = btnDangNhap;
    }

    private static Button TaoNutVaiTro(string text, int rightMargin)
    {
        // Dùng AutoSize để chữ "Khách hàng", "Lễ tân", "Quản lý" luôn hiển thị đủ
        var btn = new Button
        {
            Text = text,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            MinimumSize = new Size(86, 34),
            Height = 34,
            Padding = new Padding(10, 4, 10, 4),
            Margin = new Padding(0, 0, rightMargin, 0),
            UseMnemonic = false
        };
        UIHelper.DinhDangNutBamPhu(btn);
        return btn;
    }

    private void ChonVaiTro(string vaiTro)
    {
        UIHelper.DinhDangNutBamPhu(btnVaiTroKhach);
        UIHelper.DinhDangNutBamPhu(btnVaiTroLeTan);
        UIHelper.DinhDangNutBamPhu(btnVaiTroQuanLy);

        Button btnChon;
        switch (vaiTro)
        {
            case "Khách hàng":
                txtTenDangNhap.Text = "khach1";
                txtMatKhau.Text = "123456";
                btnChon = btnVaiTroKhach;
                break;
            case "Quản lý":
                txtTenDangNhap.Text = "quanly";
                txtMatKhau.Text = "123456";
                btnChon = btnVaiTroQuanLy;
                break;
            default:
                txtTenDangNhap.Text = "letan1";
                txtMatKhau.Text = "123456";
                btnChon = btnVaiTroLeTan;
                break;
        }

        UIHelper.DinhDangNutBamNavy(btnChon);
    }

    private void BtnDangNhap_Click(object? sender, EventArgs e)
    {
        SessionManager.Logout();
        var tenDangNhap = txtTenDangNhap.Text.Trim();
        var matKhau = txtMatKhau.Text;

        if (string.IsNullOrEmpty(tenDangNhap) || string.IsNullOrEmpty(matKhau))
        {
            MessageBox.Show(this, "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu!", "Lỗi", MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        UIHelper.RunSafe(() =>
        {
            var thanhCong = _taiKhoanBLL.Login(tenDangNhap, matKhau, out var thongBao);
            if (!thanhCong)
            {
                MessageBox.Show(this, thongBao, "Đăng nhập thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Hide();
            try
            {
                // Điều hướng màn hình chính theo vai trò
                switch (SessionManager.CurrentUser?.RoleId)
                {
                    case AppConstants.MaVTKhachHang:
                        using (var frmKhach = new CustomerMainForm())
                        {
                            frmKhach.ShowDialog();
                        }

                        break;
                    case AppConstants.MaVTQuanLy:
                    case AppConstants.MaVTAdmin:
                        using (var frmQuanLy = new ManagerMainForm())
                        {
                            frmQuanLy.ShowDialog();
                        }

                        break;
                    case AppConstants.MaVTLeTan:
                    default:
                        using (var frmLeTan = new ReceptionistMainForm())
                        {
                            frmLeTan.ShowDialog();
                        }

                        break;
                }
            }
            finally
            {
                if (!IsDisposed)
                {
                    txtMatKhau.Text = string.Empty;
                    Show();
                }
            }
        }, "đăng nhập hệ thống", this);
    }
}