using Common.Helpers;
using DAL.Interfaces;
using Entity.Entities;

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

    // Lấy danh sách nhật ký hệ thống
    public List<SystemLog> GetLogs(DateTime? fromDate = null, DateTime? toDate = null, string? usernameKeyword = null)
    {
        return new List<SystemLog>();
    }
}
