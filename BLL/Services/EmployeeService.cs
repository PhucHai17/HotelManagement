using System.Text.RegularExpressions;
using BLL.Interfaces;
using Entity.Enums;

namespace BLL.Services;

// Service quản lý hồ sơ, tài khoản và phân quyền nhân viên
public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepo;
    private readonly IAccountRepository _accountRepo;
    private readonly ISystemLogRepository _logRepo;

    public EmployeeService()
        : this(new EmployeeRepository(), new AccountRepository(), new SystemLogRepository())
    {
    }

    public EmployeeService(
        IEmployeeRepository employeeRepo,
        IAccountRepository accountRepo,
        ISystemLogRepository logRepo)
    {
        _employeeRepo = employeeRepo ?? throw new ArgumentNullException(nameof(employeeRepo));
        _accountRepo = accountRepo ?? throw new ArgumentNullException(nameof(accountRepo));
        _logRepo = logRepo ?? throw new ArgumentNullException(nameof(logRepo));
    }

    // Kiểm tra quyền quản lý nhân viên
    private static bool HasManagePermission(out string message)
    {
        return SessionManager.CheckPermission(
            out message,
            "Bạn không có quyền quản lý nhân viên!",
            AppConstants.QuyenQuanLyNhanVien);
    }

    // Kiểm tra người dùng hiện tại có quyền Quản trị hệ thống (Admin) hay không
    private static bool IsSystemAdmin()
    {
        return SessionManager.HasPermission(AppConstants.QuyenQuanTriHeThong);
    }

    // Kiểm tra dữ liệu chung của nhân viên (Họ tên, SĐT, CCCD 12 số)
    private static bool ValidateEmployeeInfo(Employee employee, out string message)
    {
        if (string.IsNullOrWhiteSpace(employee.FullName))
        {
            message = "Họ tên nhân viên không được để trống!";
            return false;
        }

        var phone = (employee.PhoneNumber ?? string.Empty).Trim();
        if (!Regex.IsMatch(phone, @"^0\d{9,10}$"))
        {
            message = "Số điện thoại không hợp lệ (phải gồm 10-11 chữ số, bắt đầu bằng số 0)!";
            return false;
        }

        var cccd = (employee.NationalId ?? string.Empty).Trim();
        if (!Regex.IsMatch(cccd, @"^\d{12}$"))
        {
            message = "Số CCCD không hợp lệ (bắt buộc đúng 12 chữ số)!";
            return false;
        }

        employee.FullName = employee.FullName.Trim();
        employee.PhoneNumber = phone;
        employee.NationalId = cccd;
        employee.Position = string.IsNullOrWhiteSpace(employee.Position) ? "Lễ tân" : employee.Position.Trim();

        message = string.Empty;
        return true;
    }

    // Lấy toàn bộ danh sách nhân viên
    public List<Employee> GetAll()
    {
        SessionManager.EnsureAnyPermission("Bạn không có quyền xem danh sách nhân viên!", AppConstants.QuyenQuanLyNhanVien);
        return _employeeRepo.GetAll();
    }

    // Tìm kiếm nhân viên theo từ khóa
    public List<Employee> Search(string keyword)
    {
        SessionManager.EnsureAnyPermission("Bạn không có quyền tìm kiếm nhân viên!", AppConstants.QuyenQuanLyNhanVien);
        return _employeeRepo.Search(keyword);
    }

    // Thêm nhân viên mới kèm tài khoản đã băm mật khẩu SHA-256
    public bool AddEmployee(Employee employee, string username, string password, int roleId, out string message)
    {
        if (!HasManagePermission(out message)) return false;
        if (!ValidateEmployeeInfo(employee, out message)) return false;

        var cleanUser = (username ?? string.Empty).Trim();
        if (cleanUser.Length < 3 || cleanUser.Contains(' '))
        {
            message = "Tên đăng nhập phải từ 3 ký tự trở lên và không chứa khoảng trắng!";
            return false;
        }

        if (_accountRepo.Exists(cleanUser))
        {
            message = "Tên đăng nhập đã tồn tại trong hệ thống!";
            return false;
        }

        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
        {
            message = "Mật khẩu phải từ 6 ký tự trở lên!";
            return false;
        }

        if (roleId != AppConstants.MaVTLeTan && roleId != AppConstants.MaVTQuanLy && roleId != AppConstants.MaVTAdmin)
        {
            message = "Vai trò nhân viên không hợp lệ!";
            return false;
        }

        // Chỉ Admin (có quyền QuanTriHeThong) mới được tạo tài khoản vai trò Admin
        if (roleId == AppConstants.MaVTAdmin && !IsSystemAdmin())
        {
            message = "Chỉ Quản trị viên (Admin) mới có quyền tạo tài khoản vai trò Quản trị viên!";
            return false;
        }

        var account = new Account
        {
            RoleId = roleId,
            Username = cleanUser,
            PasswordHash = PasswordHelper.HashPassword(password),
            Status = AppConstants.AccountStatusActive
        };

        employee.ManagerId = SessionManager.CurrentEmployee?.EmployeeId;
        employee.HireDate = string.IsNullOrWhiteSpace(employee.HireDate)
            ? DateTime.Today.ToString("yyyy-MM-dd")
            : employee.HireDate;
        employee.Status = AppConstants.EmployeeStatusWorking;

        var newId = _employeeRepo.CreateEmployeeWithAccount(employee, account);
        if (newId > 0)
        {
            if (SessionManager.CurrentUser != null)
            {
                _logRepo.LogAction(
                    SessionManager.CurrentUser.AccountId,
                    $"Thêm nhân viên mới: {employee.FullName} ({cleanUser})",
                    "NhanVien");
            }
            message = "Thêm nhân viên thành công!";
            return true;
        }

        message = "Không thể thêm nhân viên vào cơ sở dữ liệu!";
        return false;
    }

    // Cập nhật thông tin nhân viên
    public bool UpdateEmployee(Employee employee, int roleId, out string message)
    {
        if (!HasManagePermission(out message)) return false;
        if (!ValidateEmployeeInfo(employee, out message)) return false;

        var existing = _employeeRepo.GetById(employee.EmployeeId);
        if (existing == null)
        {
            message = "Không tìm thấy hồ sơ nhân viên!";
            return false;
        }

        if (roleId != AppConstants.MaVTLeTan && roleId != AppConstants.MaVTQuanLy && roleId != AppConstants.MaVTAdmin)
        {
            message = "Vai trò nhân viên không hợp lệ!";
            return false;
        }

        // Quản lý không được chỉnh sửa hoặc gán vai trò Admin
        if (!IsSystemAdmin() && (existing.Account?.RoleId == AppConstants.MaVTAdmin || roleId == AppConstants.MaVTAdmin))
        {
            message = "Quản lý không có quyền thao tác hoặc gán vai trò Quản trị viên (Admin)!";
            return false;
        }

        // Không cho phép tự thay đổi vai trò của chính mình
        if (existing.AccountId == SessionManager.CurrentUser?.AccountId && roleId != existing.Account?.RoleId)
        {
            message = "Không thể tự thay đổi vai trò của chính mình!";
            return false;
        }

        // Không cho phép hạ vai trò của Admin cuối cùng đang hoạt động
        if (existing.Account?.RoleId == AppConstants.MaVTAdmin
            && roleId != AppConstants.MaVTAdmin
            && !AccountStatusHelper.IsLocked(existing.Account?.Status)
            && _employeeRepo.CountActiveAdmins() <= 1)
        {
            message = "Không thể hạ vai trò của Quản trị viên (Admin) cuối cùng đang hoạt động!";
            return false;
        }

        employee.AccountId = existing.AccountId;
        var ok = _employeeRepo.UpdateEmployee(employee, roleId);
        if (ok)
        {
            if (SessionManager.CurrentUser != null)
            {
                _logRepo.LogAction(
                    SessionManager.CurrentUser.AccountId,
                    $"Cập nhật nhân viên: {employee.FullName} (NV{employee.EmployeeId:D3})",
                    "NhanVien");
            }
            message = "Cập nhật thông tin nhân viên thành công!";
            return true;
        }

        message = "Cập nhật thông tin nhân viên thất bại!";
        return false;
    }

    // Khóa hoặc mở khóa tài khoản nhân viên
    public bool ToggleAccountLock(int employeeId, out string message)
    {
        if (!HasManagePermission(out message)) return false;

        var emp = _employeeRepo.GetById(employeeId);
        if (emp == null)
        {
            message = "Không tìm thấy nhân viên!";
            return false;
        }

        if (emp.AccountId == SessionManager.CurrentUser?.AccountId)
        {
            message = "Không thể tự khóa tài khoản đang đăng nhập!";
            return false;
        }

        // Quản lý không được khóa hoặc mở khóa tài khoản Admin
        if (emp.Account?.RoleId == AppConstants.MaVTAdmin && !IsSystemAdmin())
        {
            message = "Quản lý không có quyền khóa hoặc mở khóa tài khoản Quản trị viên (Admin)!";
            return false;
        }

        var isCurrentlyLocked = AccountStatusHelper.IsLocked(emp.Account?.Status);

        // Không cho phép khóa Admin cuối cùng đang hoạt động
        if (!isCurrentlyLocked
            && emp.Account?.RoleId == AppConstants.MaVTAdmin
            && _employeeRepo.CountActiveAdmins() <= 1)
        {
            message = "Không thể khóa Quản trị viên (Admin) cuối cùng đang hoạt động!";
            return false;
        }

        var targetState = isCurrentlyLocked ? AccountStatus.Active : AccountStatus.Locked;
        var newAccountStatus = AccountStatusHelper.ToDatabaseValue(targetState);
        var newEmployeeStatus = AccountStatusHelper.ToEmployeeStatus(targetState);
        var actionText = isCurrentlyLocked ? "Mở khóa" : "Khóa";
        var displayUsername = emp.Account?.Username ?? $"NV{emp.EmployeeId:D3}";
        var ok = _employeeRepo.SetAccountLockStatus(emp.AccountId, newAccountStatus, newEmployeeStatus);
        if (ok)
        {
            if (SessionManager.CurrentUser != null)
            {
                _logRepo.LogAction(
                    SessionManager.CurrentUser.AccountId,
                    $"{actionText} tài khoản nhân viên: {displayUsername} ({emp.FullName})",
                    "TaiKhoan");
            }
            message = $"Đã {actionText.ToLowerInvariant()} tài khoản {displayUsername}!";
            return true;
        }

        message = "Thao tác cập nhật trạng thái tài khoản thất bại!";
        return false;
    }
}
