namespace HotelManagementSystem.Models;

// Lớp thực thể loại phòng
public class LoaiPhong
{
    public int MaLoai { get; set; }
    public string TenLoai { get; set; } = string.Empty;
    public double GiaCoBan { get; set; }
    public int SucChua { get; set; } = 2;
    public string MoTa { get; set; } = string.Empty;
    public string TienNghi { get; set; } = "Wi-Fi, Điều hòa, TV, Ban công, Minibar, Bồn tắm";
    public string HinhAnh { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"{TenLoai} - {GiaCoBan:N0} đ/đêm";
    }
}