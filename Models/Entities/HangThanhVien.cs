namespace HotelManagementSystem.Models;

// Lớp thực thể hạng thành viên của khách hàng
public class HangThanhVien
{
    public int MaHang { get; set; }
    public string TenHang { get; set; } = "Thường";
    public int DiemToiThieu { get; set; } = 0;
    public double PhanTramGiam { get; set; } = 0.0;
}