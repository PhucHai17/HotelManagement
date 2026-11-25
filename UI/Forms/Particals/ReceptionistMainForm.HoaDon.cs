namespace UI.Forms;

using System.Drawing.Printing;
using Common.Constants;
using Common.Helpers;
using Entity.Entities;
public partial class ReceptionistMainForm
{
    private TextBox txtTimHoaDon = null!;
    private ComboBox cboLocTrangThaiHD = null!;
    private DataGridView dgvDanhSachHoaDon = null!;
    private Label lblChiTietTieuDeHD = null!;
    private Label lblThongTinKhachHD = null!;
    private DataGridView dgvChiTietKhoanTien = null!;
    private ComboBox cboPhuongThucTT = null!;
    private Button btnThanhToanHoaDon = null!;
    private Button btnInHoaDon = null!;
    private Invoice? _hoaDonDangChon;
    private List<Invoice> _tatCaHoaDon = new();

    private void KhoiTaoTabHoaDon()
    {
        var tbl = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = new Padding(0)
        };
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55f));
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45f));

        var pnlLeft = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(18, 14, 18, 14),
            Margin = new Padding(0, 0, 8, 0),
            BorderStyle = BorderStyle.FixedSingle
        };

        var tblLeft = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 3,
            ColumnCount = 1,
            Margin = new Padding(0)
        };
        tblLeft.RowStyles.Add(new RowStyle(SizeType.Absolute, 34f));
        tblLeft.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        tblLeft.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

        var lblTieuDe = new Label
        {
            Text = "Danh sách hóa đơn",
            Font = new Font(AppConstants.FontChuChinh, 13f, FontStyle.Bold),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };

        var flowFilterHD = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            Padding = new Padding(0, 4, 0, 8),
            Margin = new Padding(0)
        };

        txtTimHoaDon = new TextBox
        {
            Width = 210,
            Height = 30,
            Margin = new Padding(0, 2, 8, 4),
            PlaceholderText = "Số HĐ hoặc tên khách..."
        };
        txtTimHoaDon.TextChanged += (s, e) => LocDanhSachHoaDon();

        cboLocTrangThaiHD = new ComboBox
        {
            Width = 155,
            Height = 30,
            Margin = new Padding(0, 2, 8, 4),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cboLocTrangThaiHD.Items.AddRange(new object[]
        {
            "Tất cả trạng thái",
            AppConstants.HDChuaThanhToan,
            AppConstants.HDDaThanhToan
        });
        cboLocTrangThaiHD.SelectedIndex = 0;
        cboLocTrangThaiHD.SelectedIndexChanged += (s, e) => LocDanhSachHoaDon();

        var btnLamMoiHD = new Button
        {
            Text = "Làm mới",
            Width = 92,
            Height = 32,
            Margin = new Padding(0, 1, 0, 4),
            UseMnemonic = false
        };
        UIHelper.DinhDangNutBamPhu(btnLamMoiHD);
        btnLamMoiHD.Click += (s, e) =>
        {
            txtTimHoaDon.Clear();
            cboLocTrangThaiHD.SelectedIndex = 0;
            TaiDuLieuHoaDon();
        };

        flowFilterHD.Controls.AddRange(new Control[] { txtTimHoaDon, cboLocTrangThaiHD, btnLamMoiHD });

        dgvDanhSachHoaDon = new DataGridView { Dock = DockStyle.Fill, Margin = new Padding(0) };
        UIHelper.DinhDangBangDuLieu(dgvDanhSachHoaDon);
        dgvDanhSachHoaDon.Columns.Add("SoHD", "Số HĐ");
        dgvDanhSachHoaDon.Columns.Add("Khach", "Khách");
        dgvDanhSachHoaDon.Columns.Add("TongTien", "Tổng tiền");
        dgvDanhSachHoaDon.Columns.Add("TrangThai", "Trạng thái");

        dgvDanhSachHoaDon.Columns["SoHD"].FillWeight = 18f;
        dgvDanhSachHoaDon.Columns["SoHD"].MinimumWidth = 68;
        dgvDanhSachHoaDon.Columns["Khach"].FillWeight = 30f;
        dgvDanhSachHoaDon.Columns["Khach"].MinimumWidth = 105;
        dgvDanhSachHoaDon.Columns["TongTien"].FillWeight = 24f;
        dgvDanhSachHoaDon.Columns["TongTien"].MinimumWidth = 95;
        dgvDanhSachHoaDon.Columns["TongTien"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        dgvDanhSachHoaDon.Columns["TrangThai"].FillWeight = 28f;
        dgvDanhSachHoaDon.Columns["TrangThai"].MinimumWidth = 112;
        dgvDanhSachHoaDon.SelectionChanged += DgvDanhSachHoaDon_SelectionChanged;

        dgvDanhSachHoaDon.CellPainting += (s, e) =>
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == dgvDanhSachHoaDon.Columns["TrangThai"].Index && e.Value != null)
            {
                string tt = e.Value.ToString() ?? "";
                if (tt == AppConstants.HDDaThanhToan)
                    UIHelper.VeBadge(e, tt, Color.FromArgb(220, 252, 231), Color.FromArgb(21, 128, 61));
                else
                    UIHelper.VeBadge(e, tt, Color.FromArgb(254, 226, 226), Color.FromArgb(185, 28, 28));
            }
        };

        tblLeft.Controls.Add(lblTieuDe, 0, 0);
        tblLeft.Controls.Add(flowFilterHD, 0, 1);
        tblLeft.Controls.Add(dgvDanhSachHoaDon, 0, 2);
        pnlLeft.Controls.Add(tblLeft);
        tbl.Controls.Add(pnlLeft, 0, 0);
        
        var pnlRight = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(20, 14, 20, 14),
            Margin = new Padding(8, 0, 0, 0),
            BorderStyle = BorderStyle.FixedSingle
        };

        var tblRight = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 5,
            Margin = new Padding(0)
        };
        tblRight.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        tblRight.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        tblRight.RowStyles.Add(new RowStyle(SizeType.Absolute, 210f));
        tblRight.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        tblRight.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

        lblChiTietTieuDeHD = new Label
        {
            Text = "Chi tiết hóa đơn",
            Font = new Font(AppConstants.FontChuChinh, 13f, FontStyle.Bold),
            ForeColor = AppConstants.MauChuChinh,
            Dock = DockStyle.Fill,
            AutoSize = true,
            Padding = new Padding(0, 0, 0, 4),
            UseMnemonic = false
        };
        tblRight.Controls.Add(lblChiTietTieuDeHD, 0, 0);

        lblThongTinKhachHD = new Label
        {
            Text = "Chọn một hóa đơn bên trái để xem chi tiết",
            Font = new Font(AppConstants.FontChuChinh, 9.5f, FontStyle.Regular),
            ForeColor = Color.FromArgb(71, 85, 105),
            Dock = DockStyle.Fill,
            AutoSize = true,
            Padding = new Padding(0, 0, 0, 10),
            UseMnemonic = false
        };
        tblRight.Controls.Add(lblThongTinKhachHD, 0, 1);

        dgvChiTietKhoanTien = new DataGridView
        {
            Dock = DockStyle.Fill,
            ScrollBars = ScrollBars.Vertical
        };
        UIHelper.DinhDangBangDuLieu(dgvChiTietKhoanTien);
        dgvChiTietKhoanTien.Columns.Add("Khoan", "Khoản");
        dgvChiTietKhoanTien.Columns.Add("Tien", "Tiền");
        dgvChiTietKhoanTien.Columns["Khoan"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        dgvChiTietKhoanTien.Columns["Khoan"].FillWeight = 52f;
        dgvChiTietKhoanTien.Columns["Khoan"].MinimumWidth = 110;
        dgvChiTietKhoanTien.Columns["Tien"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        dgvChiTietKhoanTien.Columns["Tien"].FillWeight = 48f;
        dgvChiTietKhoanTien.Columns["Tien"].MinimumWidth = 105;
        dgvChiTietKhoanTien.Columns["Tien"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        dgvChiTietKhoanTien.Columns["Tien"].DefaultCellStyle.Padding = new Padding(0, 0, 12, 0);
        dgvChiTietKhoanTien.Columns["Tien"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
        dgvChiTietKhoanTien.Columns["Tien"].HeaderCell.Style.Padding = new Padding(0, 0, 12, 0);
        tblRight.Controls.Add(dgvChiTietKhoanTien, 0, 2);
        
        var pnlThanhToanGroup = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            Padding = new Padding(0, 14, 0, 0),
            Margin = new Padding(0)
        };

        var lblPTTT = new Label
        {
            Text = "Hình thức:",
            AutoSize = true,
            Margin = new Padding(0, 10, 6, 8),
            ForeColor = Color.FromArgb(71, 85, 105)
        };

        cboPhuongThucTT = new ComboBox
        {
            Width = 150,
            Height = 32,
            Margin = new Padding(0, 5, 12, 8),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cboPhuongThucTT.Items.AddRange(new object[] { "Tiền mặt", "Chuyển khoản", "Thẻ ngân hàng" });
        cboPhuongThucTT.SelectedIndex = 0;

        btnThanhToanHoaDon = new Button
        {
            Text = "Thanh toán",
            Width = 128,
            Height = 38,
            Margin = new Padding(0, 1, 8, 6),
            UseMnemonic = false
        };
        UIHelper.DinhDangNutBamChinh(btnThanhToanHoaDon);
        btnThanhToanHoaDon.Click += (s, e) => XuLyThanhToanHoaDonTab();

        btnInHoaDon = new Button
        {
            Text = "In hóa đơn",
            Width = 124,
            Height = 38,
            Margin = new Padding(0, 1, 0, 6),
            UseMnemonic = false
        };
        UIHelper.DinhDangNutBamPhu(btnInHoaDon);
        btnInHoaDon.Click += (s, e) => HienThiVaInHoaDon();

        pnlThanhToanGroup.Controls.AddRange(new Control[]
        {
            lblPTTT, cboPhuongThucTT, btnThanhToanHoaDon, btnInHoaDon
        });
        tblRight.Controls.Add(pnlThanhToanGroup, 0, 3);

        pnlRight.Controls.Add(tblRight);
        tbl.Controls.Add(pnlRight, 1, 0);

        pnlHoaDon.Controls.Add(tbl);
    }

    private void TaiDuLieuHoaDon()
    {
        UIHelper.RunSafe(() =>
        {
            _tatCaHoaDon = _hoaDonBLL.GetAll();
            LocDanhSachHoaDon();
        }, "tải danh sách hóa đơn", this);
    }

    private void LocDanhSachHoaDon()
    {
        int idDangChon = _hoaDonDangChon?.InvoiceId ?? 0;
        string tuKhoa = txtTimHoaDon.Text.Trim().ToLowerInvariant();
        string trangThaiLoc = cboLocTrangThaiHD.SelectedItem?.ToString() ?? "Tất cả trạng thái";

        dgvDanhSachHoaDon.Rows.Clear();

        int rowToSelect = 0;
        int currentIndex = 0;

        foreach (var hd in _tatCaHoaDon)
        {
            string tenTrangThai = LayTenTrangThaiHD(hd);

            if (trangThaiLoc != "Tất cả trạng thái" && tenTrangThai != trangThaiLoc)
                continue;

            if (!string.IsNullOrEmpty(tuKhoa))
            {
                bool khopMa = hd.DisplayCode.ToLowerInvariant().Contains(tuKhoa);
                bool khopKhach = hd.CustomerName.ToLowerInvariant().Contains(tuKhoa);
                if (!khopMa && !khopKhach)
                    continue;
            }

            int idx = dgvDanhSachHoaDon.Rows.Add(
                hd.DisplayCode,
                hd.CustomerName,
                FormatHelper.FormatCurrency(hd.TotalAmount),
                tenTrangThai);
            dgvDanhSachHoaDon.Rows[idx].Tag = hd;

            if (hd.InvoiceId == idDangChon)
                rowToSelect = currentIndex;

            currentIndex++;
        }

        if (dgvDanhSachHoaDon.Rows.Count > 0)
        {
            dgvDanhSachHoaDon.ClearSelection();
            dgvDanhSachHoaDon.Rows[rowToSelect].Selected = true;
            dgvDanhSachHoaDon.CurrentCell = dgvDanhSachHoaDon.Rows[rowToSelect].Cells[0];
            CapNhatChiTietHoaDon(dgvDanhSachHoaDon.Rows[rowToSelect].Tag as Invoice);
        }
        else
        {
            CapNhatChiTietHoaDon(null);
        }
    }

    private void DgvDanhSachHoaDon_SelectionChanged(object? sender, EventArgs e)
    {
        CapNhatChiTietHoaDon(dgvDanhSachHoaDon.CurrentRow?.Tag as Invoice);
    }

    private void CapNhatChiTietHoaDon(Invoice? hd)
    {
        _hoaDonDangChon = hd;
        dgvChiTietKhoanTien.Rows.Clear();

        if (hd == null)
        {
            lblChiTietTieuDeHD.Text = "Chi tiết hóa đơn";
            lblThongTinKhachHD.Text = "Không có hóa đơn phù hợp";
            btnThanhToanHoaDon.Enabled = false;
            btnInHoaDon.Enabled = false;
            cboPhuongThucTT.Enabled = false;
            return;
        }

        string tenTrangThai = LayTenTrangThaiHD(hd);
        lblChiTietTieuDeHD.Text = $"Chi tiết hóa đơn {hd.DisplayCode}";
        lblThongTinKhachHD.Text = $"Khách hàng: {hd.CustomerName}  •  Trạng thái: {tenTrangThai}";

        dgvChiTietKhoanTien.Rows.Add("Tiền phòng", FormatHelper.FormatCurrency(hd.RoomCharge));
        dgvChiTietKhoanTien.Rows.Add("Dịch vụ", FormatHelper.FormatCurrency(hd.ServiceCharge));
        dgvChiTietKhoanTien.Rows.Add("VAT", FormatHelper.FormatCurrency(hd.VatAmount));
        int tongIdx = dgvChiTietKhoanTien.Rows.Add("Tổng cộng", FormatHelper.FormatCurrency(hd.TotalAmount));
        if (tongIdx >= 0)
        {
            dgvChiTietKhoanTien.Rows[tongIdx].DefaultCellStyle.Font =
                new Font(AppConstants.FontChuChinh, 10.5f, FontStyle.Bold);
            dgvChiTietKhoanTien.Rows[tongIdx].DefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
        }

        bool chuaThanhToan = tenTrangThai == AppConstants.HDChuaThanhToan;
        btnThanhToanHoaDon.Enabled = chuaThanhToan;
        cboPhuongThucTT.Enabled = chuaThanhToan;
        btnInHoaDon.Enabled = true;
    }

    private void XuLyThanhToanHoaDonTab()
    {
        if (_hoaDonDangChon == null) return;

        string phuongThuc = cboPhuongThucTT.SelectedItem?.ToString() ?? "Tiền mặt";
        var xacNhan = MessageBox.Show(
            this,
            $"Xác nhận thanh toán hóa đơn {_hoaDonDangChon.DisplayCode} của khách {_hoaDonDangChon.CustomerName}?\n\n" +
            $"Tổng thanh toán: {FormatHelper.FormatCurrency(_hoaDonDangChon.TotalAmount)}\n" +
            $"Hình thức: {phuongThuc}",
            "Xác nhận thanh toán",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (xacNhan != DialogResult.Yes) return;

        UIHelper.RunSafe(() =>
        {
            bool kq = _hoaDonBLL.Pay(_hoaDonDangChon.InvoiceId, phuongThuc, out string msg);
            MessageBox.Show(this, msg, "Thông báo", MessageBoxButtons.OK,
                kq ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            if (kq)
            {
                TaiDuLieuHoaDon();
                TaiDuLieuTongQuan();
            }
        }, "thanh toán hóa đơn", this);
    }

    private void HienThiVaInHoaDon()
    {
        if (_hoaDonDangChon == null) return;
        var hd = _hoaDonDangChon;

        string noiDungPhieu =
            "==============================================\r\n" +
            "              KHÁCH SẠN HOTELPRO              \r\n" +
            "             PHIẾU THANH TOÁN                 \r\n" +
            "==============================================\r\n" +
            $"Số hóa đơn : {hd.DisplayCode}\r\n" +
            $"Khách hàng : {hd.CustomerName}\r\n" +
            $"Ngày xuất  : {DateTime.Now:dd/MM/yyyy HH:mm}\r\n" +
            $"Trạng thái : {LayTenTrangThaiHD(hd)}\r\n" +
            "----------------------------------------------\r\n" +
            $"Tiền phòng : {FormatHelper.FormatCurrency(hd.RoomCharge),30}\r\n" +
            $"Dịch vụ    : {FormatHelper.FormatCurrency(hd.ServiceCharge),30}\r\n" +
            $"Thuế VAT   : {FormatHelper.FormatCurrency(hd.VatAmount),30}\r\n" +
            "----------------------------------------------\r\n" +
            $"TỔNG CỘNG  : {FormatHelper.FormatCurrency(hd.TotalAmount),30}\r\n" +
            "==============================================\r\n" +
            "       Cảm ơn quý khách và hẹn gặp lại!       ";

        using var frmPreview = new Form
        {
            Text = $"Xem trước hóa đơn - {hd.DisplayCode}",
            Size = new Size(460, 440),
            StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            MinimizeBox = false,
            BackColor = Color.White
        };

        var txtPreview = new TextBox
        {
            Multiline = true,
            ReadOnly = true,
            Dock = DockStyle.Fill,
            Font = new Font("Consolas", 10f, FontStyle.Regular),
            BackColor = Color.White,
            BorderStyle = BorderStyle.None,
            Text = noiDungPhieu
        };

        var pnlBottom = new Panel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(12, 8, 12, 8) };
        var btnInNgay = new Button
        {
            Text = "In phiếu",
            Width = 120,
            Height = 34,
            Location = new Point(185, 8),
            UseMnemonic = false
        };
        UIHelper.DinhDangNutBamChinh(btnInNgay);
        btnInNgay.Click += (s, e) =>
        {
            UIHelper.RunSafe(() =>
            {
                using var doc = new PrintDocument();
                doc.DocumentName = $"HoaDon_{hd.DisplayCode}";
                doc.PrintPage += (ps, pe) =>
                {
                    pe.Graphics?.DrawString(
                        noiDungPhieu,
                        new Font("Consolas", 10.5f),
                        Brushes.Black,
                        new RectangleF(40, 40, pe.MarginBounds.Width, pe.MarginBounds.Height));
                };

                using var dlg = new PrintDialog { Document = doc };
                if (dlg.ShowDialog(frmPreview) == DialogResult.OK)
                {
                    doc.Print();
                }
            }, "in hóa đơn", frmPreview);
        };

        var btnDong = new Button
        {
            Text = "Đóng",
            Width = 100,
            Height = 34,
            Location = new Point(315, 8),
            UseMnemonic = false
        };
        UIHelper.DinhDangNutBamPhu(btnDong);
        btnDong.Click += (s, e) => frmPreview.Close();

        pnlBottom.Controls.AddRange(new Control[] { btnInNgay, btnDong });
        frmPreview.Controls.Add(txtPreview);
        frmPreview.Controls.Add(pnlBottom);
        frmPreview.ShowDialog(this);
    }
    private static string LayTenTrangThaiHD(Invoice hd)
    {
        var raw = hd.Status.ToString();
        if (raw.Equals("Paid", StringComparison.OrdinalIgnoreCase) ||
            raw.Equals("DaThanhToan", StringComparison.OrdinalIgnoreCase) ||
            raw == AppConstants.HDDaThanhToan)
        {
            return AppConstants.HDDaThanhToan;
        }

        return AppConstants.HDChuaThanhToan;
    }
}