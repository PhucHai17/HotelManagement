using System.Data.Common;
using DAL.Interfaces;
using Entity.Entities;

namespace DAL.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly DbConnection _dbConnection;

    public AccountRepository(DbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public Account? GetById(int id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Account> GetAll()
    {
        throw new NotImplementedException();
    }

    public int Add(Account entity)
    {
        throw new NotImplementedException();
    }

    public bool Update(Account entity)
    {
        throw new NotImplementedException();
    }

    public bool Delete(int id)
    {
        throw new NotImplementedException();
    }

    public Account? GetByUsername(string username)
    {
        throw new NotImplementedException();
    }

    public bool ChangePassword(int accountId, string newPasswordHash)
    {
        throw new NotImplementedException();
    }
}