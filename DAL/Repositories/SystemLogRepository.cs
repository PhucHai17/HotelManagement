using System.Globalization;
using System.Text;
using Common.Helpers;
using DAL.Interfaces;
using Entity.Entities;
using Microsoft.Data.Sqlite;

namespace DAL.Repositories;

public class SystemLogRepository : BaseRepository, ISystemLogRepository
{
    // Ghi nhật ký hoạt động hệ thống
    public void LogAction(int accountId, string action, string affectedTable)
    {
        try
        {
            DatabaseHelper.ExecuteNonQuery(
                "INSERT INTO NhatKyHeThong (MaTK, HanhDong, BangTacDong, ThoiGian) VALUES (@MaTK, @HanhDong, @BangTacDong, @ThoiGian);",
                CreateParameter("@MaTK", accountId),
                CreateParameter("@HanhDong", action),
                CreateParameter("@BangTacDong", affectedTable),
                CreateParameter("@ThoiGian", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")));
        }
        catch (Exception ex)
        {
            AppLogger.Error(ex, "Ghi nhật ký hoạt động hệ thống");
        }
    }
    
    public List<SystemLog> GetLogs(string? keyword = null, string? tableFilter = null, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var sql = new StringBuilder(@"
            SELECT nk.MaNK, nk.MaTK, nk.HanhDong, nk.BangTacDong, nk.ThoiGian, tk.TenDangNhap, nv.HoTen
            FROM NhatKyHeThong nk
            LEFT JOIN TaiKhoan tk ON nk.MaTK = tk.MaTK
            LEFT JOIN NhanVien nv ON tk.MaTK = nv.MaTK
            WHERE 1=1");

        var parameters = new List<SqliteParameter>();

        if (!string.IsNullOrWhiteSpace(tableFilter) && !tableFilter.Equals("Tất cả", StringComparison.OrdinalIgnoreCase))
        {
            sql.Append(" AND LOWER(nk.BangTacDong) = LOWER(@TableFilter)");
            parameters.Add(CreateParameter("@TableFilter", tableFilter.Trim()));
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var cleanKeyword = keyword.Trim();
            sql.Append(" AND (LOWER(nk.HanhDong) LIKE LOWER(@Keyword) OR LOWER(nv.HoTen) LIKE LOWER(@Keyword) OR LOWER(tk.TenDangNhap) LIKE LOWER(@Keyword))");
            parameters.Add(CreateParameter("@Keyword", $"%{cleanKeyword}%"));
        }

        sql.Append(" ORDER BY nk.MaNK DESC;");

        var table = DatabaseHelper.ExecuteDataTable(sql.ToString(), parameters.ToArray());
        var list = new List<SystemLog>();

        foreach (DataRow row in table.Rows)
        {
            var rawTime = GetValue<string>(row, "ThoiGian", string.Empty);
            var logDate = ParseLogDate(rawTime);

            if (fromDate.HasValue && logDate.HasValue && logDate.Value.Date < fromDate.Value.Date)
                continue;

            if (toDate.HasValue && logDate.HasValue && logDate.Value.Date > toDate.Value.Date)
                continue;

            var tenDangNhap = GetValue<string>(row, "TenDangNhap");
            var accountUsername = !string.IsNullOrWhiteSpace(tenDangNhap) ? tenDangNhap.Trim() : "Hệ thống";

            list.Add(new SystemLog
            {
                LogId = GetValue<int>(row, "MaNK"),
                AccountId = GetValue<int>(row, "MaTK"),
                Action = GetValue<string>(row, "HanhDong", string.Empty),
                AffectedTable = GetValue<string>(row, "BangTacDong", string.Empty),
                Timestamp = rawTime,
                Username = accountUsername
            });
        }

        return list.OrderByDescending(x => ParseLogDate(x.Timestamp) ?? DateTime.MinValue)
                   .ThenByDescending(x => x.LogId)
                   .ToList();
    }
    
    private static DateTime? ParseLogDate(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return null;
        if (DateTime.TryParseExact(raw, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var d1)) return d1;
        if (DateTime.TryParseExact(raw, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var d2)) return d2;
        if (DateTime.TryParseExact(raw, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var d3)) return d3;
        if (DateTime.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.None, out var d4)) return d4;
        if (DateTime.TryParse(raw, out var d5)) return d5;
        return null;
    }
}
