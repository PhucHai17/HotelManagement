using Entity.Enums;

namespace Entity.Entities;

public class Room : BaseEntity
{
    public string RoomNumber { get; set; } = string.Empty;
    public int Floor { get; set; }
    public RoomStatus Status { get; set; } = RoomStatus.Available;
    //FG keys
    public int RoomId { get; set; }
    public virtual RoomType RoomType { get; set; } = null!;
    public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();
    
}