namespace Entity.Entities;

public class RoomType :BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public decimal BasePrice { get; set; } 
    public int Capacity { get; set; }
    public string Description { get; set; } = string.Empty;
    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}