namespace HotelManagementSystem.Models;

// Đánh giá trải nghiệm của khách hàng sau khi trả phòng
public class DanhGia
{
    public int MaDG { get; set; }
    public int MaKH { get; set; }
    public int MaDP { get; set; }
    public int SoSao { get; set; } = 5;
    public string NoiDung { get; set; } = string.Empty;

    public string TenKhach { get; set; } = string.Empty;
}