namespace Entity.DTOs;

public class RevenueReportItem
{
    public string Period { get; set; } = string.Empty;
    public double Revenue { get; set; }
    public double OccupancyRate { get; set; }
    public int GuestCount { get; set; }
}