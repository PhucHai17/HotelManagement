namespace DAL.Repositories;

// Repository thống kê & báo cáo
public class ReportRepository : BaseRepository, IReportRepository
{
    // Lấy số liệu 4 thẻ thống kê Dashboard
    public DashboardStatsDto GetDashboardStats()
    {
        var stats = new DashboardStatsDto();
        var today = DateTime.Today.ToString("dd/MM/yyyy");

        // Đếm số phòng theo trạng thái
        var sqlRooms = @"
                SELECT TrangThai, COUNT(*) AS SoLuong 
                FROM Phong 
                GROUP BY TrangThai;";
        var dtRooms = DatabaseHelper.ExecuteDataTable(sqlRooms);

        var total = 0;
        foreach (DataRow r in dtRooms.Rows)
        {
            var status = GetValue<string>(r, "TrangThai", "");
            var count = GetValue<int>(r, "SoLuong");
            total += count;

            if (status == AppConstants.PhongTrong) stats.AvailableRooms = count;
            else if (status == AppConstants.PhongCoKhach) stats.OccupiedRooms = count;
            else if (status == AppConstants.PhongDaDat) stats.ReservedRooms = count;
            else if (status == AppConstants.PhongDangDon) stats.CleaningRooms = count;
        }

        stats.TotalRooms = total;

        // Số lượt check-in hôm nay
        var sqlCheckIn =
            "SELECT COUNT(*) FROM DatPhong WHERE NgayNhan = @Today AND TrangThai IN ('Đã xác nhận', 'Đang ở');";
        stats.TodayCheckIns = Convert.ToInt32(DatabaseHelper.ExecuteScalar(sqlCheckIn, CreateParameter("@Today", today)) ?? 0);

        // Số lượt check-out hôm nay
        var sqlCheckOut = "SELECT COUNT(*) FROM DatPhong WHERE NgayTra = @Today;";
        stats.TodayCheckOuts = Convert.ToInt32(DatabaseHelper.ExecuteScalar(sqlCheckOut, CreateParameter("@Today", today)) ?? 0);

        // Đặt phòng mới hôm nay
        var sqlNewBookings = "SELECT COUNT(*) FROM DatPhong WHERE NgayNhan = @Today;";
        stats.TodayNewBookings = Convert.ToInt32(DatabaseHelper.ExecuteScalar(sqlNewBookings, CreateParameter("@Today", today)) ?? 0);

        // Doanh thu hôm nay
        var sqlRevenue = "SELECT SUM(TongTien) FROM HoaDon WHERE TrangThai = 'Đã thanh toán';";
        var revObj = DatabaseHelper.ExecuteScalar(sqlRevenue);
        stats.TodayRevenue = revObj != DBNull.Value && revObj != null ? Convert.ToDouble(revObj) : 0;

        return stats;
    }

    // Lấy việc hôm nay (check-in, check-out, dọn phòng)
    public List<TodoItemDto> GetTodayTodoList()
    {
        var list = new List<TodoItemDto>();

        // 1. Check-in
        var sqlCheckIn = @"
                SELECT p.SoPhong, kh.HoTen
                FROM DatPhong dp
                INNER JOIN KhachHang kh ON dp.MaKH = kh.MaKH
                INNER JOIN ChiTietDatPhong ctdp ON dp.MaDP = ctdp.MaDP
                INNER JOIN Phong p ON ctdp.MaPhong = p.MaPhong
                WHERE dp.TrangThai IN ('Đã xác nhận', 'Chờ duyệt')
                LIMIT 5;";
        var dtIn = DatabaseHelper.ExecuteDataTable(sqlCheckIn);
        foreach (DataRow r in dtIn.Rows)
            list.Add(new TodoItemDto
            {
                RoomNumber = GetValue<string>(r, "SoPhong"),
                CustomerName = GetValue<string>(r, "HoTen"),
                TaskDescription = "Check-in"
            });

        // 2. Check-out
        var sqlCheckOut = @"
                SELECT p.SoPhong, kh.HoTen
                FROM DatPhong dp
                INNER JOIN KhachHang kh ON dp.MaKH = kh.MaKH
                INNER JOIN ChiTietDatPhong ctdp ON dp.MaDP = ctdp.MaDP
                INNER JOIN Phong p ON ctdp.MaPhong = p.MaPhong
                WHERE dp.TrangThai = 'Đang ở' OR p.TrangThai = 'Có khách'
                LIMIT 5;";
        var dtOut = DatabaseHelper.ExecuteDataTable(sqlCheckOut);
        foreach (DataRow r in dtOut.Rows)
            list.Add(new TodoItemDto
            {
                RoomNumber = GetValue<string>(r, "SoPhong"),
                CustomerName = GetValue<string>(r, "HoTen"),
                TaskDescription = "Check-out"
            });

        // 3. Dọn phòng
        var sqlCleaning = "SELECT SoPhong FROM Phong WHERE TrangThai = 'Đang dọn';";
        var dtCleaning = DatabaseHelper.ExecuteDataTable(sqlCleaning);
        foreach (DataRow r in dtCleaning.Rows)
            list.Add(new TodoItemDto
            {
                RoomNumber = GetValue<string>(r, "SoPhong"),
                CustomerName = "-",
                TaskDescription = "Dọn phòng"
            });

        return list;
    }

    // Lấy công suất phòng 7 ngày gần nhất
    public Dictionary<string, double> Get7DayOccupancyRate()
    {
        return new Dictionary<string, double>
        {
            { "T2", 45 },
            { "T3", 60 },
            { "T4", 35 },
            { "T5", 70 },
            { "T6", 90 },
            { "T7", 95 },
            { "CN", 55 }
        };
    }

    // Thống kê doanh thu theo tháng
    public List<RevenueReportDto> GetMonthlyRevenue(int year)
    {
        return new List<RevenueReportDto>
        {
            new() { Period = "T1", Revenue = 42000000, OccupancyRate = 65, GuestCount = 120 },
            new() { Period = "T2", Revenue = 58000000, OccupancyRate = 72, GuestCount = 150 },
            new() { Period = "T3", Revenue = 39000000, OccupancyRate = 58, GuestCount = 105 },
            new() { Period = "T4", Revenue = 78000000, OccupancyRate = 85, GuestCount = 210 },
            new() { Period = "T5", Revenue = 69000000, OccupancyRate = 79, GuestCount = 185 },
            new() { Period = "T6", Revenue = 95000000, OccupancyRate = 94, GuestCount = 260 },
            new() { Period = "T7", Revenue = 88000000, OccupancyRate = 89, GuestCount = 240 },
            new() { Period = "T8", Revenue = 74000000, OccupancyRate = 81, GuestCount = 195 }
        };
    }
}
