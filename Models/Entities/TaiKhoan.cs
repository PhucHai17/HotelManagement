namespace HotelManagementSystem.Models;

// Lớp thực thể tài khoản đăng nhập
public class TaiKhoan
{
    public TaiKhoan()
    {
    }

    public TaiKhoan(int maTK, int maVT, string tenDangNhap, string matKhauHash, string trangThai)
    {
        MaTK = maTK;
        MaVT = maVT;
        TenDangNhap = tenDangNhap;
        MatKhauHash = matKhauHash;
        TrangThai = trangThai;
    }

    public int MaTK { get; set; }
    public int MaVT { get; set; }
    public string TenDangNhap { get; set; } = string.Empty;
    public string MatKhauHash { get; set; } = string.Empty;
    public int SoLanSaiMK { get; set; }
    public string TrangThai { get; set; } = "Hoạt động";

    // Thuộc tính mở rộng để hiển thị giao diện
    public string TenVaiTro { get; set; } = string.Empty;
}