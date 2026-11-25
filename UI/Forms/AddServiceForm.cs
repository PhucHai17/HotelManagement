using BLL;
using BLL.Interfaces;
using Common.Constants;
using Common.Helpers;
using HotelManagementSystem.Models.Entities;

namespace UI.Forms;

public class AddServiceForm : Form
{
 private readonly IHotelService _dichVuBLL = AppServices.Hotel;

    private TextBox txtTenDV = null!;
    private TextBox txtDonGia = null!;
    private ComboBox cboDonVi = null!;
    private Button btnLuu = null!;
    private Button btnHuy = null!;

    public AddServiceForm()
    {
        KhoiTaoGiaoDien();
        UIHelper.ApDungFontTimesNewRoman(this);
    }

    private void KhoiTaoGiaoDien()
    {
        Text = "Thêm Dịch Vụ Mới";
        Size = new Size(480, 330);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Color.White;

        var pnlHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 52,
            BackColor = AppConstants.MauXanhNavy
        };
        var lblTitle = new Label
        {
            Text = "THÊM DỊCH VỤ MỚI",
            ForeColor = Color.White,
            Font = new Font(AppConstants.FontChuChinh, 13f, FontStyle.Bold),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter
        };
        pnlHeader.Controls.Add(lblTitle);
        Controls.Add(pnlHeader);

        int startY = 74;
        int stepY = 46;

        var lblTen = new Label { Text = "Tên dịch vụ:", Location = new Point(28, startY), AutoSize = true };
        txtTenDV = new TextBox { Location = new Point(165, startY - 3), Width = 270, Height = 28 };

        var lblGia = new Label { Text = "Đơn giá (VNĐ):", Location = new Point(28, startY + stepY), AutoSize = true };
        txtDonGia = new TextBox { Location = new Point(165, startY + stepY - 3), Width = 270, Height = 28, Text = "0" };

        var lblDonVi = new Label { Text = "Đơn vị tính:", Location = new Point(28, startY + stepY * 2), AutoSize = true };
        cboDonVi = new ComboBox
        {
            Location = new Point(165, startY + stepY * 2 - 3),
            Width = 270,
            DropDownStyle = ComboBoxStyle.DropDown
        };
        cboDonVi.Items.AddRange(new object[] { "Chai", "Lon", "Phần", "Lượt", "Kg", "Giờ" });
        cboDonVi.SelectedIndex = 0;

        btnLuu = new Button
        {
            Text = "Lưu dịch vụ",
            Location = new Point(165, startY + stepY * 3 + 6),
            Width = 150,
            Height = 36,
            UseMnemonic = false
        };
        UIHelper.DinhDangNutBamChinh(btnLuu);
        btnLuu.Click += BtnLuu_Click;

        btnHuy = new Button
        {
            Text = "Đóng",
            Location = new Point(325, startY + stepY * 3 + 6),
            Width = 110,
            Height = 36,
            UseMnemonic = false
        };
        UIHelper.DinhDangNutBamPhu(btnHuy);
        btnHuy.Click += (s, e) => Close();

        Controls.AddRange(new Control[] { lblTen, txtTenDV, lblGia, txtDonGia, lblDonVi, cboDonVi, btnLuu, btnHuy });
    }

    private void BtnLuu_Click(object? sender, EventArgs e)
    {
        string tenDV = txtTenDV.Text.Trim();
        string donVi = cboDonVi.Text.Trim();

        if (string.IsNullOrWhiteSpace(tenDV))
        {
            MessageBox.Show(this, "Vui lòng nhập tên dịch vụ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtTenDV.Focus();
            return;
        }

        if (!double.TryParse(txtDonGia.Text.Trim(), out double donGia) || donGia < 0)
        {
            MessageBox.Show(this, "Đơn giá không hợp lệ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtDonGia.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(donVi))
        {
            MessageBox.Show(this, "Vui lòng nhập đơn vị tính!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            cboDonVi.Focus();
            return;
        }

        UIHelper.RunSafe(() =>
        {
            var dv = new Service
            {
                Name = tenDV,
                UnitPrice = donGia,
                Unit = donVi
            };
            
            bool thanhCong = _dichVuBLL.SaveService(dv, out string msg);
            MessageBox.Show(this, msg, "Thông báo", MessageBoxButtons.OK,
                thanhCong ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            if (thanhCong)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }, "thêm dịch vụ mới", this);
    }
}