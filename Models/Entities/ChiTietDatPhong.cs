namespace HotelManagementSystem.Models;

// Chi tiết phòng được đặt trong đơn
public class ChiTietDatPhong
{
    public int MaCTDP { get; set; }
    public int MaDP { get; set; }
    public int MaPhong { get; set; }
    public double GiaTaiThoiDiem { get; set; }
    public string NgayNhanTT { get; set; } = string.Empty;
    public string NgayTraTT { get; set; } = string.Empty;

    // Bổ sung thông tin phòng
    public string SoPhong { get; set; } = string.Empty;
    public string TenLoai { get; set; } = string.Empty;
}