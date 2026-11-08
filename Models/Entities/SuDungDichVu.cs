namespace HotelManagementSystem.Models;

// Lớp thực thể sử dụng dịch vụ cho từng phòng đã đặt
public class SuDungDichVu
{
    public int MaSDDV { get; set; }
    public int MaCTDP { get; set; }
    public int MaDV { get; set; }
    public int SoLuong { get; set; } = 1;
    public double DonGia { get; set; }
    public string NgaySD { get; set; } = string.Empty;

    // Bổ sung thông tin dịch vụ
    public string TenDV { get; set; } = string.Empty;
    public string DonVi { get; set; } = string.Empty;
    public string SoPhong { get; set; } = string.Empty;
    public double ThanhTien => SoLuong * DonGia;
}