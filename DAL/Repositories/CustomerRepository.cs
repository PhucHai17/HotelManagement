namespace DAL.Repositories;

public class CustomerRepository : BaseRepository ,ICustomerRepository
{
    // SELECT dùng chung: khách + tên hạng thành viên + số lần đã lưu trú
    private const string SelectSql = @"
        SELECT kh.*, h.TenHang,
               (SELECT COUNT(*) FROM DatPhong dp WHERE dp.MaKH = kh.MaKH) AS SoLanO
        FROM KhachHang kh LEFT JOIN HangThanhVien h ON kh.MaHang = h.MaHang ";

    // Lấy danh sách tất cả khách hàng
    public List<Customer> GetAll()
    {
        return QueryList(SelectSql + " ORDER BY kh.MaKH DESC;", MapRow);
    }

    // Tìm kiếm khách theo tên, SĐT hoặc CCCD (wireframe trang 6); từ khóa rỗng thì trả về tất cả
    public List<Customer> Search(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword)) return GetAll();
        return QueryList(
            SelectSql +
            " WHERE kh.HoTen LIKE @TuKhoa OR kh.SDT LIKE @TuKhoa OR kh.CCCD LIKE @TuKhoa ORDER BY kh.MaKH DESC;",
            MapRow, CreateParameter("@TuKhoa", $"%{keyword}%"));
    }

    // Lấy thông tin khách hàng theo mã tài khoản đăng nhập
    public Customer? GetByAccountId(int accountId)
    {
        return QuerySingle(SelectSql + " WHERE kh.MaTK = @MaTK LIMIT 1;", MapRow, CreateParameter("@MaTK", accountId));
    }

    public Customer? GetByIdentityCardOrPhone(string keyword)
    {
        return QuerySingle(SelectSql + " WHERE kh.CCCD = @TuKhoa OR kh.SDT = @TuKhoa LIMIT 1;",
            MapRow, CreateParameter("@TuKhoa", keyword));
    }

    public int AddCustomer(Customer customer)
    {
        var sql = @"
            INSERT INTO KhachHang (MaTK, MaHang, HoTen, SDT, Email, CCCD, DiemTichLuy)
            VALUES (@MaTK, @MaHang, @HoTen, @SDT, @Email, @CCCD, @DiemTichLuy);
            SELECT last_insert_rowid();";
        return InsertAndGetId(sql,
            CreateParameter("@MaTK", customer.AccountId),
            CreateParameter("@MaHang", customer.TierId > 0 ? customer.TierId : 1),
            CreateParameter("@HoTen", customer.FullName),
            CreateParameter("@SDT", customer.PhoneNumber),
            CreateParameter("@Email", customer.Email),
            CreateParameter("@CCCD", customer.NationalId),
            CreateParameter("@DiemTichLuy", customer.LoyaltyPoints));
    }

    // Cập nhật thông tin khách hàng
    public bool Update(Customer customer)
    {
        var affected = DatabaseHelper.ExecuteNonQuery(
            "UPDATE KhachHang SET HoTen = @HoTen, SDT = @SDT, Email = @Email, CCCD = @CCCD WHERE MaKH = @MaKH;",
            CreateParameter("@HoTen", customer.FullName), CreateParameter("@SDT", customer.PhoneNumber),
            CreateParameter("@Email", customer.Email), CreateParameter("@CCCD", customer.NationalId),
            CreateParameter("@MaKH", customer.CustomerId));
        return affected > 0;
    }

    // Chuyển DataRow thành Customer
    private Customer MapRow(DataRow row)
    {
        return new Customer
        {
            CustomerId = GetValue<int>(row, "MaKH"),
            AccountId = GetNullableInt(row, "MaTK"),
            TierId = GetValue(row, "MaHang", 1),
            FullName = GetValue<string>(row, "HoTen", ""),
            PhoneNumber = GetValue<string>(row, "SDT", ""),
            Email = GetValue<string>(row, "Email", ""),
            NationalId = GetValue<string>(row, "CCCD", ""),
            LoyaltyPoints = GetValue<int>(row, "DiemTichLuy"),
            TierName = GetValue<string>(row, "TenHang", "Thường"),
            StayCount = GetValue<int>(row, "SoLanO")
        };
    }
}