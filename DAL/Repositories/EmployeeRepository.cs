namespace DAL.Repositories;

public class EmployeeRepository : BaseRepository, IEmployeeRepository
{
    private const string BaseSelectSql = @"
        SELECT nv.*, tk.MaVT, tk.TenDangNhap, tk.TrangThai AS TaiKhoanTrangThai, vt.TenVT
        FROM NhanVien nv
        LEFT JOIN TaiKhoan tk ON nv.MaTK = tk.MaTK
        LEFT JOIN VaiTro vt ON tk.MaVT = vt.MaVT";
    
    public List<Employee> GetAll()
    {
        var sql = $"{BaseSelectSql} ORDER BY nv.MaNV DESC;";
        var table = DatabaseHelper.ExecuteDataTable(sql);
        var list = new List<Employee>();
        foreach (DataRow row in table.Rows)
        {
            list.Add(MapRow(row));
        }
        return list;
    }

    // Tìm kiếm nhân viên theo họ tên, sđt, cccd hoặc email bắt đầu bằng từ khóa
    public List<Employee> Search(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return GetAll();

        var cleanKeyword = keyword.Trim();
        var sql = $@"
            {BaseSelectSql}
            WHERE LOWER(nv.HoTen) LIKE LOWER(@ContainsKeyword)
               OR nv.SDT LIKE @ContainsKeyword
               OR nv.CCCD LIKE @ContainsKeyword
               OR LOWER(tk.TenDangNhap) LIKE LOWER(@PrefixKeyword)
            ORDER BY nv.MaNV DESC;";

        var table = DatabaseHelper.ExecuteDataTable(sql,
            CreateParameter("@ContainsKeyword", $"%{cleanKeyword}%"),
            CreateParameter("@PrefixKeyword", $"{cleanKeyword}%"));
        var list = new List<Employee>();
        foreach (DataRow row in table.Rows)
        {
            list.Add(MapRow(row));
        }
        return list;
    }
    
    public Employee? GetByAccountId(int accountId)
    {
        var sql = $"{BaseSelectSql} WHERE nv.MaTK = @MaTK LIMIT 1;";
        var table = DatabaseHelper.ExecuteDataTable(sql, CreateParameter("@MaTK", accountId));
        return table.Rows.Count > 0 ? MapRow(table.Rows[0]) : null;
    }
    
    public Employee? GetById(int employeeId)
    {
        var sql = $"{BaseSelectSql} WHERE nv.MaNV = @MaNV LIMIT 1;";
        var table = DatabaseHelper.ExecuteDataTable(sql, CreateParameter("@MaNV", employeeId));
        return table.Rows.Count > 0 ? MapRow(table.Rows[0]) : null;
    }
    
    public int CreateEmployeeWithAccount(Employee employee, Account account)
    {
        using var connection = DBConnection.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            int newAccountId;
            using (var cmdAcc = connection.CreateCommand())
            {
                cmdAcc.Transaction = transaction;
                cmdAcc.CommandText = @"
                    INSERT INTO TaiKhoan (MaVT, TenDangNhap, MatKhauHash, TrangThai)
                    VALUES (@MaVT, @TenDangNhap, @MatKhauHash, @TrangThai);
                    SELECT last_insert_rowid();";

                var normalizedStatus = AccountStatusHelper.NormalizeDatabaseValue(
                    account.Status ?? AppConstants.AccountStatusActive);

                cmdAcc.Parameters.AddWithValue("@MaVT", account.RoleId);
                cmdAcc.Parameters.AddWithValue("@TenDangNhap", account.Username);
                cmdAcc.Parameters.AddWithValue("@MatKhauHash", account.PasswordHash);
                cmdAcc.Parameters.AddWithValue("@TrangThai", normalizedStatus);

                newAccountId = Convert.ToInt32(cmdAcc.ExecuteScalar());
            }

            if (newAccountId <= 0)
            {
                transaction.Rollback();
                return 0;
            }

            int newEmployeeId;
            using (var cmdEmp = connection.CreateCommand())
            {
                cmdEmp.Transaction = transaction;
                cmdEmp.CommandText = @"
                    INSERT INTO NhanVien (MaTK, MaQL, HoTen, SDT, CCCD, ChucVu, NgayVaoLam, TrangThai)
                    VALUES (@MaTK, @MaQL, @HoTen, @SDT, @CCCD, @ChucVu, @NgayVaoLam, @TrangThai);
                    SELECT last_insert_rowid();";

                cmdEmp.Parameters.AddWithValue("@MaTK", newAccountId);
                cmdEmp.Parameters.AddWithValue("@MaQL", (object?)employee.ManagerId ?? DBNull.Value);
                cmdEmp.Parameters.AddWithValue("@HoTen", employee.FullName);
                cmdEmp.Parameters.AddWithValue("@SDT", employee.PhoneNumber);
                cmdEmp.Parameters.AddWithValue("@CCCD", employee.NationalId);
                cmdEmp.Parameters.AddWithValue("@ChucVu", employee.Position);
                cmdEmp.Parameters.AddWithValue("@NgayVaoLam", employee.HireDate);
                cmdEmp.Parameters.AddWithValue("@TrangThai", employee.Status);

                newEmployeeId = Convert.ToInt32(cmdEmp.ExecuteScalar());
            }

            transaction.Commit();
            return newEmployeeId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
    
    public bool UpdateEmployee(Employee employee, int roleId)
    {
        return UpdateEmployee(employee, roleId, null);
    }
    
    public bool UpdateEmployee(Employee employee, int roleId, string? newPasswordHash)
    {
        using var connection = DBConnection.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            using (var cmdEmp = connection.CreateCommand())
            {
                cmdEmp.Transaction = transaction;
                cmdEmp.CommandText = @"
                    UPDATE NhanVien
                    SET HoTen = @HoTen,
                        SDT = @SDT,
                        CCCD = @CCCD,
                        ChucVu = @ChucVu
                    WHERE MaNV = @MaNV;";

                cmdEmp.Parameters.AddWithValue("@HoTen", employee.FullName);
                cmdEmp.Parameters.AddWithValue("@SDT", employee.PhoneNumber);
                cmdEmp.Parameters.AddWithValue("@CCCD", employee.NationalId);
                cmdEmp.Parameters.AddWithValue("@ChucVu", employee.Position);
                cmdEmp.Parameters.AddWithValue("@MaNV", employee.EmployeeId);
                cmdEmp.ExecuteNonQuery();
            }

            if (employee.AccountId > 0)
            {
                using var cmdAcc = connection.CreateCommand();
                cmdAcc.Transaction = transaction;
                var newUsername = employee.Account?.Username?.Trim();
                var pwdHash = !string.IsNullOrWhiteSpace(newPasswordHash)
                    ? newPasswordHash
                    : employee.Account?.PasswordHash;

                var updates = new List<string> { "MaVT = @MaVT" };
                cmdAcc.Parameters.AddWithValue("@MaVT", roleId);
                cmdAcc.Parameters.AddWithValue("@MaTK", employee.AccountId);

                if (!string.IsNullOrWhiteSpace(newUsername))
                {
                    updates.Add("TenDangNhap = @TenDangNhap");
                    cmdAcc.Parameters.AddWithValue("@TenDangNhap", newUsername);
                }

                if (!string.IsNullOrWhiteSpace(pwdHash))
                {
                    updates.Add("MatKhauHash = @MatKhauHash");
                    cmdAcc.Parameters.AddWithValue("@MatKhauHash", pwdHash);
                }

                cmdAcc.CommandText = $"UPDATE TaiKhoan SET {string.Join(", ", updates)} WHERE MaTK = @MaTK;";
                cmdAcc.ExecuteNonQuery();
            }

            transaction.Commit();
            return true;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    // Cập nhật trạng thái khóa hoặc mở khóa tài khoản và hồ sơ nhân viên
    public bool SetAccountLockStatus(int accountId, string accountStatus, string employeeStatus)
    {
        using var connection = DBConnection.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            using (var cmdAcc = connection.CreateCommand())
            {
                cmdAcc.Transaction = transaction;
                cmdAcc.CommandText = "UPDATE TaiKhoan SET TrangThai = @TrangThai WHERE MaTK = @MaTK;";
                cmdAcc.Parameters.AddWithValue("@TrangThai", accountStatus);
                cmdAcc.Parameters.AddWithValue("@MaTK", accountId);
                cmdAcc.ExecuteNonQuery();
            }

            using (var cmdEmp = connection.CreateCommand())
            {
                cmdEmp.Transaction = transaction;
                cmdEmp.CommandText = "UPDATE NhanVien SET TrangThai = @TrangThai WHERE MaTK = @MaTK;";
                cmdEmp.Parameters.AddWithValue("@TrangThai", employeeStatus);
                cmdEmp.Parameters.AddWithValue("@MaTK", accountId);
                cmdEmp.ExecuteNonQuery();
            }

            transaction.Commit();
            return true;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
    
    public int CountActiveAdmins()
    {
        var sql = $@"
            SELECT COUNT(*)
            FROM TaiKhoan tk
            INNER JOIN NhanVien nv ON tk.MaTK = nv.MaTK
            WHERE tk.MaVT = @MaVT
              AND {AccountStatusHelper.BuildNotLockedSqlCondition("tk.TrangThai")};";

        var result = DatabaseHelper.ExecuteScalar(sql, CreateParameter("@MaVT", AppConstants.MaVTAdmin));
        return result != null ? Convert.ToInt32(result) : 0;
    }

    private Employee MapRow(DataRow row)
    {
        var accountId = GetValue<int>(row, "MaTK");
        Account? account = null;

        if (accountId > 0 && row.Table.Columns.Contains("TenDangNhap") && row["TenDangNhap"] != DBNull.Value)
        {
            account = new Account
            {
                AccountId = accountId,
                RoleId = GetValue<int>(row, "MaVT"),
                Username = GetValue<string>(row, "TenDangNhap", ""),
                Status = AccountStatusHelper.NormalizeDatabaseValue(
                    GetValue<string>(row, "TaiKhoanTrangThai", AppConstants.AccountStatusActive)),
                RoleName = row.Table.Columns.Contains("TenVT") ? GetValue<string>(row, "TenVT", "") : ""
            };
        }
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
            Status = GetValue<string>(row, "TrangThai", "Đang làm việc"),
            Account = account
        };
    }
}
