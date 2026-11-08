namespace HotelManagementSystem.Models;

// Thông tin khách đi cùng
public class KhachDiCung
{
    public int MaKDC { get; set; }
    public int MaCTDP { get; set; }
    public string HoTen { get; set; } = string.Empty;
    public string CCCD { get; set; } = string.Empty;
}