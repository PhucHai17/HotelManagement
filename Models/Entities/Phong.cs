namespace HotelManagementSystem.Models;

// Lớp thực thể phòng khách sạn
public class Phong
{
    public int MaPhong { get; set; }
    public int MaLoai { get; set; }
    public string SoPhong { get; set; } = string.Empty;
    public int Tang { get; set; } = 1;
    public string TrangThai { get; set; } = "Trống"; // Trống, Có khách, Đã đặt, Đang dọn

    // Thông tin mở rộng từ bảng LoaiPhong
    public string TenLoai { get; set; } = string.Empty;
    public double GiaCoBan { get; set; }
    public int SucChua { get; set; } = 2;

    public override string ToString()
    {
        return $"{SoPhong} - {TenLoai}";
    }
}