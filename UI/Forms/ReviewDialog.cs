using Common.Constants;
using Common.Helpers;
using Entity.Entities;

namespace UI.Forms;

// Hộp thoại đánh giá kỳ nghỉ (gửi đánh giá mới hoặc xem đánh giá đã gửi)
public class ReviewDialog : Form
{
    private readonly Booking _booking;
    private readonly Review? _existing;
    private readonly Label[] _lblSao = new Label[5];
    private TextBox txtNoiDung = null!;
    private int _soSao;

    public int SoSao => _soSao;
    public string NoiDung => txtNoiDung.Text.Trim();

    public ReviewDialog(Booking booking, Review? existing = null)
    {
        _booking = booking ?? throw new ArgumentNullException(nameof(booking));
        _existing = existing;
        _soSao = existing?.Stars ?? 5;
        Font = new Font("Times New Roman", 10f, FontStyle.Regular);
        KhoiTaoGiaoDien();
        CapNhatSao();
    }

    private void KhoiTaoGiaoDien()
    {
        var chiXem = _existing != null;

        Text = chiXem ? "HotelPro - Đánh giá của bạn" : "HotelPro - Đánh giá kỳ nghỉ";
        ClientSize = new Size(444, 400);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Color.White;

        var pnlHeader = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = AppConstants.MauXanhNavy };
        pnlHeader.Controls.Add(new Label
        {
            Text = chiXem ? "ĐÁNH GIÁ CỦA BẠN" : "ĐÁNH GIÁ KỲ NGHỈ",
            ForeColor = Color.White,
            Font = new Font("Times New Roman", 16f, FontStyle.Bold),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter
        });

        var pnlFooter = new Panel { Dock = DockStyle.Bottom, Height = 60, BackColor = AppConstants.MauNenChung };

        var btnDong = new Button
        {
            Text = "Đóng",
            Size = new Size(90, 38),
            Location = new Point(330, 11),
            UseMnemonic = false,
            Cursor = Cursors.Hand
        };
        UIHelper.DinhDangNutBamPhu(btnDong);
        btnDong.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };
        pnlFooter.Controls.Add(btnDong);

        if (!chiXem)
        {
            var btnGui = new Button
            {
                Text = "Gửi đánh giá",
                Font = new Font("Times New Roman", 10f, FontStyle.Bold),
                Size = new Size(130, 38),
                Location = new Point(190, 11),
                UseMnemonic = false,
                Cursor = Cursors.Hand
            };
            UIHelper.DinhDangNutBamChinh(btnGui);
            btnGui.Click += (_, _) =>
            {
                if (_soSao < 1)
                {
                    MessageBox.Show(this, "Vui lòng chọn số sao!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                DialogResult = DialogResult.OK;
                Close();
            };
            pnlFooter.Controls.Add(btnGui);
            AcceptButton = btnGui;
        }

        var phong = string.IsNullOrWhiteSpace(_booking.RoomNumber) ? "" : $" - Phòng {_booking.RoomNumber}";
        var lblThongTin = new Label
        {
            Text = $"{_booking.DisplayCode} - {_booking.RoomTypeName}{phong}",
            Font = new Font("Times New Roman", 11f, FontStyle.Bold),
            ForeColor = AppConstants.MauChuChinh,
            Location = new Point(24, 72),
            Size = new Size(396, 26),
            AutoEllipsis = true
        };

        var lblSao = new Label
        {
            Text = "Số sao",
            ForeColor = AppConstants.MauChuPhu,
            Location = new Point(24, 108),
            AutoSize = true
        };

        for (var i = 0; i < 5; i++)
        {
            var soSao = i + 1;
            var lbl = new Label
            {
                Text = "★",
                Font = new Font("Segoe UI Symbol", 22f, FontStyle.Regular),
                Size = new Size(44, 40),
                Location = new Point(24 + i * 46, 128),
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = chiXem ? Cursors.Default : Cursors.Hand
            };
            if (!chiXem)
            {
                lbl.Click += (_, _) => { _soSao = soSao; CapNhatSao(); };
            }
            _lblSao[i] = lbl;
        }

        var lblNhanXet = new Label
        {
            Text = "Nhận xét",
            ForeColor = AppConstants.MauChuPhu,
            Location = new Point(24, 180),
            AutoSize = true
        };

        txtNoiDung = new TextBox
        {
            Location = new Point(24, 204),
            Size = new Size(396, 124),
            Multiline = true,
            MaxLength = 500,
            ScrollBars = ScrollBars.Vertical,
            ReadOnly = chiXem,
            Text = _existing?.Content ?? string.Empty,
            PlaceholderText = "Chia sẻ trải nghiệm của bạn..."
        };

        Controls.Add(lblThongTin);
        Controls.Add(lblSao);
        Controls.AddRange(_lblSao);
        Controls.Add(lblNhanXet);
        Controls.Add(txtNoiDung);
        Controls.Add(pnlFooter);
        Controls.Add(pnlHeader);
    }

    // Tô vàng các ngôi sao đã chọn
    private void CapNhatSao()
    {
        for (var i = 0; i < _lblSao.Length; i++)
        {
            _lblSao[i].ForeColor = i < _soSao ? AppConstants.MauVangCam : AppConstants.MauVienInput;
        }
    }
}
