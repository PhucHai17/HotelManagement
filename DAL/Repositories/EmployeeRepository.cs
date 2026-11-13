namespace DAL.Repositories;

public class EmployeeRepository :BaseRepository ,IEmployeeRepository
{
    // Lấy nhân viên theo mã tài khoản đăng nhập
    public List<Employee> GetAll()
    {
        throw new NotImplementedException();
    }

    public List<Employee> Search(string keyword)
    {
        throw new NotImplementedException();
    }

    public Employee? GetByAccountId(int accountId)
    {
        return QuerySingle("SELECT * FROM NhanVien WHERE MaTK = @MaTK LIMIT 1;", MapRow, CreateParameter("@MaTK", accountId));
    }

    // Lấy nhân viên theo mã nhân viên
    public Employee? GetById(int employeeId)
    {
        return QuerySingle("SELECT * FROM NhanVien WHERE MaNV = @MaNV LIMIT 1;", MapRow, CreateParameter("@MaNV", employeeId));
    }

    public int CreateEmployeeWithAccount(Employee employee, Account account)
    {
        throw new NotImplementedException();
    }

    public bool UpdateEmployee(Employee employee, int roleId)
    {
        throw new NotImplementedException();
    }

    public bool SetAccountLockStatus(int accountId, string accountStatus, string employeeStatus)
    {
        throw new NotImplementedException();
    }

    public int CountActiveAdmins()
    {
        throw new NotImplementedException();
    }

    // Chuyển DataRow thành Employee
    private Employee MapRow(DataRow row)
    {
        return new Employee
        {
            EmployeeId = GetValue<int>(row, "MaNV"),
            AccountId = GetValue<int>(row, "MaTK"),
            ManagerId = GetNullableInt(row, "MaQL"),
            FullName = GetValue<string>(row, "HoTen", ""),
            PhoneNumber = GetValue<string>(row, "SDT", ""),
            NationalId = GetValue<string>(row, "CCCD", ""),
            Position = GetValue<string>(row, "ChucVu", "Lễ tân"),
            HireDate = GetValue<string>(row, "NgayVaoLam", ""),
            Status = GetValue<string>(row, "TrangThai", "Đang làm việc")
        };
    }
}