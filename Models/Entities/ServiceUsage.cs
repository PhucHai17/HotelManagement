namespace Entity.Entities;

public class ServiceUsage : BaseEntity
{
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount => Quantity * UnitPrice;
    public DateTime UsageTime { get; set; } = DateTime.Now;
    public int BookingDetailId { get; set; }
    public virtual BookingDetail BookingDetail { get; set; } = null!;
    public int ServiceId { get; set; }
    public Service Service { get; set; } = null!;
    
}