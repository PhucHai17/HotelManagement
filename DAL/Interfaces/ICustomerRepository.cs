namespace DAL.Interfaces;

// Interface repository khách hàng
public interface ICustomerRepository
{
    List<Customer> GetAll();
    List<Customer> Search(string keyword);
    Customer? GetByAccountId(int accountId);
    Customer? GetByIdentityCardOrPhone(string keyword);
    int AddCustomer(Customer customer);
    bool Update(Customer customer);
}
