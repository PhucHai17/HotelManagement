using Entity.Entities;

namespace BLL.Interfaces;

// Interface service nhật ký hệ thống
public interface ISystemLogService
{
    List<SystemLog> GetLogs(string? keyword = null, string? tableFilter = null, DateTime? fromDate = null, DateTime? toDate = null);
    void LogAction(int accountId, string action, string affectedTable);
}
