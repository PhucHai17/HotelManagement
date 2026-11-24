namespace BLL.Interfaces;

// Interface dịch vụ báo cáo
public interface IReportService
{
    DashboardStatsDto GetDashboardStats();
    Dictionary<string, double> Get7DayOccupancyRate();
    List<RevenueReportDto> GetMonthlyRevenue(int year);
    List<TodoItemDto> GetTodayTodoList();
    bool ExportRevenueReportToExcel(string filePath, List<RevenueReportDto> reportList, out string message);
    public List<RevenueReportDto> GetDailyRevenue(DateTime fromDate, DateTime toDate);

}
