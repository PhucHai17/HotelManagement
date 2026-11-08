namespace HotelManagementSystem.Models;

// Thông tin ca dọn dẹp phòng
public class DonDepPhong
{
    public int MaDD { get; set; }
    public int MaPhong { get; set; }
    public int? MaNV { get; set; }
    public string TrangThai { get; set; } = "Cần dọn"; // Cần dọn, Đang dọn, Đã xong
    public string NgayXong { get; set; } = string.Empty;

    public string SoPhong { get; set; } = string.Empty;
    public string TenNhanVien { get; set; } = string.Empty;
}