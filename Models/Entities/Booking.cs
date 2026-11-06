using Entity.Enums;

namespace Entity.Entities;

public class Booking : BaseEntity
{
    public DateTime BookingDate { get; set; } = DateTime.Now;
    public DateTime ExpectedCheckIn { get; set; } 
    public DateTime ExpectedCheckout { get; set; }
    public DateTime ? ActualCheckIn { get; set; } 
    public DateTime ? ActualCheckOut { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public decimal TotalAmount { get; set; }
    public int CustomerId { get; set; }
    public virtual Customer Customer { get; set; } = null!;
    public virtual Employee ? Employee { get; set; }
    public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}