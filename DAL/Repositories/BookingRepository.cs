using Microsoft.Data.Sqlite;

namespace DAL.Repositories;

public class BookingRepository : BaseRepository, IBookingRepository
{
    public List<Booking> GetAll()
    {
        var sql = @"
            SELECT dp.*, kh.HoTen AS TenKhach, kh.SDT AS SDTKhach, kh.CCCD,
                   p.SoPhong, lp.TenLoai AS TenLoaiPhong,
                   ctdp.GiaTaiThoiDiem
            FROM DatPhong dp
            INNER JOIN KhachHang kh ON dp.MaKH = kh.MaKH
            LEFT JOIN ChiTietDatPhong ctdp ON dp.MaDP = ctdp.MaDP
            LEFT JOIN Phong p ON ctdp.MaPhong = p.MaPhong
            LEFT JOIN LoaiPhong lp ON p.MaLoai = lp.MaLoai
            ORDER BY dp.MaDP DESC;";

        var table = DatabaseHelper.ExecuteDataTable(sql);
        var list = new List<Booking>();
        foreach (DataRow r in table.Rows) list.Add(MapToBooking(r));
        return list;
    }

    public List<Booking> Search(string keyword, string status, string fromDate)
    {
        var sql = @"
            SELECT dp.*, kh.HoTen AS TenKhach, kh.SDT AS SDTKhach, kh.CCCD,
                   p.SoPhong, lp.TenLoai AS TenLoaiPhong,
                   ctdp.GiaTaiThoiDiem
            FROM DatPhong dp
            INNER JOIN KhachHang kh ON dp.MaKH = kh.MaKH
            LEFT JOIN ChiTietDatPhong ctdp ON dp.MaDP = ctdp.MaDP
            LEFT JOIN Phong p ON ctdp.MaPhong = p.MaPhong
            LEFT JOIN LoaiPhong lp ON p.MaLoai = lp.MaLoai
            WHERE 1=1 ";

        var parameters = new List<SqliteParameter>();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            sql += " AND (kh.HoTen LIKE @TuKhoa OR ('DP' || SUBSTR('000' || dp.MaDP, -3)) LIKE @TuKhoa OR p.SoPhong LIKE @TuKhoa OR kh.CCCD LIKE @TuKhoa) ";
            parameters.Add(CreateParameter("@TuKhoa", $"%{keyword}%"));
        }

        if (!string.IsNullOrWhiteSpace(status) && status != "Tất cả")
        {
            sql += " AND dp.TrangThai = @TrangThai ";
            parameters.Add(CreateParameter("@TrangThai", status));
        }

        if (!string.IsNullOrWhiteSpace(fromDate))
        {
            sql += " AND dp.NgayNhan >= @TuNgay ";
            parameters.Add(CreateParameter("@TuNgay", fromDate));
        }

        sql += " ORDER BY dp.MaDP DESC;";

        var table = DatabaseHelper.ExecuteDataTable(sql, parameters.ToArray());
        var list = new List<Booking>();
        foreach (DataRow r in table.Rows) list.Add(MapToBooking(r));
        return list;
    }

    public List<Booking> GetByCustomerId(int customerId)
    {
        var sql = @"
            SELECT dp.*, kh.HoTen AS TenKhach, kh.SDT AS SDTKhach, kh.CCCD,
                   p.SoPhong, lp.TenLoai AS TenLoaiPhong,
                   ctdp.GiaTaiThoiDiem
            FROM DatPhong dp
            INNER JOIN KhachHang kh ON dp.MaKH = kh.MaKH
            LEFT JOIN ChiTietDatPhong ctdp ON dp.MaDP = ctdp.MaDP
            LEFT JOIN Phong p ON ctdp.MaPhong = p.MaPhong
            LEFT JOIN LoaiPhong lp ON p.MaLoai = lp.MaLoai
            WHERE dp.MaKH = @MaKH
            ORDER BY dp.MaDP DESC;";

        var table = DatabaseHelper.ExecuteDataTable(sql, CreateParameter("@MaKH", customerId));
        var list = new List<Booking>();
        foreach (DataRow r in table.Rows) list.Add(MapToBooking(r));
        return list;
    }

    // Tạo đơn đặt phòng và chi tiết đặt phòng trong cùng một giao dịch
    public int CreateBooking(Booking booking, int roomId, double priceAtBooking)
    {
        var bookingId = 0;
        using var connection = DatabaseHelper.GetConnection();
        using var transaction = connection.BeginTransaction();
        try
        {
            var sqlBooking = @"
                INSERT INTO DatPhong (MaKH, MaNV, MaKM, NgayNhan, NgayTra, SoKhach, TienCoc, KenhDat, TrangThai)
                VALUES (@MaKH, @MaNV, @MaKM, @NgayNhan, @NgayTra, @SoKhach, @TienCoc, @KenhDat, @TrangThai);
                SELECT last_insert_rowid();";

            using var cmdBooking = new SqliteCommand(sqlBooking, connection, transaction);
            cmdBooking.Parameters.Add(CreateParameter("@MaKH", booking.CustomerId));
            cmdBooking.Parameters.Add(CreateParameter("@MaNV", (object?)booking.EmployeeId ?? DBNull.Value));
            cmdBooking.Parameters.Add(CreateParameter("@MaKM", (object?)booking.PromotionId ?? DBNull.Value));
            cmdBooking.Parameters.Add(CreateParameter("@NgayNhan", booking.CheckInDate));
            cmdBooking.Parameters.Add(CreateParameter("@NgayTra", booking.CheckOutDate));
            cmdBooking.Parameters.Add(CreateParameter("@SoKhach", booking.GuestCount));
            cmdBooking.Parameters.Add(CreateParameter("@TienCoc", booking.Deposit));
            cmdBooking.Parameters.Add(CreateParameter("@KenhDat", string.IsNullOrWhiteSpace(booking.Channel) ? "Trực tuyến" : booking.Channel));
            cmdBooking.Parameters.Add(CreateParameter("@TrangThai", ToDbStatus(booking.Status)));

            var idObj = cmdBooking.ExecuteScalar();
            bookingId = idObj != null ? Convert.ToInt32(idObj) : 0;

            if (bookingId > 0 && roomId > 0)
            {
                var sqlDetail = @"
                    INSERT INTO ChiTietDatPhong (MaDP, MaPhong, GiaTaiThoiDiem, NgayNhanTT, NgayTraTT)
                    VALUES (@MaDP, @MaPhong, @Gia, '', '');";

                using var cmdDetail = new SqliteCommand(sqlDetail, connection, transaction);
                cmdDetail.Parameters.Add(CreateParameter("@MaDP", bookingId));
                cmdDetail.Parameters.Add(CreateParameter("@MaPhong", roomId));
                cmdDetail.Parameters.Add(CreateParameter("@Gia", priceAtBooking));
                cmdDetail.ExecuteNonQuery();

                var sqlRoom = "UPDATE Phong SET TrangThai = @TrangThai WHERE MaPhong = @MaPhong;";
                using var cmdRoom = new SqliteCommand(sqlRoom, connection, transaction);
                cmdRoom.Parameters.Add(CreateParameter("@TrangThai", AppConstants.PhongDaDat));
                cmdRoom.Parameters.Add(CreateParameter("@MaPhong", roomId));
                cmdRoom.ExecuteNonQuery();
            }

            transaction.Commit();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            AppLogger.Error(ex, "Lỗi tạo đơn đặt phòng trong giao dịch (BookingRepository.CreateBooking)");
            throw;
        }

        return bookingId;
    }

    public bool UpdateStatus(int bookingId, string newStatus)
    {
        var sql = "UPDATE DatPhong SET TrangThai = @TrangThai WHERE MaDP = @MaDP;";
        var result = DatabaseHelper.ExecuteNonQuery(sql,
            CreateParameter("@TrangThai", newStatus),
            CreateParameter("@MaDP", bookingId));
        return result > 0;
    }

    public Booking? GetById(int bookingId)
    {
        var sql = @"
            SELECT dp.*, kh.HoTen AS TenKhach, kh.SDT AS SDTKhach, kh.CCCD,
                   p.SoPhong, lp.TenLoai AS TenLoaiPhong,
                   ctdp.GiaTaiThoiDiem, ctdp.MaCTDP, ctdp.MaPhong
            FROM DatPhong dp
            INNER JOIN KhachHang kh ON dp.MaKH = kh.MaKH
            LEFT JOIN ChiTietDatPhong ctdp ON dp.MaDP = ctdp.MaDP
            LEFT JOIN Phong p ON ctdp.MaPhong = p.MaPhong
            LEFT JOIN LoaiPhong lp ON p.MaLoai = lp.MaLoai
            WHERE dp.MaDP = @MaDP LIMIT 1;";

        var table = DatabaseHelper.ExecuteDataTable(sql, CreateParameter("@MaDP", bookingId));
        return table.Rows.Count > 0 ? MapToBooking(table.Rows[0]) : null;
    }

    public List<Booking> GetPendingCheckIns()
    {
        var sql = @"
            SELECT dp.*, kh.HoTen AS TenKhach, kh.SDT AS SDTKhach, kh.CCCD,
                   p.SoPhong, lp.TenLoai AS TenLoaiPhong,
                   ctdp.GiaTaiThoiDiem
            FROM DatPhong dp
            INNER JOIN KhachHang kh ON dp.MaKH = kh.MaKH
            LEFT JOIN ChiTietDatPhong ctdp ON dp.MaDP = ctdp.MaDP
            LEFT JOIN Phong p ON ctdp.MaPhong = p.MaPhong
            LEFT JOIN LoaiPhong lp ON p.MaLoai = lp.MaLoai
            WHERE dp.TrangThai IN (@ChoDuyet, @DaXacNhan)
            ORDER BY dp.MaDP ASC;";

        var table = DatabaseHelper.ExecuteDataTable(sql,
            CreateParameter("@ChoDuyet", AppConstants.DPChoDuyet),
            CreateParameter("@DaXacNhan", AppConstants.DPDaXacNhan));
        var list = new List<Booking>();
        foreach (DataRow r in table.Rows) list.Add(MapToBooking(r));
        return list;
    }

    public Booking? GetActiveBookingByRoomNumber(string roomNumber)
    {
        var sql = @"
            SELECT dp.*, kh.HoTen AS TenKhach, kh.SDT AS SDTKhach, kh.CCCD,
                   p.SoPhong, lp.TenLoai AS TenLoaiPhong,
                   ctdp.GiaTaiThoiDiem
            FROM DatPhong dp
            INNER JOIN KhachHang kh ON dp.MaKH = kh.MaKH
            INNER JOIN ChiTietDatPhong ctdp ON dp.MaDP = ctdp.MaDP
            INNER JOIN Phong p ON ctdp.MaPhong = p.MaPhong
            LEFT JOIN LoaiPhong lp ON p.MaLoai = lp.MaLoai
            WHERE p.SoPhong = @SoPhong AND (dp.TrangThai = @DangO OR p.TrangThai = @CoKhach)
            ORDER BY dp.MaDP DESC LIMIT 1;";

        var table = DatabaseHelper.ExecuteDataTable(sql,
            CreateParameter("@SoPhong", roomNumber),
            CreateParameter("@DangO", AppConstants.DPDangO),
            CreateParameter("@CoKhach", AppConstants.PhongCoKhach));
        return table.Rows.Count > 0 ? MapToBooking(table.Rows[0]) : null;
    }

    public int? GetActiveBookingDetailIdByRoomNumber(string roomNumber)
    {
        var sql = @"
            SELECT ctdp.MaCTDP
            FROM ChiTietDatPhong ctdp
            INNER JOIN DatPhong dp ON ctdp.MaDP = dp.MaDP
            INNER JOIN Phong p ON ctdp.MaPhong = p.MaPhong
            WHERE p.SoPhong = @SoPhong AND (dp.TrangThai = @DangO OR p.TrangThai = @CoKhach)
            ORDER BY dp.MaDP DESC, ctdp.MaCTDP DESC
            LIMIT 1;";

        var val = DatabaseHelper.ExecuteScalar(sql,
            CreateParameter("@SoPhong", roomNumber),
            CreateParameter("@DangO", AppConstants.DPDangO),
            CreateParameter("@CoKhach", AppConstants.PhongCoKhach));
        return val != null && val != DBNull.Value ? Convert.ToInt32(val) : null;
    }

    public bool CheckIn(int bookingId, string identityCard, string actualCheckInDate)
    {
        var sqlDP = "UPDATE DatPhong SET TrangThai = @DangO WHERE MaDP = @MaDP;";
        DatabaseHelper.ExecuteNonQuery(sqlDP,
            CreateParameter("@DangO", AppConstants.DPDangO),
            CreateParameter("@MaDP", bookingId));

        var sqlCT = "UPDATE ChiTietDatPhong SET NgayNhanTT = @NgayNhanTT WHERE MaDP = @MaDP;";
        DatabaseHelper.ExecuteNonQuery(sqlCT,
            CreateParameter("@NgayNhanTT", actualCheckInDate),
            CreateParameter("@MaDP", bookingId));

        var sqlRoom = @"
            UPDATE Phong 
            SET TrangThai = @CoKhach 
            WHERE MaPhong IN (SELECT MaPhong FROM ChiTietDatPhong WHERE MaDP = @MaDP);";
        DatabaseHelper.ExecuteNonQuery(sqlRoom,
            CreateParameter("@CoKhach", AppConstants.PhongCoKhach),
            CreateParameter("@MaDP", bookingId));

        return true;
    }

    public bool CheckOut(int bookingId, string actualCheckOutDate)
    {
        var sqlDP = "UPDATE DatPhong SET TrangThai = @DaHoanTat WHERE MaDP = @MaDP;";
        DatabaseHelper.ExecuteNonQuery(sqlDP,
            CreateParameter("@DaHoanTat", AppConstants.DPDaHoanTat),
            CreateParameter("@MaDP", bookingId));

        var sqlCT = "UPDATE ChiTietDatPhong SET NgayTraTT = @NgayTraTT WHERE MaDP = @MaDP;";
        DatabaseHelper.ExecuteNonQuery(sqlCT,
            CreateParameter("@NgayTraTT", actualCheckOutDate),
            CreateParameter("@MaDP", bookingId));

        var sqlRoom = @"
            UPDATE Phong 
            SET TrangThai = @DangDon 
            WHERE MaPhong IN (SELECT MaPhong FROM ChiTietDatPhong WHERE MaDP = @MaDP);";
        DatabaseHelper.ExecuteNonQuery(sqlRoom,
            CreateParameter("@DangDon", AppConstants.PhongDangDon),
            CreateParameter("@MaDP", bookingId));

        return true;
    }

    public static string ToDbStatus(BookingStatus status) => status switch
    {
        BookingStatus.Pending => AppConstants.DPChoDuyet,
        BookingStatus.Confirmed => AppConstants.DPDaXacNhan,
        BookingStatus.CheckedIn => AppConstants.DPDangO,
        BookingStatus.CheckedOut => AppConstants.DPDaHoanTat,
        BookingStatus.Cancelled => AppConstants.DPDaHuy,
        _ => AppConstants.DPChoDuyet
    };

    public static BookingStatus FromDbStatus(string? status) => status?.Trim() switch
    {
        AppConstants.DPChoDuyet => BookingStatus.Pending,
        AppConstants.DPDaXacNhan => BookingStatus.Confirmed,
        AppConstants.DPDangO => BookingStatus.CheckedIn,
        AppConstants.DPDaHoanTat => BookingStatus.CheckedOut,
        AppConstants.DPDaHuy => BookingStatus.Cancelled,
        _ => BookingStatus.Pending
    };

    private Booking MapToBooking(DataRow r)
    {
        var rawStatus = GetValue<string>(r, "TrangThai", AppConstants.DPChoDuyet);
        var dp = new Booking
        {
            BookingId = GetValue<int>(r, "MaDP"),
            CustomerId = GetValue<int>(r, "MaKH"),
            EmployeeId = r.IsNull("MaNV") ? null : GetValue<int>(r, "MaNV"),
            PromotionId = r.IsNull("MaKM") ? null : GetValue<int>(r, "MaKM"),
            CheckInDate = GetValue<string>(r, "NgayNhan", ""),
            CheckOutDate = GetValue<string>(r, "NgayTra", ""),
            GuestCount = GetValue(r, "SoKhach", 1),
            Deposit = GetValue<double>(r, "TienCoc"),
            Channel = GetValue<string>(r, "KenhDat", "Trực tiếp"),
            Status = FromDbStatus(rawStatus),
            CustomerName = GetValue<string>(r, "TenKhach", ""),
            CustomerPhone = GetValue<string>(r, "SDTKhach", ""),
            CustomerNationalId = GetValue<string>(r, "CCCD", ""),
            RoomNumber = GetValue<string>(r, "SoPhong", ""),
            RoomTypeName = GetValue<string>(r, "TenLoaiPhong", "")
        };

        var price = GetValue<double>(r, "GiaTaiThoiDiem", AppConstants.FallbackNightlyRate);
        var nights = FormatHelper.CalculateNights(FormatHelper.ParseDate(dp.CheckInDate),
            FormatHelper.ParseDate(dp.CheckOutDate));
        dp.TotalAmount = price * nights;

        return dp;
    }
}
