namespace DAL.Interfaces;

// Interface repository nhật ký hệ thống
public interface ISystemLogRepository
{
    void LogAction(int accountId, string action, string affectedTable);
    List<SystemLog> GetLogs(string? keyword = null, string? tableFilter = null, DateTime? fromDate = null, DateTime? toDate = null);
}
