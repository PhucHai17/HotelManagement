namespace HotelManagementSystem.Models;
// Lớp thực thể khách hàng
public class KhachHang
{
    public int MaKH { get; set; }
    public int? MaTK { get; set; }
    public int MaHang { get; set; } = 1;
    public string HoTen { get; set; } = string.Empty;
    public string SDT { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string CCCD { get; set; } = string.Empty;
    public int DiemTichLuy { get; set; } = 0;

    // Dữ liệu mở rộng để hiển thị lên bảng
    public string TenHang { get; set; } = "Thường";
    public int SoLanO { get; set; } = 0;
    public string GhiChu { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"{HoTen} ({SDT})";
    }
}