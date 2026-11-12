namespace Entity.Entities;

public class AuditLog
{
    public int LogId { get; set; }
    public int AccountId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string AffectedTable { get; set; } = string.Empty;
    public string Timestamp { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
}