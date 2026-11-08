namespace HotelManagementSystem.Models;

// Nhật ký ghi nhận các thao tác trên hệ thống
public class NhatKyHeThong
{
    public int MaNK { get; set; }
    public int MaTK { get; set; }
    public string HanhDong { get; set; } = string.Empty;
    public string BangTacDong { get; set; } = string.Empty;
    public string ThoiGian { get; set; } = string.Empty;

    public string TenDangNhap { get; set; } = string.Empty;
}