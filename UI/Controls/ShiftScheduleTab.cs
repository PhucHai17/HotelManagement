using BLL;
using Common.Constants;
using Common.Helpers;
using Entity.Entities;
using UI.Helpers;

namespace UI.Controls;

/// <summary>Manager-facing shift assignment and handover report.</summary>
public sealed class ShiftScheduleTab : UserControl
{
    private readonly DateTimePicker _from = new() { Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy", Value = DateTime.Today.AddDays(-6), Width = 130 };
    private readonly DateTimePicker _to = new() { Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy", Value = DateTime.Today, Width = 130 };
    private readonly DateTimePicker _workDate = new() { Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy", Value = DateTime.Today, Width = 130 };
    private readonly ComboBox _employees = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 210 };
    private readonly ComboBox _shifts = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 180 };
    private readonly DataGridView _grid = new() { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false, MultiSelect = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
    private List<ShiftSchedule> _schedules = new();
    private List<WorkShift> _workShifts = new();

    public ShiftScheduleTab()
    {
        BuildUi();
        LoadChoices();
        LoadSchedules();
    }

    private void BuildUi()
    {
        BackColor = Color.FromArgb(243, 244, 246);
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Padding = new Padding(12) };
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var title = new Label { Text = "PHÂN CA & BÁO CÁO GIAO CA", AutoSize = true, Font = new Font("Times New Roman", 15, FontStyle.Bold), ForeColor = AppConstants.MauXanhNavy, Margin = new Padding(0, 0, 0, 10) };
        var filters = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true, BackColor = Color.White, Padding = new Padding(10), Margin = new Padding(0, 0, 0, 8) };
        filters.Controls.AddRange(new Control[] { LabelFor("Từ ngày"), _from, LabelFor("Đến ngày"), _to, Button("Xem giao ca", LoadSchedules, true), Button("Xuất CSV", ExportCsv) });

        var editor = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true, BackColor = Color.White, Padding = new Padding(10), Margin = new Padding(0, 0, 0, 8) };
        editor.Controls.AddRange(new Control[] { LabelFor("Ngày làm"), _workDate, LabelFor("Nhân viên"), _employees, LabelFor("Ca làm"), _shifts,
            Button("Phân ca", AddSchedule, true), Button("Cập nhật ca", UpdateSchedule), Button("Xóa ca", DeleteSchedule), Button("Làm mới", () => { LoadChoices(); LoadSchedules(); }) });

        var top = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, AutoSize = true };
        top.Controls.Add(title, 0, 0);
        top.Controls.Add(filters, 0, 1);
        top.Controls.Add(editor, 0, 2);

        UIHelper.DinhDangBangDuLieu(_grid);
        _grid.Columns.Add("ScheduleId", "Mã lịch");
        _grid.Columns.Add("WorkDate", "Ngày làm");
        _grid.Columns.Add("Employee", "Nhân viên");
        _grid.Columns.Add("Shift", "Ca làm");
        _grid.Columns.Add("Hours", "Thời gian");
        _grid.SelectionChanged += (_, _) => CopySelectedToEditor();
        root.Controls.Add(top, 0, 0);
        root.Controls.Add(new Label { Text = "Lịch phân ca và giao ca trong khoảng ngày đã chọn", Dock = DockStyle.Fill, AutoSize = true, Font = new Font("Times New Roman", 10, FontStyle.Bold), Margin = new Padding(0, 4, 0, 6) }, 0, 1);
        root.Controls.Add(_grid, 0, 2);
        Controls.Add(root);
    }

    private void LoadChoices()
    {
        try
        {
            var employees = AppServices.Employee.GetAll()
                .Where(e => e.Account?.RoleId != AppConstants.MaVTQuanLy && e.Account?.RoleId != AppConstants.MaVTAdmin)
                .Where(e => !AccountStatusHelper.IsLocked(e.Account?.Status) && e.Status != AppConstants.EmployeeStatusLocked)
                .OrderBy(e => e.FullName).ToList();
            _employees.DataSource = employees;
            _employees.DisplayMember = nameof(Employee.FullName);
            _employees.ValueMember = nameof(Employee.EmployeeId);
            _workShifts = AppServices.ShiftSchedule.GetWorkShifts();
            _shifts.DataSource = _workShifts;
            _shifts.DisplayMember = nameof(WorkShift.HandoverNote);
            _shifts.ValueMember = nameof(WorkShift.ShiftId);
        }
        catch (Exception ex) { UIHelper.RunSafe(() => throw ex, "tải danh sách ca và nhân viên", FindForm()); }
    }

    public void LoadSchedules()
    {
        if (_from.Value.Date > _to.Value.Date)
        {
            MessageBox.Show(FindForm(), "Ngày bắt đầu không được sau ngày kết thúc.", "Khoảng ngày không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        UIHelper.RunSafe(() =>
        {
            _schedules = AppServices.ShiftSchedule.GetAll(_from.Value.Date, _to.Value.Date);
            var shifts = _workShifts.ToDictionary(s => s.ShiftId, s => s);
            _grid.Rows.Clear();
            foreach (var item in _schedules)
            {
                var id = int.TryParse(item.ShiftType, out var shiftId) ? shiftId : _workShifts.FirstOrDefault(s => s.HandoverNote == item.ShiftType)?.ShiftId ?? 0;
                var shift = shifts.GetValueOrDefault(id);
                var time = shift == null ? "" : $"{shift.StartTime:HH:mm} – {shift.EndTime:HH:mm}";
                var row = _grid.Rows.Add(item.ScheduleId, FormatDate(item.WorkDate), item.EmployeeName, item.ShiftType, time);
                _grid.Rows[row].Tag = item;
            }
        }, "tải lịch phân ca và báo cáo giao ca", FindForm());
    }

    private void AddSchedule()
    {
        if (_employees.SelectedItem is not Employee employee || _shifts.SelectedItem is not WorkShift shift) { MessageBox.Show(FindForm(), "Cần có nhân viên và ca làm việc để phân ca.", "Thiếu dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
        var item = new ShiftSchedule { EmployeeId = employee.EmployeeId, ManagerId = SessionManager.CurrentEmployee?.EmployeeId, WorkDate = _workDate.Value.ToString("yyyy-MM-dd"), ShiftType = shift.ShiftId.ToString(), EmployeeName = employee.FullName };
        UIHelper.RunSafe(() => { var ok = AppServices.ShiftSchedule.Add(item, out var message); MessageBox.Show(FindForm(), message, "Phân ca", MessageBoxButtons.OK, ok ? MessageBoxIcon.Information : MessageBoxIcon.Warning); if (ok) LoadSchedules(); }, "thêm lịch phân ca", FindForm());
    }

    private void UpdateSchedule()
    {
        if (SelectedSchedule() is not { } item || _employees.SelectedItem is not Employee employee || _shifts.SelectedItem is not WorkShift shift) { MessageBox.Show(FindForm(), "Chọn một lịch ca cần cập nhật.", "Chưa chọn lịch", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
        item.EmployeeId = employee.EmployeeId; item.EmployeeName = employee.FullName; item.WorkDate = _workDate.Value.ToString("yyyy-MM-dd"); item.ShiftType = shift.ShiftId.ToString();
        UIHelper.RunSafe(() => { var ok = AppServices.ShiftSchedule.Update(item, out var message); MessageBox.Show(FindForm(), message, "Cập nhật ca", MessageBoxButtons.OK, ok ? MessageBoxIcon.Information : MessageBoxIcon.Warning); if (ok) LoadSchedules(); }, "cập nhật lịch phân ca", FindForm());
    }

    private void DeleteSchedule()
    {
        if (SelectedSchedule() is not { } item) { MessageBox.Show(FindForm(), "Chọn một lịch ca cần xóa.", "Chưa chọn lịch", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
        if (MessageBox.Show(FindForm(), $"Xóa lịch ca của {item.EmployeeName} ngày {FormatDate(item.WorkDate)}?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        UIHelper.RunSafe(() => { var ok = AppServices.ShiftSchedule.Delete(item.ScheduleId, out var message); MessageBox.Show(FindForm(), message, "Xóa ca", MessageBoxButtons.OK, ok ? MessageBoxIcon.Information : MessageBoxIcon.Warning); if (ok) LoadSchedules(); }, "xóa lịch phân ca", FindForm());
    }

    private ShiftSchedule? SelectedSchedule() => _grid.SelectedRows.Count > 0 ? _grid.SelectedRows[0].Tag as ShiftSchedule : null;
    private void CopySelectedToEditor()
    {
        var item = SelectedSchedule();
        if (item == null) return;
        if (DateTime.TryParse(item.WorkDate, out var date)) _workDate.Value = date.Date;
        _employees.SelectedValue = item.EmployeeId;
        var id = int.TryParse(item.ShiftType, out var shiftId) ? shiftId : _workShifts.FirstOrDefault(s => s.HandoverNote == item.ShiftType)?.ShiftId ?? 0;
        _shifts.SelectedValue = id;
    }

    private void ExportCsv()
    {
        using var dialog = new SaveFileDialog { Filter = "CSV (*.csv)|*.csv", FileName = $"BaoCaoGiaoCa_{DateTime.Today:yyyyMMdd}.csv" };
        if (dialog.ShowDialog(FindForm()) != DialogResult.OK) return;
        UIHelper.RunSafe(() =>
        {
            var lines = new List<string> { "\"Mã lịch\",\"Ngày làm\",\"Nhân viên\",\"Ca làm\",\"Thời gian\"" };
            var byId = _workShifts.ToDictionary(s => s.ShiftId, s => s);
            foreach (var item in _schedules)
            {
                var id = int.TryParse(item.ShiftType, out var parsed) ? parsed : _workShifts.FirstOrDefault(s => s.HandoverNote == item.ShiftType)?.ShiftId ?? 0;
                var time = byId.TryGetValue(id, out var shift) ? $"{shift.StartTime:HH:mm}-{shift.EndTime:HH:mm}" : "";
                lines.Add($"\"{item.ScheduleId}\",\"{FormatDate(item.WorkDate)}\",\"{Csv(item.EmployeeName)}\",\"{Csv(item.ShiftType)}\",\"{time}\"");
            }
            File.WriteAllText(dialog.FileName, string.Join(Environment.NewLine, lines), new System.Text.UTF8Encoding(true));
            MessageBox.Show(FindForm(), "Đã xuất báo cáo giao ca.", "Xuất báo cáo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }, "xuất báo cáo giao ca", FindForm());
    }

    private static Label LabelFor(string text) => new() { Text = text, AutoSize = true, Margin = new Padding(4, 9, 5, 0), ForeColor = Color.FromArgb(71, 85, 105) };
    private static Button Button(string text, Action action, bool primary = false)
    {
        var button = new Button { Text = text, AutoSize = true, Height = 34, UseMnemonic = false, Margin = new Padding(5, 1, 3, 1), Cursor = Cursors.Hand };
        if (primary) UIHelper.DinhDangNutBamChinh(button); else UIHelper.DinhDangNutBamPhu(button);
        button.Click += (_, _) => action();
        return button;
    }
    private static string FormatDate(string value) => DateTime.TryParse(value, out var date) ? date.ToString("dd/MM/yyyy") : value;
    private static string Csv(string value) => value.Replace("\"", "\"\"");
}
