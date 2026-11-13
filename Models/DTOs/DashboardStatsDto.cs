namespace Entity.DTOs;

// DTO thống kê tổng quan Dashboard
public class DashboardStatsDto
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

// Việc cần làm hôm nay
public class TodoItemDto
{
    public string RoomNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string TaskDescription { get; set; } = string.Empty; // "Check-in", "Check-out", "Dọn phòng"
}

// DTO báo cáo doanh thu
public class RevenueReportDto
{
    public string Period { get; set; } = string.Empty;
    public double OccupancyRate { get; set; }
    public int GuestCount { get; set; }
}
