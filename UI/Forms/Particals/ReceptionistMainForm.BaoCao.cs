using System.Text;
using Common.Constants;
using Common.Helpers;
using Entity.DTOs;
using UI.Controls;

namespace UI.Forms;

public partial class ReceptionistMainForm
{
    private DateTimePicker dtpTuBaoCao = null!;
    private DateTimePicker dtpDenBaoCao = null!;
    private ComboBox cboLoaiBaoCao = null!;
    private Label lblChartTitleBC = null!;
    private SimpleBarChartControl bieuDoThang = null!;
    private DataGridView dgvChiTietBaoCao = null!;
    private StatCardControl cardBCTongDoanhThu = null!;
    private StatCardControl cardBCCongSuat = null!;
    private StatCardControl cardBCSoKhach = null!;
    private List<RevenueReportDto> _dsBaoCaoHienTai = new();
    private bool _dangCapNhatBoLoc;

    // Khởi tạo giao diện tab Báo cáo
    private void KhoiTaoTabBaoCao()
    {
        var fontChuan = new Font(AppConstants.FontChuChinh, 10f);

        pnlBaoCao.BackColor = Color.FromArgb(243, 244, 246);
        pnlBaoCao.Font = fontChuan;

        var tblRootBaoCao = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 1,
            Margin = new Padding(0)
        };
        tblRootBaoCao.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        tblRootBaoCao.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

        // Thanh bộ lọc trên cùng
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

        var flowFilterBC = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            Padding = new Padding(0),
            Margin = new Padding(0)
        };

        var lblLoai = new Label
        {
            Text = "Loại báo cáo:",
            Font = fontChuan,
            AutoSize = true,
            Margin = new Padding(0, 8, 6, 6),
            ForeColor = Color.FromArgb(71, 85, 105),
            UseMnemonic = false
        };
        cboLoaiBaoCao = new ComboBox
        {
            Width = 225,
            Height = 32,
            Font = fontChuan,
            Margin = new Padding(0, 3, 12, 6),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cboLoaiBaoCao.Items.AddRange(new object[]
        {
            "Doanh thu & Giao ca theo ngày",
            "Doanh thu theo tháng",
            "Công suất phòng theo tháng"
        });
        cboLoaiBaoCao.SelectedIndex = 0;

        var lblTu = new Label
        {
            Text = "Từ ngày:",
            Font = fontChuan,
            AutoSize = true,
            Margin = new Padding(0, 8, 6, 6),
            ForeColor = Color.FromArgb(71, 85, 105),
            UseMnemonic = false
        };
        dtpTuBaoCao = new DateTimePicker
        {
            Width = 135,
            Height = 32,
            Font = fontChuan,
            Margin = new Padding(0, 3, 12, 6),
            Format = DateTimePickerFormat.Custom,
            CustomFormat = "dd/MM/yyyy",
            Value = DateTime.Today.AddDays(-6)
        };
        dtpTuBaoCao.ValueChanged += (s, e) =>
        {
            if (!_dangCapNhatBoLoc) TaiDuLieuBaoCao();
        };

        var lblDen = new Label
        {
            Text = "Đến ngày:",
            Font = fontChuan,
            AutoSize = true,
            Margin = new Padding(0, 8, 6, 6),
            ForeColor = Color.FromArgb(71, 85, 105),
            UseMnemonic = false
        };
        dtpDenBaoCao = new DateTimePicker
        {
            Width = 135,
            Height = 32,
            Font = fontChuan,
            Margin = new Padding(0, 3, 12, 6),
            Format = DateTimePickerFormat.Custom,
            CustomFormat = "dd/MM/yyyy",
            Value = DateTime.Today
        };
        dtpDenBaoCao.ValueChanged += (s, e) =>
        {
            if (!_dangCapNhatBoLoc) TaiDuLieuBaoCao();
        };

        cboLoaiBaoCao.SelectedIndexChanged += (s, e) =>
        {
            _dangCapNhatBoLoc = true;
            try
            {
                if (cboLoaiBaoCao.SelectedIndex == 0)
                {
                    dtpTuBaoCao.Value = DateTime.Today.AddDays(-6);
                    dtpDenBaoCao.Value = DateTime.Today;
                }
                else
                {
                    dtpTuBaoCao.Value = new DateTime(DateTime.Today.Year, 1, 1);
                    dtpDenBaoCao.Value = DateTime.Today;
                }
            }
            finally
            {
                _dangCapNhatBoLoc = false;
            }

            TaiDuLieuBaoCao();
        };

        var btnHomNay = new Button
        {
            Text = "Hôm nay",
            Width = 92,
            Height = 34,
            UseMnemonic = false,
            Margin = new Padding(0, 1, 8, 6)
        };
        UIHelper.DinhDangNutBamPhu(btnHomNay);
        btnHomNay.Click += (s, e) =>
        {
            _dangCapNhatBoLoc = true;
            try
            {
                cboLoaiBaoCao.SelectedIndex = 0;
                dtpTuBaoCao.Value = DateTime.Today;
                dtpDenBaoCao.Value = DateTime.Today;
            }
            finally
            {
                _dangCapNhatBoLoc = false;
            }

            TaiDuLieuBaoCao();
        };

        var btnXem = new Button
        {
            Text = "Xem",
            Width = 82,
            Height = 34,
            UseMnemonic = false,
            Margin = new Padding(0, 1, 8, 6)
        };
        UIHelper.DinhDangNutBamChinh(btnXem);
        btnXem.Click += (s, e) => TaiDuLieuBaoCao();

        var btnXuatExcel = new Button
        {
            Text = "Xuất Excel",
            Width = 115,
            Height = 34,
            UseMnemonic = false,
            Margin = new Padding(0, 1, 0, 6)
        };
        UIHelper.DinhDangNutBamPhu(btnXuatExcel);
        btnXuatExcel.Click += (s, e) => XuLyXuatExcel();

        flowFilterBC.Controls.AddRange(new Control[]
        {
            lblLoai, cboLoaiBaoCao, lblTu, dtpTuBaoCao, lblDen, dtpDenBaoCao, btnHomNay, btnXem, btnXuatExcel
        });
        pnlTop.Controls.Add(flowFilterBC);
        tblRootBaoCao.Controls.Add(pnlTop, 0, 0);

        // Khu vực nội dung chính
        var tbl = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = new Padding(0)
        };
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65f));
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35f));

        // Cột trái: Biểu đồ + Bảng chi tiết
        var pnlLeftChart = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(18, 14, 18, 14),
            Margin = new Padding(0, 0, 8, 0),
            BorderStyle = BorderStyle.FixedSingle
        };
        var tblLeftChartLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 3,
            ColumnCount = 1,
            Margin = new Padding(0)
        };
        tblLeftChartLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34f));
        tblLeftChartLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 55f));
        tblLeftChartLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 45f));

        lblChartTitleBC = new Label
        {
            Text = "Doanh thu & Giao ca theo ngày",
            Font = new Font(AppConstants.FontChuChinh, 13f, FontStyle.Bold),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            UseMnemonic = false
        };
        bieuDoThang = new SimpleBarChartControl { Dock = DockStyle.Fill };

        dgvChiTietBaoCao = new DataGridView
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 10, 0, 0)
        };
        UIHelper.DinhDangBangDuLieu(dgvChiTietBaoCao);
        dgvChiTietBaoCao.Columns.Add("KyBaoCao", "Ngày / Tháng");
        dgvChiTietBaoCao.Columns.Add("DoanhThu", "Doanh thu");
        dgvChiTietBaoCao.Columns.Add("ChiSoPhu", "Số hóa đơn / Công suất");
        dgvChiTietBaoCao.Columns["KyBaoCao"].FillWeight = 30;
        dgvChiTietBaoCao.Columns["DoanhThu"].FillWeight = 40;
        dgvChiTietBaoCao.Columns["ChiSoPhu"].FillWeight = 30;

        tblLeftChartLayout.Controls.Add(lblChartTitleBC, 0, 0);
        tblLeftChartLayout.Controls.Add(bieuDoThang, 0, 1);
        tblLeftChartLayout.Controls.Add(dgvChiTietBaoCao, 0, 2);
        pnlLeftChart.Controls.Add(tblLeftChartLayout);
        tbl.Controls.Add(pnlLeftChart, 0, 0);

        // Cột phải: 3 thẻ tổng hợp số liệu
        var pnlRightSummary = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(18, 14, 18, 14),
            Margin = new Padding(8, 0, 0, 0),
            BorderStyle = BorderStyle.FixedSingle,
            AutoScroll = true
        };

        var tblRightLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 5,
            Margin = new Padding(0)
        };
        tblRightLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36f));
        tblRightLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 106f));
        tblRightLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 106f));
        tblRightLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 106f));
        tblRightLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

        var lblSumTitle = new Label
        {
            Text = "Tổng hợp số liệu",
            Font = new Font(AppConstants.FontChuChinh, 13f, FontStyle.Bold),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            UseMnemonic = false
        };

        cardBCTongDoanhThu = new StatCardControl
        {
            Title = "Tổng doanh thu",
            Value = "0 đ",
            TopAccentColor = Color.FromArgb(59, 130, 246),
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 0, 0, 8)
        };

        cardBCCongSuat = new StatCardControl
        {
            Title = "Công suất phòng TB",
            Value = "0%",
            TopAccentColor = Color.FromArgb(245, 158, 11),
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 0, 0, 8)
        };

        cardBCSoKhach = new StatCardControl
        {
            Title = "Tổng hóa đơn đã thu",
            Value = "0",
            TopAccentColor = Color.FromArgb(16, 185, 129),
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 0, 0, 8)
        };

        tblRightLayout.Controls.Add(lblSumTitle, 0, 0);
        tblRightLayout.Controls.Add(cardBCTongDoanhThu, 0, 1);
        tblRightLayout.Controls.Add(cardBCCongSuat, 0, 2);
        tblRightLayout.Controls.Add(cardBCSoKhach, 0, 3);
        pnlRightSummary.Controls.Add(tblRightLayout);
        tbl.Controls.Add(pnlRightSummary, 1, 0);

        tblRootBaoCao.Controls.Add(tbl, 0, 1);
        pnlBaoCao.Controls.Add(tblRootBaoCao);

        pnlBaoCao.VisibleChanged += (s, e) =>
        {
            if (pnlBaoCao.Visible)
                TaiDuLieuBaoCao();
        };
    }

    // Tải dữ liệu biểu đồ và thống kê báo cáo theo khoảng thời gian
    public void TaiDuLieuBaoCao()
    {
        var tuNgay = dtpTuBaoCao != null ? dtpTuBaoCao.Value.Date : DateTime.Today.AddDays(-6);
        var denNgay = dtpDenBaoCao != null ? dtpDenBaoCao.Value.Date : DateTime.Today;

        if (tuNgay > denNgay)
        {
            MessageBox.Show(this, "Từ ngày không được lớn hơn Đến ngày!", "Cảnh báo",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        UIHelper.RunSafe(() =>
        {
            int cheDo = cboLoaiBaoCao?.SelectedIndex ?? 0;
            bool laQuanLy = SessionManager.CurrentUser?.RoleId == AppConstants.MaVTQuanLy
                            || SessionManager.CurrentUser?.RoleId == AppConstants.MaVTAdmin;
            string phamVi = laQuanLy
                ? "Toàn bộ nhân viên"
                : $"Nhân viên: {SessionManager.DisplayName}";
            var duLieuVe = new List<KeyValuePair<string, double>>();
            double tongDoanhThu = 0;
            double tongCongSuat = 0;
            var tongLuot = 0;

            dgvChiTietBaoCao.Rows.Clear();

            if (cheDo == 0)
            {
                // Chế độ 1: Doanh thu & Giao ca theo từng ngày
                lblChartTitleBC.Text = $"Doanh thu theo ngày ({tuNgay:dd/MM/yyyy} - {denNgay:dd/MM/yyyy}) - {phamVi}";
                dgvChiTietBaoCao.Columns["KyBaoCao"].HeaderText = "Ngày";
                dgvChiTietBaoCao.Columns["DoanhThu"].HeaderText = "Doanh thu ngày";
                dgvChiTietBaoCao.Columns["ChiSoPhu"].HeaderText = "Số hóa đơn thu";
                cardBCSoKhach.Title = "Tổng hóa đơn đã thu";

                _dsBaoCaoHienTai = _baoCaoBLL.GetDailyRevenue(tuNgay, denNgay);
                var statsHienTai = _baoCaoBLL.GetDashboardStats();

                foreach (var item in _dsBaoCaoHienTai)
                {
                    duLieuVe.Add(new KeyValuePair<string, double>(item.Period, item.Revenue));
                    tongDoanhThu += item.Revenue;
                    tongLuot += item.GuestCount;

                    dgvChiTietBaoCao.Rows.Add(
                        item.Period,
                        FormatHelper.FormatCurrency(item.Revenue),
                        $"{item.GuestCount} hóa đơn"
                    );
                }

                cardBCCongSuat.Title = "Công suất phòng hiện tại";
                cardBCCongSuat.Value = $"{statsHienTai.OccupancyRate:N1}%";
            }
            else
            {
                // Chế độ 2 & 3: Doanh thu hoặc Công suất phòng theo tháng
                var dsGoc = _baoCaoBLL.GetMonthlyRevenue(denNgay.Year);
                var thangBatDau = (tuNgay.Year == denNgay.Year) ? tuNgay.Month : 1;
                var thangKetThuc = denNgay.Month;

                _dsBaoCaoHienTai = dsGoc.Where(item =>
                {
                    if (item.Period.StartsWith("T", StringComparison.OrdinalIgnoreCase)
                        && int.TryParse(item.Period.Substring(1), out var m))
                    {
                        return m >= thangBatDau && m <= thangKetThuc;
                    }

                    return true;
                }).ToList();
                
                bool laBieuDoCongSuat = (cheDo == 2);
                lblChartTitleBC.Text = laBieuDoCongSuat
                    ? $"Công suất phòng theo tháng (Năm {denNgay.Year}) - {phamVi}"
                    : $"Doanh thu theo tháng (Năm {denNgay.Year}) - {phamVi}";

                dgvChiTietBaoCao.Columns["KyBaoCao"].HeaderText = "Tháng";
                dgvChiTietBaoCao.Columns["DoanhThu"].HeaderText = "Doanh thu";
                dgvChiTietBaoCao.Columns["ChiSoPhu"].HeaderText = "Công suất / Lượt khách";
                cardBCCongSuat.Title = "Công suất phòng TB";
                cardBCSoKhach.Title = "Số lượt khách lưu trú";

                foreach (var item in _dsBaoCaoHienTai)
                {
                    duLieuVe.Add(new KeyValuePair<string, double>(
                        item.Period,
                        laBieuDoCongSuat ? item.OccupancyRate : item.Revenue));

                    tongDoanhThu += item.Revenue;
                    tongCongSuat += item.OccupancyRate;
                    tongLuot += item.GuestCount;

                    dgvChiTietBaoCao.Rows.Add(
                        item.Period,
                        FormatHelper.FormatCurrency(item.Revenue),
                        $"{item.OccupancyRate:N1}% ({item.GuestCount} khách)"
                    );
                }

                var congSuatTB = _dsBaoCaoHienTai.Count > 0 ? tongCongSuat / _dsBaoCaoHienTai.Count : 0;
                cardBCCongSuat.Value = $"{congSuatTB:N1}%";
            }

            bieuDoThang.UpdateData(duLieuVe);
            bieuDoThang.Invalidate();

            cardBCTongDoanhThu.Value = FormatHelper.FormatCurrency(tongDoanhThu);
            cardBCSoKhach.Value = $"{tongLuot:N0}";
            cardBCTongDoanhThu.Invalidate();
            cardBCCongSuat.Invalidate();
            cardBCSoKhach.Invalidate();
        }, "tải báo cáo doanh thu", this);
    }

    // Xuất báo cáo doanh thu ra file Excel (UTF-8 BOM không lỗi font tiếng Việt)
    private void XuLyXuatExcel()
    {
        if (_dsBaoCaoHienTai == null || _dsBaoCaoHienTai.Count == 0)
            TaiDuLieuBaoCao();

        if (_dsBaoCaoHienTai == null || _dsBaoCaoHienTai.Count == 0)
        {
            MessageBox.Show(this, "Không có dữ liệu báo cáo trong khoảng thời gian đã chọn để xuất file!",
                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        using var sfd = new SaveFileDialog
        {
            Filter = "File Excel CSV (*.csv)|*.csv",
            FileName = $"BaoCaoDoanhThu_{DateTime.Today:yyyyMMdd}.csv"
        };

        if (sfd.ShowDialog() == DialogResult.OK)
        {
            UIHelper.RunSafe(() =>
            {
                bool theoNgay = (cboLoaiBaoCao?.SelectedIndex ?? 0) == 0;
                var sb = new StringBuilder();
                sb.AppendLine(theoNgay
                    ? "\"Ngày\",\"Doanh thu (VNĐ)\",\"Số hóa đơn đã thu\""
                    : "\"Tháng\",\"Doanh thu (VNĐ)\",\"Công suất phòng (%)\",\"Số lượt khách\"");

                double tongDoanhThu = 0;
                int tongLuot = 0;

                foreach (var item in _dsBaoCaoHienTai)
                {
                    tongDoanhThu += item.Revenue;
                    tongLuot += item.GuestCount;

                    if (theoNgay)
                        sb.AppendLine($"\"{item.Period}\",\"{item.Revenue:N0}\",\"{item.GuestCount}\"");
                    else
                        sb.AppendLine(
                            $"\"{item.Period}\",\"{item.Revenue:N0}\",\"{item.OccupancyRate:N1}%\",\"{item.GuestCount}\"");
                }

                sb.AppendLine(theoNgay
                    ? $"\"Tổng cộng\",\"{tongDoanhThu:N0}\",\"{tongLuot}\""
                    : $"\"Tổng cộng\",\"{tongDoanhThu:N0}\",\"\",\"{tongLuot}\"");

                File.WriteAllText(sfd.FileName, sb.ToString(), new UTF8Encoding(true));
                MessageBox.Show(this, "Xuất file báo cáo thành công!", "Xuất file",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }, "xuất báo cáo doanh thu ra CSV", this);
        }
    }
}