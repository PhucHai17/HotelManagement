using Entity.Constants;

namespace Entity.Entities;

public class Account
{
    public int AccountId { get; set; }
    public int RoleId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public int FailedLoginCount { get; set; }
    public string Status { get; set; } = AccountStatus.Active;

    public Role? Role { get; set; }
}
}