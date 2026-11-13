namespace DAL.Interfaces;

// Interface repository báo cáo
public interface IReportRepository
{
    DashboardStatsDto GetDashboardStats();
    List<TodoItemDto> GetTodayTodoList();
    Dictionary<string, double> Get7DayOccupancyRate();
    List<RevenueReportDto> GetMonthlyRevenue(int year);
}
