using BLL;
using BLL.Interfaces;
using Common.Constants;
using Common.Helpers;
using Entity.Entities;
using Entity.Enums;

namespace UI.Forms;

public class AddBookingForm : Form
{
    private readonly IBookingService _datPhongBLL = AppServices.Booking;
    private readonly ICustomerService _khachHangBLL = AppServices.Customer;
    private readonly IRoomService _phongBLL = AppServices.Room;

    private TextBox txtHoTen = null!;
    private TextBox txtSDT = null!;
    private TextBox txtCCCD = null!;
    private ComboBox cboLoaiPhong = null!;
    private ComboBox cboPhong = null!;
    private DateTimePicker dtpNgayNhan = null!;
    private DateTimePicker dtpNgayTra = null!;
    private NumericUpDown numSoKhach = null!;
    private TextBox txtTienCoc = null!;
    private Button btnLuu = null!;
    private Button btnHuy = null!;

    public AddBookingForm()
    {
        Font = new Font("Times New Roman", 10f, FontStyle.Regular);
        KhoiTaoGiaoDien();
        NapDuLieuLoaiPhong();
    }

    // Khởi tạo giao diện hộp thoại thêm đặt phòng mới tại quầy
    private void KhoiTaoGiaoDien()
    {
        Text = "Thêm Đặt Phòng Mới";
        Size = new Size(560, 595);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Color.White;

        var fontNoiDung = new Font("Times New Roman", 10f, FontStyle.Regular);
        var fontNutChinh = new Font("Times New Roman", 10f, FontStyle.Bold);

        var pnlHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 58,
            BackColor = AppConstants.MauXanhNavy
        };
        var lblTitle = new Label
        {
            Text = "THÊM ĐẶT PHÒNG MỚI",
            ForeColor = Color.White,
            Font = new Font("Times New Roman", 16f, FontStyle.Bold),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter
        };
        pnlHeader.Controls.Add(lblTitle);
        Controls.Add(pnlHeader);

        var startY = 80;
        var stepY = 44;

        TaoNhanVaO("Họ tên khách:", 32, startY, fontNoiDung, out txtHoTen);
        TaoNhanVaO("Số điện thoại:", 32, startY + stepY, fontNoiDung, out txtSDT);
        TaoNhanVaO("Số CCCD / Hộ chiếu:", 32, startY + stepY * 2, fontNoiDung, out txtCCCD);

        var lblLoai = new Label
        {
            Text = "Loại phòng:",
            Location = new Point(32, startY + stepY * 3),
            AutoSize = true,
            Font = fontNoiDung
        };
        cboLoaiPhong = new ComboBox
        {
            Location = new Point(200, startY + stepY * 3 - 3),
            Width = 310,
            Font = fontNoiDung,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cboLoaiPhong.SelectedIndexChanged += (s, e) => NapDuLieuPhongTheoLoai();
        Controls.Add(lblLoai);
        Controls.Add(cboLoaiPhong);

        var lblPhong = new Label
        {
            Text = "Chọn phòng:",
            Location = new Point(32, startY + stepY * 4),
            AutoSize = true,
            Font = fontNoiDung
        };
        cboPhong = new ComboBox
        {
            Location = new Point(200, startY + stepY * 4 - 3),
            Width = 310,
            Font = fontNoiDung,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        Controls.Add(lblPhong);
        Controls.Add(cboPhong);

        var lblNhan = new Label
        {
            Text = "Ngày nhận phòng:",
            Location = new Point(32, startY + stepY * 5),
            AutoSize = true,
            Font = fontNoiDung
        };
        dtpNgayNhan = new DateTimePicker
        {
            Location = new Point(200, startY + stepY * 5 - 3),
            Width = 310,
            Font = fontNoiDung,
            Format = DateTimePickerFormat.Custom,
            CustomFormat = AppConstants.DateFormat
        };
        Controls.Add(lblNhan);
        Controls.Add(dtpNgayNhan);

        var lblTra = new Label
        {
            Text = "Ngày trả phòng:",
            Location = new Point(32, startY + stepY * 6),
            AutoSize = true,
            Font = fontNoiDung
        };
        dtpNgayTra = new DateTimePicker
        {
            Location = new Point(200, startY + stepY * 6 - 3),
            Width = 310,
            Font = fontNoiDung,
            Format = DateTimePickerFormat.Custom,
            CustomFormat = AppConstants.DateFormat,
            Value = DateTime.Today.AddDays(1)
        };
        Controls.Add(lblTra);
        Controls.Add(dtpNgayTra);

        var lblKhach = new Label
        {
            Text = "Số lượng khách:",
            Location = new Point(32, startY + stepY * 7),
            AutoSize = true,
            Font = fontNoiDung
        };
        numSoKhach = new NumericUpDown
        {
            Location = new Point(200, startY + stepY * 7 - 3),
            Width = 110,
            Font = fontNoiDung,
            Minimum = 1,
            Maximum = 10,
            Value = 2
        };
        Controls.Add(lblKhach);
        Controls.Add(numSoKhach);

        TaoNhanVaO("Tiền cọc (VNĐ):", 32, startY + stepY * 8, fontNoiDung, out txtTienCoc);
        txtTienCoc.Text = "0";

        btnLuu = new Button
        {
            Text = "Xác nhận đặt phòng",
            Location = new Point(200, startY + stepY * 9 + 6),
            Width = 190,
            Height = 38,
            UseMnemonic = false
        };
        UIHelper.DinhDangNutBamChinh(btnLuu);
        btnLuu.Font = fontNutChinh;
        btnLuu.Click += BtnLuu_Click;

        btnHuy = new Button
        {
            Text = "Đóng",
            Location = new Point(400, startY + stepY * 9 + 6),
            Width = 110,
            Height = 38,
            UseMnemonic = false
        };
        UIHelper.DinhDangNutBamPhu(btnHuy);
        btnHuy.Font = fontNoiDung;
        btnHuy.Click += (s, e) => Close();

        Controls.Add(btnLuu);
        Controls.Add(btnHuy);
    }

    private void TaoNhanVaO(string nhan, int x, int y, Font font, out TextBox txt)
    {
        var lbl = new Label
        {
            Text = nhan,
            Location = new Point(x, y),
            AutoSize = true,
            Font = font
        };
        txt = new TextBox
        {
            Location = new Point(x + 168, y - 3),
            Width = 310,
            Height = 28,
            Font = font
        };
        Controls.Add(lbl);
        Controls.Add(txt);
    }

    // Nạp danh sách loại phòng vào hộp chọn
    private void NapDuLieuLoaiPhong()
    {
        UIHelper.RunSafe(() =>
        {
            var ds = _phongBLL.GetAllRoomTypes();
            cboLoaiPhong.DataSource = ds;
            cboLoaiPhong.DisplayMember = nameof(RoomType.Name);
            cboLoaiPhong.ValueMember = nameof(RoomType.RoomTypeId);

            if (cboLoaiPhong.Items.Count > 0) cboLoaiPhong.SelectedIndex = 0;
        }, "tải danh sách loại phòng", this);
    }

    // Nạp danh sách phòng trống theo loại phòng đang chọn
    private void NapDuLieuPhongTheoLoai()
    {
        UIHelper.RunSafe(() =>
        {
            if (cboLoaiPhong.SelectedValue is int maLoai)
            {
                var dsPhong = _phongBLL.GetAvailableRooms(maLoai);
                cboPhong.DataSource = dsPhong;
                cboPhong.DisplayMember = nameof(Room.RoomNumber);
                cboPhong.ValueMember = nameof(Room.RoomId);
            }
        }, "tải danh sách phòng trống", this);
    }

    // Kiểm tra dữ liệu và tạo đơn đặt phòng mới tại quầy
    private void BtnLuu_Click(object? sender, EventArgs e)
    {
        var hoTen = txtHoTen.Text.Trim();
        var sdt = txtSDT.Text.Trim();
        var cccd = txtCCCD.Text.Trim();

        if (string.IsNullOrWhiteSpace(hoTen))
        {
            MessageBox.Show(this, "Vui lòng nhập họ tên khách hàng!", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtHoTen.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(sdt))
        {
            MessageBox.Show(this, "Vui lòng nhập số điện thoại!", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtSDT.Focus();
            return;
        }

        if (dtpNgayTra.Value.Date <= dtpNgayNhan.Value.Date)
        {
            MessageBox.Show(this, "Ngày trả phòng phải sau ngày nhận phòng!", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            dtpNgayTra.Focus();
            return;
        }

        if (cboPhong.SelectedValue == null || cboPhong.SelectedItem is not Room phongChon)
        {
            MessageBox.Show(this, "Không có phòng trống thuộc loại này!", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        double tienCoc = 0;
        if (!string.IsNullOrWhiteSpace(txtTienCoc.Text) &&
            (!double.TryParse(txtTienCoc.Text.Trim(), out tienCoc) || tienCoc < 0))
        {
            MessageBox.Show(this, "Tiền cọc không hợp lệ!", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtTienCoc.Focus();
            return;
        }

        var maPhong = (int)cboPhong.SelectedValue;

        UIHelper.RunSafe(() =>
        {
            var kh = _khachHangBLL.GetByIdentityCardOrPhone(sdt);
            if (kh == null && !string.IsNullOrWhiteSpace(cccd))
                kh = _khachHangBLL.GetByIdentityCardOrPhone(cccd);

            if (kh == null)
            {
                kh = new Customer
                {
                    FullName = hoTen,
                    PhoneNumber = sdt,
                    NationalId = cccd,
                    TierId = 1
                };
                if (!_khachHangBLL.SaveCustomer(kh, out var thongBaoKhach))
                {
                    MessageBox.Show(this, thongBaoKhach, "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            var nhanVienHienTai = SessionManager.CurrentEmployee;
            if (nhanVienHienTai == null || nhanVienHienTai.EmployeeId <= 0)
            {
                MessageBox.Show(this, "Phiên đăng nhập nhân viên không hợp lệ. Vui lòng đăng nhập lại!",
                    "Hết phiên làm việc", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var dp = new Booking
            {
                CustomerId = kh.CustomerId,
                EmployeeId = nhanVienHienTai.EmployeeId,
                CheckInDate = dtpNgayNhan.Value.ToString(AppConstants.DateFormat),
                CheckOutDate = dtpNgayTra.Value.ToString(AppConstants.DateFormat),
                GuestCount = (int)numSoKhach.Value,
                Deposit = tienCoc,
                Channel = "Trực tiếp",
                Status = BookingStatus.Confirmed
            };

            var thanhCong = _datPhongBLL.CreateBooking(dp, maPhong, phongChon.BasePrice, out var thongBao);
            if (thanhCong)
            {
                MessageBox.Show(this, thongBao, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show(this, thongBao, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }, "lưu đơn đặt phòng", this);
    }
}
