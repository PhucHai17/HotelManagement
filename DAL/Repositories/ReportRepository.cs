using System.Globalization;

namespace DAL.Repositories;

// Repository thống kê & báo cáo
public class ReportRepository : BaseRepository, IReportRepository
{
    public DashboardStatsDto GetDashboardStats()
    {
        var stats = new DashboardStatsDto();
        var today = DateTime.Today.ToString("yyyy-MM-dd");

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

        var sqlCheckIn = @"
            SELECT COUNT(*) 
            FROM DatPhong 
            WHERE substr(NgayNhan, 1, 10) = @Today 
              AND TrangThai IN ('Chờ duyệt', 'Đã xác nhận', 'Đang ở');";
        stats.TodayCheckIns = Convert.ToInt32(
            DatabaseHelper.ExecuteScalar(sqlCheckIn, CreateParameter("@Today", today)) ?? 0);

        var sqlCheckOut = @"
            SELECT COUNT(*) 
            FROM DatPhong 
            WHERE substr(NgayTra, 1, 10) = @Today 
              AND TrangThai IN ('Đang ở', 'Đã trả phòng');";
        stats.TodayCheckOuts = Convert.ToInt32(
            DatabaseHelper.ExecuteScalar(sqlCheckOut, CreateParameter("@Today", today)) ?? 0);

        var sqlNewBookings = @"
            SELECT COUNT(*) 
            FROM DatPhong 
            WHERE substr(NgayNhan, 1, 10) = @Today;";
        stats.TodayNewBookings = Convert.ToInt32(
            DatabaseHelper.ExecuteScalar(sqlNewBookings, CreateParameter("@Today", today)) ?? 0);

        var sqlRevenue = "SELECT COALESCE(SUM(TongTien), 0) FROM HoaDon WHERE TrangThai = 'Đã thanh toán';";
        var revObj = DatabaseHelper.ExecuteScalar(sqlRevenue);
        stats.TodayRevenue = revObj != DBNull.Value && revObj != null ? Convert.ToDouble(revObj) : 0;

        return stats;
    }

    // Lấy việc hôm nay (check-in, check-out, dọn phòng)
    public List<TodoItemDto> GetTodayTodoList()
    {
        var list = new List<TodoItemDto>();
        var today = DateTime.Today.ToString("yyyy-MM-dd");

        var sqlCheckIn = @"
                SELECT p.SoPhong, kh.HoTen
                FROM DatPhong dp
                INNER JOIN KhachHang kh ON dp.MaKH = kh.MaKH
                INNER JOIN ChiTietDatPhong ctdp ON dp.MaDP = ctdp.MaDP
                INNER JOIN Phong p ON ctdp.MaPhong = p.MaPhong
                WHERE dp.TrangThai IN ('Đã xác nhận', 'Chờ duyệt')
                AND substr(dp.NgayNhan, 1, 10) <= @Today
            ORDER BY dp.NgayNhan ASC
            LIMIT 10;";
        var dtIn = DatabaseHelper.ExecuteDataTable(sqlCheckIn, CreateParameter("@Today", today));
        foreach (DataRow r in dtIn.Rows)
            list.Add(new TodoItemDto
            {
                RoomNumber = GetValue<string>(r, "SoPhong"),
                CustomerName = GetValue<string>(r, "HoTen"),
                TaskDescription = "Check-in"
            });

        var sqlCheckOut = @"
                SELECT DISTINCT p.SoPhong, kh.HoTen
                FROM DatPhong dp
                INNER JOIN KhachHang kh ON dp.MaKH = kh.MaKH
                INNER JOIN ChiTietDatPhong ctdp ON dp.MaDP = ctdp.MaDP
                INNER JOIN Phong p ON ctdp.MaPhong = p.MaPhong
                WHERE dp.TrangThai = 'Đang ở' OR p.TrangThai = 'Có khách'
                ORDER BY dp.NgayTra ASC
            LIMIT 10;";
        var dtOut = DatabaseHelper.ExecuteDataTable(sqlCheckOut);
        foreach (DataRow r in dtOut.Rows)
            list.Add(new TodoItemDto
            {
                RoomNumber = GetValue<string>(r, "SoPhong"),
                CustomerName = GetValue<string>(r, "HoTen"),
                TaskDescription = "Check-out"
            });

        var sqlCleaning = "SELECT SoPhong FROM Phong WHERE TrangThai = 'Đang dọn' ORDER BY SoPhong ASC;";
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
        var result = new Dictionary<string, double>();
        var totalRooms = Convert.ToInt32(DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM Phong;") ?? 0);
        if (totalRooms <= 0) return result;

        var occupiedNow = Convert.ToInt32(
            DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM Phong WHERE TrangThai = 'Có khách';") ?? 0);

        for (var i = 6; i >= 0; i--)
        {
            var date = DateTime.Today.AddDays(-i);
            var dateStr = date.ToString("yyyy-MM-dd");

            var sql = @"
                SELECT COUNT(DISTINCT ctdp.MaPhong)
                FROM DatPhong dp
                INNER JOIN ChiTietDatPhong ctdp ON dp.MaDP = ctdp.MaDP
                WHERE dp.TrangThai NOT IN ('Đã hủy', 'Chờ duyệt')
                  AND (
                      CASE WHEN instr(dp.NgayNhan, '/') > 0
                           THEN substr(dp.NgayNhan, 7, 4) || '-' || substr(dp.NgayNhan, 4, 2) || '-' || substr(dp.NgayNhan, 1, 2)
                           ELSE substr(dp.NgayNhan, 1, 10)
                      END
                  ) <= @Ngay
                  AND (
                      (
                          CASE WHEN instr(dp.NgayTra, '/') > 0
                               THEN substr(dp.NgayTra, 7, 4) || '-' || substr(dp.NgayTra, 4, 2) || '-' || substr(dp.NgayTra, 1, 2)
                               ELSE substr(dp.NgayTra, 1, 10)
                          END
                      ) > @Ngay
                      OR
                      (
                          CASE WHEN instr(dp.NgayNhan, '/') > 0
                               THEN substr(dp.NgayNhan, 7, 4) || '-' || substr(dp.NgayNhan, 4, 2) || '-' || substr(dp.NgayNhan, 1, 2)
                               ELSE substr(dp.NgayNhan, 1, 10)
                          END
                      ) = @Ngay
                  );";

            var count = Convert.ToInt32(
                DatabaseHelper.ExecuteScalar(sql, CreateParameter("@Ngay", dateStr)) ?? 0);

            if (i == 0 && occupiedNow > count) count = occupiedNow;

            result[date.ToString("dd/MM")] = Math.Round(count * 100.0 / totalRooms, 1);
        }

        return result;
    }

    // Thống kê doanh thu theo tháng thực tế từ cơ sở dữ liệu
    public List<RevenueReportDto> GetMonthlyRevenue(int year)
    {
        var result = new List<RevenueReportDto>();
        var currentYear = DateTime.Today.Year;
        var maxMonth = (year < currentYear) ? 12 : (year == currentYear ? DateTime.Today.Month : 0);
        if (maxMonth <= 0) return result;

        var sqlRevenue = @"
            SELECT 
                CASE 
                    WHEN instr(NgayLap, '/') > 0 THEN CAST(substr(NgayLap, 4, 2) AS INT)
                    ELSE CAST(substr(NgayLap, 6, 2) AS INT)
                END AS Thang,
                COALESCE(SUM(TongTien), 0) AS DoanhThu
            FROM HoaDon
            WHERE (TrangThai = 'Đã thanh toán' OR TrangThai = 'Paid')
              AND (
                  CASE 
                      WHEN instr(NgayLap, '/') > 0 THEN CAST(substr(NgayLap, 7, 4) AS INT)
                      ELSE CAST(substr(NgayLap, 1, 4) AS INT)
                  END
              ) = @Nam
            GROUP BY Thang;";

        var revenueByMonth = new Dictionary<int, double>();
        var dtRevenue = DatabaseHelper.ExecuteDataTable(sqlRevenue, CreateParameter("@Nam", year));
        foreach (DataRow r in dtRevenue.Rows)
        {
            var thang = Convert.ToInt32(r["Thang"]);
            revenueByMonth[thang] = Convert.ToDouble(r["DoanhThu"]);
        }

        var sqlGuests = @"
            SELECT 
                CASE 
                    WHEN instr(dp.NgayNhan, '/') > 0 THEN CAST(substr(dp.NgayNhan, 4, 2) AS INT)
                    ELSE CAST(substr(dp.NgayNhan, 6, 2) AS INT)
                END AS Thang,
                COALESCE(SUM(COALESCE(dp.SoKhach, 1)), 0) AS TongKhach
            FROM DatPhong dp
            WHERE dp.TrangThai NOT IN ('Đã hủy', 'Cancelled')
              AND (
                  CASE 
                      WHEN instr(dp.NgayNhan, '/') > 0 THEN CAST(substr(dp.NgayNhan, 7, 4) AS INT)
                      ELSE CAST(substr(dp.NgayNhan, 1, 4) AS INT)
                  END
              ) = @Nam
            GROUP BY Thang;";

        var guestsByMonth = new Dictionary<int, int>();
        var dtGuests = DatabaseHelper.ExecuteDataTable(sqlGuests, CreateParameter("@Nam", year));
        foreach (DataRow r in dtGuests.Rows)
        {
            var thang = Convert.ToInt32(r["Thang"]);
            guestsByMonth[thang] = Convert.ToInt32(r["TongKhach"]);
        }

        var totalRooms = Convert.ToInt32(DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM Phong;") ?? 0);
        if (totalRooms <= 0) totalRooms = 1;

        var sqlBookings = @"
            SELECT dp.MaDP, dp.NgayNhan, dp.NgayTra, COALESCE(dp.SoKhach, 1) AS SoKhach,
                   COUNT(DISTINCT ctdp.MaPhong) AS SoPhong
            FROM DatPhong dp
            LEFT JOIN ChiTietDatPhong ctdp ON dp.MaDP = ctdp.MaDP
            WHERE dp.TrangThai NOT IN ('Đã hủy', 'Cancelled')
            GROUP BY dp.MaDP, dp.NgayNhan, dp.NgayTra, dp.SoKhach;";

        var dtBookings = DatabaseHelper.ExecuteDataTable(sqlBookings);

        for (var m = 1; m <= maxMonth; m++)
        {
            var startOfMonth = new DateTime(year, m, 1);
            var daysInMonth = DateTime.DaysInMonth(year, m);
            var startOfNextMonth = startOfMonth.AddMonths(1);
            var occupiedRoomNights = 0;
            var monthGuests = 0;

            foreach (DataRow r in dtBookings.Rows)
            {
                if (!TryParseFlexibleDate(r["NgayNhan"]?.ToString(), out var checkIn)) continue;
                if (!TryParseFlexibleDate(r["NgayTra"]?.ToString(), out var checkOut) || checkOut <= checkIn)
                {
                    checkOut = checkIn.AddDays(1);
                }

                if (checkIn.Year == year && checkIn.Month == m)
                {
                    monthGuests += Convert.ToInt32(r["SoKhach"]);
                }

                var overlapStart = checkIn > startOfMonth ? checkIn : startOfMonth;
                var overlapEnd = checkOut < startOfNextMonth ? checkOut : startOfNextMonth;
                if (overlapEnd > overlapStart)
                {
                    var nights = (overlapEnd - overlapStart).Days;
                    var roomCount = Math.Max(1, Convert.ToInt32(r["SoPhong"]));
                    occupiedRoomNights += nights * roomCount;
                }
            }

            var totalCapacity = totalRooms * daysInMonth;
            var occupancyRate = totalCapacity > 0
                ? Math.Round(occupiedRoomNights * 100.0 / totalCapacity, 1)
                : 0;
            if (occupancyRate > 100.0) occupancyRate = 100.0;

            revenueByMonth.TryGetValue(m, out var rev);
            var guestCount = guestsByMonth.TryGetValue(m, out var g) && g > 0 ? g : monthGuests;

            result.Add(new RevenueReportDto
            {
                Period = $"T{m}",
                Revenue = rev,
                OccupancyRate = occupancyRate,
                GuestCount = guestCount
            });
        }

        return result;
    }
    
    private static bool TryParseFlexibleDate(string? input, out DateTime date)
    {
        date = DateTime.MinValue;
        if (string.IsNullOrWhiteSpace(input)) return false;
        var s = input.Trim();
        if (s.Length > 10 && (s[10] == ' ' || s[10] == 'T')) s = s.Substring(0, 10);
        string[] formats = { "dd/MM/yyyy", "d/M/yyyy", "yyyy-MM-dd", "yyyy/MM/dd" };
        return DateTime.TryParseExact(s, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out date)
               || DateTime.TryParse(s, out date);
    }
}
