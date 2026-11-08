namespace HotelManagementSystem.Models;

// Lớp thực thể mã khuyến mãi
public class KhuyenMai
{
    public int MaKM { get; set; }
    public string MaCode { get; set; } = string.Empty;
    public double PhanTramGiam { get; set; }
    public string TuNgay { get; set; } = string.Empty;
    public string DenNgay { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"{MaCode} (-{PhanTramGiam}%)";
    }
}