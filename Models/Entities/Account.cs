using Entity.Enums;

namespace Entity.Entities;

public class Account : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role;
    public bool IsActive { get; set; }
    
}