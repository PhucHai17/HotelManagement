

using Common.Constants;
using Entity.Enums;

namespace Common.Helpers;
public static class AccountStatusHelper
{
    private static readonly string[] LockedTokens =
    [
        AppConstants.AccountStatusLocked,
        AppConstants.AccountStatusLockedDisplay,
        "khoa",
        "bị khóa",
        "dakhoa",
        "inactive",
        "locked"
    ];

    // Danh sách giá trị hoạt động
    private static readonly string[] ActiveTokens =
    [
        AppConstants.AccountStatusActive,
        AppConstants.AccountStatusActiveDisplay,
        AppConstants.EmployeeStatusWorking,
        "active"
    ];

    private static readonly HashSet<string> LockedTokenSet = new(LockedTokens, StringComparer.OrdinalIgnoreCase);

    // Kiểm tra chuỗi trạng thái có thuộc trạng thái bị khóa hay không
    public static bool IsLocked(string? rawStatus)
    {
        if (string.IsNullOrWhiteSpace(rawStatus)) return false;
        return LockedTokenSet.Contains(rawStatus.Trim());
    }

    // Chuyển chuỗi trạng thái thô sang enum AccountStatus
    public static AccountStatus Parse(string? rawStatus)
    {
        return IsLocked(rawStatus) ? AccountStatus.Locked : AccountStatus.Active;
    }
    public static bool IsLocked(AccountStatus status)
    {
        return status == AccountStatus.Locked;
    }
    // Chuyển enum sang giá trị lưu trong cột TaiKhoan.TrangThai
    public static string ToDatabaseValue(AccountStatus status)
    {
        return status == AccountStatus.Locked
            ? AppConstants.AccountStatusLocked
            : AppConstants.AccountStatusActive;
    }

    // Chuẩn hóa chuỗi thô thành giá trị chuẩn lưu trong cột TaiKhoan.TrangThai
    public static string NormalizeDatabaseValue(string? rawStatus)
    {
        return ToDatabaseValue(Parse(rawStatus));
    }

    // Lấy trạng thái làm việc của nhân viên tương ứng với trạng thái khóa
    public static string ToEmployeeStatus(bool isLocked)
    {
        return isLocked ? AppConstants.EmployeeStatusLocked : AppConstants.EmployeeStatusWorking;
    }

    public static string ToEmployeeStatus(AccountStatus status)
    {
        return ToEmployeeStatus(status == AccountStatus.Locked);
    }

    // Lấy nhãn tiếng Việt hiển thị trên giao diện
    public static string ToDisplayText(string? rawStatus)
    {
        return IsLocked(rawStatus)
            ? AppConstants.AccountStatusLockedDisplay
            : AppConstants.AccountStatusActiveDisplay;
    }

    public static string ToDisplayText(AccountStatus status)
    {
        return status == AccountStatus.Locked
            ? AppConstants.AccountStatusLockedDisplay
            : AppConstants.AccountStatusActiveDisplay;
    }

    // Tạo danh sách literal SQL từ một nguồn token
    private static string BuildSqlInValues(IEnumerable<string> tokens)
    {
        return string.Join(", ", tokens
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .SelectMany(t =>
            {
                var trimmed = t.Trim();
                return new[] { trimmed, trimmed.ToLowerInvariant() };
            })
            .Distinct()
            .Select(t => $"'{t.Replace("'", "''")}'"));
    }

    // Điều kiện SQL lọc tài khoản đang bị khóa
    public static string BuildLockedSqlCondition(string columnExpression)
    {
        return $"LOWER(TRIM(COALESCE({columnExpression}, ''))) IN ({BuildSqlInValues(LockedTokens)})";
    }

    // Điều kiện SQL lọc tài khoản không bị khóa
    public static string BuildNotLockedSqlCondition(string columnExpression)
    {
        return $"({columnExpression} IS NULL OR LOWER(TRIM({columnExpression})) NOT IN ({BuildSqlInValues(LockedTokens)}))";
    }

    // Điều kiện SQL nhận diện trạng thái hoạt động cũ/mới hoặc rỗng để chuẩn hóa DB
    public static string BuildActiveOrEmptySqlCondition(string columnExpression)
    {
        return $"({columnExpression} IS NULL OR TRIM({columnExpression}) = '' OR LOWER(TRIM({columnExpression})) IN ({BuildSqlInValues(ActiveTokens)}))";
    }
}
