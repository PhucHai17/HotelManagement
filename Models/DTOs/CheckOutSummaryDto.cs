namespace Entity.DTOs;

public class CheckOutSummaryDto
{
    public int BookingId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime CheckInDate { get; set; }
    public DateTime CheckoutDate { get; set; }
    public int TotalNights { get; set; }
    public decimal RoomTotal { get; set; }
    public decimal ServiceTotal { get; set; }
    public decimal Discount { get; set; }
    public decimal GrandTotal => (RoomTotal + ServiceTotal) - Discount;
    
}