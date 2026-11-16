namespace DAL.Repositories;

public class RoomTypeRepository : BaseRepository, IRoomTypeRepository
{
    public List<RoomType> GetAll()
    {
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
