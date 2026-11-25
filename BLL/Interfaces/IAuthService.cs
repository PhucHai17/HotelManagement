using Entity.Entities;

namespace BLL.Interfaces;

public interface IAuthService
{
    Account? Authenticate(string username, string password);
    bool Login(string username, string password, out string message);
    bool Register(string fullName, string phone, string email, string identityCard, string password, string confirmPassword, out string message);
    bool ChangePassword(int accountId, string oldPassword, string newPassword, string confirmNewPassword, out string message);
    bool HasPermission(string permissionName);
}