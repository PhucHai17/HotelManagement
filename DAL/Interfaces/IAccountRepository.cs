using Entity.Entities;

namespace DAL.Interfaces;

public interface IAccountRepository : IGenericRepository<Account>
{
    Account? GetByUsername(string username);
    bool ChangePassword(int accountId, string newPasswordHash);
}
