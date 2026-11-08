namespace HotelManagementSystem.Models;

// Lớp thực thể đơn đặt phòng
public class DatPhong
{
    public int MaDP { get; set; }
    public int MaKH { get; set; }
    public int? MaNV { get; set; }
    public int? MaKM { get; set; }
    public string NgayNhan { get; set; } = string.Empty;
    public string NgayTra { get; set; } = string.Empty;
    public int SoKhach { get; set; } = 1;
    public double TienCoc { get; set; } = 0.0;
    public string KenhDat { get; set; } = "Trực tiếp"; // Trực tiếp, Trực tuyến
    public string TrangThai { get; set; } = "Chờ duyệt"; // Chờ duyệt, Đã xác nhận, Đang ở, Đã hoàn tất, Đã hủy

    // Thông tin mở rộng hiển thị danh sách
    public string TenKhach { get; set; } = string.Empty;
    public string SDTKhach { get; set; } = string.Empty;
    public string CCCD { get; set; } = string.Empty;
    public string SoPhong { get; set; } = string.Empty;
    public string TenLoaiPhong { get; set; } = string.Empty;
    public double TongTien { get; set; }

    // Hiển thị mã dạng DP001
    public string MaHienThi => $"DP{MaDP:D3}";
}