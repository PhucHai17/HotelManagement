using System.Text.RegularExpressions;
using BLL;
using Common.Constants;
using Common.Helpers;
using Entity.Entities;

namespace UI.Forms;

public class EmployeeDialog : Form
{
    private readonly Employee? _currentEmployee;
    private readonly bool _isNew;

    private TextBox txtFullName = null!;
    private TextBox txtEmail = null!;
    private TextBox txtPassword = null!;
    private TextBox txtPhoneNumber = null!;
    private TextBox txtNationalId = null!;
    private TextBox txtPosition = null!;
    private ComboBox cboRole = null!;

    public EmployeeDialog(Employee? employee = null)
    {
        _currentEmployee = employee;
        _isNew = employee == null;

        Font = new Font("Times New Roman", 10f, FontStyle.Regular);
        InitComponents();
        LoadInitialData();
    }
    
    private void InitComponents()
    {
        Text = _isNew ? "HotelPro - Thêm nhân viên mới" : "HotelPro - Sửa thông tin nhân viên";
        Size = new Size(560, 690);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Color.White;

        var pnlHeader = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = AppConstants.MauXanhNavy };
        pnlHeader.Controls.Add(new Label
        {
            Text = _isNew ? "THÊM NHÂN VIÊN MỚI" : "SỬA THÔNG TIN NHÂN VIÊN",
            Font = new Font("Times New Roman", 13f, FontStyle.Bold),
            ForeColor = Color.White,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            UseMnemonic = false
        });

        var pnlFooter = new Panel { Dock = DockStyle.Bottom, Height = 56, BackColor = AppConstants.MauNenChung, BorderStyle = BorderStyle.FixedSingle };
        var btnSave = new Button { Text = "Lưu thông tin", Size = new Size(165, 36), Location = new Point(245, 10), UseMnemonic = false, Cursor = Cursors.Hand };
        UIHelper.DinhDangNutBamNavy(btnSave);
        btnSave.Click += (s, e) => HandleSave();

        var btnCancel = new Button { Text = "Hủy bỏ", Size = new Size(110, 36), Location = new Point(420, 10), UseMnemonic = false, Cursor = Cursors.Hand };
        UIHelper.DinhDangNutBamPhu(btnCancel);
        btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

        pnlFooter.Controls.AddRange(new Control[] { btnSave, btnCancel });

        var pnlContent = new Panel { Dock = DockStyle.Fill, Padding = new Padding(24, 12, 24, 12), AutoScroll = true };

        var y = 10;
        txtFullName = AddInputField(pnlContent, "Họ và tên (*):", y, "Nhập họ tên nhân viên..."); y += 62;
        txtEmail = AddInputField(pnlContent, "Email (*):", y, "vidu: letan@hotelpro.com"); y += 62;
        txtPassword = AddInputField(pnlContent, _isNew ? "Mật khẩu (*):" : "Mật khẩu mới (để trống nếu giữ nguyên):", y,
            _isNew ? "Tối thiểu 6 ký tự..." : "Để trống nếu không đổi mật khẩu...", true); y += 62;
        txtPhoneNumber = AddInputField(pnlContent, "Số điện thoại (*):", y, "10-11 chữ số bắt đầu bằng 0..."); y += 62;
        txtNationalId = AddInputField(pnlContent, "Số CCCD (*):", y, "Đúng 12 chữ số..."); y += 62;
        txtPosition = AddInputField(pnlContent, "Chức vụ (*):", y, "Lễ tân"); y += 62;

        var lblRole = new Label
        {
            Text = "Vai trò (*):",
            Font = new Font("Times New Roman", 9.5f, FontStyle.Bold),
            ForeColor = AppConstants.MauChuChinh,
            Location = new Point(24, y),
            AutoSize = true,
            UseMnemonic = false
        };
        cboRole = new ComboBox
        {
            Font = new Font("Times New Roman", 10f, FontStyle.Regular),
            Location = new Point(24, y + 24),
            Size = new Size(490, 26),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        pnlContent.Controls.AddRange(new Control[] { lblRole, cboRole });

        Controls.Add(pnlContent);
        Controls.Add(pnlFooter);
        Controls.Add(pnlHeader);
    }
    
    private static TextBox AddInputField(Panel parent, string labelText, int y, string placeholder = "", bool isPassword = false)
    {
        var lbl = new Label
        {
            Text = labelText,
            Font = new Font("Times New Roman", 9.5f, FontStyle.Bold),
            ForeColor = AppConstants.MauChuChinh,
            Location = new Point(24, y),
            AutoSize = true,
            UseMnemonic = false
        };
        var txt = new TextBox
        {
            Font = new Font("Times New Roman", 10f, FontStyle.Regular),
            Location = new Point(24, y + 24),
            Size = new Size(490, 26),
            PlaceholderText = placeholder,
            UseSystemPasswordChar = isPassword
        };
        parent.Controls.Add(lbl);
        parent.Controls.Add(txt);
        return txt;
    }
    
    private void LoadInitialData()
    {
        cboRole.Items.Clear();
        if (_isNew)
        {
            cboRole.Items.Add(new RoleItem(AppConstants.MaVTLeTan, "Lễ tân"));
            cboRole.SelectedIndex = 0;
            txtPosition.Text = "Lễ tân";
            return;
        }

        if (_currentEmployee == null) return;
        txtFullName.Text = _currentEmployee.FullName;
        txtEmail.Text = _currentEmployee.Account?.Username ?? string.Empty;
        txtPhoneNumber.Text = _currentEmployee.PhoneNumber;
        txtNationalId.Text = _currentEmployee.NationalId;
        txtPosition.Text = string.IsNullOrWhiteSpace(_currentEmployee.Position) ? "Lễ tân" : _currentEmployee.Position;

        var currentRoleId = _currentEmployee.Account?.RoleId ?? AppConstants.MaVTLeTan;
        var roleName = currentRoleId == AppConstants.MaVTAdmin ? "Quản trị viên"
                     : currentRoleId == AppConstants.MaVTQuanLy ? "Quản lý" : "Lễ tân";
        cboRole.Items.Add(new RoleItem(currentRoleId, roleName));
        cboRole.SelectedIndex = 0;
    }
    
    private void HandleSave()
    {
        var fullName = txtFullName.Text.Trim();
        var email = txtEmail.Text.Trim();
        var phone = txtPhoneNumber.Text.Trim();
        var cccd = txtNationalId.Text.Trim();
        var position = string.IsNullOrWhiteSpace(txtPosition.Text) ? "Lễ tân" : txtPosition.Text.Trim();

        if (string.IsNullOrWhiteSpace(fullName))
        {
            MessageBox.Show(this, "Họ và tên nhân viên không được để trống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtFullName.Focus();
            return;
        }
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@') || !email.Contains('.') || email.Contains(' '))
        {
            MessageBox.Show(this, "Email không hợp lệ (bắt buộc có ký tự '@', dấu '.' và không chứa khoảng trắng)!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtEmail.Focus();
            return;
        }
        if (!Regex.IsMatch(phone, @"^0\d{9,10}$"))
        {
            MessageBox.Show(this, "Số điện thoại không hợp lệ (phải gồm 10-11 chữ số và bắt đầu bằng số 0)!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtPhoneNumber.Focus();
            return;
        }
        if (!Regex.IsMatch(cccd, @"^\d{12}$"))
        {
            MessageBox.Show(this, "Số CCCD không hợp lệ (bắt buộc đúng 12 chữ số)!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtNationalId.Focus();
            return;
        }

        var roleId = (cboRole.SelectedItem as RoleItem)?.RoleId ?? AppConstants.MaVTLeTan;

        UIHelper.RunSafe(() =>
        {
            bool success;
            string message;
            if (_isNew)
            {
                var password = txtPassword.Text;
                if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
                {
                    MessageBox.Show(this, "Mật khẩu phải từ 6 ký tự trở lên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPassword.Focus();
                    return;
                }

                var newEmployee = new Employee { FullName = fullName, PhoneNumber = phone, NationalId = cccd, Position = position };
                success = AppServices.Employee.AddEmployee(newEmployee, email, password, roleId, out message);
            }
            else
            {
                if (_currentEmployee == null) return;
                var newPassword = txtPassword.Text.Trim();
                if (!string.IsNullOrEmpty(newPassword) && newPassword.Length < 6)
                {
                    MessageBox.Show(this, "Mật khẩu mới phải từ 6 ký tự trở lên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPassword.Focus();
                    return;
                }

                _currentEmployee.FullName = fullName;
                _currentEmployee.PhoneNumber = phone;
                _currentEmployee.NationalId = cccd;
                _currentEmployee.Position = position;
                _currentEmployee.Account ??= new Account();
                _currentEmployee.Account.Username = email;
                success = AppServices.Employee.UpdateEmployee(_currentEmployee, roleId, string.IsNullOrWhiteSpace(newPassword) ? null : newPassword, out message);
            }

            MessageBox.Show(this, message, "Thông báo", MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (success) { DialogResult = DialogResult.OK; Close(); }
        }, "lưu thông tin nhân viên", this);
    }

    private sealed record RoleItem(int RoleId, string RoleName)
    {
        public override string ToString() => RoleName;
    }
}
