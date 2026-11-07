namespace Entity.DTOs;

public class BookingDto
{
    public int CustomerId { get; set; }
    public int ? EmployeeId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckoutDate { get; set; }
    public List<int> RoomIds { get; set; } = new List<int>();
}