namespace DAL.Repositories;

public class AuditLogRepository : BaseRepository
{
    public void WriteLog(int accountId, string action, string affectedTable)
    {
        try
        {
            DatabaseHelper.ExecuteNonQuery(
                "INSERT INTO NhatKyHeThong (MaTK, HanhDong, BangTacDong, ThoiGian) VALUES (@MaTK, @HanhDong, @BangTacDong, @ThoiGian);",
                CreateParameter("@MaTK", accountId), CreateParameter("@HanhDong", action),
                CreateParameter("@BangTacDong", affectedTable),
                CreateParameter("@ThoiGian", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")));
        }
        catch
        {
            // bo qua looix
        }
    }
}