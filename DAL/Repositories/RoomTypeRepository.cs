namespace DAL.Repositories;

public class RoomTypeRepository : BaseRepository, IRoomTypeRepository
{
    private static bool _daKiemTraDuLieuMau;
    private static readonly object _khoaKiemTra = new();

    public List<RoomType> GetAll()
    {
        DamBaoDuLieuHangPhongMau();
        var sql = "SELECT * FROM LoaiPhong ORDER BY MaLoai ASC;";
        var table = DatabaseHelper.ExecuteDataTable(sql);
        var list = new List<RoomType>();
        foreach (DataRow r in table.Rows) list.Add(MapToRoomType(r));
        return list;
    }

    public RoomType? GetById(int roomTypeId)
    {
        var sql = "SELECT * FROM LoaiPhong WHERE MaLoai = @MaLoai LIMIT 1;";
        var table = DatabaseHelper.ExecuteDataTable(sql, CreateParameter("@MaLoai", roomTypeId));
        return table.Rows.Count > 0 ? MapToRoomType(table.Rows[0]) : null;
    }

    public bool Add(RoomType entity)
    {
        var sql = @"
            INSERT INTO LoaiPhong (TenLoai, GiaCoBan, SucChua, MoTa)
            VALUES (@TenLoai, @GiaCoBan, @SucChua, @MoTa);";
        return DatabaseHelper.ExecuteNonQuery(sql,
            CreateParameter("@TenLoai", entity.Name),
            CreateParameter("@GiaCoBan", entity.BasePrice),
            CreateParameter("@SucChua", entity.Capacity),
            CreateParameter("@MoTa", entity.Description)) > 0;
    }

    public bool Update(RoomType entity)
    {
        var sql = @"
            UPDATE LoaiPhong
            SET TenLoai = @TenLoai, GiaCoBan = @GiaCoBan, SucChua = @SucChua, MoTa = @MoTa
            WHERE MaLoai = @MaLoai;";
        return DatabaseHelper.ExecuteNonQuery(sql,
            CreateParameter("@TenLoai", entity.Name),
            CreateParameter("@GiaCoBan", entity.BasePrice),
            CreateParameter("@SucChua", entity.Capacity),
            CreateParameter("@MoTa", entity.Description),
            CreateParameter("@MaLoai", entity.RoomTypeId)) > 0;
    }

    public bool Delete(int roomTypeId)
    {
        var sql = "DELETE FROM LoaiPhong WHERE MaLoai = @MaLoai;";
        return DatabaseHelper.ExecuteNonQuery(sql, CreateParameter("@MaLoai", roomTypeId)) > 0;
    }

    // Đảm bảo đầy đủ dữ liệu 6 hạng phòng và các phòng mẫu trong cơ sở dữ liệu
    public static void DamBaoDuLieuHangPhongMau()
    {
        if (_daKiemTraDuLieuMau) return;

        lock (_khoaKiemTra)
        {
            if (_daKiemTraDuLieuMau) return;

            try
            {
                var sqlLoaiPhong = @"
                    INSERT INTO LoaiPhong (TenLoai, GiaCoBan, SucChua, MoTa)
                    SELECT 'Superior', 650000, 2, 'Phòng Superior 28m2 thiết kế thanh lịch, nội thất gỗ ấm cúng, ban công ngắm cảnh thành phố thoáng đãng'
                    WHERE NOT EXISTS (SELECT 1 FROM LoaiPhong WHERE LOWER(TenLoai) = 'superior');

                    INSERT INTO LoaiPhong (TenLoai, GiaCoBan, SucChua, MoTa)
                    SELECT 'Family', 1200000, 4, 'Phòng gia đình 45m2 gồm 2 giường đôi lớn, không gian sinh hoạt chung rộng rãi và tiện nghi cao cấp cho cả nhà'
                    WHERE NOT EXISTS (SELECT 1 FROM LoaiPhong WHERE LOWER(TenLoai) = 'family');

                    INSERT INTO LoaiPhong (TenLoai, GiaCoBan, SucChua, MoTa)
                    SELECT 'Presidential', 3500000, 4, 'Phòng Tổng Thống 90m2 hoàng gia với góc nhìn panorama toàn cảnh, quầy bar riêng, bồn tắm sục và dịch vụ quản gia 24/7'
                    WHERE NOT EXISTS (SELECT 1 FROM LoaiPhong WHERE LOWER(TenLoai) = 'presidential');";
                DatabaseHelper.ExecuteNonQuery(sqlLoaiPhong);

                var sqlPhong = @"
                    INSERT INTO Phong (MaLoai, SoPhong, Tang, TrangThai)
                    SELECT lp.MaLoai, '107', 1, 'Trống'
                    FROM LoaiPhong lp
                    WHERE LOWER(lp.TenLoai) = 'superior'
                      AND NOT EXISTS (SELECT 1 FROM Phong WHERE SoPhong = '107');

                    INSERT INTO Phong (MaLoai, SoPhong, Tang, TrangThai)
                    SELECT lp.MaLoai, '207', 2, 'Trống'
                    FROM LoaiPhong lp
                    WHERE LOWER(lp.TenLoai) = 'superior'
                      AND NOT EXISTS (SELECT 1 FROM Phong WHERE SoPhong = '207');

                    INSERT INTO Phong (MaLoai, SoPhong, Tang, TrangThai)
                    SELECT lp.MaLoai, '304', 3, 'Trống'
                    FROM LoaiPhong lp
                    WHERE LOWER(lp.TenLoai) = 'superior'
                      AND NOT EXISTS (SELECT 1 FROM Phong WHERE SoPhong = '304');

                    INSERT INTO Phong (MaLoai, SoPhong, Tang, TrangThai)
                    SELECT lp.MaLoai, '208', 2, 'Trống'
                    FROM LoaiPhong lp
                    WHERE LOWER(lp.TenLoai) = 'family'
                      AND NOT EXISTS (SELECT 1 FROM Phong WHERE SoPhong = '208');

                    INSERT INTO Phong (MaLoai, SoPhong, Tang, TrangThai)
                    SELECT lp.MaLoai, '305', 3, 'Trống'
                    FROM LoaiPhong lp
                    WHERE LOWER(lp.TenLoai) = 'family'
                      AND NOT EXISTS (SELECT 1 FROM Phong WHERE SoPhong = '305');

                    INSERT INTO Phong (MaLoai, SoPhong, Tang, TrangThai)
                    SELECT lp.MaLoai, '306', 3, 'Trống'
                    FROM LoaiPhong lp
                    WHERE LOWER(lp.TenLoai) = 'family'
                      AND NOT EXISTS (SELECT 1 FROM Phong WHERE SoPhong = '306');

                    INSERT INTO Phong (MaLoai, SoPhong, Tang, TrangThai)
                    SELECT lp.MaLoai, '307', 3, 'Trống'
                    FROM LoaiPhong lp
                    WHERE LOWER(lp.TenLoai) = 'presidential'
                      AND NOT EXISTS (SELECT 1 FROM Phong WHERE SoPhong = '307');

                    INSERT INTO Phong (MaLoai, SoPhong, Tang, TrangThai)
                    SELECT lp.MaLoai, '308', 3, 'Trống'
                    FROM LoaiPhong lp
                    WHERE LOWER(lp.TenLoai) = 'presidential'
                      AND NOT EXISTS (SELECT 1 FROM Phong WHERE SoPhong = '308');";
                DatabaseHelper.ExecuteNonQuery(sqlPhong);

                var checkBangGia = Convert.ToInt32(DatabaseHelper.ExecuteScalar(
                    "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='BangGia';") ?? 0);
                if (checkBangGia > 0)
                {
                    var sqlBangGia = @"
                        INSERT INTO BangGia (MaLoai, TuNgay, DenNgay, Gia, LoaiGia)
                        SELECT lp.MaLoai, '2026-01-01', '2026-12-31', lp.GiaCoBan, 'NgayThuong'
                        FROM LoaiPhong lp
                        WHERE LOWER(lp.TenLoai) IN ('superior', 'family', 'presidential')
                          AND NOT EXISTS (SELECT 1 FROM BangGia bg WHERE bg.MaLoai = lp.MaLoai);";
                    DatabaseHelper.ExecuteNonQuery(sqlBangGia);
                }

                _daKiemTraDuLieuMau = true;
            }
            catch (Exception ex)
            {
                AppLogger.Error(ex, "Đảm bảo dữ liệu hạng phòng mẫu");
            }
        }
    }

    private RoomType MapToRoomType(DataRow r)
    {
        return new RoomType
        {
            RoomTypeId = GetValue<int>(r, "MaLoai"),
            Name = GetValue<string>(r, "TenLoai", ""),
            BasePrice = GetValue<double>(r, "GiaCoBan"),
            Capacity = GetValue(r, "SucChua", 2),
            Description = GetValue<string>(r, "MoTa", ""),
            ImagePath = string.Empty
        };
    }
}
