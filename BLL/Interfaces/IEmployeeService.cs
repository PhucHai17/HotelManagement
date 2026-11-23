namespace BLL.Interfaces;

// Interface dịch vụ quản lý nhân viên
public interface IEmployeeService
{
    List<Employee> GetAll();
    List<Employee> Search(string keyword);
    bool AddEmployee(Employee employee, string username, string password, int roleId, out string message);
    bool UpdateEmployee(Employee employee, int roleId, out string message);
    bool UpdateEmployee(Employee employee, int roleId, string? newPassword, out string message);
    bool ToggleAccountLock(int employeeId, out string message);
}
