using Common.Constants;

namespace DAL.Repositories;

// Repository truy vấn tài khoản
public class AccountRepository : BaseRepository, IAccountRepository
{
    // Kiểm tra đăng nhập bằng cách đối chiếu hash SHA-256 qua PasswordHelper
    public Account? Login(string username, string password)
    {
        
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
            return null;

        var sql = $@"
                SELECT tk.*, vt.TenVT 
                FROM TaiKhoan tk
                LEFT JOIN VaiTro vt ON tk.MaVT = vt.MaVT
                WHERE LOWER(tk.TenDangNhap) = LOWER(@TenDangNhap)
                  AND {AccountStatusHelper.BuildNotLockedSqlCondition("tk.TrangThai")}
                LIMIT 1;";

        var table = DatabaseHelper.ExecuteDataTable(sql, CreateParameter("@TenDangNhap", username));
        if (table.Rows.Count == 0) return null;

        var account = MapToAccount(table.Rows[0]);
        if (AccountStatusHelper.IsLocked(account.Status))
            return null;

        if (!PasswordHelper.VerifyPassword(password, account.PasswordHash))
            return null;

        return account;
    }

    // Tạo tài khoản mới
    public int CreateAccount(Account account)
    {
        var sql = @"
                INSERT INTO TaiKhoan (MaVT, TenDangNhap, MatKhauHash, TrangThai)
                VALUES (@MaVT, @TenDangNhap, @MatKhauHash, @TrangThai);
                SELECT last_insert_rowid();";

        var normalizedStatus = account.Status.ToString();
        var id = DatabaseHelper.ExecuteScalar(
            sql,
            CreateParameter("@MaVT", account.RoleId),
            CreateParameter("@TenDangNhap", account.Username),
            CreateParameter("@MatKhauHash", account.PasswordHash),
            CreateParameter("@TrangThai", normalizedStatus));

        return id != null ? Convert.ToInt32(id) : 0;
    }

    // Đổi mật khẩu
    public bool ChangePassword(int accountId, string newPassword)
    {
        var sql = "UPDATE TaiKhoan SET MatKhauHash = @MatKhauMoi WHERE MaTK = @MaTK;";
        var result = DatabaseHelper.ExecuteNonQuery(sql,
            CreateParameter("@MatKhauMoi", newPassword),
            CreateParameter("@MaTK", accountId));
        return result > 0;
    }

    // Kiểm tra tên đăng nhập đã tồn tại chưa
    public bool Exists(string username)
    {
        var sql = "SELECT COUNT(*) FROM TaiKhoan WHERE LOWER(TenDangNhap) = LOWER(@TenDangNhap);";
        var count = Convert.ToInt64(DatabaseHelper.ExecuteScalar(sql, CreateParameter("@TenDangNhap", username)) ?? 0);
        return count > 0;
    }

    // Lấy danh sách tên quyền của vai trò từ bảng VaiTro_Quyen
    public List<string> GetPermissionsByRoleId(int roleId)
    {
        var sql = @"
                SELECT q.TenQuyen
                FROM VaiTro_Quyen vq
                INNER JOIN Quyen q ON vq.MaQuyen = q.MaQuyen
                WHERE vq.MaVT = @MaVT;";

        var table = DatabaseHelper.ExecuteDataTable(sql, CreateParameter("@MaVT", roleId));
        var permissions = new List<string>();
        foreach (DataRow row in table.Rows)
        {
            var perm = row["TenQuyen"]?.ToString();
            if (!string.IsNullOrWhiteSpace(perm)) permissions.Add(perm.Trim());
        }

        return permissions;
    }

    // Chuyển DataRow thành đối tượng Account
    private static Account MapToAccount(DataRow row)
    {
        return new Account
        {
            AccountId = Convert.ToInt32(row["MaTK"]),
            RoleId = Convert.ToInt32(row["MaVT"]),
            Username = row["TenDangNhap"]?.ToString() ?? string.Empty,
            PasswordHash = row["MatKhauHash"]?.ToString() ?? string.Empty,
            FailedAttempts = row["SoLanSaiMK"] != DBNull.Value ? Convert.ToInt32(row["SoLanSaiMK"]) : 0,
            Status = AccountStatusHelper.NormalizeDatabaseValue(row["TrangThai"]?.ToString() ?? AppConstants.AccountStatusActive),
            RoleName = row.Table.Columns.Contains("TenVT") ? row["TenVT"]?.ToString() ?? string.Empty : string.Empty
        };
    }
}
