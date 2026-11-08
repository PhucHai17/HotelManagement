namespace HotelManagementSystem.Models;

// Lịch phân ca làm việc của nhân viên
public class LichPhanCa
{
    public int MaLPC { get; set; }
    public int MaNV { get; set; }
    public int? MaQL { get; set; }
    public string NgayLam { get; set; } = string.Empty;
    public string LoaiCa { get; set; } = "Ca sáng"; // Ca sáng, Ca chiều, Ca đêm

    public string TenNV { get; set; } = string.Empty;
}