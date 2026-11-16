namespace Entity.Entities;

public class Employee : Person
{
    public int EmployeeId { get; set; }
    public int AccountId { get; set; }
    public int ? ManagerId { get; set; }
    public string Position { get; set; } = "Lễ tân";
    public string HireDate { get; set; } = string.Empty;
    public string Status { get; set; } = "Đang làm việc";
    // Tài khoản liên kết
    public Account? Account { get; set; }
}
   