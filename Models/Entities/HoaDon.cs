namespace HotelManagementSystem.Models;

// Lớp thực thể hóa đơn thanh toán
public class HoaDon
{
    public int MaHD { get; set; }
    public int MaDP { get; set; }
    public int? MaNV { get; set; }
    public int? MaCa { get; set; }
    public double TienPhong { get; set; }
    public double TienDV { get; set; }
    public double GiamGia { get; set; }
    public double VAT { get; set; } // Thường là 8% hoặc 10%
    public double TongTien { get; set; }
    public string TrangThai { get; set; } = "Chưa thanh toán"; // Đã thanh toán, Chưa thanh toán
    public string NgayLap { get; set; } = string.Empty;

    // Thông tin mở rộng
    public string TenKhach { get; set; } = string.Empty;
    public string SoPhong { get; set; } = string.Empty;
    public string MaHienThi => $"HD{MaHD:D3}";
}