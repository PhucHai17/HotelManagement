namespace BLL.Interfaces;

public interface ICustomerService
{
    List<Customer> GetAll();
    List<Customer> Search(string keyword);
    Customer? GetByIdentityCardOrPhone(string keyword);
    bool SaveCustomer(Customer customer, out string message);
}
