namespace DAL.Interfaces;

// Interface repository tài khoản
public interface IAccountRepository
{
    Account? Login(string username, string password);
    int CreateAccount(Account account);
    bool ChangePassword(int accountId, string newPassword);
    bool Exists(string username);
    List<string> GetPermissionsByRoleId(int roleId);
}
