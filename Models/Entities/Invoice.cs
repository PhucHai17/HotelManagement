using Entity.Enums;

namespace Entity.Entities;

public class Invoice
{
    public string InvoiceCode { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; } = DateTime.Now;
    public decimal RoomCharge { get; set; }
    public decimal ServiceCharge { get; set; }
    public decimal Discount { get; set; }
    public decimal FinalAmount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }

    // Foreign keys
    public int BookingId { get; set; }
    public virtual Booking Booking { get; set; } = null!;

    public int EmployeeId { get; set; }
    public virtual Employee Employee { get; set; } = null!;
}

