using System.Text;
using BLL.Interfaces;

namespace BLL.Services;

// Service báo cáo, thống kê & xuất Excel
public class ReportService : IReportService
{
    private readonly IReportRepository _reportRepo;

    public ReportService() : this(new ReportRepository())
    {
    }

    public ReportService(IReportRepository reportRepo)
    {
        _reportRepo = reportRepo ?? throw new ArgumentNullException(nameof(reportRepo));
    }

    // Lấy số liệu tổng quan Dashboard
    public DashboardStatsDto GetDashboardStats()
    {
        SessionManager.EnsureAnyPermission(
            "Bạn không có quyền xem thống kê tổng quan!",
            AppConstants.QuyenQuanLyPhong,
            AppConstants.QuyenQuanLyDatPhong,
            AppConstants.QuyenXemBaoCao);
        return _reportRepo.GetDashboardStats();
    }

    // Lấy công suất phòng 7 ngày gần nhất
    public Dictionary<string, double> Get7DayOccupancyRate()
    {
        SessionManager.EnsureAnyPermission(
            "Bạn không có quyền xem công suất phòng!",
            AppConstants.QuyenQuanLyPhong,
            AppConstants.QuyenQuanLyDatPhong,
            AppConstants.QuyenXemBaoCao);
        return _reportRepo.Get7DayOccupancyRate();
    }

    // Lấy doanh thu theo tháng (yêu cầu quyền XemBaoCao)
    public List<RevenueReportDto> GetMonthlyRevenue(int year)
    {
        SessionManager.EnsureAnyPermission("Bạn không có quyền xem báo cáo doanh thu!",
            AppConstants.QuyenXemBaoCao,
            AppConstants.QuyenQuanLyPhong,
            AppConstants.QuyenQuanLyDatPhong);
        return _reportRepo.GetMonthlyRevenue(year , LayMaNhanVienLocBaoCao());
    }

    // Lấy việc cần làm hôm nay
    public List<TodoItemDto> GetTodayTodoList()
    {
        SessionManager.EnsureAnyPermission(
            "Bạn không có quyền xem danh sách công việc hôm nay!",
            AppConstants.QuyenQuanLyPhong,
            AppConstants.QuyenQuanLyDatPhong,
            AppConstants.QuyenXemBaoCao);
        return _reportRepo.GetTodayTodoList();
    }

    // Xuất báo cáo doanh thu ra CSV (mở bằng Excel, yêu cầu quyền XemBaoCao)
    public bool ExportRevenueReportToExcel(string filePath, List<RevenueReportDto> reportList, out string message)
    {
        if (!SessionManager.CheckPermission(
                out message,
                "Bạn không có quyền xuất báo cáo doanh thu!",
                AppConstants.QuyenXemBaoCao))
        {
            return false;
        }
        try
        {
            var sb = new StringBuilder();
            sb.AppendLine("Tháng,Doanh Thu (VNĐ),Công Suất (%),Số Lượt Khách");

            double totalRevenue = 0;
            var totalGuests = 0;

            foreach (var item in reportList)
            {
                sb.AppendLine($"{item.Period},{item.Revenue},{item.OccupancyRate}%,{item.GuestCount}");
                totalRevenue += item.Revenue;
                totalGuests += item.GuestCount;
            }

            sb.AppendLine($"Tổng cộng,{totalRevenue},,{totalGuests}");

            File.WriteAllText(filePath, sb.ToString(), new UTF8Encoding(true));

            message = "Xuất file báo cáo thành công!";
            return true;
        }
        catch (Exception ex)
        {
            AppLogger.Error(ex, "Xuất báo cáo doanh thu ra CSV");
            message = $"Lỗi khi xuất file: {ex.Message}";
            return false;
        }
    }

    public List<RevenueReportDto> GetDailyRevenue(DateTime fromDate, DateTime toDate)
    {
        SessionManager.EnsureAnyPermission(
            "Bạn không có quyền xem báo cáo doanh thu!",
            AppConstants.QuyenXemBaoCao,
            AppConstants.QuyenQuanLyPhong,
            AppConstants.QuyenQuanLyDatPhong);
        return _reportRepo.GetDailyRevenue(fromDate, toDate , LayMaNhanVienLocBaoCao());
    }
    private static int? LayMaNhanVienLocBaoCao()
    {
        var roleId = SessionManager.CurrentUser?.RoleId;
        if (roleId == AppConstants.MaVTQuanLy || roleId == AppConstants.MaVTAdmin)
            return null;

        return SessionManager.CurrentEmployee?.EmployeeId ?? -1;
    }
}
