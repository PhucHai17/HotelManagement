namespace Entity.DTOs;

public class DashboardStatistics
{
    public int AvailableRooms { get; set; }
    public int OccupiedRooms { get; set; }
    public int ReservedRooms { get; set; }
    public int CleaningRooms { get; set; }
    public int TotalRooms { get; set; }
    public int TodayCheckIns { get; set; }
    public int TodayCheckOuts { get; set; }
    public double TodayRevenue { get; set; }
    public int TodayNewBookings { get; set; }
    public double OccupancyRate => TotalRooms > 0 ? OccupiedRooms * 100.0 / TotalRooms : 0;
}