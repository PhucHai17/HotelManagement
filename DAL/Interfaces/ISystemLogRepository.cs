namespace DAL.Interfaces;

// Interface repository nhật ký
public interface ISystemLogRepository
{
    void LogAction(int accountId, string action, string affectedTable);
    List<SystemLog> GetLogs(DateTime? fromDate = null, DateTime? toDate = null, string? usernameKeyword = null);
}
