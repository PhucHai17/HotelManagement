using Common.Constants;
using Common.Helpers;
using Entity.Entities;
using HotelManagementSystem.Models.Entities;

namespace UI.Forms;

public partial class ReceptionistMainForm
{
    private TextBox txtTimDichVu = null!;
    private DataGridView dgvDanhMucDichVu = null!;
    private ComboBox cboGhiDVPhong = null!;
    private ComboBox cboGhiDVDichVu = null!;
    private NumericUpDown numGhiDVSoLuong = null!;
    private Label lblDonGiaDV = null!;
    private Label lblTamTinhDV = null!;
    private DataGridView dgvDichVuVuaGhi = null!;
    private List<Service> _tatCaDichVu = new();

    private void KhoiTaoTabDichVu()
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
        
        var pnlLeftDV = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(18, 14, 18, 14),
            Margin = new Padding(0, 0, 8, 0),
            BorderStyle = BorderStyle.FixedSingle
        };

        var tblLeftDV = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 4,
            ColumnCount = 1,
            Margin = new Padding(0)
        };
        tblLeftDV.RowStyles.Add(new RowStyle(SizeType.Absolute, 34f));
        tblLeftDV.RowStyles.Add(new RowStyle(SizeType.Absolute, 40f));
        tblLeftDV.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
        tblLeftDV.RowStyles.Add(new RowStyle(SizeType.Absolute, 48f));

        var lblTieuDeDV = new Label
        {
            Text = "Danh mục dịch vụ",
            Font = new Font(AppConstants.FontChuChinh, 13f, FontStyle.Bold),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };

        txtTimDichVu = new TextBox
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 2, 0, 6),
            PlaceholderText = "Tìm tên dịch vụ (nước suối, giặt ủi, ăn sáng...)..."
        };
        txtTimDichVu.TextChanged += (s, e) => LocDanhMucDichVu();

        dgvDanhMucDichVu = new DataGridView { Dock = DockStyle.Fill, Margin = new Padding(0) };
        UIHelper.DinhDangBangDuLieu(dgvDanhMucDichVu);
        dgvDanhMucDichVu.Columns.Add("TenDV", "Dịch vụ");
        dgvDanhMucDichVu.Columns.Add("DonGia", "Đơn giá");
        dgvDanhMucDichVu.Columns.Add("DonVi", "Đơn vị");

        dgvDanhMucDichVu.Columns["TenDV"].MinimumWidth = 160;
        dgvDanhMucDichVu.Columns["TenDV"].FillWeight = 130;
        dgvDanhMucDichVu.Columns["DonGia"].MinimumWidth = 110;
        dgvDanhMucDichVu.Columns["DonGia"].FillWeight = 80;
        dgvDanhMucDichVu.Columns["DonGia"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        dgvDanhMucDichVu.Columns["DonVi"].MinimumWidth = 75;
        dgvDanhMucDichVu.Columns["DonVi"].FillWeight = 55;
        dgvDanhMucDichVu.Columns["DonVi"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        dgvDanhMucDichVu.SelectionChanged += (s, e) =>
        {
            if (dgvDanhMucDichVu.CurrentRow?.Tag is Service dv && dv.ServiceId > 0)
            {
                cboGhiDVDichVu.SelectedValue = dv.ServiceId;
            }
        };

        var pnlBtnThemDV = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 10, 0, 0) };
        var btnThemDV = new Button
        {
            Text = "+ Thêm dịch vụ",
            Location = new Point(0, 10),
            Width = 155,
            Height = 36,
            UseMnemonic = false
        };
        UIHelper.DinhDangNutBamPhu(btnThemDV);
        btnThemDV.Click += (s, e) =>
        {
            using var frm = new AddServiceForm();
            UIHelper.ApDungFontTimesNewRoman(frm);
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                TaiDuLieuDichVu();
            }
        };
        pnlBtnThemDV.Controls.Add(btnThemDV);

        tblLeftDV.Controls.Add(lblTieuDeDV, 0, 0);
        tblLeftDV.Controls.Add(txtTimDichVu, 0, 1);
        tblLeftDV.Controls.Add(dgvDanhMucDichVu, 0, 2);
        tblLeftDV.Controls.Add(pnlBtnThemDV, 0, 3);
        pnlLeftDV.Controls.Add(tblLeftDV);
        tbl.Controls.Add(pnlLeftDV, 0, 0);
        
        var pnlRightGhiDV = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            BackColor = Color.White,
            Padding = new Padding(20, 14, 20, 14),
            Margin = new Padding(8, 0, 0, 0),
            BorderStyle = BorderStyle.FixedSingle
        };

        var lblTieuDeGhi = new Label
        {
            Text = "Ghi dịch vụ cho phòng",
            Font = new Font(AppConstants.FontChuChinh, 13f, FontStyle.Bold),
            Location = new Point(16, 14),
            AutoSize = true
        };

        int y = 56;
        var lblPhong = new Label
        {
            Text = "Phòng đang có khách",
            Location = new Point(16, y),
            AutoSize = true,
            ForeColor = Color.FromArgb(71, 85, 105)
        };
        cboGhiDVPhong = new ComboBox
        {
            Location = new Point(16, y + 24),
            Width = Math.Max(220, pnlRightGhiDV.Width - 36),
            Height = 30,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            DropDownStyle = ComboBoxStyle.DropDownList
        };

        var lblDV = new Label
        {
            Text = "Dịch vụ sử dụng",
            Location = new Point(16, y + 68),
            AutoSize = true,
            ForeColor = Color.FromArgb(71, 85, 105)
        };
        cboGhiDVDichVu = new ComboBox
        {
            Location = new Point(16, y + 92),
            Width = Math.Max(220, pnlRightGhiDV.Width - 36),
            Height = 30,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cboGhiDVDichVu.SelectedIndexChanged += (s, e) => CapNhatTamTinhDichVu();

        var lblSL = new Label
        {
            Text = "Số lượng",
            Location = new Point(16, y + 136),
            AutoSize = true,
            ForeColor = Color.FromArgb(71, 85, 105)
        };
        numGhiDVSoLuong = new NumericUpDown
        {
            Location = new Point(16, y + 160),
            Width = 110,
            Height = 30,
            Minimum = 1,
            Maximum = 100,
            Value = 1
        };
        numGhiDVSoLuong.ValueChanged += (s, e) => CapNhatTamTinhDichVu();

        lblDonGiaDV = new Label
        {
            Text = "Đơn giá: 0 đ",
            Location = new Point(142, y + 148),
            AutoSize = true,
            ForeColor = Color.FromArgb(71, 85, 105)
        };

        lblTamTinhDV = new Label
        {
            Text = "Tạm tính: 0 đ",
            Location = new Point(142, y + 170),
            AutoSize = true,
            Font = new Font(AppConstants.FontChuChinh, 10.5f, FontStyle.Bold),
            ForeColor = Color.FromArgb(29, 78, 216)
        };

        var btnGhiVaoHoaDon = new Button
        {
            Text = "Thêm vào hóa đơn phòng",
            Location = new Point(16, y + 206),
            Width = 235,
            Height = 40,
            UseMnemonic = false
        };
        UIHelper.DinhDangNutBamChinh(btnGhiVaoHoaDon);
        btnGhiVaoHoaDon.Click += (s, e) => XuLyGhiDichVuChoPhong();

        var lblVuaGhi = new Label
        {
            Text = "Dịch vụ vừa ghi trong phiên",
            Location = new Point(16, y + 264),
            AutoSize = true,
            Font = new Font(AppConstants.FontChuChinh, 10.5f, FontStyle.Bold),
            ForeColor = Color.FromArgb(51, 65, 85)
        };

        dgvDichVuVuaGhi = new DataGridView
        {
            Location = new Point(16, y + 290),
            Width = Math.Max(220, pnlRightGhiDV.Width - 36),
            Height = 165,
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
        };
        UIHelper.DinhDangBangDuLieu(dgvDichVuVuaGhi);
        dgvDichVuVuaGhi.Columns.Add("Phong", "Phòng");
        dgvDichVuVuaGhi.Columns.Add("DichVu", "Dịch vụ");
        dgvDichVuVuaGhi.Columns.Add("SL", "SL");
        dgvDichVuVuaGhi.Columns.Add("ThanhTien", "Thành tiền");
        dgvDichVuVuaGhi.Columns["Phong"].FillWeight = 55;
        dgvDichVuVuaGhi.Columns["DichVu"].FillWeight = 115;
        dgvDichVuVuaGhi.Columns["SL"].FillWeight = 40;
        dgvDichVuVuaGhi.Columns["ThanhTien"].FillWeight = 85;
        dgvDichVuVuaGhi.Columns["ThanhTien"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

        pnlRightGhiDV.Controls.AddRange(new Control[]
        {
            lblTieuDeGhi, lblPhong, cboGhiDVPhong, lblDV, cboGhiDVDichVu,
            lblSL, numGhiDVSoLuong, lblDonGiaDV, lblTamTinhDV,
            btnGhiVaoHoaDon, lblVuaGhi, dgvDichVuVuaGhi
        });
        tbl.Controls.Add(pnlRightGhiDV, 1, 0);

        pnlDichVu.Controls.Add(tbl);
    }

    private void TaiDuLieuDichVu()
    {
        UIHelper.RunSafe(() =>
        {
            _tatCaDichVu = _dichVuBLL.GetAll();
            LocDanhMucDichVu();

            cboGhiDVDichVu.DataSource = null;
            cboGhiDVDichVu.DisplayMember = nameof(Service.Name);
            cboGhiDVDichVu.ValueMember = nameof(Service.ServiceId);
            cboGhiDVDichVu.DataSource = _tatCaDichVu;

            var dsPhong = _phongBLL.GetAll().FindAll(p => p.Status == AppConstants.PhongCoKhach);
            cboGhiDVPhong.DataSource = null;
            cboGhiDVPhong.DisplayMember = nameof(Room.RoomNumber);
            cboGhiDVPhong.ValueMember = nameof(Room.RoomNumber);
            cboGhiDVPhong.DataSource = dsPhong;

            CapNhatTamTinhDichVu();
        }, "tải danh mục dịch vụ", this);
    }

    private void LocDanhMucDichVu()
    {
        string tuKhoa = txtTimDichVu.Text.Trim().ToLowerInvariant();
        dgvDanhMucDichVu.Rows.Clear();

        foreach (var dv in _tatCaDichVu)
        {
            if (!string.IsNullOrEmpty(tuKhoa) && !dv.Name.ToLowerInvariant().Contains(tuKhoa))
                continue;

            int idx = dgvDanhMucDichVu.Rows.Add(dv.Name, FormatHelper.FormatCurrency(dv.UnitPrice), dv.Unit);
            dgvDanhMucDichVu.Rows[idx].Tag = dv;
        }
    }

    private void CapNhatTamTinhDichVu()
    {
        if (cboGhiDVDichVu.SelectedItem is Service dv)
        {
            double thanhTien = dv.UnitPrice * (double)numGhiDVSoLuong.Value;
            lblDonGiaDV.Text = $"Đơn giá: {FormatHelper.FormatCurrency(dv.UnitPrice)} / {dv.Unit}";
            lblTamTinhDV.Text = $"Tạm tính: {FormatHelper.FormatCurrency(thanhTien)}";
        }
        else
        {
            lblDonGiaDV.Text = "Đơn giá: 0 đ";
            lblTamTinhDV.Text = "Tạm tính: 0 đ";
        }
    }

    private void XuLyGhiDichVuChoPhong()
    {
        if (cboGhiDVPhong.SelectedValue == null)
        {
            MessageBox.Show(this, "Hiện không có phòng nào đang có khách lưu trú!", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (cboGhiDVDichVu.SelectedValue is not int maDV || maDV <= 0 || cboGhiDVDichVu.SelectedItem is not Service dvChon)
        {
            MessageBox.Show(this, "Vui lòng chọn dịch vụ cần ghi!", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        UIHelper.RunSafe(() =>
        {
            string soPhong = cboGhiDVPhong.SelectedValue.ToString() ?? "";
            int soLuong = (int)numGhiDVSoLuong.Value;
            double thanhTien = dvChon.UnitPrice * soLuong;

            bool kq = _dichVuBLL.RecordServiceUsage(soPhong, maDV, soLuong, out string msg);
            MessageBox.Show(this, msg, "Thông báo", MessageBoxButtons.OK,
                kq ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            if (kq)
            {
                dgvDichVuVuaGhi.Rows.Insert(0,
                    $"P.{soPhong}",
                    dvChon.Name,
                    soLuong,
                    FormatHelper.FormatCurrency(thanhTien));

                numGhiDVSoLuong.Value = 1;
                CapNhatTamTinhDichVu();
            }
        }, "ghi dịch vụ cho phòng", this);
    }
}