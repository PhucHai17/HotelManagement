using BLL;
using BLL.Interfaces;
using Common.Constants;
using Common.Helpers;
using UI.Controls;
using UI.Helpers;

namespace UI.Forms;

// Form đăng ký tài khoản khách hàng
public class RegisterForm : Form
{
    private readonly IAuthService _taiKhoanBLL = AppServices.Auth;
    private Button btnDangKy = null!;
    private LinkLabel lnkDangNhap = null!;
    private RoundedTextBox txtCCCD = null!;
    private RoundedTextBox txtEmail = null!;
    private RoundedTextBox txtHoTen = null!;
    private RoundedTextBox txtMatKhau = null!;
    private RoundedTextBox txtNhapLaiMK = null!;
    private RoundedTextBox txtSDT = null!;

    public RegisterForm()
    {
        KhoiTaoGiaoDien();
        UIHelper.ApDungFontChinh(this);
    }

    private void KhoiTaoGiaoDien()
    {
        Text = "HotelPro - Tạo tài khoản";
        Size = new Size(980, 660);
        MinimumSize = new Size(900, 600);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        BackColor = AppConstants.MauNen;

        const int formWidth = 420;
        var (_, _, flowForm) = AuthLayoutHelper.BuildAuthSplitLayout(
            this,
            "Khách sạn tiện nghi - Dịch vụ chu đáo",
            formWidth);

        // Tiêu đề form
        var lblHeader = AuthLayoutHelper.CreateHeaderLabel("Tạo tài khoản", bottomMargin: 18);
        flowForm.Controls.Add(lblHeader);

        // Hàng Họ tên + SĐT nằm ngang 2 cột
        var flowNamePhone = new FlowLayoutPanel
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            BackColor = AppConstants.MauNen,
            Margin = new Padding(0, 0, 0, 12),
            Padding = Padding.Empty
        };

        var pnlHoTen = AuthLayoutHelper.CreateLabeledInput(
            "Họ tên",
            214,
            out txtHoTen,
            placeholder: "Nguyễn Văn A",
            bottomMargin: 0);
        pnlHoTen.Margin = new Padding(0, 0, 14, 0);

        var pnlSDT = AuthLayoutHelper.CreateLabeledInput(
            "SĐT",
            192,
            out txtSDT,
            placeholder: "09xxxxxxxx",
            bottomMargin: 0);
        pnlSDT.Margin = Padding.Empty;

        flowNamePhone.Controls.Add(pnlHoTen);
        flowNamePhone.Controls.Add(pnlSDT);
        flowForm.Controls.Add(flowNamePhone);

        // Các trường nhập đầy đủ chiều rộng
        flowForm.Controls.Add(AuthLayoutHelper.CreateLabeledInput(
            "Email",
            formWidth,
            out txtEmail,
            placeholder: "email@example.com",
            bottomMargin: 12));

        flowForm.Controls.Add(AuthLayoutHelper.CreateLabeledInput(
            "CCCD / Hộ chiếu",
            formWidth,
            out txtCCCD,
            placeholder: "Nhập số CCCD hoặc hộ chiếu...",
            bottomMargin: 12));

        flowForm.Controls.Add(AuthLayoutHelper.CreateLabeledInput(
            "Mật khẩu",
            formWidth,
            out txtMatKhau,
            placeholder: "Tối thiểu 6 ký tự...",
            isPassword: true,
            bottomMargin: 12));

        flowForm.Controls.Add(AuthLayoutHelper.CreateLabeledInput(
            "Nhập lại mật khẩu",
            formWidth,
            out txtNhapLaiMK,
            placeholder: "Nhập lại mật khẩu...",
            isPassword: true,
            bottomMargin: 18));

        // Nút Đăng ký chính (vàng đồng bo góc có hover)
        btnDangKy = new Button
        {
            Text = "Đăng ký",
            Size = new Size(formWidth, 44),
            Margin = new Padding(0, 0, 0, 14)
        };
        UIHelper.DinhDangNutBamChinh(btnDangKy);
        btnDangKy.Click += BtnDangKy_Click;
        flowForm.Controls.Add(btnDangKy);

        // Liên kết quay lại đăng nhập căn giữa
        lnkDangNhap = AuthLayoutHelper.CreateCenteredSwitchLink(
            "Đã có tài khoản? ",
            "Đăng nhập",
            formWidth,
            bottomMargin: 0);
        lnkDangNhap.LinkClicked += (_, _) => Close();
        flowForm.Controls.Add(lnkDangNhap);

        AcceptButton = btnDangKy;
    }

    private void BtnDangKy_Click(object? sender, EventArgs e)
    {
        UIHelper.RunSafe(() =>
        {
            var thanhCong = _taiKhoanBLL.Register(
                txtHoTen.Text,
                txtSDT.Text,
                txtEmail.Text,
                txtCCCD.Text,
                txtMatKhau.Text,
                txtNhapLaiMK.Text,
                out var thongBao);

            if (thanhCong)
            {
                MessageBox.Show(this, thongBao, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            else
            {
                MessageBox.Show(this, thongBao, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }, "đăng ký tài khoản", this);
    }
}
