using Entity.Enums;

namespace Entity.Entities;

public class Account {
    public int AccountId { get; set; }
    public int RoleId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public int FailedLoginCount { get; set; }
    public string Status { get; set; } = String.Empty;
    public string RoleName { get; set; } = string.Empty;
    public int FailedAttempts { get; set; }

    public Account() { }
    public Account(int accountId, int roleId, string username, string passwordHash, string status)
    {
        AccountId = accountId; RoleId = roleId; Username = username; PasswordHash = passwordHash; Status = status;
    }
    
}