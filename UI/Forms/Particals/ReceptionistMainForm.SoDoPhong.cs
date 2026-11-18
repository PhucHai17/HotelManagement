using Common.Constants;
using Common.Helpers;
using Entity.Entities;
using Entity.Enums;
using UI.Controls;

namespace UI.Forms;

public partial class ReceptionistMainForm
{
        private StatCardControl cardPhongTrong = null!;
        private StatCardControl cardDangCoKhach = null!;
        private StatCardControl cardCheckInHomNay = null!;
        private StatCardControl cardCheckOutHomNay = null!;
        private TableLayoutPanel tblLướiPhong = null!;
        private TextBox txtCheckInNhanhCCCD = null!;
        private TextBox txtCheckInNhanhSDT = null!;
        private TextBox txtCheckInNhanhKhach = null!;
        private Label lblCheckInNhanhPhong = null!;
        private DateTimePicker dtpCheckInNhanhNgayTra = null!;
        private Label lblTieuDeSoDoTang = null!;
        private Button btnDaDonXong = null!;
        private Room? _phongDangChon;
        private int _tangHienTai = 0; // 0: Tầng 1 & 2

        private void KhoiTaoTabSoDoPhong()
        {
            var tblRootSoDo = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1,
                Margin = new Padding(0)
            };
            tblRootSoDo.RowStyles.Add(new RowStyle(SizeType.Absolute, 95f));
            tblRootSoDo.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

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

            cardPhongTrong = new StatCardControl
                { Title = "Phòng trống", TopAccentColor = Color.FromArgb(16, 185, 129), Dock = DockStyle.Fill };
            cardDangCoKhach = new StatCardControl
                { Title = "Đang có khách", TopAccentColor = Color.FromArgb(239, 68, 68), Dock = DockStyle.Fill };
            cardCheckInHomNay = new StatCardControl
                { Title = "Check-in hôm nay", TopAccentColor = Color.FromArgb(245, 158, 11), Dock = DockStyle.Fill };
            cardCheckOutHomNay = new StatCardControl
                { Title = "Check-out hôm nay", TopAccentColor = Color.FromArgb(59, 130, 246), Dock = DockStyle.Fill };

            tblCards.Controls.Add(cardPhongTrong, 0, 0);
            tblCards.Controls.Add(cardDangCoKhach, 1, 0);
            tblCards.Controls.Add(cardCheckInHomNay, 2, 0);
            tblCards.Controls.Add(cardCheckOutHomNay, 3, 0);
            tblRootSoDo.Controls.Add(tblCards, 0, 0);

            var tblBody = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = new Padding(0)
            };
            tblBody.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            tblBody.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 315f));

            var pnlCenter = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Margin = new Padding(0, 0, 8, 0),
                Padding = new Padding(16, 12, 16, 12),
                BorderStyle = BorderStyle.FixedSingle
            };

            var tblCenterLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Margin = new Padding(0)
            };
            tblCenterLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 45f));
            tblCenterLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            tblCenterLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36f));

            var pnlFilterTang = new Panel { Dock = DockStyle.Fill, Margin = new Padding(0) };
            lblTieuDeSoDoTang = new Label
            {
                Text = "Sơ đồ phòng - Tầng 1, 2",
                Font = new Font(AppConstants.FontChuChinh, 13f, FontStyle.Bold),
                Location = new Point(0, 8),
                AutoSize = true
            };
            pnlFilterTang.Controls.Add(lblTieuDeSoDoTang);

            var pnlNutTang = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(0, 5, 0, 0)
            };
            var lblTangChon = new Label
            {
                Text = "Tầng:", AutoSize = true, Font = new Font(AppConstants.FontChuChinh, 10.5f),
                Margin = new Padding(0, 6, 6, 0)
            };
            pnlNutTang.Controls.Add(lblTangChon);

            string[] cacTang = { "1", "2", "3", "Tất cả" };
            foreach (var tang in cacTang)
            {
                var btnTang = new Button
                {
                    Text = tang,
                    Width = (tang == "Tất cả" ? 68 : 46),
                    Height = 30,
                    Margin = new Padding(3, 0, 3, 0),
                    Cursor = Cursors.Hand
                };
                UIHelper.DinhDangNutBamPhu(btnTang);
                btnTang.Click += (s, e) =>
                {
                    if (tang == "Tất cả")
                    {
                        _tangHienTai = 0;
                        lblTieuDeSoDoTang.Text = "Sơ đồ phòng - Toàn bộ các tầng";
                    }
                    else
                    {
                        _tangHienTai = int.Parse(tang);
                        lblTieuDeSoDoTang.Text = $"Sơ đồ phòng - Tầng {_tangHienTai}";
                    }

                    NapDanhSachThePhong();
                };
                pnlNutTang.Controls.Add(btnTang);
            }

            pnlFilterTang.Controls.Add(pnlNutTang);
            tblCenterLayout.Controls.Add(pnlFilterTang, 0, 0);

            tblLướiPhong = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                AutoScroll = true,
                Margin = new Padding(0),
                Padding = new Padding(4, 16, 4, 4)
            };
            tblLướiPhong.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            tblLướiPhong.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            tblLướiPhong.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            tblLướiPhong.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            tblCenterLayout.Controls.Add(tblLướiPhong, 0, 1);

            var pnlChuThich = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0),
                Padding = new Padding(0, 8, 0, 0)
            };
            ThemChuThich(pnlChuThich, "Trống", Color.FromArgb(220, 252, 231));
            ThemChuThich(pnlChuThich, "Có khách", Color.FromArgb(254, 226, 226));
            ThemChuThich(pnlChuThich, "Đã đặt", Color.FromArgb(254, 243, 199));
            ThemChuThich(pnlChuThich, "Đang dọn", Color.FromArgb(224, 242, 254));
            tblCenterLayout.Controls.Add(pnlChuThich, 0, 2);

            pnlCenter.Controls.Add(tblCenterLayout);
            tblBody.Controls.Add(pnlCenter, 0, 0);

            var pnlRightCheckIn = new Panel
            {
                Dock = DockStyle.Fill,
                Width = 315,
                AutoScroll = true,
                BackColor = Color.White,
                Padding = new Padding(16),
                Margin = new Padding(8, 0, 0, 0),
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblTieuDeCheckIn = new Label
            {
                Text = "Check-in nhanh",
                Font = new Font(AppConstants.FontChuChinh, 13f, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 35
            };
            pnlRightCheckIn.Controls.Add(lblTieuDeCheckIn);
            lblTieuDeCheckIn.SendToBack();

            int y = 65;
            ThemNhanVaInput(pnlRightCheckIn, "Mã đặt phòng / CCCD", y, out txtCheckInNhanhCCCD);
            ThemNhanVaInput(pnlRightCheckIn, "Họ tên khách", y + 66, out txtCheckInNhanhKhach);
            ThemNhanVaInput(pnlRightCheckIn, "Số điện thoại", y + 132, out txtCheckInNhanhSDT);

            var lblP = new Label
            {
                Text = "Phòng", Location = new Point(16, y + 198), AutoSize = true,
                ForeColor = Color.FromArgb(100, 116, 139)
            };
            lblCheckInNhanhPhong = new Label
            {
                Text = "102 - Deluxe",
                Font = new Font(AppConstants.FontChuChinh, 11.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(16, y + 220),
                AutoSize = true
            };
            pnlRightCheckIn.Controls.AddRange(new Control[] { lblP, lblCheckInNhanhPhong });

            var lblNgay = new Label
            {
                Text = "Ngày trả phòng", Location = new Point(16, y + 256), AutoSize = true,
                ForeColor = Color.FromArgb(100, 116, 139)
            };
            dtpCheckInNhanhNgayTra = new DateTimePicker
            {
                Location = new Point(16, y + 280),
                Width = 279,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy",
                Value = DateTime.Today.AddDays(1)
            };
            pnlRightCheckIn.Controls.AddRange(new Control[] { lblNgay, dtpCheckInNhanhNgayTra });

            var btnQuickCheckIn = new Button
            {
                Text = "Check-in",
                Location = new Point(16, y + 332),
                Width = 122,
                Height = 38,
                UseMnemonic = false
            };
            UIHelper.DinhDangNutBamChinh(btnQuickCheckIn);
            btnQuickCheckIn.Click += (s, e) => XuLyCheckInNhanh();

            var btnQuickCheckOut = new Button
            {
                Text = "Check-out",
                Location = new Point(146, y + 332),
                Width = 126,
                Height = 38,
                UseMnemonic = false
            };
            UIHelper.DinhDangNutBamPhu(btnQuickCheckOut);
            btnQuickCheckOut.Click += (s, e) => XuLyCheckOutNhanh();

            btnDaDonXong = new Button
            {
                Text = "Đã dọn xong",
                Location = new Point(16, y + 380),
                Width = 256,
                Height = 38,
                Enabled = false,
                UseMnemonic = false
            };
            UIHelper.DinhDangNutBamPhu(btnDaDonXong);
            btnDaDonXong.Click += (s, e) => XuLyHoanTatDonPhong();

            pnlRightCheckIn.Controls.AddRange(new Control[] { btnQuickCheckIn, btnQuickCheckOut, btnDaDonXong });
            tblBody.Controls.Add(pnlRightCheckIn, 1, 0);
            tblRootSoDo.Controls.Add(tblBody, 0, 1);
            pnlSoDoPhong.Controls.Add(tblRootSoDo);
        }

        private void ThemNhanVaInput(Panel p, string nhan, int y, out TextBox txt)
        {
            var lbl = new Label { Text = nhan, Location = new Point(16, y), AutoSize = true, ForeColor = Color.FromArgb(100, 116, 139), UseMnemonic = false };
            var caoNhan = Math.Max(lbl.PreferredHeight, TextRenderer.MeasureText(nhan, lbl.Font).Height);
            var chieuRongO = Math.Max(220, p.Width - 36);
            txt = new TextBox
            {
                Location = new Point(16, y + caoNhan + 4),
                Width = chieuRongO,
                Height = 30,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            p.Controls.Add(lbl);
            p.Controls.Add(txt);
        }

        private void ThemChuThich(FlowLayoutPanel p, string ten, Color mau)
        {
            var pnlBox = new Panel { Size = new Size(18, 18), BackColor = mau, BorderStyle = BorderStyle.FixedSingle, Margin = new Padding(8, 2, 4, 0) };
            var lbl = new Label { Text = ten, AutoSize = true, Font = new Font(AppConstants.FontChuChinh, 10f), Margin = new Padding(0, 3, 16, 0) };
            p.Controls.Add(pnlBox);
            p.Controls.Add(lbl);
        }

        private void TaiDuLieuSoDoPhong()
        {
            UIHelper.RunSafe(() =>
            {
                var tk = _baoCaoBLL.GetDashboardStats();
                cardPhongTrong.Value = tk.AvailableRooms.ToString();
                cardDangCoKhach.Value = tk.OccupiedRooms.ToString();
                cardCheckInHomNay.Value = tk.TodayCheckIns.ToString();
                cardCheckOutHomNay.Value = tk.TodayCheckOuts.ToString();

                NapDanhSachThePhong();
            }, "tải dữ liệu sơ đồ phòng", this);
        }

        private void NapDanhSachThePhong()
        {
            UIHelper.RunSafe(() =>
            {
                tblLướiPhong.Controls.Clear();
                tblLướiPhong.RowStyles.Clear();

                List<Room> ds;
                if (_tangHienTai == 0)
                {
                    // 12 phòng tầng 1 và 2 (101-106, 201-206)
                    ds = _phongBLL.GetAll().FindAll(p => p.Floor == 1 || p.Floor == 2);
                }
                else
                {
                    ds = _phongBLL.GetByFloor(_tangHienTai);
                }

                int soDong = (int)Math.Ceiling(ds.Count / 4.0);
                tblLướiPhong.RowCount = soDong + 1;

                // Chiều cao chuẩn mỗi dòng phòng (88px)
                for (int r = 0; r < soDong; r++)
                {
                    tblLướiPhong.RowStyles.Add(new RowStyle(SizeType.Absolute, 88f));
                }

                // Hàng đệm giữ thẻ không bị kéo giãn
                tblLướiPhong.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

                for (int i = 0; i < ds.Count; i++)
                {
                    var phong = ds[i];
                    var card = new RoomCardControl { RoomInfo = phong, Dock = DockStyle.Fill };
                    card.OnRoomSelected += ChonPhongTrenSoDo;

                    int col = i % 4;
                    int row = i / 4;
                    tblLướiPhong.Controls.Add(card, col, row);
                }
            }, "nạp danh sách thẻ phòng", this);
        }

        private void ChonPhongTrenSoDo(Room phong)
        {
            UIHelper.RunSafe(() =>
            {
                _phongDangChon = phong;
                lblCheckInNhanhPhong.Text = $"{phong.RoomNumber} - {phong.RoomTypeName}";
                bool dangDon = phong.Status == AppConstants.PhongDangDon;
                btnDaDonXong.Enabled = dangDon;
                if (dangDon)
                {
                    btnDaDonXong.BackColor = Color.FromArgb(16, 185, 129);
                    btnDaDonXong.ForeColor = Color.White;
                }
                else
                {
                    UIHelper.DinhDangNutBamPhu(btnDaDonXong);
                }

                if (phong.Status == AppConstants.PhongCoKhach)
                {
                    var don = _datPhongBLL.GetActiveBookingByRoomNumber(phong.RoomNumber);
                    if (don != null)
                    {
                        txtCheckInNhanhKhach.Text = don.CustomerName;
                        txtCheckInNhanhCCCD.Text = don.CustomerNationalId;
                        var kh = !string.IsNullOrWhiteSpace(don.CustomerNationalId)
                            ? _khachHangBLL.GetByIdentityCardOrPhone(don.CustomerNationalId)
                            : null;
                        txtCheckInNhanhSDT.Text = kh?.PhoneNumber ?? string.Empty;
                        dtpCheckInNhanhNgayTra.Value = FormatHelper.ParseDate(don.CheckOutDate);
                    }
                }
                else
                {
                    txtCheckInNhanhKhach.Text = string.Empty;
                    txtCheckInNhanhCCCD.Text = string.Empty;
                    txtCheckInNhanhSDT.Text = string.Empty;
                }
            }, "tải thông tin phòng đang chọn", this);
        }

        private void XuLyCheckInNhanh()
{
    if (_phongDangChon == null)
    {
        MessageBox.Show("Vui lòng click chọn 1 phòng trên sơ đồ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
    }

    if (_phongDangChon.Status == AppConstants.PhongCoKhach)
    {
        MessageBox.Show($"Phòng {_phongDangChon.RoomNumber} hiện đang có khách!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
    }

    if (_phongDangChon.Status == AppConstants.PhongDangDon)
    {
        MessageBox.Show($"Phòng {_phongDangChon.RoomNumber} đang dọn dẹp, vui lòng bấm \"Đã dọn xong\" trước khi check-in!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
    }

    string hoTen = txtCheckInNhanhKhach.Text.Trim();
    string cccd = txtCheckInNhanhCCCD.Text.Trim();
    string sdt = txtCheckInNhanhSDT.Text.Trim();

    if (string.IsNullOrEmpty(hoTen) || string.IsNullOrEmpty(cccd))
    {
        MessageBox.Show("Vui lòng nhập đầy đủ Mã đặt phòng/CCCD và Họ tên khách!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
    }

    UIHelper.RunSafe(() =>
    {
        // Tìm đơn đặt phòng tương ứng
        var don = _datPhongBLL.GetAll().Find(d =>
            d.RoomNumber == _phongDangChon.RoomNumber &&
            (d.Status == BookingStatus.Confirmed || d.Status == BookingStatus.Pending));

        if (don != null)
        {
            bool kq = _datPhongBLL.CheckIn(don.BookingId, cccd, out string msg);
            MessageBox.Show(this, msg, "Kết quả Check-in", MessageBoxButtons.OK,
                kq ? MessageBoxIcon.Information : MessageBoxIcon.Error);
            TaiDuLieuSoDoPhong();
        }
        else
        {
            if (dtpCheckInNhanhNgayTra.Value.Date <= DateTime.Today)
            {
                MessageBox.Show(this, "Ngày trả phòng phải sau ngày hôm nay!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 1. Tìm hoặc tạo mới thông tin khách hàng
            var kh = _khachHangBLL.GetByIdentityCardOrPhone(cccd);
            int maKH = kh?.CustomerId ?? 0;
            if (maKH == 0)
            {
                if (string.IsNullOrWhiteSpace(sdt) || sdt.Length < 9)
                {
                    MessageBox.Show(this, "Vui lòng nhập số điện thoại khách hàng (tối thiểu 9 số)!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCheckInNhanhSDT.Focus();
                    return;
                }

                var khMoi = new Customer
                {
                    FullName = hoTen,
                    NationalId = cccd,
                    PhoneNumber = sdt
                };

                if (!_khachHangBLL.SaveCustomer(khMoi, out string msgKH))
                {
                    MessageBox.Show(this, msgKH, "Lỗi lưu khách hàng", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                maKH = khMoi.CustomerId > 0
                    ? khMoi.CustomerId
                    : (_khachHangBLL.GetByIdentityCardOrPhone(cccd)?.CustomerId ?? 0);
            }

            // 2. Tạo đơn đặt phòng
            var dpMoi = new Booking
            {
                CustomerId = maKH,
                EmployeeId = SessionManager.CurrentEmployee?.EmployeeId,
                CheckInDate = DateTime.Today.ToString("yyyy-MM-dd"),
                CheckOutDate = dtpCheckInNhanhNgayTra.Value.ToString("yyyy-MM-dd"),
                GuestCount = 1,
                Channel = "Trực tiếp"
            };

            bool taoDonOK = _datPhongBLL.CreateBooking(dpMoi, _phongDangChon.RoomId, _phongDangChon.BasePrice,
                out string msgTaoDon);
            if (!taoDonOK || dpMoi.BookingId <= 0)
            {
                MessageBox.Show(this, msgTaoDon, "Không thể tạo đơn đặt phòng", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            // 3. Xác nhận Check-in để đơn chuyển sang trạng thái CheckedIn
            bool checkInOK = _datPhongBLL.CheckIn(dpMoi.BookingId, cccd, out string msgCheckIn);
            if (checkInOK)
            {
                _phongBLL.UpdateStatus(_phongDangChon.RoomId, AppConstants.PhongCoKhach, out _);
                MessageBox.Show(this, $"Check-in thành công phòng {_phongDangChon.RoomNumber} cho khách {hoTen}!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                TaiDuLieuSoDoPhong();
            }
            else
            {
                MessageBox.Show(this, msgCheckIn, "Lỗi Check-in", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }, "xử lý check-in nhanh", this);
}

            private void XuLyCheckOutNhanh()
        {
            if (_phongDangChon == null)
            {
                MessageBox.Show("Vui lòng click chọn 1 phòng đang có khách trên sơ đồ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            UIHelper.RunSafe(() =>
            {
                var don = _datPhongBLL.GetActiveBookingByRoomNumber(_phongDangChon.RoomNumber);
                if (don == null)
                {
                    MessageBox.Show(this, $"Phòng {_phongDangChon.RoomNumber} hiện không có khách đang lưu trú!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                XuLyChuyenMenu("Check-in/Check-out");
                ChonDonDeCheckOut(don.BookingId);
            }, "chuyển sang check-out nhanh", this);
        }
        private void XuLyHoanTatDonPhong()
        {
            if (_phongDangChon == null)
            {
                MessageBox.Show("Vui lòng click chọn 1 phòng đang dọn trên sơ đồ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_phongDangChon.Status != AppConstants.PhongDangDon)
            {
                MessageBox.Show($"Phòng {_phongDangChon.RoomNumber} hiện không ở trạng thái Đang dọn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var xacNhan = MessageBox.Show(
                this,
                $"Xác nhận phòng {_phongDangChon.RoomNumber} đã dọn xong và chuyển sang Trống?",
                "Hoàn tất dọn phòng",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (xacNhan != DialogResult.Yes) return;

            UIHelper.RunSafe(() =>
            {
                bool kq = _phongBLL.UpdateStatus(_phongDangChon.RoomId, AppConstants.PhongTrong, out string msg);
                if (kq)
                {
                    _phongDangChon.Status = AppConstants.PhongTrong;
                    btnDaDonXong.Enabled = false;
                    UIHelper.DinhDangNutBamPhu(btnDaDonXong);
                    MessageBox.Show(this, $"Phòng {_phongDangChon.RoomNumber} đã dọn xong và sẵn sàng đón khách!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    TaiDuLieuSoDoPhong();
                }
                else
                {
                    MessageBox.Show(this, msg, "Lỗi cập nhật trạng thái", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }, "cập nhật trạng thái dọn phòng", this);
        }
}