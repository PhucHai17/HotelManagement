namespace Entity.Entities;

public class BookingDetail : BaseEntity
{
    public decimal NightlyPrice { get; set; }
    public string Notes { get; set; } = string.Empty;
    public int BookingId { get; set; }
    public virtual Booking Booking { get; set; } = null!;
    public int RoomId { get; set; }
    public virtual Room Room { get; set; } = null!;
    public virtual ICollection<Service> ServiceUsages { get; set; } = new List<Service>();
}