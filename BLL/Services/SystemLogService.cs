using BLL.Interfaces;
using Common.Constants;
using Common.Helpers;
using DAL.Interfaces;
using DAL.Repositories;
using Entity.Entities;

namespace BLL.Services;

// Service quản lý và truy vấn nhật ký hệ thống
public class SystemLogService : ISystemLogService
{
    private readonly ISystemLogRepository _logRepo;

    public SystemLogService() : this(new SystemLogRepository())
    {
    }

    public SystemLogService(ISystemLogRepository logRepo)
    {
        _logRepo = logRepo ?? throw new ArgumentNullException(nameof(logRepo));
    }
    
    private static bool HasViewPermission()
    {
        if (SessionManager.CurrentUser == null)
            return true;

        var roleId = SessionManager.CurrentUser.RoleId;
        if (roleId == AppConstants.MaVTQuanLy || roleId == AppConstants.MaVTAdmin)
            return true;

        return SessionManager.HasAnyPermission(AppConstants.QuyenXemNhatKy, AppConstants.QuyenQuanTriHeThong);
    }
    
    public List<SystemLog> GetLogs(string? keyword = null, string? tableFilter = null, DateTime? fromDate = null, DateTime? toDate = null)
    {
        if (!HasViewPermission())
            return new List<SystemLog>();

        return _logRepo.GetLogs(keyword, tableFilter, fromDate, toDate);
    }

    // Ghi nhật ký hành động hệ thống
    public void LogAction(int accountId, string action, string affectedTable)
    {
        _logRepo.LogAction(accountId, action, affectedTable);
    }
}
