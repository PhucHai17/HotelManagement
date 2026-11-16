namespace DAL.Repositories;

// Repository hóa đơn & thanh toán
public class InvoiceRepository : BaseRepository, IInvoiceRepository
{
    // Lấy tất cả hóa đơn
    public List<Invoice> GetAll()
    {
        var sql = @"
                SELECT hd.*, kh.HoTen AS TenKhach, p.SoPhong
                FROM HoaDon hd
                INNER JOIN DatPhong dp ON hd.MaDP = dp.MaDP
                INNER JOIN KhachHang kh ON dp.MaKH = kh.MaKH
                LEFT JOIN ChiTietDatPhong ctdp ON dp.MaDP = ctdp.MaDP
                LEFT JOIN Phong p ON ctdp.MaPhong = p.MaPhong
                ORDER BY hd.MaHD DESC;";

        var table = DatabaseHelper.ExecuteDataTable(sql);
        var list = new List<Invoice>();
        foreach (DataRow r in table.Rows) list.Add(MapToInvoice(r));
        return list;
    }

    // Lấy hóa đơn theo mã
    public Invoice? GetById(int invoiceId)
    {
        var sql = @"
                SELECT hd.*, kh.HoTen AS TenKhach, p.SoPhong
                FROM HoaDon hd
                INNER JOIN DatPhong dp ON hd.MaDP = dp.MaDP
                INNER JOIN KhachHang kh ON dp.MaKH = kh.MaKH
                LEFT JOIN ChiTietDatPhong ctdp ON dp.MaDP = ctdp.MaDP
                LEFT JOIN Phong p ON ctdp.MaPhong = p.MaPhong
                WHERE hd.MaHD = @MaHD LIMIT 1;";

        var table = DatabaseHelper.ExecuteDataTable(sql, CreateParameter("@MaHD", invoiceId));
        if (table.Rows.Count > 0) return MapToInvoice(table.Rows[0]);
        return null;
    }

    // Tạo hóa đơn mới
    public int CreateInvoice(Invoice invoice)
    {
        var sql = @"
                INSERT INTO HoaDon (MaDP, MaNV, TienPhong, TienDV, GiamGia, TongTien, TrangThai,NgayLap)
                VALUES (@MaDP, @MaNV, @TienPhong, @TienDV, @GiamGia, @TongTien, @TrangThai,@NgayLap);
                SELECT last_insert_rowid();";

        var id = DatabaseHelper.ExecuteScalar(sql,
            CreateParameter("@MaDP", invoice.BookingId),
            CreateParameter("@MaNV", invoice.EmployeeId),
            CreateParameter("@TienPhong", invoice.RoomCharge),
            CreateParameter("@TienDV", invoice.ServiceCharge),
            CreateParameter("@GiamGia", invoice.Discount),
            CreateParameter("@TongTien", invoice.VatAmount),
            CreateParameter("@TrangThai", invoice.Status.ToString()),
            CreateParameter("@NgayLap", invoice.IssuedDate)
                );
        return Convert.ToInt32(id ?? 0);
    }

    // Thanh toán hóa đơn & lưu giao dịch
    public bool PayInvoice(int invoiceId, string paymentMethod)
    {
        var invoice = GetById(invoiceId);

        if (invoice == null)
            return false;

        var sqlHD = @"
            UPDATE HoaDon
            SET TrangThai = @TrangThai
            WHERE MaHD = @MaHD;";

        var result = DatabaseHelper.ExecuteNonQuery(
            sqlHD,
            CreateParameter("@TrangThai", InvoiceStatus.Paid.ToString()),
            CreateParameter("@MaHD", invoiceId)
        );

        if (result <= 0)
            return false;

        var sqlTT = @"
            INSERT INTO ThanhToan
            (MaHD, SoTien, HinhThuc, LoaiGD, NgayTT)
            VALUES
            (@MaHD, @SoTien, @HinhThuc, @LoaiGD, @NgayTT);";

        DatabaseHelper.ExecuteNonQuery(
            sqlTT,
            CreateParameter("@MaHD", invoiceId),
            CreateParameter("@SoTien", invoice.TotalAmount),
            CreateParameter("@HinhThuc", paymentMethod),
            CreateParameter("@LoaiGD", "Thanh toán hóa đơn"),
            CreateParameter("@NgayTT", DateTime.Now.ToString("dd/MM/yyyy HH:mm"))
        );

        return true;
    }

    private Invoice MapToInvoice(DataRow r)
    {
        return new Invoice
        {
            InvoiceId = GetValue<int>(r, "MaHD"),
            BookingId = GetValue<int>(r, "MaDP"),

            EmployeeId = r.IsNull("MaNV")
                ? null
                : GetValue<int>(r, "MaNV"),

            RoomCharge = GetValue<double>(r, "TienPhong"),
            ServiceCharge = GetValue<double>(r, "TienDV"),
            Discount = GetValue<double>(r, "GiamGia"),

            VatAmount = r.Table.Columns.Contains("VatAmount")
                ? GetValue<double>(r, "VatAmount")
                : 0,

            TotalAmount = GetValue<double>(r, "TongTien"),

            Status = Enum.TryParse<InvoiceStatus>(
                GetValue<string>(r, "TrangThai", "Unpaid"),
                true,
                out var status
            ) ? status : InvoiceStatus.Unpaid,

            IssuedDate = GetValue<string>(r, "NgayLap", ""),
            CustomerName = GetValue<string>(r, "TenKhach", ""),
            RoomNumber = GetValue<string>(r, "SoPhong", "")
        };
    }
}
