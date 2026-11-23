using Common.Constants;
using Common.Helpers;
using Entity.Entities;
using Entity.Enums;

namespace UI.Forms;

public partial class ReceptionistMainForm
{
    private DataGridView dgvChoNhanPhong = null!;
    private TextBox txtCheckInCCCD = null!;
    private NumericUpDown numCheckInKhachDiCung = null!;
    private Button btnXacNhanCheckIn = null!;
    private int _maDPDangChonCheckIn = 0;

    private ComboBox cboChonPhongCheckOut = null!;
    private Label lblThongTinKhachCheckOut = null!;
    private DataGridView dgvCheckOutChiTiet = null!;
    private ComboBox cboHinhThucTT = null!;
    private Button btnThanhToanVaCheckOut = null!;
    private int _maDPDangChonCheckOut = 0;
    private bool _dangNapComboCheckOut = false;
    
    private sealed class CheckOutRoomItem
    {
        public int BookingId { get; init; }
        public string DisplayText { get; init; } = string.Empty;
        public override string ToString() => DisplayText;
    }

    private void KhoiTaoTabCheckInOut()
    {
        var tbl = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1
        };
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
        var pnlLeftCheckIn = new Panel
        {
            Dock = DockStyle.Fill,
            Width = 400,
            AutoScroll = true,
            BackColor = Color.White,
            Padding = new Padding(18, 16, 18, 16),
            Margin = new Padding(0, 0, 8, 0),
            BorderStyle = BorderStyle.FixedSingle
        };

        var lblTieuDeIn = new Label
        {
            Text = "Check-in: danh sách chờ nhận phòng",
            Font = new Font(AppConstants.FontChuChinh, 13f, FontStyle.Bold),
            Dock = DockStyle.Top,
            Height = 35
        };
        pnlLeftCheckIn.Controls.Add(lblTieuDeIn);

        dgvChoNhanPhong = new DataGridView { Dock = DockStyle.Top, Height = 220 };
        UIHelper.DinhDangBangDuLieu(dgvChoNhanPhong);
        dgvChoNhanPhong.Columns.Add("Ma", "Mã");
        dgvChoNhanPhong.Columns.Add("Khach", "Khách");
        dgvChoNhanPhong.Columns.Add("Phong", "Phòng");
        dgvChoNhanPhong.Columns["Ma"].MinimumWidth = 80;
        dgvChoNhanPhong.Columns["Ma"].FillWeight = 24f;
        dgvChoNhanPhong.Columns["Khach"].MinimumWidth = 135;
        dgvChoNhanPhong.Columns["Khach"].FillWeight = 52f;
        dgvChoNhanPhong.Columns["Phong"].MinimumWidth = 75;
        dgvChoNhanPhong.Columns["Phong"].FillWeight = 24f;
        dgvChoNhanPhong.SelectionChanged += DgvChoNhanPhong_SelectionChanged;
        pnlLeftCheckIn.Controls.Add(dgvChoNhanPhong);
        lblTieuDeIn.SendToBack();

        int yIn = 286;
        var lblCCCD = new Label { Text = "Số CCCD / Hộ chiếu", Location = new Point(18, yIn), AutoSize = true, ForeColor = Color.FromArgb(71, 85, 105) };
        txtCheckInCCCD = new TextBox { Location = new Point(18, yIn + 24), Width = 362, Height = 30, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };

        var lblKDC = new Label { Text = "Số khách đi cùng", Location = new Point(18, yIn + 68), AutoSize = true, ForeColor = Color.FromArgb(71, 85, 105) };
        numCheckInKhachDiCung = new NumericUpDown { Location = new Point(18, yIn + 92), Width = 120, Height = 30, Minimum = 0, Maximum = 10, Value = 0 };

        btnXacNhanCheckIn = new Button
        {
            Text = "Xác nhận check-in",
            Location = new Point(18, yIn + 140),
            Width = 200,
            Height = 40,
            UseMnemonic = false
        };
        UIHelper.DinhDangNutBamChinh(btnXacNhanCheckIn);
        btnXacNhanCheckIn.Click += (s, e) => XuLyXacNhanCheckIn();

        pnlLeftCheckIn.Controls.AddRange(new Control[] { lblCCCD, txtCheckInCCCD, lblKDC, numCheckInKhachDiCung, btnXacNhanCheckIn });
        tbl.Controls.Add(pnlLeftCheckIn, 0, 0);
        var pnlRightCheckOut = new Panel
        {
            Dock = DockStyle.Fill,
            Width = 400,
            AutoScroll = true,
            BackColor = Color.White,
            Padding = new Padding(18, 16, 18, 16),
            Margin = new Padding(8, 0, 0, 0),
            BorderStyle = BorderStyle.FixedSingle
        };

        var lblTieuDeOut = new Label
        {
            Text = "Check-out: tính tiền",
            Font = new Font(AppConstants.FontChuChinh, 13f, FontStyle.Bold),
            Dock = DockStyle.Top,
            Height = 35
        };
        var pnlThongTinKhachOut = new Panel
        {
            Dock = DockStyle.Top,
            Height = 124,
            BackColor = Color.FromArgb(248, 250, 252),
            Padding = new Padding(10, 8, 10, 8),
            Margin = new Padding(0, 0, 0, 10),
            BorderStyle = BorderStyle.FixedSingle
        };

        var lblChonPhongOut = new Label
        {
            Text = "Chọn phòng / khách cần check-out:",
            Font = new Font(AppConstants.FontChuChinh, 9.5f, FontStyle.Regular),
            ForeColor = Color.FromArgb(71, 85, 105),
            Location = new Point(10, 6),
            AutoSize = true
        };

        cboChonPhongCheckOut = new ComboBox
        {
            Location = new Point(10, 26),
            Width = 340,
            Height = 28,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cboChonPhongCheckOut.SelectedIndexChanged += (s, e) =>
        {
            if (_dangNapComboCheckOut) return;
            if (cboChonPhongCheckOut.SelectedItem is CheckOutRoomItem item)
            {
                ChonDonDeCheckOut(item.BookingId);
            }
        };

        lblThongTinKhachCheckOut = new Label
        {
            Text = "Chưa chọn phòng đang lưu trú",
            Font = new Font(AppConstants.FontChuChinh, 9.8f, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42),
            Location = new Point(10, 60),
            AutoSize = true,
            UseMnemonic = false
        };

        pnlThongTinKhachOut.Controls.AddRange(new Control[] { lblChonPhongOut, cboChonPhongCheckOut, lblThongTinKhachCheckOut });

        dgvCheckOutChiTiet = new DataGridView { Dock = DockStyle.Top, Height = 200 };
        UIHelper.DinhDangBangDuLieu(dgvCheckOutChiTiet);
        dgvCheckOutChiTiet.Columns.Add("Khoan", "Khoản");
        dgvCheckOutChiTiet.Columns.Add("ThanhTien", "Thành tiền");
        dgvCheckOutChiTiet.Columns["Khoan"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        dgvCheckOutChiTiet.Columns["Khoan"].FillWeight = 56f;
        dgvCheckOutChiTiet.Columns["Khoan"].MinimumWidth = 120;
        dgvCheckOutChiTiet.Columns["ThanhTien"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        dgvCheckOutChiTiet.Columns["ThanhTien"].FillWeight = 44f;
        dgvCheckOutChiTiet.Columns["ThanhTien"].MinimumWidth = 110;
        dgvCheckOutChiTiet.Columns["ThanhTien"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        dgvCheckOutChiTiet.Columns["ThanhTien"].DefaultCellStyle.Padding = new Padding(0, 0, 10, 0);
        dgvCheckOutChiTiet.Columns["ThanhTien"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
        dgvCheckOutChiTiet.Columns["ThanhTien"].HeaderCell.Style.Padding = new Padding(0, 0, 10, 0);
        dgvCheckOutChiTiet.EnableHeadersVisualStyles = false;
        dgvCheckOutChiTiet.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(238, 242, 246);
        dgvCheckOutChiTiet.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(30, 41, 59);
        dgvCheckOutChiTiet.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(238, 242, 246);
        dgvCheckOutChiTiet.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 41, 59);
        pnlRightCheckOut.Controls.Add(dgvCheckOutChiTiet);
        pnlRightCheckOut.Controls.Add(pnlThongTinKhachOut);
        pnlRightCheckOut.Controls.Add(lblTieuDeOut);

        int yOut = 392;
        var lblHTTT = new Label { Text = "Hình thức thanh toán", Location = new Point(18, yOut), AutoSize = true, ForeColor = Color.FromArgb(71, 85, 105) };
        cboHinhThucTT = new ComboBox { Location = new Point(18, yOut + 24), Width = 362, Height = 30, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, DropDownStyle = ComboBoxStyle.DropDownList };
        cboHinhThucTT.Items.AddRange(new object[] { "Tiền mặt / Thẻ / Chuyển khoản", "Tiền mặt", "Thẻ ngân hàng", "Chuyển khoản QR" });
        cboHinhThucTT.SelectedIndex = 0;

        btnThanhToanVaCheckOut = new Button
        {
            Text = "Thanh toán và check-out",
            Location = new Point(18, yOut + 74),
            Width = 280,
            MinimumSize = new Size(250, 44),
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowOnly,
            Padding = new Padding(16, 0, 16, 0),
            UseMnemonic = false,
            Height = 44,
            Font = new Font(AppConstants.FontChuChinh, 10.5f, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            BackColor = AppConstants.MauXanhNavy,
            ForeColor = Color.White,
            Cursor = Cursors.Hand
        };
        btnThanhToanVaCheckOut.FlatAppearance.BorderSize = 0;
        btnThanhToanVaCheckOut.Click += (s, e) => XuLyThanhToanVaCheckOut();

        pnlRightCheckOut.Controls.AddRange(new Control[] { lblHTTT, cboHinhThucTT, btnThanhToanVaCheckOut });
        tbl.Controls.Add(pnlRightCheckOut, 1, 0);

        pnlCheckInOut.Controls.Add(tbl);
        
        pnlCheckInOut.VisibleChanged += (s, e) =>
        {
            if (pnlCheckInOut.Visible)
                TaiDuLieuCheckInOut();
        };
    }

    private void TaiDuLieuCheckInOut()
    {
        UIHelper.RunSafe(() =>
        { dgvChoNhanPhong.Rows.Clear();
            var ds = _datPhongBLL.GetPendingCheckIns();
            foreach (var dp in ds)
            {
                int idx = dgvChoNhanPhong.Rows.Add(dp.DisplayCode, dp.CustomerName, dp.RoomNumber);
                dgvChoNhanPhong.Rows[idx].Tag = dp;
            }

            if (dgvChoNhanPhong.Rows.Count > 0)
            {
                dgvChoNhanPhong.ClearSelection();
                dgvChoNhanPhong.Rows[0].Selected = true;
                dgvChoNhanPhong.CurrentCell = dgvChoNhanPhong.Rows[0].Cells[0];
                ChonDonDeCheckIn(ds[0]);
            }
            else
            {
                ChonDonDeCheckIn(null);
            }

            var dsDangO = _datPhongBLL.GetAll()
                .FindAll(d => d.Status == BookingStatus.CheckedIn);

            _dangNapComboCheckOut = true;
            cboChonPhongCheckOut.Items.Clear();
            foreach (var dp in dsDangO)
            {
                cboChonPhongCheckOut.Items.Add(new CheckOutRoomItem
                {
                    BookingId = dp.BookingId,
                    DisplayText = $"Phòng {dp.RoomNumber} - {dp.CustomerName} ({dp.DisplayCode})"
                });
            }
            _dangNapComboCheckOut = false;
            if (_maDPDangChonCheckOut > 0)
            {
                ChonDonDeCheckOut(_maDPDangChonCheckOut);
            }
            else if (dsDangO.Count > 0)
            {
                ChonDonDeCheckOut(dsDangO[0].BookingId);
            }
            else
            {
                _maDPDangChonCheckOut = 0;
                lblThongTinKhachCheckOut.Text = "Hiện không có phòng nào đang lưu trú";
                dgvCheckOutChiTiet.Rows.Clear();
            }
        }, "tải dữ liệu check-in / check-out", this);
    }

    private void DgvChoNhanPhong_SelectionChanged(object? sender, EventArgs e)
    {
        if (dgvChoNhanPhong.CurrentRow?.Tag is Booking dp)
        {
            ChonDonDeCheckIn(dp);
        }
    }
    private void ChonDonDeCheckIn(Booking? dp)
    {
        if (dp == null)
        {
            _maDPDangChonCheckIn = 0;
            txtCheckInCCCD.Clear();
            numCheckInKhachDiCung.Value = 0;
            return;
        }

        _maDPDangChonCheckIn = dp.BookingId;
        txtCheckInCCCD.Text = dp.CustomerNationalId;
        numCheckInKhachDiCung.Value = Math.Clamp(dp.GuestCount - 1, (int)numCheckInKhachDiCung.Minimum, (int)numCheckInKhachDiCung.Maximum);
    }
    private void XuLyXacNhanCheckIn()
    {
        if (_maDPDangChonCheckIn <= 0 && dgvChoNhanPhong.Rows.Count > 0)
        {
            var dpDau = dgvChoNhanPhong.CurrentRow?.Tag as Booking ?? dgvChoNhanPhong.Rows[0].Tag as Booking;
            ChonDonDeCheckIn(dpDau);
        }

        if (_maDPDangChonCheckIn <= 0)
        {
            MessageBox.Show("Vui lòng chọn 1 đơn từ danh sách chờ nhận phòng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        UIHelper.RunSafe(() =>
        {
            bool kq = _datPhongBLL.CheckIn(_maDPDangChonCheckIn, txtCheckInCCCD.Text.Trim(), out string msg);
            MessageBox.Show(this, msg, "Kết quả Check-in", MessageBoxButtons.OK, kq ? MessageBoxIcon.Information : MessageBoxIcon.Error);
            TaiDuLieuCheckInOut();
            TaiDuLieuSoDoPhong();
        }, "xác nhận check-in", this);
    }

    public void ChonDonDeCheckOut(int maDP)
    {
        UIHelper.RunSafe(() =>
        {
            _maDPDangChonCheckOut = maDP;
            var don = _datPhongBLL.GetById(maDP);
            if (don == null) return;

            _dangNapComboCheckOut = true;
            for (int i = 0; i < cboChonPhongCheckOut.Items.Count; i++)
            {
                if (cboChonPhongCheckOut.Items[i] is CheckOutRoomItem item && item.BookingId == maDP)
                {
                    cboChonPhongCheckOut.SelectedIndex = i;
                    break;
                }
            }
            _dangNapComboCheckOut = false;

            lblThongTinKhachCheckOut.Text =
                $"Khách: {don.CustomerName}  |  Phòng: {don.RoomNumber}  |  Mã: {don.DisplayCode}\n" +
                $"Nhận phòng: {don.CheckInDate}";

            var (tienPhong, tienDV, giamGia, vat, tongTien, soDem) = _datPhongBLL.CalculateCheckOutBilling(maDP);

            dgvCheckOutChiTiet.Rows.Clear();
            dgvCheckOutChiTiet.Rows.Add($"Tiền phòng ({don.RoomNumber}) x {soDem} đêm", FormatHelper.FormatCurrency(tienPhong));
            dgvCheckOutChiTiet.Rows.Add("Dịch vụ phát sinh", FormatHelper.FormatCurrency(tienDV));
            dgvCheckOutChiTiet.Rows.Add("Phụ thu / Giảm giá", FormatHelper.FormatCurrency(-giamGia));
            if (vat > 0)
            {
                dgvCheckOutChiTiet.Rows.Add("Thuế VAT (8%)", FormatHelper.FormatCurrency(vat));
            }
            int tongIdx = dgvCheckOutChiTiet.Rows.Add("Tổng cộng", FormatHelper.FormatCurrency(tongTien));
            if (tongIdx >= 0)
            {
                dgvCheckOutChiTiet.Rows[tongIdx].DefaultCellStyle.Font = new Font(AppConstants.FontChuChinh, 10.5f, FontStyle.Bold);
            }
        }, "tính tiền check-out", this);
    }

    private void XuLyThanhToanVaCheckOut()
    {
        if (_maDPDangChonCheckOut <= 0)
        {
            MessageBox.Show("Vui lòng chọn 1 phòng đang có khách để tính tiền check-out!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        UIHelper.RunSafe(() =>
        {
            string hinhThuc = cboHinhThucTT.SelectedItem?.ToString() ?? "Tiền mặt";
            bool thanhCong = _datPhongBLL.CheckOutAndPay(_maDPDangChonCheckOut, hinhThuc, out string msg);

            if (thanhCong)
            {
                MessageBox.Show(this, msg, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _maDPDangChonCheckOut = 0;
                dgvCheckOutChiTiet.Rows.Clear();
                TaiDuLieuCheckInOut();
                TaiDuLieuSoDoPhong();
                TaiDuLieuTongQuan();
            }
            else
            {
                MessageBox.Show(this, msg, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }, "thanh toán và check-out", this);
    }
}