namespace DAL.Repositories;

public class SystemLogRepository : BaseRepository ,ISystemLogRepository
{
    public void LogAction(int accountId, string action, string affectedTable)
    {
        throw new NotImplementedException();
    }

    public List<SystemLog> GetLogs(DateTime? fromDate = null, DateTime? toDate = null, string? usernameKeyword = null)
    {
        throw new NotImplementedException();
    }
}