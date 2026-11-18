
using Common.Constants;
using Common.Helpers;
using UI.Controls;

namespace UI.Forms;
public partial class ReceptionistMainForm
    {
        private StatCardControl cardTQPhongTrong = null!;
        private StatCardControl cardTQCongSuat = null!;
        private StatCardControl cardTQDoanhThu = null!;
        private StatCardControl cardTQDatMoi = null!;
        private SimpleBarChartControl bieuDo7Ngay = null!;
        private DataGridView dgvViecCanLam = null!;

        private void KhoiTaoTabTongQuan()
        {
            var tblRootTongQuan = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1,
                Margin = new Padding(0)
            };
            tblRootTongQuan.RowStyles.Add(new RowStyle(SizeType.Absolute, 95f));
            tblRootTongQuan.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            // 4 thẻ thống kê trên cùng (mỗi thẻ 25%)
            var tblCards = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                Margin = new Padding(0, 0, 0, 8)
            };
            tblCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            tblCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            tblCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            tblCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));

            cardTQPhongTrong = new StatCardControl { Title = "Phòng trống", TopAccentColor = Color.FromArgb(16, 185, 129), Dock = DockStyle.Fill };
            cardTQCongSuat = new StatCardControl { Title = "Công suất phòng", TopAccentColor = Color.FromArgb(245, 158, 11), Dock = DockStyle.Fill };
            cardTQDoanhThu = new StatCardControl { Title = "Doanh thu hôm nay", TopAccentColor = Color.FromArgb(59, 130, 246), Dock = DockStyle.Fill };
            cardTQDatMoi = new StatCardControl { Title = "Đặt mới hôm nay", TopAccentColor = Color.FromArgb(239, 68, 68), Dock = DockStyle.Fill };

            tblCards.Controls.Add(cardTQPhongTrong, 0, 0);
            tblCards.Controls.Add(cardTQCongSuat, 1, 0);
            tblCards.Controls.Add(cardTQDoanhThu, 2, 0);
            tblCards.Controls.Add(cardTQDatMoi, 3, 0);
            tblRootTongQuan.Controls.Add(tblCards, 0, 0);

            // Bố cục dưới chia đôi (50% - 50%)
            var tblBody = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = new Padding(0)
            };
            tblBody.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            tblBody.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));

            // Cột 1: Biểu đồ công suất 7 ngày
            var pnlLeftChart = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(18, 16, 18, 16),
                Margin = new Padding(0, 0, 8, 0),
                BorderStyle = BorderStyle.FixedSingle
            };
            var tblChartLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1,
                Margin = new Padding(0)
            };
            tblChartLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35f));
            tblChartLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            var lblChartTitle = new Label
            {
                Text = "Công suất 7 ngày gần nhất",
                Font = new Font(AppConstants.FontChuChinh, 13f, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            bieuDo7Ngay = new SimpleBarChartControl { Dock = DockStyle.Fill };
            tblChartLayout.Controls.Add(lblChartTitle, 0, 0);
            tblChartLayout.Controls.Add(bieuDo7Ngay, 0, 1);
            pnlLeftChart.Controls.Add(tblChartLayout);
            tblBody.Controls.Add(pnlLeftChart, 0, 0);

            // Cột 2: Việc cần làm hôm nay
            var pnlRightTasks = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(18, 16, 18, 16),
                Margin = new Padding(8, 0, 0, 0),
                BorderStyle = BorderStyle.FixedSingle
            };
            var tblTasksLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1,
                Margin = new Padding(0)
            };
            tblTasksLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35f));
            tblTasksLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            var lblTaskTitle = new Label
            {
                Text = "Việc cần làm hôm nay",
                Font = new Font(AppConstants.FontChuChinh, 13f, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            dgvViecCanLam = new DataGridView { Dock = DockStyle.Fill };
            UIHelper.DinhDangBangDuLieu(dgvViecCanLam);
            dgvViecCanLam.Columns.Add("Phong", "Phòng");
            dgvViecCanLam.Columns.Add("Khach", "Khách");
            dgvViecCanLam.Columns.Add("Viec", "Việc");
            dgvViecCanLam.Columns["Phong"].MinimumWidth = 75;
            dgvViecCanLam.Columns["Phong"].FillWeight = 45;
            dgvViecCanLam.Columns["Khach"].MinimumWidth = 150;
            dgvViecCanLam.Columns["Khach"].FillWeight = 120;
            dgvViecCanLam.Columns["Viec"].MinimumWidth = 115;
            dgvViecCanLam.Columns["Viec"].FillWeight = 75;

            dgvViecCanLam.CellPainting += (s, e) =>
            {
                if (e.RowIndex >= 0 && e.ColumnIndex == dgvViecCanLam.Columns["Viec"].Index && e.Value != null)
                {
                    string v = e.Value.ToString() ?? "";
                    if (v == "Check-in")
                        UIHelper.VeBadge(e, v, Color.FromArgb(219, 234, 254), Color.FromArgb(29, 78, 216));
                    else if (v == "Check-out")
                        UIHelper.VeBadge(e, v, Color.FromArgb(254, 243, 199), Color.FromArgb(180, 83, 9));
                    else if (v == "Dọn phòng")
                        UIHelper.VeBadge(e, v, Color.FromArgb(220, 252, 231), Color.FromArgb(4, 120, 87));
                }
            };

            tblTasksLayout.Controls.Add(lblTaskTitle, 0, 0);
            tblTasksLayout.Controls.Add(dgvViecCanLam, 0, 1);
            pnlRightTasks.Controls.Add(tblTasksLayout);
            tblBody.Controls.Add(pnlRightTasks, 1, 0);

            tblRootTongQuan.Controls.Add(tblBody, 0, 1);
            pnlTongQuan.Controls.Add(tblRootTongQuan);
        }

        private void TaiDuLieuTongQuan()
        {
            UIHelper.RunSafe(() =>
            {
                var tk = _baoCaoBLL.GetDashboardStats();
                cardTQPhongTrong.Value = tk.AvailableRooms.ToString();
                cardTQCongSuat.Value = $"{tk.OccupancyRate:N0}%";
                cardTQDoanhThu.Value = FormatHelper.FormatCurrency(tk.TodayRevenue);
                cardTQDatMoi.Value = tk.TodayNewBookings.ToString();

                var dict7 = _baoCaoBLL.Get7DayOccupancyRate();
                bieuDo7Ngay.UpdateData(dict7);
                bieuDo7Ngay.Invalidate();
                double congSuat = tk.OccupancyRate > 0
                    ? tk.OccupancyRate
                    : (tk.TotalRooms > 0 ? tk.OccupiedRooms * 100.0 / tk.TotalRooms : 0);
                dgvViecCanLam.Rows.Clear();
                var dsViec = _baoCaoBLL.GetTodayTodoList();
                foreach (var v in dsViec)
                {
                    dgvViecCanLam.Rows.Add(v.RoomNumber, v.CustomerName, v.TaskDescription);
                }
            }, "tải dữ liệu tổng quan", this);
        }
        
    }
