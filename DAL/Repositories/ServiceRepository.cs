using HotelManagementSystem.Models.Entities;

namespace DAL.Repositories;

public class ServiceRepository : BaseRepository ,IServiceRepository
{
    private Service MapToService(DataRow r) => new Service
    {
        ServiceId = GetValue<int>(r, "MaDV"),
        Name = GetValue<string>(r, "TenDV", ""),
        UnitPrice = GetValue<double>(r, "DonGia"),
        Unit = GetValue<string>(r, "DonVi", "")
    };
    public List<Service> GetAll()
    {
        var sql = "SELECT * FROM DichVu ORDER BY MaDV ASC;";
        var table = DatabaseHelper.ExecuteDataTable(sql);
        var list = new List<Service>();
        foreach (DataRow r in table.Rows) list.Add(MapToService(r));
        return list;
    }

    public Service? GetById(int id)
    {
        var sql = "SELECT * FROM DichVu WHERE MaDV = @MaDV LIMIT 1;";
        var table = DatabaseHelper.ExecuteDataTable(sql, CreateParameter("@MaDV", id));
        return table.Rows.Count > 0 ? MapToService(table.Rows[0]) : null;
    }

    public bool Add(Service entity)
    {
        var id = AddService(entity);
        if (id > 0)
        {
            entity.ServiceId = id;
            return true;
        }
        return false;
    }

    public bool Update(Service entity)
    {
        var sql = "UPDATE DichVu SET TenDV = @TenDV, DonGia = @DonGia, DonVi = @DonVi WHERE MaDV = @MaDV;";
        return DatabaseHelper.ExecuteNonQuery(sql,
            CreateParameter("@TenDV", entity.Name),
            CreateParameter("@DonGia", entity.UnitPrice),
            CreateParameter("@DonVi", entity.Unit),
            CreateParameter("@MaDV", entity.ServiceId)) > 0;
    }

    public bool Delete(int id)
    {
        var sql = "DELETE FROM DichVu WHERE MaDV = @MaDV;";
        return DatabaseHelper.ExecuteNonQuery(sql, CreateParameter("@MaDV", id)) > 0;
    }

    public int AddService(Service service)
    {
        var sql = @"
                INSERT INTO DichVu (TenDV, DonGia, DonVi)
                VALUES (@TenDV, @DonGia, @DonVi);
                SELECT last_insert_rowid();";

        var id = DatabaseHelper.ExecuteScalar(sql,
            CreateParameter("@TenDV", service.Name),
            CreateParameter("@DonGia", service.UnitPrice),
            CreateParameter("@DonVi", service.Unit));

        return id != null ? Convert.ToInt32(id) : 0;
    }

    public int RecordServiceUsage(int bookingDetailId, int serviceId, int quantity, double unitPrice, string usageDate)
    {
        var sql = @"
                INSERT INTO SuDungDichVu (MaCTDP, MaDV, SoLuong, DonGia, NgaySD)
                VALUES (@MaCTDP, @MaDV, @SoLuong, @DonGia, @NgaySD);
                SELECT last_insert_rowid();";

        var id = DatabaseHelper.ExecuteScalar(sql,
            CreateParameter("@MaCTDP", bookingDetailId),
            CreateParameter("@MaDV", serviceId),
            CreateParameter("@SoLuong", quantity),
            CreateParameter("@DonGia", unitPrice),
            CreateParameter("@NgaySD", usageDate));

        return id != null ? Convert.ToInt32(id) : 0;
    }

    public List<ServiceUsage> GetUsagesByBookingDetailId(int bookingDetailId)
    {
        var sql = @"
                SELECT sd.*, dv.TenDV, dv.DonVi, p.SoPhong
                FROM SuDungDichVu sd
                INNER JOIN DichVu dv ON sd.MaDV = dv.MaDV
                INNER JOIN ChiTietDatPhong ctdp ON sd.MaCTDP = ctdp.MaCTDP
                INNER JOIN Phong p ON ctdp.MaPhong = p.MaPhong
                WHERE sd.MaCTDP = @MaCTDP;";

        var table = DatabaseHelper.ExecuteDataTable(sql, CreateParameter("@MaCTDP", bookingDetailId));
        var list = new List<ServiceUsage>();
        foreach (DataRow r in table.Rows)
            list.Add(new ServiceUsage
            {
                UsageId = GetValue<int>(r, "MaSDDV"),
                BookingDetailId = GetValue<int>(r, "MaCTDP"),
                ServiceId = GetValue<int>(r, "MaDV"),
                Quantity = GetValue(r, "SoLuong", 1),
                UnitPrice = GetValue<double>(r, "DonGia"),
                UsageDate = GetValue<string>(r, "NgaySD", ""),
                ServiceName = GetValue<string>(r, "TenDV", ""),
                Unit = GetValue<string>(r, "DonVi", ""),
                RoomNumber = GetValue<string>(r, "SoPhong", "")
            });
        return list;
    }

    public List<ServiceUsage> GetUsagesByBookingId(int bookingId)
    {
        var sql = @"
                SELECT sd.*, dv.TenDV, dv.DonVi, p.SoPhong
                FROM SuDungDichVu sd
                INNER JOIN DichVu dv ON sd.MaDV = dv.MaDV
                INNER JOIN ChiTietDatPhong ctdp ON sd.MaCTDP = ctdp.MaCTDP
                INNER JOIN Phong p ON ctdp.MaPhong = p.MaPhong
                WHERE ctdp.MaDP = @MaDP;";

        var table = DatabaseHelper.ExecuteDataTable(sql, CreateParameter("@MaDP", bookingId));
        var list = new List<ServiceUsage>();
        foreach (DataRow r in table.Rows)
            list.Add(new ServiceUsage
            {
                UsageId = GetValue<int>(r, "MaSDDV"),
                BookingDetailId = GetValue<int>(r, "MaCTDP"),
                ServiceId = GetValue<int>(r, "MaDV"),
                Quantity = GetValue(r, "SoLuong", 1),
                UnitPrice = GetValue<double>(r, "DonGia"),
                UsageDate = GetValue<string>(r, "NgaySD", ""),
                ServiceName = GetValue<string>(r, "TenDV", ""),
                Unit = GetValue<string>(r, "DonVi", ""),
                RoomNumber = GetValue<string>(r, "SoPhong", "")
            });
        return list;
    }

    public List<BookingDetail> GetCustomerBookingDetailsForService(int customerId)
    {
        var sql = @"
                SELECT ctdp.MaCTDP, ctdp.MaDP, ctdp.MaPhong, p.SoPhong, lp.TenLoai
                FROM ChiTietDatPhong ctdp
                INNER JOIN DatPhong dp ON ctdp.MaDP = dp.MaDP
                INNER JOIN Phong p ON ctdp.MaPhong = p.MaPhong
                INNER JOIN LoaiPhong lp ON p.MaLoai = lp.MaLoai
                WHERE dp.MaKH = @MaKH AND dp.TrangThai <> 'Đã hủy'
                ORDER BY dp.MaDP DESC, ctdp.MaCTDP DESC
                LIMIT 5;";

        var table = DatabaseHelper.ExecuteDataTable(sql, CreateParameter("@MaKH", customerId));
        var list = new List<BookingDetail>();
        foreach (DataRow r in table.Rows)
        {
            list.Add(new BookingDetail
            {
                BookingDetailId = GetValue<int>(r, "MaCTDP"),
                BookingId = GetValue<int>(r, "MaDP"),
                RoomId = GetValue<int>(r, "MaPhong"),
                RoomNumber = GetValue<string>(r, "SoPhong", ""),
                RoomTypeName = GetValue<string>(r, "TenLoai", "")
            });
        }
        return list;
    }

    public List<ServiceUsage> GetUsagesByCustomerId(int customerId)
    {
        var sql = @"
                SELECT sd.*, dv.TenDV, dv.DonVi, p.SoPhong
                FROM SuDungDichVu sd
                INNER JOIN DichVu dv ON sd.MaDV = dv.MaDV
                INNER JOIN ChiTietDatPhong ctdp ON sd.MaCTDP = ctdp.MaCTDP
                INNER JOIN DatPhong dp ON ctdp.MaDP = dp.MaDP
                INNER JOIN Phong p ON ctdp.MaPhong = p.MaPhong
                WHERE dp.MaKH = @MaKH
                ORDER BY sd.MaSDDV DESC;";

        var table = DatabaseHelper.ExecuteDataTable(sql, CreateParameter("@MaKH", customerId));
        var list = new List<ServiceUsage>();
        foreach (DataRow r in table.Rows)
        {
            list.Add(new ServiceUsage
            {
                UsageId = GetValue<int>(r, "MaSDDV"),
                BookingDetailId = GetValue<int>(r, "MaCTDP"),
                ServiceId = GetValue<int>(r, "MaDV"),
                Quantity = GetValue(r, "SoLuong", 1),
                UnitPrice = GetValue<double>(r, "DonGia"),
                UsageDate = GetValue<string>(r, "NgaySD", ""),
                ServiceName = GetValue<string>(r, "TenDV", ""),
                Unit = GetValue<string>(r, "DonVi", ""),
                RoomNumber = GetValue<string>(r, "SoPhong", "")
            });
        }
        return list;
    }
}