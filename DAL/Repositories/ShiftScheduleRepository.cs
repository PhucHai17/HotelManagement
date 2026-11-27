namespace DAL.Repositories;

public sealed class ShiftScheduleRepository : BaseRepository, IShiftScheduleRepository
{
    private static bool _schemaReady;
    private static readonly object SchemaLock = new();

    public List<ShiftSchedule> GetAll(DateTime? fromDate = null, DateTime? toDate = null, int? employeeId = null)
    {
        EnsureSchema();
        var sql = @"
            SELECT lp.MaLich, lp.MaNV, lp.MaQL, lp.NgayLam,
                   COALESCE(cv.TenCa, 'Ca không xác định') AS TenCa,
                   nv.HoTen AS HoTen
            FROM LichPhanCa lp
            INNER JOIN NhanVien nv ON nv.MaNV = lp.MaNV
            LEFT JOIN CaLamViec cv ON cv.MaCa = lp.MaCa
            WHERE 1 = 1";
        var parameters = new List<Microsoft.Data.Sqlite.SqliteParameter>();
        if (fromDate.HasValue)
        {
            sql += " AND date(lp.NgayLam) >= date(@TuNgay)";
            parameters.Add(CreateParameter("@TuNgay", fromDate.Value.ToString("yyyy-MM-dd")));
        }
        if (toDate.HasValue)
        {
            sql += " AND date(lp.NgayLam) <= date(@DenNgay)";
            parameters.Add(CreateParameter("@DenNgay", toDate.Value.ToString("yyyy-MM-dd")));
        }
        if (employeeId.HasValue)
        {
            sql += " AND lp.MaNV = @MaNV";
            parameters.Add(CreateParameter("@MaNV", employeeId.Value));
        }
        sql += " ORDER BY date(lp.NgayLam), nv.HoTen, lp.MaLich;";

        var table = DatabaseHelper.ExecuteDataTable(sql, parameters.ToArray());
        var result = new List<ShiftSchedule>();
        foreach (DataRow row in table.Rows)
        {
            result.Add(new ShiftSchedule
            {
                ScheduleId = GetValue<int>(row, "MaLich"),
                EmployeeId = GetValue<int>(row, "MaNV"),
                ManagerId = GetNullableInt(row, "MaQL"),
                WorkDate = GetValue<string>(row, "NgayLam", ""),
                ShiftType = GetValue<string>(row, "TenCa", "Ca không xác định"),
                EmployeeName = GetValue<string>(row, "HoTen", "")
            });
        }
        return result;
    }

    public List<WorkShift> GetWorkShifts()
    {
        EnsureSchema();
        var table = DatabaseHelper.ExecuteDataTable("SELECT MaCa, TenCa, GioBatDau, GioKetThuc FROM CaLamViec ORDER BY MaCa;");
        var result = new List<WorkShift>();
        foreach (DataRow row in table.Rows)
        {
            var start = TimeSpan.TryParse(GetValue<string>(row, "GioBatDau", "08:00"), out var s) ? s : new TimeSpan(8, 0, 0);
            var end = TimeSpan.TryParse(GetValue<string>(row, "GioKetThuc", "16:00"), out var e) ? e : new TimeSpan(16, 0, 0);
            result.Add(new WorkShift { ShiftId = GetValue<int>(row, "MaCa"), HandoverNote = GetValue<string>(row, "TenCa", ""), StartTime = DateTime.Today.Add(start), EndTime = DateTime.Today.Add(end) });
        }
        return result;
    }

    public int Add(ShiftSchedule schedule)
    {
        EnsureSchema();
        return Convert.ToInt32(DatabaseHelper.ExecuteScalar(@"
            INSERT INTO LichPhanCa (MaNV, MaQL, NgayLam, MaCa)
            VALUES (@MaNV, @MaQL, @NgayLam, @MaCa);
            SELECT last_insert_rowid();",
            CreateParameter("@MaNV", schedule.EmployeeId), CreateParameter("@MaQL", schedule.ManagerId),
            CreateParameter("@NgayLam", schedule.WorkDate), CreateParameter("@MaCa", ParseShiftId(schedule.ShiftType)) ) ?? 0);
    }

    public bool Update(ShiftSchedule schedule)
    {
        EnsureSchema();
        return DatabaseHelper.ExecuteNonQuery(@"
            UPDATE LichPhanCa SET MaNV=@MaNV, MaQL=@MaQL, NgayLam=@NgayLam, MaCa=@MaCa WHERE MaLich=@MaLich;",
            CreateParameter("@MaNV", schedule.EmployeeId), CreateParameter("@MaQL", schedule.ManagerId),
            CreateParameter("@NgayLam", schedule.WorkDate), CreateParameter("@MaCa", ParseShiftId(schedule.ShiftType)),
            CreateParameter("@MaLich", schedule.ScheduleId)) > 0;
    }

    public bool Delete(int scheduleId)
    {
        EnsureSchema();
        return DatabaseHelper.ExecuteNonQuery("DELETE FROM LichPhanCa WHERE MaLich=@MaLich;", CreateParameter("@MaLich", scheduleId)) > 0;
    }

    private static int ParseShiftId(string value) => int.TryParse(value, out var id) ? id : 1;

    private static void EnsureSchema()
    {
        if (_schemaReady) return;
        lock (SchemaLock)
        {
            if (_schemaReady) return;
            DatabaseHelper.ExecuteNonQuery(@"
                CREATE TABLE IF NOT EXISTS CaLamViec (
                    MaCa INTEGER PRIMARY KEY AUTOINCREMENT,
                    TenCa TEXT NOT NULL,
                    GioBatDau TEXT NOT NULL,
                    GioKetThuc TEXT NOT NULL);
                CREATE TABLE IF NOT EXISTS LichPhanCa (
                    MaLich INTEGER PRIMARY KEY AUTOINCREMENT,
                    MaNV INTEGER NOT NULL,
                    MaQL INTEGER NULL,
                    NgayLam TEXT NOT NULL,
                    MaCa INTEGER NOT NULL,
                    UNIQUE(MaNV, NgayLam, MaCa));");
            foreach (var seed in new[] { ("Ca sáng", "06:00", "14:00"), ("Ca chiều", "14:00", "22:00"), ("Ca đêm", "22:00", "06:00") })
                DatabaseHelper.ExecuteNonQuery("INSERT INTO CaLamViec (TenCa, GioBatDau, GioKetThuc) SELECT @Ten, @Start, @End WHERE NOT EXISTS (SELECT 1 FROM CaLamViec WHERE TenCa=@Ten);", CreateParameter("@Ten", seed.Item1), CreateParameter("@Start", seed.Item2), CreateParameter("@End", seed.Item3));
            _schemaReady = true;
        }
    }
}
