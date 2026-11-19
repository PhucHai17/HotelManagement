using BLL;
using Common.Constants;
using Common.Helpers;
using Entity.Entities;
using UI.Helpers;

namespace UI.Forms;

public class RoomDetailForm : Form
{
    private readonly RoomType _roomType;
    private Button btnChon = null!;
    private Button btnDong = null!;
    private Panel pnlFooter = null!;

    public RoomDetailForm(RoomType roomType)
    {
        _roomType = roomType ?? throw new ArgumentNullException(nameof(roomType));
        Font = new Font("Times New Roman", 10f, FontStyle.Regular);
        KhoiTaoGiaoDien();
    }

    // Khởi tạo bố cục hộp thoại xem chi tiết hạng phòng
    private void KhoiTaoGiaoDien()
    {
        Text = $"HotelPro - Chi tiết hạng phòng {_roomType.Name}";
        Size = new Size(580, 680);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Color.White;

        const int contentWidth = 516;
        const int leftMargin = 24;

        var pnlHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 56,
            BackColor = AppConstants.MauXanhNavy
        };
        var lblHeaderTitle = new Label
        {
            Text = $"CHI TIẾT HẠNG PHÒNG {_roomType.Name.ToUpper()}",
            ForeColor = Color.White,
            Font = new Font("Times New Roman", 16f, FontStyle.Bold),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter
        };
        pnlHeader.Controls.Add(lblHeaderTitle);

        pnlFooter = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 62,
            BackColor = AppConstants.MauNenChung,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(0)
        };

        btnChon = new Button
        {
            Text = "Chọn phòng này",
            Font = new Font("Times New Roman", 10f, FontStyle.Bold),
            Location = new Point(268, 12),
            Size = new Size(165, 38),
            UseMnemonic = false,
            Cursor = Cursors.Hand
        };
        UIHelper.DinhDangNutBamNavy(btnChon);
        btnChon.Click += (s, e) =>
        {
            DialogResult = DialogResult.OK;
            Close();
        };

        btnDong = new Button
        {
            Text = "Đóng",
            Font = new Font("Times New Roman", 10f, FontStyle.Regular),
            Location = new Point(445, 12),
            Size = new Size(95, 38),
            UseMnemonic = false,
            Cursor = Cursors.Hand
        };
        UIHelper.DinhDangNutBamPhu(btnDong);
        btnDong.Click += (s, e) =>
        {
            DialogResult = DialogResult.Cancel;
            Close();
        };

        pnlFooter.Controls.Add(btnChon);
        pnlFooter.Controls.Add(btnDong);

        void CapNhatViTriNutFooter()
        {
            btnDong.Location = new Point(445, 12);
            btnChon.Location = new Point(268, 12);
            btnDong.BringToFront();
            btnChon.BringToFront();
        }

        pnlFooter.Resize += (s, e) => CapNhatViTriNutFooter();
        Load += (s, e) => CapNhatViTriNutFooter();

        var pnlContent = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            AutoScroll = false,
            Padding = new Padding(0)
        };

        var picHeroImage = new PictureBox
        {
            Location = new Point(leftMargin, 16),
            Size = new Size(contentWidth, 210),
            BackColor = Color.FromArgb(245, 247, 250),
            Image = ImageHelper.LoadRoomImage(_roomType.Name),
            SizeMode = PictureBoxSizeMode.Zoom
        };

        var lblTenPhong = new Label
        {
            Text = $"Hạng phòng: {_roomType.Name}",
            Font = new Font("Times New Roman", 14f, FontStyle.Bold),
            ForeColor = AppConstants.MauChuChinh,
            Location = new Point(leftMargin, 238),
            Size = new Size(230, 28),
            TextAlign = ContentAlignment.MiddleLeft
        };

        var lblGiaPhong = new Label
        {
            Text = $"Giá: {FormatHelper.FormatCurrency(_roomType.BasePrice)} / đêm",
            Font = new Font("Times New Roman", 13f, FontStyle.Bold),
            ForeColor = AppConstants.MauVangCamDam,
            Location = new Point(260, 238),
            Size = new Size(280, 28),
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleRight
        };

        var soPhongTrong = 0;
        try
        {
            soPhongTrong = AppServices.Room.GetAvailableRooms(_roomType.RoomTypeId).Count;
        }
        catch (Exception ex)
        {
            AppLogger.Error(ex, "Lấy số lượng phòng trống theo loại phòng");
        }

        var lblSucChua = new Label
        {
            Text = $"• Sức chứa: {_roomType.Capacity} người lớn",
            Font = new Font("Times New Roman", 10f, FontStyle.Regular),
            ForeColor = AppConstants.MauChuPhu,
            Location = new Point(leftMargin, 270),
            Size = new Size(230, 24),
            TextAlign = ContentAlignment.MiddleLeft
        };

        var lblTrangThai = new Label
        {
            Text = soPhongTrong > 0 ? $"• Tình trạng: Còn {soPhongTrong} phòng trống" : "• Tình trạng: Tạm hết phòng",
            Font = new Font("Times New Roman", 10f, FontStyle.Bold),
            ForeColor = soPhongTrong > 0 ? AppConstants.MauXanhLa : AppConstants.MauDo,
            Location = new Point(260, 270),
            Size = new Size(280, 24),
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleRight
        };

        var pnlDivider1 = new Panel
        {
            Location = new Point(leftMargin, 298),
            Size = new Size(contentWidth, 1),
            BackColor = AppConstants.MauVien
        };

        var lblTieuDeMoTa = new Label
        {
            Text = "Mô tả phòng",
            Font = new Font("Times New Roman", 12f, FontStyle.Bold),
            ForeColor = AppConstants.MauChuChinh,
            Location = new Point(leftMargin, 306),
            AutoSize = true
        };

        var noiDungMoTa = string.IsNullOrWhiteSpace(_roomType.Description)
            ? "Phòng nghỉ được thiết kế sang trọng, thoáng đãng với tầm nhìn đẹp mắt và trang thiết bị hiện đại, mang lại không gian thư giãn tối ưu cho quý khách trong suốt kỳ nghỉ."
            : _roomType.Description;

        var lblMoTa = new Label
        {
            Text = noiDungMoTa,
            Font = new Font("Times New Roman", 10f, FontStyle.Regular),
            ForeColor = AppConstants.MauChuChinh,
            Location = new Point(leftMargin, 328),
            Size = new Size(contentWidth, 44)
        };

        var pnlDivider2 = new Panel
        {
            Location = new Point(leftMargin, 378),
            Size = new Size(contentWidth, 1),
            BackColor = AppConstants.MauVien
        };

        var lblTieuDeTienNghi = new Label
        {
            Text = "Tiện nghi nổi bật",
            Font = new Font("Times New Roman", 12f, FontStyle.Bold),
            ForeColor = AppConstants.MauChuChinh,
            Location = new Point(leftMargin, 386),
            AutoSize = true
        };

        var tblTienNghi = new TableLayoutPanel
        {
            Location = new Point(leftMargin, 408),
            Size = new Size(contentWidth, 104),
            ColumnCount = 2,
            RowCount = 4,
            Margin = new Padding(0),
            Padding = new Padding(0)
        };
        tblTienNghi.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
        tblTienNghi.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
        for (var r = 0; r < 4; r++)
        {
            tblTienNghi.RowStyles.Add(new RowStyle(SizeType.Absolute, 26f));
        }

        var rawAmenities = string.IsNullOrWhiteSpace(_roomType.Amenities)
            ? "Wi-Fi miễn phí, Điều hòa 2 chiều, TV truyền hình cáp, Ban công ngắm cảnh, Minibar, Bồn tắm nằm"
            : _roomType.Amenities;

        var dsTienNghi = rawAmenities
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToList();

        if (dsTienNghi.Count == 0)
        {
            dsTienNghi = new List<string>
            {
                "Wi-Fi tốc độ cao miễn phí",
                "Điều hòa không khí 2 chiều",
                "TV màn hình phẳng truyền hình cáp",
                "Ban công ngắm cảnh thoáng mát",
                "Minibar & Nước suối miễn phí",
                "Bồn tắm nằm & Vòi sen cao cấp"
            };
        }

        for (var i = 0; i < Math.Min(dsTienNghi.Count, 8); i++)
        {
            var col = i % 2;
            var row = i / 2;
            var lblItem = new Label
            {
                Text = $"✓  {dsTienNghi[i]}",
                Font = new Font("Times New Roman", 10f, FontStyle.Regular),
                ForeColor = AppConstants.MauChuChinh,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoEllipsis = true
            };
            tblTienNghi.Controls.Add(lblItem, col, row);
        }

        pnlContent.Controls.Add(picHeroImage);
        pnlContent.Controls.Add(lblTenPhong);
        pnlContent.Controls.Add(lblGiaPhong);
        pnlContent.Controls.Add(lblSucChua);
        pnlContent.Controls.Add(lblTrangThai);
        pnlContent.Controls.Add(pnlDivider1);
        pnlContent.Controls.Add(lblTieuDeMoTa);
        pnlContent.Controls.Add(lblMoTa);
        pnlContent.Controls.Add(pnlDivider2);
        pnlContent.Controls.Add(lblTieuDeTienNghi);
        pnlContent.Controls.Add(tblTienNghi);

        Controls.Add(pnlContent);
        Controls.Add(pnlFooter);
        Controls.Add(pnlHeader);

        CapNhatViTriNutFooter();
    }
}
