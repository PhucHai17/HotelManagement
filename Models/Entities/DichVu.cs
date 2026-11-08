namespace HotelManagementSystem.Models;

// Lớp thực thể dịch vụ khách sạn
public class DichVu
{
    public int MaDV { get; set; }
    public string TenDV { get; set; } = string.Empty;
    public double DonGia { get; set; }
    public string DonVi { get; set; } = string.Empty; // suất, kg, món, lượt, vé
    public string MoTa { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"{TenDV} - {DonGia:N0} đ/{DonVi}";
    }
}