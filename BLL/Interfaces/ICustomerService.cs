namespace BLL.Interfaces;

public interface ICustomerService
{
    List<Customer> GetAll();
    List<Customer> Search(string keyword);
    Customer? GetByAccountId(int accountId);
    Customer? GetByIdentityCardOrPhone(string keyword);
    bool SaveCustomer(Customer customer, out string message);
}
