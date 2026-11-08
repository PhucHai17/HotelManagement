namespace HotelManagementSystem.Models;

// Lớp thực thể giao dịch thanh toán
public class ThanhToan
{
    public int MaTT { get; set; }
    public int MaHD { get; set; }
    public double SoTien { get; set; }
    public string HinhThuc { get; set; } = "Tiền mặt"; // Tiền mặt, Thẻ, Chuyển khoản
    public string LoaiGD { get; set; } = "Thanh toán hóa đơn";
    public string NgayTT { get; set; } = string.Empty;
}