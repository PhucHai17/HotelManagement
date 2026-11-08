namespace HotelManagementSystem.Models;

// DTO thống kê nhanh cho Dashboard Tổng quan & Sơ đồ phòng
public class ThongKeDashboard
{
    public int PhongTrong { get; set; }
    public int DangCoKhach { get; set; }
    public int DaDat { get; set; }
    public int DangDon { get; set; }
    public int TongSoPhong { get; set; }

    public int CheckInHomNay { get; set; }
    public int CheckOutHomNay { get; set; }
    public double DoanhThuHomNay { get; set; }
    public int DatMoiHomNay { get; set; }
    public double CongSuatPhong => TongSoPhong > 0 ? DangCoKhach * 100.0 / TongSoPhong : 0;
}

// Danh sách việc cần làm trong ngày (Check-in, Check-out, Dọn phòng)
public class ViecCanLam
{
    public string Phong { get; set; } = string.Empty;
    public string Khach { get; set; } = string.Empty;
    public string Viec { get; set; } = string.Empty; // "Check-in", "Check-out", "Dọn phòng"
}

// DTO phục vụ xuất báo cáo và vẽ biểu đồ doanh thu
public class BaoCaoDoanhThu
{
    public string ThoiGian { get; set; } = string.Empty; // Ví dụ: "T1", "T2" hoặc ngày
    public double DoanhThu { get; set; }
    public double CongSuat { get; set; }
    public int SoLuotKhach { get; set; }
}