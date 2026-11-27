using BLL.Interfaces;

namespace BLL.Services;

public sealed class ShiftScheduleService : IShiftScheduleService
{
    private readonly IShiftScheduleRepository _repo;
    public ShiftScheduleService() : this(new ShiftScheduleRepository()) { }
    public ShiftScheduleService(IShiftScheduleRepository repo) => _repo = repo;

    private static bool CanManage => SessionManager.CurrentUser?.RoleId == AppConstants.MaVTQuanLy || SessionManager.CurrentUser?.RoleId == AppConstants.MaVTAdmin;
    public List<ShiftSchedule> GetAll(DateTime fromDate, DateTime toDate)
    {
        SessionManager.EnsureAnyPermission("Bạn không có quyền xem lịch phân ca!", AppConstants.QuyenQuanLyNhanVien, AppConstants.QuyenXemBaoCao);
        var employeeId = CanManage ? null : SessionManager.CurrentEmployee?.EmployeeId;
        return _repo.GetAll(fromDate.Date, toDate.Date, employeeId);
    }
    public List<WorkShift> GetWorkShifts() => _repo.GetWorkShifts();
    public bool Add(ShiftSchedule schedule, out string message)
    {
        if (!CanManage) { message = "Chỉ quản lý hoặc quản trị viên được phân ca."; return false; }
        if (schedule.EmployeeId <= 0 || !DateTime.TryParse(schedule.WorkDate, out _)) { message = "Vui lòng chọn nhân viên và ngày làm hợp lệ."; return false; }
        try { var id = _repo.Add(schedule); message = id > 0 ? "Đã thêm lịch phân ca." : "Không thể thêm lịch phân ca."; return id > 0; }
        catch (Exception ex) { AppLogger.Error(ex, "Thêm lịch phân ca"); message = ex.Message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase) ? "Nhân viên đã được phân ca này trong ngày." : "Không thể thêm lịch phân ca."; return false; }
    }
    public bool Update(ShiftSchedule schedule, out string message)
    {
        if (!CanManage) { message = "Chỉ quản lý hoặc quản trị viên được sửa lịch ca."; return false; }
        try { var ok = _repo.Update(schedule); message = ok ? "Đã cập nhật lịch phân ca." : "Không tìm thấy lịch phân ca."; return ok; }
        catch (Exception ex) { AppLogger.Error(ex, "Sửa lịch phân ca"); message = "Không thể cập nhật lịch phân ca."; return false; }
    }
    public bool Delete(int scheduleId, out string message)
    {
        if (!CanManage) { message = "Chỉ quản lý hoặc quản trị viên được xóa lịch ca."; return false; }
        var ok = _repo.Delete(scheduleId); message = ok ? "Đã xóa lịch phân ca." : "Không tìm thấy lịch phân ca."; return ok;
    }
}
