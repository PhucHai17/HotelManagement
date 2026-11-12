namespace Entity.Entities;

public class Employee :Person
{
    public string EmployeeCode { get; set; } =  string.Empty;
    public string FullName { get; set; }= string.Empty;
    public string Position { get; set; } = string.Empty;
    public string PhonNumber { get; set; }= string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime HireDate { get; set; } 
    public decimal Salary { get; set; }
    public Account ? Account { get; set; }
    public virtual ICollection<Booking> ProcessedBookings { get; set; } = new List<Booking>();
    public virtual ICollection<Invoice> ProcessedInvoices { get; set; } = new List<Invoice>();
}