namespace Common.Constants ;

public class AppConstants
{
    public const string ApplicationName = "Hệ Thống Quản Lý Khách Sạn Và Đặt Phòng";
    public const string DbFileName = "Hotelmanagement.db";
    public const string CurrencyCulture = "vi-VN";
    public const string DefaultFontName = "Times New Roman";
    public const string DateFormat = "dd/MM/yyyy";
    public const string DateTimeFormat = "dd/MM/yyyy HH:mm";
    public const string IsoDateFormat = "yyyy-MM-dd";
    public const double VatRate = 0.08;
    public const double FallbackNightlyRate = 500000;
    public const int FallbackEmployeeId = 2;
    public const int FallbackCustomerId = 1;
    public const string TimeFormat = "HH:mm";

    // Font chữ hệ thống
    public const string FontChuChinh = "Segoe UI";
    public const string PrimaryFontFamily = "Segoe UI";

    // Bảng màu giao diện (theo wireframe HotelPro)
    public static readonly Color MauXanhNavy = Color.FromArgb(30, 51, 90);         // Xanh Navy chính (#1E335A)
    public static readonly Color MauXanhNavyDam = Color.FromArgb(20, 35, 64);       // Xanh Navy đậm (#142340)
    public static readonly Color MauVangCam = Color.FromArgb(201, 162, 77);        // Vàng đồng nổi bật (#C9A24D)
    public static readonly Color MauVangCamDam = Color.FromArgb(182, 142, 59);     // Vàng đồng hover (#B68E3B)
    public static readonly Color MauChuTrenVang = Color.FromArgb(30, 41, 59);      // Chữ đậm trên nút vàng (#1E293B)
    public static readonly Color MauXanhLa = Color.FromArgb(22, 163, 74);          // Xanh lá (#16A34A)
    public static readonly Color MauDo = Color.FromArgb(220, 38, 38);              // Đỏ (#DC2626)
    public static readonly Color MauVang = Color.FromArgb(217, 154, 43);           // Vàng cam (#D99A2B)
    public static readonly Color MauXanhDuong = Color.FromArgb(37, 99, 235);       // Xanh dương (#2563EB)
    public static readonly Color MauXam = Color.FromArgb(100, 116, 139);           // Xám trung tính (#64748B)
    public static readonly Color MauNen = Color.FromArgb(248, 250, 252);           // Nền sáng (#F8FAFC)
    public static readonly Color MauNenChung = Color.FromArgb(243, 245, 249);      // Nền dashboard (#F3F5F9)
    public static readonly Color MauVien = Color.FromArgb(226, 232, 240);          // Viền thẻ (#E2E8F0)
    public static readonly Color MauVienInput = Color.FromArgb(203, 213, 225);     // Viền ô nhập & nút phụ (#CBD5E1)
    public static readonly Color MauChuChinh = Color.FromArgb(30, 41, 59);         // Chữ chính đậm (#1E293B)
    public static readonly Color MauChuPhu = Color.FromArgb(100, 116, 139);        // Chữ phụ (#64748B)

    // Màu thẻ trạng thái phòng
    public static readonly Color MauPhongTrong = Color.FromArgb(220, 252, 231);
    public static readonly Color MauChuPhongTrong = Color.FromArgb(22, 101, 52);
    public static readonly Color MauPhongCoKhach = Color.FromArgb(254, 226, 226);
    public static readonly Color MauChuPhongCoKhach = Color.FromArgb(185, 28, 28);
    public static readonly Color MauPhongDaDat = Color.FromArgb(254, 243, 199);
    public static readonly Color MauChuPhongDaDat = Color.FromArgb(180, 83, 9);
    public static readonly Color MauPhongDangDon = Color.FromArgb(219, 234, 254);
    public static readonly Color MauChuPhongDangDon = Color.FromArgb(29, 78, 216);

    // Tên màu tiếng Anh tương thích
    public static Color PrimaryColor => MauXanhNavy;
    public static Color SecondaryColor => MauVangCam;
    public static Color SuccessColor => MauXanhLa;
    public static Color DangerColor => MauDo;
    public static Color WarningColor => MauVang;
    public static Color BackgroundColor => MauNen;
    public static Color BorderColor => MauVien;
    public static Color TextPrimaryColor => MauChuChinh;
    public static Color TextSecondaryColor => MauChuPhu;

    // Trạng thái phòng
    public const string PhongTrong = "Trống";
    public const string PhongCoKhach = "Có khách";
    public const string PhongDaDat = "Đã đặt";
    public const string PhongDangDon = "Đang dọn";

    // Trạng thái đặt phòng
    public const string DPChoDuyet = "Chờ duyệt";
    public const string DPDaXacNhan = "Đã xác nhận";
    public const string DPDangO = "Đang ở";
    public const string DPDaHoanTat = "Đã hoàn tất";
    public const string DPDaHuy = "Đã hủy";

    // Trạng thái hóa đơn
    public const string HDChuaThanhToan = "Chưa thanh toán";
    public const string HDDaThanhToan = "Đã thanh toán";

    // Trạng thái tài khoản chuẩn lưu DB & hiển thị
    public const string AccountStatusActive = "HoatDong";
    public const string AccountStatusLocked = "BiKhoa";
    public const string AccountStatusActiveDisplay = "Hoạt động";
    public const string AccountStatusLockedDisplay = "Đã khóa";

    // Trạng thái nhân viên chuẩn
    public const string EmployeeStatusWorking = "Đang làm việc";
    public const string EmployeeStatusLocked = "Đã khóa";

    // Vai trò người dùng
    public const string VTRoleAdmin = "Quản lý";
    public const string VTRoleLeTan = "Lễ tân";
    public const string VTRoleKhachHang = "Khách hàng";
    public const string VTRoleQuanLy = "Quản lý";

    // Mã vai trò (khớp bảng VaiTro)
    public const int MaVTAdmin = 1;
    public const int MaVTLeTan = 2;
    public const int MaVTKhachHang = 3;
    public const int MaVTQuanLy = 4;

    // Tên quyền hệ thống
    public const string QuyenQuanTriHeThong = "QuanTriHeThong";
    public const string QuyenQuanLyPhong = "QuanLyPhong";
    public const string QuyenQuanLyDatPhong = "QuanLyDatPhong";
    public const string QuyenXemBaoCao = "XemBaoCao";
    public const string QuyenDatPhongOnline = "DatPhongOnline";
    public const string QuyenQuanLyNhanVien = "QuanLyNhanVien";
    public const string QuyenQuanLyGia = "QuanLyGia";
    public const string QuyenXemNhatKy = "XemNhatKy";
}