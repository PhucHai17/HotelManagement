using BLL;
using Common.Constants;
using Common.Helpers;
using Entity.Entities;
using UI.Forms;

namespace UI.Controls;

public class  EmployeeTab : UserControl
{
    private TextBox txtSearch = null!;
    private DataGridView dgvEmployees = null!;
    private Button btnAdd = null!;
    private Button btnEdit = null!;
    private Button btnToggleLock = null!;
    private Button btnRefresh = null!;
    private Employee? _selectedEmployee;

    public EmployeeTab()
    {
        Font = new Font("Times New Roman", 10f, FontStyle.Regular);
        InitComponents();
        LoadEmployees();
    }

    // Khởi tạo bố cục giao diện tab quản lý nhân viên
    private void InitComponents()
    {
        var tblRoot = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 1,
            Margin = new Padding(0)
        };
        tblRoot.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        tblRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

        var pnlTop = new Panel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(14, 10, 14, 8),
            Margin = new Padding(0, 0, 0, 8)
        };

        var flowTop = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            Padding = new Padding(0),
            Margin = new Padding(0)
        };

        var lblSearch = new Label
        {
            Text = "Tìm nhân viên:",
            Font = new Font("Times New Roman", 9.5f, FontStyle.Regular),
            AutoSize = true,
            Margin = new Padding(0, 8, 8, 6),
            ForeColor = Color.FromArgb(71, 85, 105),
            UseMnemonic = false
        };

        txtSearch = new TextBox
        {
            Width = 240,
            Height = 32,
            Font = new Font("Times New Roman", 10f, FontStyle.Regular),
            Margin = new Padding(0, 3, 14, 6),
            PlaceholderText = "Tên, Email, SĐT, CCCD..."
        };
        txtSearch.TextChanged += (s, e) => LoadEmployees();

        btnAdd = CreateButton("+ Thêm nhân viên", 145, OpenAddDialog, true);
        btnEdit = CreateButton("Sửa thông tin", 115, () => { if (_selectedEmployee != null) OpenEditDialog(_selectedEmployee); });
        btnToggleLock = CreateButton("Khóa / Mở khóa", 135, () => { if (_selectedEmployee != null) ToggleLockStatus(_selectedEmployee); });
        btnRefresh = CreateButton("Làm mới", 95, () => { txtSearch.Text = string.Empty; LoadEmployees(); });
        btnRefresh.Margin = new Padding(0, 2, 0, 6);

        flowTop.Controls.AddRange(new Control[] { lblSearch, txtSearch, btnAdd, btnEdit, btnToggleLock, btnRefresh });
        pnlTop.Controls.Add(flowTop);
        tblRoot.Controls.Add(pnlTop, 0, 0);

        dgvEmployees = new DataGridView
        {
            Dock = DockStyle.Fill,
            Font = new Font("Times New Roman", 10f, FontStyle.Regular)
        };
        UIHelper.DinhDangBangDuLieu(dgvEmployees);

        AddColumn(dgvEmployees, "MaNV", "Mã NV", 48, 65);
        AddColumn(dgvEmployees, "HoTen", "Họ và tên", 110, 120);
        AddColumn(dgvEmployees, "Email", "Email", 115, 130);
        AddColumn(dgvEmployees, "SDT", "Số điện thoại", 78, 90);
        AddColumn(dgvEmployees, "CCCD", "Số CCCD", 85, 100);
        AddColumn(dgvEmployees, "ChucVu", "Chức vụ", 68, 75);
        AddColumn(dgvEmployees, "VaiTro", "Vai trò", 72, 75);
        AddColumn(dgvEmployees, "TrangThai", "Trạng thái", 85, 95);

        dgvEmployees.Columns.Add(new DataGridViewButtonColumn
        {
            Name = "ColSua", HeaderText = "Sửa", Text = "Sửa", UseColumnTextForButtonValue = true, FillWeight = 46, MinimumWidth = 55
        });
        dgvEmployees.Columns.Add(new DataGridViewButtonColumn
        {
            Name = "ColKhoa", HeaderText = "Khóa", Text = "Đổi trạng thái", UseColumnTextForButtonValue = true, FillWeight = 95, MinimumWidth = 105
        });

        dgvEmployees.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        dgvEmployees.ColumnHeadersHeight = 42;
        dgvEmployees.SelectionChanged += (s, e) => UpdateButtonStates();
        dgvEmployees.CellClick += DgvEmployees_CellClick;
        dgvEmployees.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0 && _selectedEmployee != null) OpenEditDialog(_selectedEmployee); };
        dgvEmployees.CellPainting += DgvEmployees_CellPainting;

        tblRoot.Controls.Add(dgvEmployees, 0, 1);
        Controls.Add(tblRoot);
    }
    
    private static Button CreateButton(string text, int width, Action onClick, bool isPrimary = false)
    {
        var btn = new Button
        {
            Text = text,
            Width = width,
            Height = 34,
            UseMnemonic = false,
            Cursor = Cursors.Hand,
            Margin = new Padding(0, 2, 8, 6),
            Font = new Font("Times New Roman", 10f, isPrimary ? FontStyle.Bold : FontStyle.Regular)
        };
        if (isPrimary) UIHelper.DinhDangNutBamChinh(btn);
        else UIHelper.DinhDangNutBamPhu(btn);
        btn.Click += (s, e) => onClick();
        return btn;
    }
    
    private static void AddColumn(DataGridView dgv, string name, string header, float fillWeight, int minWidth)
    {
        var col = dgv.Columns[dgv.Columns.Add(name, header)];
        col.FillWeight = fillWeight;
        col.MinimumWidth = minWidth;
    }
    
    public void LoadEmployees()
    {
        UIHelper.RunSafe(() =>
        {
            var keyword = txtSearch?.Text.Trim() ?? string.Empty;
            var employees = string.IsNullOrWhiteSpace(keyword)
                ? AppServices.Employee.GetAll()
                : AppServices.Employee.Search(keyword);

            var receptionists = employees.Where(emp =>
            {
                var roleId = emp.Account?.RoleId ?? 0;
                var roleName = emp.Account?.RoleName ?? string.Empty;

                if (roleId == AppConstants.MaVTAdmin || roleId == AppConstants.MaVTQuanLy)
                    return false;

                if (roleName.Contains("Admin", StringComparison.OrdinalIgnoreCase) ||
                    roleName.Contains("Quản trị", StringComparison.OrdinalIgnoreCase) ||
                    roleName.Contains("Quản lý", StringComparison.OrdinalIgnoreCase) ||
                    roleName.Contains("QuanLy", StringComparison.OrdinalIgnoreCase))
                    return false;

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    var email = emp.Account?.Username ?? string.Empty;
                    var fullName = emp.FullName ?? string.Empty;
                    var phone = emp.PhoneNumber ?? string.Empty;
                    var cccd = emp.NationalId ?? string.Empty;

                    return fullName.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                        || phone.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                        || cccd.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                        || email.StartsWith(keyword, StringComparison.OrdinalIgnoreCase);
                }

                return true;
            }).ToList();

            dgvEmployees.Rows.Clear();
            foreach (var emp in receptionists)
            {
                var email = emp.Account?.Username ?? string.Empty;
                var roleName = !string.IsNullOrWhiteSpace(emp.Account?.RoleName)
                    ? emp.Account.RoleName
                    : (emp.Account?.RoleId == AppConstants.MaVTAdmin ? "Quản trị viên"
                        : emp.Account?.RoleId == AppConstants.MaVTQuanLy ? "Quản lý" : "Lễ tân");

                var isLocked = AccountStatusHelper.IsLocked(emp.Account?.Status) || emp.Status == AppConstants.EmployeeStatusLocked;
                var rowIndex = dgvEmployees.Rows.Add(
                    $"NV{emp.EmployeeId:D3}", emp.FullName, email, emp.PhoneNumber, emp.NationalId, emp.Position, roleName,
                    isLocked ? "Đã khóa" : "Đang làm việc");

                dgvEmployees.Rows[rowIndex].Tag = emp;
            }

            UpdateButtonStates();
        }, "tải danh sách nhân viên", this);
    }
    
    public void TaiDanhSachNhanVien() => LoadEmployees();
    private void UpdateButtonStates()
    {
        _selectedEmployee = dgvEmployees.SelectedRows.Count > 0 ? dgvEmployees.SelectedRows[0].Tag as Employee : null;
        var hasSelection = _selectedEmployee != null;
        btnEdit.Enabled = hasSelection;
        btnToggleLock.Enabled = hasSelection;
        if (hasSelection)
        {
            UIHelper.DinhDangNutBamPhu(btnEdit);
            UIHelper.DinhDangNutBamPhu(btnToggleLock);
        }
        else
        {
            UIHelper.DinhDangNutBamVoHieu(btnEdit);
            UIHelper.DinhDangNutBamVoHieu(btnToggleLock);
        }
    }
    
    private void DgvEmployees_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || dgvEmployees.Rows[e.RowIndex].Tag is not Employee emp) return;

        if (e.ColumnIndex == dgvEmployees.Columns["ColSua"].Index)
            OpenEditDialog(emp);
        else if (e.ColumnIndex == dgvEmployees.Columns["ColKhoa"].Index)
            ToggleLockStatus(emp);
    }
    
    private void DgvEmployees_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
    {
        if (e.RowIndex >= 0 && e.ColumnIndex == dgvEmployees.Columns["TrangThai"].Index && e.Value != null)
        {
            var cellText = e.Value.ToString() ?? string.Empty;
            var isLocked = cellText.Equals("Đã khóa", StringComparison.OrdinalIgnoreCase);
            var backColor = isLocked ? Color.FromArgb(254, 226, 226) : Color.FromArgb(220, 252, 231);
            var foreColor = isLocked ? Color.FromArgb(185, 28, 28) : Color.FromArgb(22, 101, 52);
            UIHelper.VeBadge(e, cellText, backColor, foreColor);
        }
    }

    // Mở hộp thoại thêm nhân viên mới
    private void OpenAddDialog()
    {
        using var dlg = new EmployeeDialog(null);
        if (dlg.ShowDialog(FindForm()) == DialogResult.OK) LoadEmployees();
    }

    // Mở hộp thoại sửa thông tin nhân viên
    private void OpenEditDialog(Employee employee)
    {
        using var dlg = new EmployeeDialog(employee);
        if (dlg.ShowDialog(FindForm()) == DialogResult.OK) LoadEmployees();
    }

    // Xử lý khóa hoặc mở khóa tài khoản nhân viên
    private void ToggleLockStatus(Employee employee)
    {
        var isLocked = AccountStatusHelper.IsLocked(employee.Account?.Status) || employee.Status == AppConstants.EmployeeStatusLocked;
        var actionText = isLocked ? "mở khóa" : "khóa";

        if (MessageBox.Show(FindForm(), $"Bạn có chắc chắn muốn {actionText} tài khoản của nhân viên {employee.FullName} (NV{employee.EmployeeId:D3})?",
                "Xác nhận thay đổi trạng thái", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        UIHelper.RunSafe(() =>
        {
            var success = AppServices.Employee.ToggleAccountLock(employee.EmployeeId, out var message);
            MessageBox.Show(FindForm(), message, "Thông báo", MessageBoxButtons.OK,
                success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            if (success) LoadEmployees();
        }, "cập nhật trạng thái tài khoản nhân viên", FindForm());
    }
}
