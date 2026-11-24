using BLL;
using Common.Constants;
using Common.Helpers;

namespace UI.Controls;

public class SystemLogTab : UserControl
{
    private DateTimePicker dtpFrom = null!;
    private DateTimePicker dtpTo = null!;
    private TextBox txtSearch = null!;
    private Button btnFilter = null!;
    private Button btnRefresh = null!;
    private DataGridView dgvLogs = null!;

    public SystemLogTab()
    {
        Font = new Font("Times New Roman", 10f, FontStyle.Regular);
        BackColor = Color.FromArgb(245, 247, 250);
        Padding = new Padding(16, 12, 16, 16);
        InitComponents();
        LoadLogs();
    }

    private void InitComponents()
    {
        var tblRoot = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 3,
            ColumnCount = 1,
            BackColor = Color.FromArgb(245, 247, 250),
            Margin = new Padding(0),
            Padding = new Padding(0)
        };
        tblRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 44f));
        tblRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 72f));
        tblRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

        var lblTitle = new Label
        {
            Text = "Nhật ký hoạt động hệ thống",
            Dock = DockStyle.Fill,
            Height = 44,
            Font = new Font("Times New Roman", 15f, FontStyle.Bold),
            ForeColor = AppConstants.MauChuChinh,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(0, 0, 0, 6),
            UseMnemonic = false,
            Margin = new Padding(0)
        };

        var pnlFilter = new Panel
        {
            Dock = DockStyle.Fill,
            Height = 60,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Margin = new Padding(0, 0, 0, 12)
        };

        var flowFilter = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Padding = new Padding(12, 11, 12, 6),
            Margin = new Padding(0),
            BackColor = Color.White
        };

        dtpFrom = new DateTimePicker
        {
            Width = 135,
            Height = 30,
            Format = DateTimePickerFormat.Custom,
            CustomFormat = "dd/MM/yyyy",
            Value = DateTime.Today.AddDays(-7),
            Font = new Font("Times New Roman", 10f, FontStyle.Regular),
            Margin = new Padding(0, 3, 14, 0)
        };

        dtpTo = new DateTimePicker
        {
            Width = 135,
            Height = 30,
            Format = DateTimePickerFormat.Custom,
            CustomFormat = "dd/MM/yyyy",
            Value = DateTime.Today,
            Font = new Font("Times New Roman", 10f, FontStyle.Regular),
            Margin = new Padding(0, 3, 14, 0)
        };

        txtSearch = new TextBox
        {
            Width = 170,
            Height = 30,
            Font = new Font("Times New Roman", 10f, FontStyle.Regular),
            PlaceholderText = "Tài khoản, hành độ...",
            Margin = new Padding(0, 3, 16, 0)
        };
        txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; LoadLogs(); } };

        btnFilter = CreateButton("Lọc nhật ký", 150, 36, LoadLogs, true);
        btnRefresh = CreateButton("Làm mới", 120, 36, ResetFilters);
        btnRefresh.Margin = new Padding(0);

        flowFilter.Controls.AddRange(new Control[]
        {
            CreateLabel("Từ ngày:"), dtpFrom,
            CreateLabel("Đến ngày:"), dtpTo,
            CreateLabel("Từ khóa:"), txtSearch,
            btnFilter, btnRefresh
        });

        pnlFilter.Controls.Add(flowFilter);

        dgvLogs = new DataGridView
        {
            Dock = DockStyle.Fill,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
            GridColor = Color.FromArgb(241, 245, 249),
            ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            EnableHeadersVisualStyles = false,
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
            ColumnHeadersHeight = 38,
            Font = new Font("Times New Roman", 10f, FontStyle.Regular),
            Margin = new Padding(0)
        };
        dgvLogs.RowTemplate.Height = 36;
        dgvLogs.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(244, 246, 249);
        dgvLogs.ColumnHeadersDefaultCellStyle.ForeColor = AppConstants.MauChuChinh;
        dgvLogs.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(244, 246, 249);
        dgvLogs.ColumnHeadersDefaultCellStyle.SelectionForeColor = AppConstants.MauChuChinh;
        dgvLogs.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 10f, FontStyle.Bold);
        dgvLogs.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 8, 0);

        dgvLogs.DefaultCellStyle.Font = new Font("Times New Roman", 10f, FontStyle.Regular);
        dgvLogs.DefaultCellStyle.ForeColor = AppConstants.MauChuChinh;
        dgvLogs.DefaultCellStyle.SelectionBackColor = Color.FromArgb(241, 245, 249);
        dgvLogs.DefaultCellStyle.SelectionForeColor = AppConstants.MauChuChinh;
        dgvLogs.DefaultCellStyle.Padding = new Padding(8, 0, 8, 0);

        dgvLogs.Columns.Add(new DataGridViewTextBoxColumn { Name = "MaNK", HeaderText = "Mã NK", Width = 95, AutoSizeMode = DataGridViewAutoSizeColumnMode.None });
        dgvLogs.Columns.Add(new DataGridViewTextBoxColumn { Name = "ThoiGian", HeaderText = "Thời gian", Width = 215, AutoSizeMode = DataGridViewAutoSizeColumnMode.None });
        dgvLogs.Columns.Add(new DataGridViewTextBoxColumn { Name = "TaiKhoan", HeaderText = "Tài khoản", Width = 140, AutoSizeMode = DataGridViewAutoSizeColumnMode.None });
        dgvLogs.Columns.Add(new DataGridViewTextBoxColumn { Name = "HanhDong", HeaderText = "Hành động", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
        dgvLogs.Columns.Add(new DataGridViewTextBoxColumn { Name = "BangTacDong", HeaderText = "Bảng tác động", Width = 180, AutoSizeMode = DataGridViewAutoSizeColumnMode.None });

        tblRoot.Controls.Add(lblTitle, 0, 0);
        tblRoot.Controls.Add(pnlFilter, 0, 1);
        tblRoot.Controls.Add(dgvLogs, 0, 2);
        Controls.Add(tblRoot);
    }
    
    private static Label CreateLabel(string text) => new()
    {
        Text = text,
        Font = new Font("Times New Roman", 10f, FontStyle.Regular),
        AutoSize = true,
        ForeColor = Color.FromArgb(71, 85, 105),
        Margin = new Padding(0, 8, 6, 0),
        UseMnemonic = false
    };
    
    private static Button CreateButton(string text, int width, int height, Action onClick, bool isPrimary = false)
    {
        var btn = new Button
        {
            Text = text,
            Size = new Size(width, height),
            MinimumSize = new Size(width, height),
            UseMnemonic = false,
            Cursor = Cursors.Hand,
            Margin = new Padding(0, 0, 10, 0),
            Padding = new Padding(10, 0, 10, 0),
            Font = new Font("Times New Roman", 10f, isPrimary ? FontStyle.Bold : FontStyle.Regular)
        };
        if (isPrimary) UIHelper.DinhDangNutBamChinh(btn);
        else UIHelper.DinhDangNutBamPhu(btn);
        btn.Click += (s, e) => onClick();
        return btn;
    }
    
    public void LoadLogs()
    {
        UIHelper.RunSafe(() =>
        {
            var keyword = txtSearch?.Text.Trim() ?? string.Empty;
            var fromDate = dtpFrom?.Value.Date;
            var toDate = dtpTo?.Value.Date;

            if (fromDate.HasValue && toDate.HasValue && fromDate.Value > toDate.Value)
            {
                MessageBox.Show(FindForm(), "Từ ngày không được lớn hơn Đến ngày!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var logs = AppServices.SystemLog.GetLogs(keyword, null, fromDate, toDate);
            dgvLogs.Rows.Clear();
            foreach (var log in logs)
            {
                dgvLogs.Rows.Add(
                    $"NK{log.LogId:D3}",
                    log.Timestamp,
                    log.Username,
                    log.Action,
                    log.AffectedTable);
            }
        }, "tải nhật ký hệ thống", this);
    }
    
    private void ResetFilters()
    {
        txtSearch.Text = string.Empty;
        dtpFrom.Value = DateTime.Today.AddDays(-7);
        dtpTo.Value = DateTime.Today;
        LoadLogs();
    }
}
