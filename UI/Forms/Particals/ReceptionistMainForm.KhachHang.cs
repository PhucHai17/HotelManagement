using Common.Constants;
using Common.Helpers;
using Entity.Entities;
using Entity.Enums;

namespace UI.Forms;

public partial class ReceptionistMainForm
{
    private TextBox txtTimKhachHang = null!;
    private DataGridView dgvKhachHang = null!;
    private Label lblHoSoMaKH = null!;
    private TextBox txtHoSoTen = null!;
    private TextBox txtHoSoSDT = null!;
    private TextBox txtHoSoCCCD = null!;
    private Label lblHoSoHang = null!;
    private Label lblHoSoThongKe = null!;
    private Customer? _khachHangDangChon;

    private void KhoiTaoTabKhachHang()
    {
        var tblRootKhachHang = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 1,
            Margin = new Padding(0)
        };
        tblRootKhachHang.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        tblRootKhachHang.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

        // 1. Thanh tìm kiếm phía trên
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

        var flowFilterKH = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            Padding = new Padding(0),
            Margin = new Padding(0)
        };

        var lblTim = new Label
        {
            Text = "Tìm khách hàng:",
            AutoSize = true,
            Margin = new Padding(0, 8, 8, 6),
            ForeColor = Color.FromArgb(71, 85, 105)
        };

        txtTimKhachHang = new TextBox
        {
            Width = 310,
            Height = 30,
            Margin = new Padding(0, 3, 10, 6),
            PlaceholderText = "Nhập họ tên, SĐT hoặc số CCCD..."
        };
        txtTimKhachHang.KeyDown += (s, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                TaiDuLieuKhachHang();
            }
        };
        txtTimKhachHang.TextChanged += (s, e) =>
        {
            if (string.IsNullOrWhiteSpace(txtTimKhachHang.Text))
                TaiDuLieuKhachHang();
        };

        var btnTimKhach = new Button
        {
            Text = "Tìm khách",
            Width = 125,
            Height = 34,
            UseMnemonic = false,
            Margin = new Padding(0, 1, 8, 6)
        };
        UIHelper.DinhDangNutBamChinh(btnTimKhach);
        btnTimKhach.Click += (s, e) => TaiDuLieuKhachHang();

        var btnLamMoiKH = new Button
        {
            Text = "Làm mới",
            Width = 105,
            Height = 34,
            UseMnemonic = false,
            Margin = new Padding(0, 1, 0, 6)
        };
        UIHelper.DinhDangNutBamPhu(btnLamMoiKH);
        btnLamMoiKH.Click += (s, e) =>
        {
            txtTimKhachHang.Clear();
            TaiDuLieuKhachHang();
        };

        flowFilterKH.Controls.AddRange(new Control[] { lblTim, txtTimKhachHang, btnTimKhach, btnLamMoiKH });
        pnlTop.Controls.Add(flowFilterKH);
        tblRootKhachHang.Controls.Add(pnlTop, 0, 0);

        //Danh sách khách  - Hồ sơ chi tiết
        var tbl = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = new Padding(0)
        };
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58f));
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42f));

        // Bảng danh sách khách hàng
        dgvKhachHang = new DataGridView { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 8, 0) };
        UIHelper.DinhDangBangDuLieu(dgvKhachHang);
        dgvKhachHang.Columns.Add("MaKH", "Mã KH");
        dgvKhachHang.Columns.Add("HoTen", "Họ tên");
        dgvKhachHang.Columns.Add("SDT", "Số điện thoại");
        dgvKhachHang.Columns.Add("CCCD", "CCCD / Hộ chiếu");
        dgvKhachHang.Columns.Add("SoLanO", "Số lần ở");

        dgvKhachHang.Columns["MaKH"].MinimumWidth = 80;
        dgvKhachHang.Columns["MaKH"].FillWeight = 55;
        dgvKhachHang.Columns["HoTen"].MinimumWidth = 140;
        dgvKhachHang.Columns["HoTen"].FillWeight = 115;
        dgvKhachHang.Columns["SDT"].MinimumWidth = 105;
        dgvKhachHang.Columns["SDT"].FillWeight = 85;
        dgvKhachHang.Columns["CCCD"].MinimumWidth = 115;
        dgvKhachHang.Columns["CCCD"].FillWeight = 90;
        dgvKhachHang.Columns["SoLanO"].MinimumWidth = 75;
        dgvKhachHang.Columns["SoLanO"].FillWeight = 55;

        dgvKhachHang.SelectionChanged += DgvKhachHang_SelectionChanged;
        tbl.Controls.Add(dgvKhachHang, 0, 0);

        //Hồ sơ khách hàng
        var pnlHoSo = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            BackColor = Color.White,
            Padding = new Padding(20, 16, 20, 16),
            Margin = new Padding(8, 0, 0, 0),
            BorderStyle = BorderStyle.FixedSingle
        };

        var lblTieuDeHoSo = new Label
        {
            Text = "Hồ sơ khách hàng",
            Font = new Font(AppConstants.FontChuChinh, 13f, FontStyle.Bold),
            Location = new Point(16, 14),
            AutoSize = true
        };

        lblHoSoMaKH = new Label
        {
            Text = "Chưa chọn khách hàng",
            Font = new Font(AppConstants.FontChuChinh, 9.5f, FontStyle.Bold),
            ForeColor = Color.FromArgb(37, 99, 235),
            Location = new Point(16, 40),
            AutoSize = true
        };
        pnlHoSo.Controls.AddRange(new Control[] { lblTieuDeHoSo, lblHoSoMaKH });

        int y = 72;
        ThemNhanVaInput(pnlHoSo, "Họ tên khách", y, out txtHoSoTen);
        ThemNhanVaInput(pnlHoSo, "Số điện thoại", y + 66, out txtHoSoSDT);
        ThemNhanVaInput(pnlHoSo, "CCCD / Hộ chiếu", y + 132, out txtHoSoCCCD);

        // Khung thông tin hạng & lượt lưu trú
        var pnlThongTinHang = new Panel
        {
            Location = new Point(16, y + 202),
            Width = Math.Max(220, pnlHoSo.Width - 36),
            Height = 64,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            BackColor = Color.FromArgb(248, 250, 252),
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(10, 8, 10, 8)
        };

        lblHoSoHang = new Label
        {
            Text = "Hạng thành viên: -",
            Font = new Font(AppConstants.FontChuChinh, 10f, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42),
            Location = new Point(10, 8),
            AutoSize = true
        };

        lblHoSoThongKe = new Label
        {
            Text = "Điểm tích lũy: 0 điểm  •  Đã lưu trú: 0 lần",
            Font = new Font(AppConstants.FontChuChinh, 9.5f, FontStyle.Regular),
            ForeColor = Color.FromArgb(71, 85, 105),
            Location = new Point(10, 32),
            AutoSize = true
        };

        pnlThongTinHang.Controls.AddRange(new Control[] { lblHoSoHang, lblHoSoThongKe });
        pnlHoSo.Controls.Add(pnlThongTinHang);

        // Nút thao tác
        var btnLuuHoSo = new Button
        {
            Text = "Lưu cập nhật",
            Location = new Point(16, y + 282),
            Width = 128,
            Height = 38,
            UseMnemonic = false
        };
        UIHelper.DinhDangNutBamChinh(btnLuuHoSo);
        btnLuuHoSo.Click += (s, e) => XuLyLuuHoSoKhach();

        var btnLichSu = new Button
        {
            Text = "Lịch sử lưu trú",
            Location = new Point(154, y + 282),
            Width = 148,
            Height = 38,
            UseMnemonic = false
        };
        UIHelper.DinhDangNutBamPhu(btnLichSu);
        btnLichSu.Click += (s, e) => HienThiLichSuLuuTru();

        pnlHoSo.Controls.AddRange(new Control[] { btnLuuHoSo, btnLichSu });
        tbl.Controls.Add(pnlHoSo, 1, 0);

        tblRootKhachHang.Controls.Add(tbl, 0, 1);
        pnlKhachHang.Controls.Add(tblRootKhachHang);
    }

    private void TaiDuLieuKhachHang()
    {
        UIHelper.RunSafe(() =>
        {
            int idDangChon = _khachHangDangChon?.CustomerId ?? 0;
            dgvKhachHang.Rows.Clear();

            string tuKhoa = txtTimKhachHang.Text.Trim();
            List<Customer> ds;

            if (string.IsNullOrWhiteSpace(tuKhoa))
            {
                ds = _khachHangBLL.GetAll();
            }
            else
            {
                ds = _khachHangBLL.Search(tuKhoa);
                if (ds.Count == 0)
                {
                    var khChinhXac = _khachHangBLL.GetByIdentityCardOrPhone(tuKhoa);
                    if (khChinhXac != null) ds.Add(khChinhXac);
                }
            }

            int rowToSelect = 0;
            for (int i = 0; i < ds.Count; i++)
            {
                var kh = ds[i];
                int idx = dgvKhachHang.Rows.Add(
                    $"KH{kh.CustomerId:D3}",
                    kh.FullName,
                    kh.PhoneNumber,
                    kh.NationalId,
                    kh.StayCount);
                dgvKhachHang.Rows[idx].Tag = kh;

                if (kh.CustomerId == idDangChon)
                    rowToSelect = idx;
            }

            if (dgvKhachHang.Rows.Count > 0)
            {
                dgvKhachHang.ClearSelection();
                dgvKhachHang.Rows[rowToSelect].Selected = true;
                dgvKhachHang.CurrentCell = dgvKhachHang.Rows[rowToSelect].Cells[0];
                CapNhatFormHoSo(dgvKhachHang.Rows[rowToSelect].Tag as Customer);
            }
            else
            {
                CapNhatFormHoSo(null);
            }
        }, "tải danh sách khách hàng", this);
    }

    private void DgvKhachHang_SelectionChanged(object? sender, EventArgs e)
    {
        CapNhatFormHoSo(dgvKhachHang.CurrentRow?.Tag as Customer);
    }

    private void CapNhatFormHoSo(Customer? kh)
    {
        _khachHangDangChon = kh;
        if (kh == null)
        {
            lblHoSoMaKH.Text = "Không có dữ liệu khách hàng";
            txtHoSoTen.Text = string.Empty;
            txtHoSoSDT.Text = string.Empty;
            txtHoSoCCCD.Text = string.Empty;
            lblHoSoHang.Text = "Hạng thành viên: -";
            lblHoSoThongKe.Text = "Điểm tích lũy: 0 điểm  •  Đã lưu trú: 0 lần";
            return;
        }

        lblHoSoMaKH.Text = $"Mã khách hàng: KH{kh.CustomerId:D3}";
        txtHoSoTen.Text = kh.FullName;
        txtHoSoSDT.Text = kh.PhoneNumber;
        txtHoSoCCCD.Text = kh.NationalId;

        var tenHang = string.IsNullOrWhiteSpace(kh.TierName) ? "Thành viên" : kh.TierName;
        lblHoSoHang.Text = $"Hạng thành viên: {tenHang}";
        lblHoSoThongKe.Text = $"Điểm tích lũy: {kh.LoyaltyPoints:N0} điểm  •  Đã lưu trú: {kh.StayCount} lần";
    }

    private void XuLyLuuHoSoKhach()
    {
        if (_khachHangDangChon == null || _khachHangDangChon.CustomerId <= 0)
        {
            MessageBox.Show(this, "Vui lòng chọn một khách hàng trong danh sách để cập nhật!", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        UIHelper.RunSafe(() =>
        {
            _khachHangDangChon.FullName = txtHoSoTen.Text.Trim();
            _khachHangDangChon.PhoneNumber = txtHoSoSDT.Text.Trim();
            _khachHangDangChon.NationalId = txtHoSoCCCD.Text.Trim();

            bool thanhCong = _khachHangBLL.SaveCustomer(_khachHangDangChon, out string msg);
            MessageBox.Show(this, msg, "Thông báo", MessageBoxButtons.OK,
                thanhCong ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            if (thanhCong)
            {
                TaiDuLieuKhachHang();
            }
        }, "lưu hồ sơ khách hàng", this);
    }

    private void HienThiLichSuLuuTru()
    {
        if (_khachHangDangChon == null || _khachHangDangChon.CustomerId <= 0)
        {
            MessageBox.Show(this, "Vui lòng chọn một khách hàng để xem lịch sử lưu trú!", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        UIHelper.RunSafe(() =>
        {
            var dsLichSu = _datPhongBLL.GetByCustomerId(_khachHangDangChon.CustomerId);
            if (dsLichSu.Count == 0)
            {
                MessageBox.Show(this, $"Khách hàng {_khachHangDangChon.FullName} chưa có lịch sử lưu trú.",
                    "Lịch sử lưu trú", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var frmLichSu = new Form
            {
                Text = $"Lịch sử lưu trú - {_khachHangDangChon.FullName} (KH{_khachHangDangChon.CustomerId:D3})",
                Size = new Size(640, 380),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.White
            };

            var dgv = new DataGridView { Dock = DockStyle.Fill };
            UIHelper.DinhDangBangDuLieu(dgv);
            dgv.Columns.Add("MaDP", "Mã đặt phòng");
            dgv.Columns.Add("Phong", "Phòng");
            dgv.Columns.Add("NgayNhan", "Ngày nhận");
            dgv.Columns.Add("NgayTra", "Ngày trả");
            dgv.Columns.Add("TrangThai", "Trạng thái");

            foreach (var dp in dsLichSu)
            {
                string trangThai = dp.Status switch
                {
                    BookingStatus.Pending => "Chờ duyệt",
                    BookingStatus.Confirmed => "Đã xác nhận",
                    BookingStatus.CheckedIn => "Đang ở",
                    BookingStatus.CheckedOut => "Đã trả phòng",
                    BookingStatus.Cancelled => "Đã hủy",
                    _ => dp.Status.ToString()
                };

                dgv.Rows.Add($"DP{dp.BookingId:D3}", dp.RoomNumber, dp.CheckInDate, dp.CheckOutDate, trangThai);
            }

            frmLichSu.Controls.Add(dgv);
            UIHelper.ApDungFontTimesNewRoman(frmLichSu);
            frmLichSu.ShowDialog(this);
        }, "xem lịch sử lưu trú khách hàng", this);
    }
}