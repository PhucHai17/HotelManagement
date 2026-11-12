using Entity.Constants;
namespace Entity.Entities;

public class Room
{ 
    public int RoomId { get; set; }
    public int RoomTypeId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public int Floor { get; set; } = 1;
    public string Status { get; set; } = RoomStatus.Available;
    public string RoomTypeName { get; set; } = string.Empty;
    // giá cơ bản của loại phòng
    public double BasePrice { get; set; }
    //sức chứa của loại phòng
    public int Capacity { get; set; } = 2;
    public override string ToString() => $"{RoomNumber} - {RoomTypeName}";
    
}