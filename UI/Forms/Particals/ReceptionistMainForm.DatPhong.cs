using Common.Constants;
using Common.Helpers;
using Entity.Entities;

namespace UI.Forms;

public partial class ReceptionistMainForm
{
        private TextBox txtTimDatPhong = null!;
        private ComboBox cboLocTrangThaiDP = null!;
        private DateTimePicker dtpLocNgayDP = null!;
        private DataGridView dgvDatPhong = null!;

        private void KhoiTaoTabDatPhong()
        {
            var tblRootDatPhong = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1,
                Margin = new Padding(0)
            };
            tblRootDatPhong.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tblRootDatPhong.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

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

            var flowFilter = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };

            var lblTim = new Label { Text = "Tìm kiếm:", AutoSize = true, Margin = new Padding(0, 8, 6, 6), ForeColor = Color.FromArgb(71, 85, 105) };
            txtTimDatPhong = new TextBox { Width = 135, Height = 30, Margin = new Padding(0, 3, 12, 6), PlaceholderText = "Mã hoặc tên khách..." };
            txtTimDatPhong.TextChanged += (s, e) => TaiDuLieuDatPhong();

            var lblTT = new Label { Text = "Trạng thái:", AutoSize = true, Margin = new Padding(0, 8, 6, 6), ForeColor = Color.FromArgb(71, 85, 105) };
            cboLocTrangThaiDP = new ComboBox { Width = 115, Height = 30, Margin = new Padding(0, 3, 12, 6), DropDownStyle = ComboBoxStyle.DropDownList };
            cboLocTrangThaiDP.Items.AddRange(new object[] { "Tất cả", "Chờ duyệt", "Đã xác nhận", "Đang ở", "Đã hoàn tất", "Đã hủy" });
            cboLocTrangThaiDP.SelectedIndex = 0;
            cboLocTrangThaiDP.SelectedIndexChanged += (s, e) => TaiDuLieuDatPhong();

            var lblTuNgay = new Label { Text = "Từ ngày:", AutoSize = true, Margin = new Padding(0, 8, 6, 6), ForeColor = Color.FromArgb(71, 85, 105) };
            dtpLocNgayDP = new DateTimePicker { Width = 140, Height = 30, Margin = new Padding(0, 3, 14, 6), Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy" };

            var btnThemDP = new Button
            {
                Text = "+ Thêm đặt phòng",
                Width = 185,
                Height = 34,
                UseMnemonic = false,
                Margin = new Padding(0, 1, 0, 6)
            };
            UIHelper.DinhDangNutBamChinh(btnThemDP);
            btnThemDP.Click += (s, e) =>
            {
                using var frm = new AddBookingForm();
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    TaiDuLieuDatPhong();
                }
            };

            flowFilter.Controls.AddRange(new Control[] { lblTim, txtTimDatPhong, lblTT, cboLocTrangThaiDP, lblTuNgay, dtpLocNgayDP, btnThemDP });
            pnlTop.Controls.Add(flowFilter);

            tblRootDatPhong.Controls.Add(pnlTop, 0, 0);

            var pnlGridContainer = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0)
            };

            dgvDatPhong = new DataGridView { Dock = DockStyle.Fill };
            UIHelper.DinhDangBangDuLieu(dgvDatPhong);

            dgvDatPhong.Columns.Add("Ma", "Mã");
            dgvDatPhong.Columns.Add("Khach", "Khách");
            dgvDatPhong.Columns.Add("Phong", "Phòng");
            dgvDatPhong.Columns.Add("Nhan", "Nhận");
            dgvDatPhong.Columns.Add("Tra", "Trả");
            dgvDatPhong.Columns.Add("TrangThai", "Trạng thái");

            dgvDatPhong.Columns["Ma"].MinimumWidth = 85;
            dgvDatPhong.Columns["Ma"].FillWeight = 55;

            dgvDatPhong.Columns["Khach"].MinimumWidth = 140;
            dgvDatPhong.Columns["Khach"].FillWeight = 110;

            dgvDatPhong.Columns["Phong"].MinimumWidth = 75;
            dgvDatPhong.Columns["Phong"].FillWeight = 50;

            dgvDatPhong.Columns["Nhan"].MinimumWidth = 120;
            dgvDatPhong.Columns["Nhan"].FillWeight = 90;

            dgvDatPhong.Columns["Tra"].MinimumWidth = 120;
            dgvDatPhong.Columns["Tra"].FillWeight = 90;

            dgvDatPhong.Columns["TrangThai"].MinimumWidth = 135;
            dgvDatPhong.Columns["TrangThai"].FillWeight = 95;

            var colHuy = new DataGridViewButtonColumn
            {
                Name = "Huy",
                HeaderText = "Thao tác",
                Text = "Hủy",
                UseColumnTextForButtonValue = true,
                MinimumWidth = 95,
                FillWeight = 65
            };
            dgvDatPhong.Columns.Add(colHuy);
            dgvDatPhong.CellClick += DgvDatPhong_CellClick;

            dgvDatPhong.CellPainting += (s, e) =>
            {
                if (e.RowIndex >= 0 && e.ColumnIndex == dgvDatPhong.Columns["TrangThai"].Index && e.Value != null)
                {
                    string tt = e.Value.ToString() ?? "";
                    if (tt == AppConstants.DPDaXacNhan || tt == AppConstants.DPDaHoanTat)
                        UIHelper.VeBadge(e, tt, Color.FromArgb(220, 252, 231), Color.FromArgb(21, 128, 61));
                    else if (tt == AppConstants.DPChoDuyet)
                        UIHelper.VeBadge(e, tt, Color.FromArgb(254, 243, 199), Color.FromArgb(180, 83, 9));
                    else if (tt == AppConstants.DPDaHuy)
                        UIHelper.VeBadge(e, tt, Color.FromArgb(254, 226, 226), Color.FromArgb(185, 28, 28));
                    else
                        UIHelper.VeBadge(e, tt, Color.FromArgb(224, 242, 254), Color.FromArgb(3, 105, 161));
                }
            };

            pnlGridContainer.Controls.Add(dgvDatPhong);
            tblRootDatPhong.Controls.Add(pnlGridContainer, 0, 1);
            pnlDatPhong.Controls.Add(tblRootDatPhong);
        }

        private void TaiDuLieuDatPhong()
        {
            UIHelper.RunSafe(() =>
            {
                dgvDatPhong.Rows.Clear();
                string tuKhoa = txtTimDatPhong.Text.Trim();
                string trangThai = cboLocTrangThaiDP.SelectedItem?.ToString() ?? "Tất cả";

                var ds = _datPhongBLL.Search(tuKhoa, trangThai, "");
                foreach (var dp in ds)
                {
                    string nhanStr = FormatHelper.FormatDate(FormatHelper.ParseDate(dp.CheckInDate));
                    string traStr = FormatHelper.FormatDate(FormatHelper.ParseDate(dp.CheckOutDate));
                    int index = dgvDatPhong.Rows.Add(dp.DisplayCode, dp.CustomerName, dp.RoomNumber, nhanStr, traStr, dp.Status);
                    dgvDatPhong.Rows[index].Tag = dp;
                }
            }, "tải danh sách đặt phòng", this);
        }

        private void DgvDatPhong_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvDatPhong.Columns["Huy"] != null && e.ColumnIndex == dgvDatPhong.Columns["Huy"].Index)
            {
                if (dgvDatPhong.Rows[e.RowIndex].Tag is Booking dp)
                {
                    if (MessageBox.Show($"Bạn có chắc chắn muốn hủy đơn đặt phòng {dp.DisplayCode} của khách {dp.CustomerName}?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        UIHelper.RunSafe(() =>
                        {
                            bool thanhCong = _datPhongBLL.CancelBooking(dp.BookingId, out string msg);
                            MessageBox.Show(this, msg, "Thông báo", MessageBoxButtons.OK, thanhCong ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
                            TaiDuLieuDatPhong();
                        }, "hủy đơn đặt phòng", this);
                    }
                }
            }
        }
    
}