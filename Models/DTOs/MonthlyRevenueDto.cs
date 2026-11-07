namespace Entity.DTOs;

public class MonthlyRevenueDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int TotalBookings { get; set; }
    public decimal RoomRevenue { get; set; }
    public decimal ServiceRevenue { get; set; }
    public decimal TotalRevenue => RoomRevenue + ServiceRevenue;
    
}