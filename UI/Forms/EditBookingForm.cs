using BLL;
using Common.Constants;
using Common.Helpers;
using Entity.Entities;
using Entity.Enums;

namespace UI.Forms;

// Hộp thoại sửa thông tin đơn đặt phòng (lễ tân)
public class EditBookingForm : Form
{
    private readonly Booking _booking;
    private DateTimePicker dtpNhan = null!;
    private DateTimePicker dtpTra = null!;
    private NumericUpDown numSoKhach = null!;
    private NumericUpDown numTienCoc = null!;

    public EditBookingForm(Booking booking)
    {
        _booking = booking ?? throw new ArgumentNullException(nameof(booking));
        Font = new Font("Times New Roman", 10f, FontStyle.Regular);
        KhoiTaoGiaoDien();
    }

    private void KhoiTaoGiaoDien()
    {
        Text = $"HotelPro - Sửa đặt phòng {_booking.DisplayCode}";
        ClientSize = new Size(440, 430);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Color.White;

        var pnlHeader = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = AppConstants.MauXanhNavy };
        pnlHeader.Controls.Add(new Label
        {
            Text = $"SỬA ĐẶT PHÒNG {_booking.DisplayCode}",
            ForeColor = Color.White,
            Font = new Font("Times New Roman", 16f, FontStyle.Bold),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter
        });

        var pnlFooter = new Panel { Dock = DockStyle.Bottom, Height = 60, BackColor = AppConstants.MauNenChung };

        var btnLuu = new Button { Text = "Lưu", Size = new Size(90, 38), Location = new Point(138, 11), UseMnemonic = false, Cursor = Cursors.Hand };
        UIHelper.DinhDangNutBamChinh(btnLuu);
        btnLuu.Click += (_, _) => Luu();

        var btnDuyet = new Button { Text = "Duyệt đơn", Size = new Size(100, 38), Location = new Point(236, 11), UseMnemonic = false, Cursor = Cursors.Hand };
        UIHelper.DinhDangNutBamNavy(btnDuyet);
        btnDuyet.Visible = _booking.Status == BookingStatus.Pending;
        btnDuyet.Click += (_, _) => Duyet();

        var btnDong = new Button { Text = "Đóng", Size = new Size(90, 38), Location = new Point(344, 11), UseMnemonic = false, Cursor = Cursors.Hand };
        UIHelper.DinhDangNutBamPhu(btnDong);
        btnDong.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };

        pnlFooter.Controls.AddRange(new Control[] { btnLuu, btnDuyet, btnDong });

        var y = 72;
        void ThemDong(string tieuDe, Control control)
        {
            Controls.Add(new Label { Text = tieuDe, ForeColor = AppConstants.MauChuPhu, Location = new Point(24, y), AutoSize = true });
            control.Location = new Point(24, y + 20);
            control.Width = 392;
            Controls.Add(control);
            y += 58;
        }

        ThemDong("Khách hàng", new TextBox { Text = _booking.CustomerName, ReadOnly = true });
        ThemDong("Phòng", new TextBox
        {
            Text = string.IsNullOrWhiteSpace(_booking.RoomNumber)
                ? $"{_booking.RoomTypeName} (chưa xếp phòng)"
                : $"{_booking.RoomNumber} - {_booking.RoomTypeName}",
            ReadOnly = true
        });

        dtpNhan = new DateTimePicker { Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy", Value = FormatHelper.ParseDate(_booking.CheckInDate) };
        dtpTra = new DateTimePicker { Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy", Value = FormatHelper.ParseDate(_booking.CheckOutDate) };

        // Hai ô ngày đặt cạnh nhau
        Controls.Add(new Label { Text = "Ngày nhận phòng", ForeColor = AppConstants.MauChuPhu, Location = new Point(24, y), AutoSize = true });
        Controls.Add(new Label { Text = "Ngày trả phòng", ForeColor = AppConstants.MauChuPhu, Location = new Point(228, y), AutoSize = true });
        dtpNhan.SetBounds(24, y + 20, 188, 28);
        dtpTra.SetBounds(228, y + 20, 188, 28);
        Controls.Add(dtpNhan);
        Controls.Add(dtpTra);
        y += 58;

        numSoKhach = new NumericUpDown { Minimum = 1, Maximum = 20, Value = Math.Clamp(_booking.GuestCount, 1, 20) };
        ThemDong("Số khách", numSoKhach);

        numTienCoc = new NumericUpDown
        {
            Minimum = 0,
            Maximum = 1_000_000_000,
            Increment = 50_000,
            ThousandsSeparator = true,
            Value = (decimal)Math.Clamp(_booking.Deposit, 0, 1_000_000_000)
        };
        ThemDong("Tiền cọc (đ)", numTienCoc);

        Controls.Add(pnlFooter);
        Controls.Add(pnlHeader);
    }

    private void Luu()
    {
        UIHelper.RunSafe(() =>
        {
            var ok = AppServices.Booking.UpdateBookingInfo(
                _booking.BookingId,
                dtpNhan.Value.Date,
                dtpTra.Value.Date,
                (int)numSoKhach.Value,
                (double)numTienCoc.Value,
                out var message);

            MessageBox.Show(this, message, "Thông báo", MessageBoxButtons.OK,
                ok ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (ok)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }, "sửa đơn đặt phòng", this);
    }

    private void Duyet()
    {
        UIHelper.RunSafe(() =>
        {
            var ok = AppServices.Booking.ConfirmBooking(_booking.BookingId, out var message);
            MessageBox.Show(this, message, "Thông báo", MessageBoxButtons.OK,
                ok ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (ok)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }, "duyệt đơn đặt phòng", this);
    }
}
