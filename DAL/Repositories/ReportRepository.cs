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
        WHERE (
            CASE WHEN instr(NgayNhan, '/') > 0
                 THEN substr(NgayNhan, 7, 4) || '-' || substr(NgayNhan, 4, 2) || '-' || substr(NgayNhan, 1, 2)
                 ELSE substr(NgayNhan, 1, 10)
            END
        ) = @Today 
          AND TrangThai IN ('Chờ duyệt', 'Đã xác nhận', 'Đang ở');";
    stats.TodayCheckIns = Convert.ToInt32(
        DatabaseHelper.ExecuteScalar(sqlCheckIn, CreateParameter("@Today", today)) ?? 0);

    var sqlCheckOut = @"
        SELECT COUNT(*) 
        FROM DatPhong 
        WHERE (
            CASE WHEN instr(NgayTra, '/') > 0
                 THEN substr(NgayTra, 7, 4) || '-' || substr(NgayTra, 4, 2) || '-' || substr(NgayTra, 1, 2)
                 ELSE substr(NgayTra, 1, 10)
            END
        ) = @Today 
          AND TrangThai IN ('Đang ở', 'Đã trả phòng');";
    stats.TodayCheckOuts = Convert.ToInt32(
        DatabaseHelper.ExecuteScalar(sqlCheckOut, CreateParameter("@Today", today)) ?? 0);

    var sqlNewBookings = @"
        SELECT COUNT(*) 
        FROM DatPhong 
        WHERE (
            CASE WHEN instr(NgayNhan, '/') > 0
                 THEN substr(NgayNhan, 7, 4) || '-' || substr(NgayNhan, 4, 2) || '-' || substr(NgayNhan, 1, 2)
                 ELSE substr(NgayNhan, 1, 10)
            END
        ) = @Today;";
    stats.TodayNewBookings = Convert.ToInt32(
        DatabaseHelper.ExecuteScalar(sqlNewBookings, CreateParameter("@Today", today)) ?? 0);

    double todayRevenue = 0;
    var countedInvoiceIds = new HashSet<int>();

    // 1. Lấy các giao dịch thu tiền phát sinh hôm nay trong bảng ThanhToan
    var sqlThanhToan = @"
        SELECT tt.MaHD AS MaHDTT, tt.SoTien, tt.NgayTT, hd.*
        FROM ThanhToan tt
        LEFT JOIN HoaDon hd ON tt.MaHD = hd.MaHD;";
    var dtThanhToan = DatabaseHelper.ExecuteDataTable(sqlThanhToan);
    foreach (DataRow r in dtThanhToan.Rows)
    {
        var ngayTT = r["NgayTT"]?.ToString();
        if (!TryParseFlexibleDate(ngayTT, out var payDate) || payDate.Date != DateTime.Today)
            continue;

        double soTienTT = r["SoTien"] != DBNull.Value ? Convert.ToDouble(r["SoTien"]) : 0;
        double tongHD = TinhTongTienChuanHoa(r);
        todayRevenue += Math.Max(soTienTT, tongHD);

        if (r["MaHDTT"] != DBNull.Value)
            countedInvoiceIds.Add(Convert.ToInt32(r["MaHDTT"]));
    }

    // 2. Cộng thêm các hóa đơn đã thanh toán hôm nay nhưng chưa ghi sang ThanhToan
    var sqlHoaDon = @"
        SELECT *
        FROM HoaDon
        WHERE TrangThai IN ('Đã thanh toán', 'Paid');";
    var dtHoaDon = DatabaseHelper.ExecuteDataTable(sqlHoaDon);
    foreach (DataRow r in dtHoaDon.Rows)
    {
        int maHD = r.Table.Columns.Contains("MaHD") && r["MaHD"] != DBNull.Value
            ? Convert.ToInt32(r["MaHD"]) : 0;
        if (maHD > 0 && countedInvoiceIds.Contains(maHD))
            continue;

        var ngayLap = r.Table.Columns.Contains("NgayLap") ? r["NgayLap"]?.ToString() : null;
        if (!TryParseFlexibleDate(ngayLap, out var invDate) || invDate.Date != DateTime.Today)
            continue;

        todayRevenue += TinhTongTienChuanHoa(r);
    }

    stats.TodayRevenue = todayRevenue;

    return stats;
}

    // Lấy việc hôm nay (check-in, check-out, dọn phòng)
    public List<TodoItemDto> GetTodayTodoList()
    {
        var list = new List<TodoItemDto>();

        var sqlCheckIn = @"
            SELECT DISTINCT p.SoPhong, kh.HoTen, dp.NgayNhan
            FROM DatPhong dp
            INNER JOIN KhachHang kh ON dp.MaKH = kh.MaKH
            INNER JOIN ChiTietDatPhong ctdp ON dp.MaDP = ctdp.MaDP
            INNER JOIN Phong p ON ctdp.MaPhong = p.MaPhong
            WHERE dp.TrangThai IN ('Đã xác nhận', 'Chờ duyệt')
            ORDER BY dp.NgayNhan ASC;";
        var dtIn = DatabaseHelper.ExecuteDataTable(sqlCheckIn);
        foreach (DataRow r in dtIn.Rows)
            list.Add(new TodoItemDto
            {
                RoomNumber = GetValue<string>(r, "SoPhong"),
                CustomerName = GetValue<string>(r, "HoTen"),
                TaskDescription = "Check-in"
            });

        var sqlCheckOut = @"
            SELECT DISTINCT p.SoPhong, kh.HoTen, dp.NgayTra
            FROM DatPhong dp
            INNER JOIN KhachHang kh ON dp.MaKH = kh.MaKH
            INNER JOIN ChiTietDatPhong ctdp ON dp.MaDP = ctdp.MaDP
            INNER JOIN Phong p ON ctdp.MaPhong = p.MaPhong
            WHERE dp.TrangThai = 'Đang ở'
            ORDER BY dp.NgayTra ASC;";
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
    public List<RevenueReportDto> GetMonthlyRevenue(int year , int? employeeId = null)
    {
        // 1. Lấy doanh thu cả năm trong 1 lần gọi để khớp 100% với báo cáo ngày
        var dsNgayCaNam = GetDailyRevenue(new DateTime(year, 1, 1), new DateTime(year, 12, 31), employeeId);
        var doanhThuTheoThang = new double[13];
        var luotKhachTheoThang = new int[13];

        foreach (var item in dsNgayCaNam)
        {
            if (DateTime.TryParseExact($"{item.Period}/{year}", "dd/MM/yyyy",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out var ngay))
            {
                doanhThuTheoThang[ngay.Month] += item.Revenue;
                luotKhachTheoThang[ngay.Month] += item.GuestCount;
            }
        }

        // 2. Lấy tổng số phòng hiện có
        var objTongPhong = DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM Phong;");
        int tongPhong = objTongPhong != null && objTongPhong != DBNull.Value ? Convert.ToInt32(objTongPhong) : 0;

        // 3. Tính tổng số đêm phòng có khách của từng tháng
        var tongDemPhongThang = new int[13];
        if (tongPhong > 0)
        {
            bool dpCoMaNV = CoCotTrongBang("DatPhong", "MaNV");
            bool hdCoMaNV = CoCotTrongBang("HoaDon", "MaNV");
            string selMaNVDP = dpCoMaNV ? "dp.MaNV AS MaNVDP" : "NULL AS MaNVDP";
            string selMaNVHD = hdCoMaNV ? "hd.MaNV AS MaNVHD" : "NULL AS MaNVHD";

            var sqlPhongDaDat = $@"
                SELECT dp.MaDP, ctdp.MaPhong, dp.NgayNhan, dp.NgayTra, {selMaNVDP}, {selMaNVHD}
                FROM DatPhong dp
                INNER JOIN ChiTietDatPhong ctdp ON dp.MaDP = ctdp.MaDP
                LEFT JOIN HoaDon hd ON hd.MaDP = dp.MaDP
                WHERE dp.TrangThai IN ('Đang ở', 'Đã hoàn tất', 'CheckedIn', 'Completed');";

            var dtBooking = DatabaseHelper.ExecuteDataTable(sqlPhongDaDat);
            var daTinhPhongDon = new HashSet<string>();

            foreach (DataRow r in dtBooking.Rows)
            {
                if (employeeId.HasValue)
                {
                    var maNV = LayMaNVThucHien(r, "MaNVDP", "MaNVHD");
                    if (maNV != employeeId.Value) continue;
                }

                string khoa = $"{r["MaDP"]}_{r["MaPhong"]}";
                if (!daTinhPhongDon.Add(khoa)) continue;

                if (!TryParseFlexibleDate(r["NgayNhan"]?.ToString(), out var ngayNhan))
                    continue;
                if (!TryParseFlexibleDate(r["NgayTra"]?.ToString(), out var ngayTra) || ngayTra.Date <= ngayNhan.Date)
                    ngayTra = ngayNhan.Date.AddDays(1);

                var tu = ngayNhan.Date;
                var den = ngayTra.Date;

                for (int month = 1; month <= 12; month++)
                {
                    var dauThang = new DateTime(year, month, 1);
                    var dauThangSau = dauThang.AddMonths(1);

                    var batDauGiao = tu > dauThang ? tu : dauThang;
                    var ketThucGiao = den < dauThangSau ? den : dauThangSau;

                    if (ketThucGiao > batDauGiao)
                        tongDemPhongThang[month] += (ketThucGiao - batDauGiao).Days;
                }
            }
        }

        // 4. Tổng hợp kết quả 12 tháng
        var result = new List<RevenueReportDto>();
        for (int month = 1; month <= 12; month++)
        {
            int soNgayTrongThang = DateTime.DaysInMonth(year, month);
            double congSuat = tongPhong > 0
                ? Math.Min(100.0, tongDemPhongThang[month] * 100.0 / (tongPhong * soNgayTrongThang))
                : 0;

            result.Add(new RevenueReportDto
            {
                Period = $"T{month}",
                Revenue = doanhThuTheoThang[month],
                OccupancyRate = congSuat,
                GuestCount = luotKhachTheoThang[month]
            });
        }

        return result;
    }

    public List<RevenueReportDto> GetDailyRevenue(DateTime fromDate, DateTime toDate, int? employeeId = null)
    {
        if (fromDate.Date > toDate.Date)
            return new List<RevenueReportDto>();

        var mapByDate = new Dictionary<DateTime, RevenueReportDto>();
        for (var d = fromDate.Date; d <= toDate.Date; d = d.AddDays(1))
        {
            mapByDate[d] = new RevenueReportDto
            {
                Period = d.ToString("dd/MM"),
                Revenue = 0,
                OccupancyRate = 0,
                GuestCount = 0
            };
        }
        bool ttCoMaNV = CoCotTrongBang("ThanhToan", "MaNV");
        bool hdCoMaNV = CoCotTrongBang("HoaDon", "MaNV");
        bool dpCoMaNV = CoCotTrongBang("DatPhong", "MaNV");

        string selMaNVTT = ttCoMaNV ? "tt.MaNV AS MaNVTT" : "NULL AS MaNVTT";
        string selMaNVHD = hdCoMaNV ? "hd.MaNV AS MaNVHD" : "NULL AS MaNVHD";
        string selMaNVDP = dpCoMaNV ? "dp.MaNV AS MaNVDP" : "NULL AS MaNVDP";
        var countedInvoiceIds = new HashSet<int>();

        // 1. Lấy các giao dịch thu tiền từ bảng ThanhToan
        var sqlThanhToan = $@"
            SELECT tt.MaHD AS MaHDTT, tt.SoTien, tt.NgayTT, {selMaNVTT}, {selMaNVHD}, {selMaNVDP}, hd.*
            FROM ThanhToan tt
            LEFT JOIN HoaDon hd ON tt.MaHD = hd.MaHD
            LEFT JOIN DatPhong dp ON hd.MaDP = dp.MaDP;";
        
        var dtThanhToan = DatabaseHelper.ExecuteDataTable(sqlThanhToan);
        foreach (DataRow r in dtThanhToan.Rows)
        {
            if (employeeId.HasValue)
            {
                var maNV = LayMaNVThucHien(r, "MaNVTT", "MaNVHD", "MaNVDP");
                if (maNV != employeeId.Value) continue;
            }
            
            if (!TryParseFlexibleDate(r["NgayTT"]?.ToString(), out var payDate))
                continue;

            var day = payDate.Date;
            if (day < fromDate.Date || day > toDate.Date)
                continue;

            double soTienTT = r["SoTien"] != DBNull.Value ? Convert.ToDouble(r["SoTien"]) : 0;
            double tongHD = TinhTongTienChuanHoa(r);
            mapByDate[day].Revenue += Math.Max(soTienTT, tongHD);
            mapByDate[day].GuestCount += 1;

            if (r["MaHDTT"] != DBNull.Value)
                countedInvoiceIds.Add(Convert.ToInt32(r["MaHDTT"]));
        }

        // 2. Cộng thêm hóa đơn đã thanh toán trực tiếp nhưng chưa ghi sang ThanhToan
        var sqlHoaDon = $@"
            SELECT {selMaNVHD}, {selMaNVDP}, hd.*
            FROM HoaDon hd
            LEFT JOIN DatPhong dp ON hd.MaDP = dp.MaDP
            WHERE hd.TrangThai IN ('Đã thanh toán', 'Paid');";
        var dtHoaDon = DatabaseHelper.ExecuteDataTable(sqlHoaDon);
        foreach (DataRow r in dtHoaDon.Rows)
        {
            if (employeeId.HasValue)
            {
                var maNV = LayMaNVThucHien(r, "MaNVHD", "MaNVDP");
                if (maNV != employeeId.Value) continue;
            }
            
            int maHD = r.Table.Columns.Contains("MaHD") && r["MaHD"] != DBNull.Value
                ? Convert.ToInt32(r["MaHD"]) : 0;
            if (maHD > 0 && countedInvoiceIds.Contains(maHD))
                continue;

            var ngayLap = r.Table.Columns.Contains("NgayLap") ? r["NgayLap"]?.ToString() : null;
            if (!TryParseFlexibleDate(ngayLap, out var invDate))
                continue;

            var day = invDate.Date;
            if (day < fromDate.Date || day > toDate.Date)
                continue;

            mapByDate[day].Revenue += TinhTongTienChuanHoa(r);
            mapByDate[day].GuestCount += 1;
        }

        return mapByDate.OrderBy(kv => kv.Key).Select(kv => kv.Value).ToList();
    }
    private static bool CoCotTrongBang(string tenBang, string tenCot)
    {
        var dt = DatabaseHelper.ExecuteDataTable($"PRAGMA table_info({tenBang});");
        foreach (DataRow row in dt.Rows)
        {
            if (string.Equals(row["name"]?.ToString(), tenCot, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    private static int? LayMaNVThucHien(DataRow r, params string[] tenCacCot)
    {
        foreach (var col in tenCacCot)
        {
            if (r.Table.Columns.Contains(col) && r[col] != DBNull.Value && int.TryParse(r[col].ToString(), out int maNV) && maNV > 0)
                return maNV;
        }
        return null;
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
    private static double TinhTongTienChuanHoa(DataRow r)
    {
        double tongTien = LayGiaTriCotSo(r, "TongTien");
        double tienPhong = LayGiaTriCotSo(r, "TienPhong");
        double tienDV = LayGiaTriCotSo(r, "TienDV", "TienDichVu");
        double giamGia = LayGiaTriCotSo(r, "GiamGia");
        double vat = LayGiaTriCotSo(r, "ThueVAT", "VAT");

        double tamTinh = Math.Max(0, tienPhong + tienDV - giamGia);
        if (vat <= 0 && tongTien > 0 && tongTien < tamTinh)
            return tamTinh + tongTien;

        return tongTien;
    }
    private static double LayGiaTriCotSo(DataRow r, params string[] tenCots)
    {
        foreach (var col in tenCots)
        {
            if (r.Table.Columns.Contains(col) && r[col] != DBNull.Value)
                return Convert.ToDouble(r[col]);
        }
        return 0;
    }
}
