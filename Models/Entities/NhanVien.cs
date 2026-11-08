namespace HotelManagementSystem.Models;

// Lớp thực thể nhân viên khách sạn
public class NhanVien
{
    public int MaNV { get; set; }
    public int MaTK { get; set; }
    public int? MaQL { get; set; }
    public string HoTen { get; set; } = string.Empty;
    public string SDT { get; set; } = string.Empty;
    public string CCCD { get; set; } = string.Empty;
    public string ChucVu { get; set; } = "Lễ tân";
    public string NgayVaoLam { get; set; } = string.Empty;
    public string TrangThai { get; set; } = "Đang làm việc";
}