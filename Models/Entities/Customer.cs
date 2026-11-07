namespace Entity.Entities;

public class Customer : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string IdentityCard { get; set; } = string.Empty;
    public string PhoneNumber { get; set; }= string.Empty;
    public string Email { get; set; }= string.Empty;
    public string Address { get; set; }= string.Empty;
    public Account ? Account { get; set; }
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

   
}

