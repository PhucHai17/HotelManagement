namespace HotelManagementSystem.Models;

// Thông tin bảo trì thiết bị trong phòng
public class BaoTriPhong
{
    public int MaBT { get; set; }
    public int MaPhong { get; set; }
    public string MoTa { get; set; } = string.Empty;
    public string TrangThai { get; set; } = "Chờ xử lý"; // Chờ xử lý, Đang sửa, Hoàn tất

    public string SoPhong { get; set; } = string.Empty;
}