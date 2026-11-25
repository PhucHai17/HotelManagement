namespace DAL.Interfaces;

// Interface repository nhân viên
public interface IEmployeeRepository
{
    List<Employee> GetAll();
    List<Employee> Search(string keyword);
    Employee? GetByAccountId(int accountId);
    Employee? GetById(int employeeId);
    int CreateEmployeeWithAccount(Employee employee, Account account);
    bool UpdateEmployee(Employee employee, int roleId);
    bool SetAccountLockStatus(int accountId, string accountStatus, string employeeStatus);
    int CountActiveAdmins();
}
