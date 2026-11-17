using Microsoft.Data.Sqlite;

namespace DAL.Repositories;

public class RoomRepository : BaseRepository, IRoomRepository
{
    public List<Room> GetAll()
    {
        var sql = @"
            SELECT p.*, lp.TenLoai, lp.GiaCoBan, lp.SucChua
            FROM Phong p
            INNER JOIN LoaiPhong lp ON p.MaLoai = lp.MaLoai
            ORDER BY p.Tang ASC, p.SoPhong ASC;";

        var table = DatabaseHelper.ExecuteDataTable(sql);
        var list = new List<Room>();
        foreach (DataRow r in table.Rows) list.Add(MapToRoom(r));
        return list;
    }

    public List<Room> GetByFloor(int floor)
    {
        var sql = @"
            SELECT p.*, lp.TenLoai, lp.GiaCoBan, lp.SucChua
            FROM Phong p
            INNER JOIN LoaiPhong lp ON p.MaLoai = lp.MaLoai
            WHERE p.Tang = @Tang
            ORDER BY p.SoPhong ASC;";

        var table = DatabaseHelper.ExecuteDataTable(sql, CreateParameter("@Tang", floor));
        var list = new List<Room>();
        foreach (DataRow r in table.Rows) list.Add(MapToRoom(r));
        return list;
    }

    public List<Room> GetAvailableRooms(int? roomTypeId = null)
    {
        var sql = @"
            SELECT p.*, lp.TenLoai, lp.GiaCoBan, lp.SucChua
            FROM Phong p
            INNER JOIN LoaiPhong lp ON p.MaLoai = lp.MaLoai
            WHERE p.TrangThai = @TrangThai " + (roomTypeId.HasValue ? " AND p.MaLoai = @MaLoai " : "") +
            " ORDER BY p.SoPhong ASC;";

        var parameters = new List<SqliteParameter>
        {
            CreateParameter("@TrangThai", AppConstants.PhongTrong)
        };
        if (roomTypeId.HasValue) parameters.Add(CreateParameter("@MaLoai", roomTypeId.Value));

        var table = DatabaseHelper.ExecuteDataTable(sql, parameters.ToArray());
        var list = new List<Room>();
        foreach (DataRow r in table.Rows) list.Add(MapToRoom(r));
        return list;
    }

    public bool Add(Room room)
    {
        var sql = @"
            INSERT INTO Phong (MaLoai, SoPhong, Tang, TrangThai)
            VALUES (@MaLoai, @SoPhong, @Tang, @TrangThai);";

        return DatabaseHelper.ExecuteNonQuery(sql,
            CreateParameter("@MaLoai", room.RoomTypeId),
            CreateParameter("@SoPhong", room.RoomNumber),
            CreateParameter("@Tang", room.Floor),
            CreateParameter("@TrangThai", room.Status)) > 0;
    }

    public bool Update(Room room)
    {
        var sql = @"
            UPDATE Phong
            SET MaLoai = @MaLoai,
                SoPhong = @SoPhong,
                Tang = @Tang,
                TrangThai = @TrangThai
            WHERE MaPhong = @MaPhong;";

        return DatabaseHelper.ExecuteNonQuery(sql,
            CreateParameter("@MaLoai", room.RoomTypeId),
            CreateParameter("@SoPhong", room.RoomNumber),
            CreateParameter("@Tang", room.Floor),
            CreateParameter("@TrangThai", room.Status),
            CreateParameter("@MaPhong", room.RoomId)) > 0;
    }

    public bool Delete(int roomId)
    {
        var sql = "DELETE FROM Phong WHERE MaPhong = @MaPhong;";
        return DatabaseHelper.ExecuteNonQuery(sql, CreateParameter("@MaPhong", roomId)) > 0;
    }

    public bool ExistsByRoomNumber(string roomNumber, int? excludeRoomId = null)
    {
        var sql = "SELECT COUNT(1) FROM Phong WHERE SoPhong = @SoPhong";
        if (excludeRoomId.HasValue)
            sql += " AND MaPhong <> @MaPhong";

        var parameters = new List<SqliteParameter>
        {
            CreateParameter("@SoPhong", roomNumber)
        };

        if (excludeRoomId.HasValue)
            parameters.Add(CreateParameter("@MaPhong", excludeRoomId.Value));

        var count = DatabaseHelper.ExecuteScalar(sql, parameters.ToArray());
        return count != null && Convert.ToInt32(count) > 0;
    }

    public bool UpdateStatus(int roomId, string newStatus)
    {
        var sql = "UPDATE Phong SET TrangThai = @TrangThai WHERE MaPhong = @MaPhong;";
        var result = DatabaseHelper.ExecuteNonQuery(sql,
            CreateParameter("@TrangThai", newStatus),
            CreateParameter("@MaPhong", roomId));
        return result > 0;
    }

    public bool UpdateStatusByRoomNumber(string roomNumber, string newStatus)
    {
        var sql = "UPDATE Phong SET TrangThai = @TrangThai WHERE SoPhong = @SoPhong;";
        var result = DatabaseHelper.ExecuteNonQuery(sql,
            CreateParameter("@TrangThai", newStatus),
            CreateParameter("@SoPhong", roomNumber));
        return result > 0;
    }

    public Room? GetById(int roomId)
    {
        var sql = @"
            SELECT p.*, lp.TenLoai, lp.GiaCoBan, lp.SucChua
            FROM Phong p
            INNER JOIN LoaiPhong lp ON p.MaLoai = lp.MaLoai
            WHERE p.MaPhong = @MaPhong LIMIT 1;";

        var table = DatabaseHelper.ExecuteDataTable(sql, CreateParameter("@MaPhong", roomId));
        return table.Rows.Count > 0 ? MapToRoom(table.Rows[0]) : null;
    }

    public Room? GetByRoomNumber(string roomNumber)
    {
        var sql = @"
            SELECT p.*, lp.TenLoai, lp.GiaCoBan, lp.SucChua
            FROM Phong p
            INNER JOIN LoaiPhong lp ON p.MaLoai = lp.MaLoai
            WHERE p.SoPhong = @SoPhong LIMIT 1;";

        var table = DatabaseHelper.ExecuteDataTable(sql, CreateParameter("@SoPhong", roomNumber));
        return table.Rows.Count > 0 ? MapToRoom(table.Rows[0]) : null;
    }

    private Room MapToRoom(DataRow row)
    {
        return new Room
        {
            RoomId = GetValue<int>(row, "MaPhong"),
            RoomTypeId = GetValue<int>(row, "MaLoai"),
            RoomNumber = GetValue<string>(row, "SoPhong", ""),
            Floor = GetValue<int>(row, "Tang", 1),
            Status = GetValue<string>(row, "TrangThai", AppConstants.PhongTrong),
            RoomTypeName = GetValue<string>(row, "TenLoai", ""),
            BasePrice = GetValue<double>(row, "GiaCoBan"),
            Capacity = GetValue<int>(row, "SucChua", 2)
        };
    }
}