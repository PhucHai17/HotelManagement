namespace HotelManagementSystem.Models;

// Lớp thực thể vai trò người dùng trong hệ thống
public class VaiTro
{
    public VaiTro()
    {
    }

    public VaiTro(int maVT, string tenVT)
    {
        MaVT = maVT;
        TenVT = tenVT;
    }

    public int MaVT { get; set; }
    public string TenVT { get; set; } = string.Empty;

    public override string ToString()
    {
        return TenVT;
    }
}