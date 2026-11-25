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

    // Kiểm tra người dùng có vai trò Quản lý hoặc Quản trị viên
    private static bool IsManagerOrAdmin()
    {
        var roleId = SessionManager.CurrentUser?.RoleId;
        return roleId == AppConstants.MaVTQuanLy || roleId == AppConstants.MaVTAdmin;
    }
    
    private static bool HasManagePermission(out string message)
    {
        if (IsManagerOrAdmin())
        {
            message = string.Empty;
            return true;
        }

        return SessionManager.CheckPermission(
            out message,
            "Bạn không có quyền quản lý nhân viên!",
            AppConstants.QuyenQuanLyNhanVien);
    }
    
    private static bool IsSystemAdmin()
    {
        var roleId = SessionManager.CurrentUser?.RoleId;
        return roleId == AppConstants.MaVTAdmin || SessionManager.HasPermission(AppConstants.QuyenQuanTriHeThong);
    }
    
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
    
    public static bool IsValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        var trimmed = email.Trim();
        if (trimmed.Contains(' ')) return false;
        var atIndex = trimmed.IndexOf('@');
        var dotIndex = trimmed.LastIndexOf('.');
        return atIndex > 0 && dotIndex > atIndex + 1 && dotIndex < trimmed.Length - 1;
    }
    
    public List<Employee> GetAll()
    {
        if (!IsManagerOrAdmin())
        {
            SessionManager.EnsureAnyPermission("Bạn không có quyền xem danh sách nhân viên!", AppConstants.QuyenQuanLyNhanVien);
        }
        return _employeeRepo.GetAll();
    }

    // Tìm kiếm nhân viên theo từ khóa
    public List<Employee> Search(string keyword)
    {
        if (!IsManagerOrAdmin())
        {
            SessionManager.EnsureAnyPermission("Bạn không có quyền tìm kiếm nhân viên!", AppConstants.QuyenQuanLyNhanVien);
        }
        return _employeeRepo.Search(keyword);
    }
    
    public bool AddEmployee(Employee employee, string username, string password, int roleId, out string message)
    {
        if (!HasManagePermission(out message)) return false;
        if (!ValidateEmployeeInfo(employee, out message)) return false;

        var cleanEmail = (username ?? string.Empty).Trim();
        if (!IsValidEmail(cleanEmail))
        {
            message = "Email không hợp lệ (phải chứa ký tự '@' và dấu '.')!";
            return false;
        }

        if (_accountRepo.Exists(cleanEmail))
        {
            message = "Email này đã được sử dụng cho tài khoản khác trong hệ thống!";
            return false;
        }

        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
        {
            message = "Mật khẩu phải từ 6 ký tự trở lên!";
            return false;
        }

        var assignedRoleId = AppConstants.MaVTLeTan;

        var account = new Account
        {
            RoleId = assignedRoleId,
            Username = cleanEmail,
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
                try
                {
                    _logRepo.LogAction(
                        SessionManager.CurrentUser.AccountId,
                        $"Thêm nhân viên mới: {employee.FullName} ({cleanEmail})",
                        "NhanVien");
                }
                catch (Exception ex)
                {
                    AppLogger.Error(ex, "Ghi nhật ký thêm nhân viên mới");
                }
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
        return UpdateEmployee(employee, roleId, null, out message);
    }

    // Cập nhật thông tin nhân viên kèm mật khẩu tùy chọn
    public bool UpdateEmployee(Employee employee, int roleId, string? newPassword, out string message)
    {
        if (!HasManagePermission(out message)) return false;
        if (!ValidateEmployeeInfo(employee, out message)) return false;

        var existing = _employeeRepo.GetById(employee.EmployeeId);
        if (existing == null)
        {
            message = "Không tìm thấy hồ sơ nhân viên!";
            return false;
        }

        var cleanEmail = employee.Account?.Username?.Trim();
        if (!string.IsNullOrWhiteSpace(cleanEmail))
        {
            if (!IsValidEmail(cleanEmail))
            {
                message = "Email không hợp lệ (phải chứa ký tự '@' và dấu '.')!";
                return false;
            }

            if (!string.Equals(cleanEmail, existing.Account?.Username, StringComparison.OrdinalIgnoreCase))
            {
                if (_accountRepo.Exists(cleanEmail))
                {
                    message = "Email này đã được sử dụng bởi tài khoản khác trong hệ thống!";
                    return false;
                }
            }

            if (employee.Account != null)
            {
                employee.Account.Username = cleanEmail;
            }
        }

        string? newPasswordHash = null;
        if (!string.IsNullOrWhiteSpace(newPassword))
        {
            var cleanPwd = newPassword.Trim();
            if (cleanPwd.Length < 6)
            {
                message = "Mật khẩu mới phải từ 6 ký tự trở lên!";
                return false;
            }

            newPasswordHash = PasswordHelper.HashPassword(cleanPwd);
            employee.Account ??= new Account();
            employee.Account.PasswordHash = newPasswordHash;
        }

        var targetRoleId = (existing.Account?.RoleId == AppConstants.MaVTLeTan)
            ? AppConstants.MaVTLeTan
            : roleId;

        if (!IsSystemAdmin() && (existing.Account?.RoleId == AppConstants.MaVTAdmin || targetRoleId == AppConstants.MaVTAdmin))
        {
            message = "Quản lý không có quyền thao tác hoặc gán vai trò Quản trị viên (Admin)!";
            return false;
        }

        if (existing.AccountId == SessionManager.CurrentUser?.AccountId && targetRoleId != existing.Account?.RoleId)
        {
            message = "Không thể tự thay đổi vai trò của chính mình!";
            return false;
        }

        if (existing.Account?.RoleId == AppConstants.MaVTAdmin
            && targetRoleId != AppConstants.MaVTAdmin
            && !AccountStatusHelper.IsLocked(existing.Account?.Status)
            && _employeeRepo.CountActiveAdmins() <= 1)
        {
            message = "Không thể hạ vai trò của Quản trị viên (Admin) cuối cùng đang hoạt động!";
            return false;
        }

        employee.AccountId = existing.AccountId;
        var ok = (_employeeRepo as EmployeeRepository)?.UpdateEmployee(employee, targetRoleId, newPasswordHash)
                 ?? _employeeRepo.UpdateEmployee(employee, targetRoleId);
        if (ok)
        {
            if (SessionManager.CurrentUser != null)
            {
                try
                {
                    var logMoTa = !string.IsNullOrWhiteSpace(newPassword)
                        ? $"Cập nhật nhân viên và đổi mật khẩu: {employee.FullName} (NV{employee.EmployeeId:D3})"
                        : $"Cập nhật nhân viên: {employee.FullName} (NV{employee.EmployeeId:D3})";

                    _logRepo.LogAction(
                        SessionManager.CurrentUser.AccountId,
                        logMoTa,
                        "NhanVien");
                }
                catch (Exception ex)
                {
                    AppLogger.Error(ex, "Ghi nhật ký cập nhật nhân viên");
                }
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

        if (emp.Account?.RoleId == AppConstants.MaVTAdmin && !IsSystemAdmin())
        {
            message = "Quản lý không có quyền khóa hoặc mở khóa tài khoản Quản trị viên (Admin)!";
            return false;
        }

        var isCurrentlyLocked = AccountStatusHelper.IsLocked(emp.Account?.Status);

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
                try
                {
                    _logRepo.LogAction(
                        SessionManager.CurrentUser.AccountId,
                        $"{actionText} tài khoản nhân viên: {displayUsername} ({emp.FullName})",
                        "TaiKhoan");
                }
                catch (Exception ex)
                {
                    AppLogger.Error(ex, "Ghi nhật ký đổi trạng thái tài khoản nhân viên");
                }
            }
            message = $"Đã {actionText.ToLowerInvariant()} tài khoản {displayUsername}!";
            return true;
        }

        message = "Thao tác cập nhật trạng thái tài khoản thất bại!";
        return false;
    }
}
