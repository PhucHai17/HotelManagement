namespace Entity.DTOs;

public class SearchRoomRequest
{
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int ? RoomTypeId { get; set; }
    public int Capacity { get; set; }
    
}