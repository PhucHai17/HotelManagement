namespace HotelManagementSystem.Models;

// Thông tin ca làm việc thực tế và bàn giao tiền mặt
public class CaLamViec
{
    public int MaCa { get; set; }
    public int MaNV { get; set; }
    public string GioBatDau { get; set; } = string.Empty;
    public string GioKetThuc { get; set; } = string.Empty;
    public double TienDauCa { get; set; } = 0.0;
    public double TienCuoiCa { get; set; } = 0.0;
    public string GhiChuBanGiao { get; set; } = string.Empty;

    public string TenNV { get; set; } = string.Empty;
}