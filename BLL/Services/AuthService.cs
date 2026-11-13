using BLL.Interfaces;
using Common.Constants;
using Common.Helpers;
using DAL.Interfaces;
using DAL.Repositories;
using Entity.Entities;

namespace BLL.Services;

public class AuthService : IAuthService
{
    private readonly ICustomerRepository _customerRepo;
    private readonly IEmployeeRepository _employeeRepo;
    private readonly ISystemLogRepository _logRepo;
    private readonly IAccountRepository _accountRepo;

    public AuthService()
        : this(new AccountRepository(), new CustomerRepository(), new EmployeeRepository(), new SystemLogRepository())
    {
    }

    public AuthService(
        IAccountRepository accountRepo,
        ICustomerRepository customerRepo,
        IEmployeeRepository employeeRepo,
        ISystemLogRepository logRepo)
    {
        _accountRepo = accountRepo ?? throw new ArgumentNullException(nameof(accountRepo));
        _customerRepo = customerRepo ?? throw new ArgumentNullException(nameof(customerRepo));
        _employeeRepo = employeeRepo ?? throw new ArgumentNullException(nameof(employeeRepo));
        _logRepo = logRepo ?? throw new ArgumentNullException(nameof(logRepo));

        // Đăng ký hàm nạp quyền từ bảng VaiTro_Quyen cho SessionManager
        SessionManager.PermissionLoader ??= _accountRepo.GetPermissionsByRoleId;
    }

    public bool HasPermission(string permissionName) => SessionManager.HasPermission(permissionName);

    public Account? Authenticate(string username, string password)
    {
        if (Login(username, password, out _))
        {
            return SessionManager.CurrentUser;
        }
        return null;
    }

    // Xử lý đăng nhập
    public bool Login(string username, string password, out string message)
    {
        message = string.Empty;

        if (string.IsNullOrWhiteSpace(username))
        {
            message = "Vui lòng nhập tên đăng nhập hoặc email!";
            return false;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            message = "Vui lòng nhập mật khẩu!";
            return false;
        }

        var user = username.Trim();
        var pass = password.Trim();

        // Truy vấn và xác thực tài khoản qua repository
        var account = _accountRepo.Login(user, pass);

        if (account == null)
        {
            message = "Tên đăng nhập hoặc mật khẩu không chính xác!";
            return false;
        }

        // Lưu phiên đăng nhập và nạp quyền từ VaiTro_Quyen
        SessionManager.CurrentUser = account;
        SessionManager.SetPermissions(_accountRepo.GetPermissionsByRoleId(account.RoleId), account.RoleId);

        // Nhân viên (Admin, Lễ tân, Quản lý)
        if (account.RoleId == AppConstants.MaVTAdmin ||
            account.RoleId == AppConstants.MaVTLeTan ||
            account.RoleId == AppConstants.MaVTQuanLy)
        {
            SessionManager.CurrentCustomer = null;
            SessionManager.CurrentEmployee = _employeeRepo.GetByAccountId(account.AccountId);
            if (SessionManager.CurrentEmployee == null)
            {
                SessionManager.Logout();
                message = "Tài khoản chưa được liên kết với hồ sơ nhân viên!";
                return false;
            }
        }
        // Khách hàng
        else if (account.RoleId == AppConstants.MaVTKhachHang)
        {
            SessionManager.CurrentEmployee = null;
            SessionManager.CurrentCustomer = _customerRepo.GetByAccountId(account.AccountId);
            if (SessionManager.CurrentCustomer == null)
            {
                SessionManager.Logout();
                message = "Tài khoản chưa được liên kết với hồ sơ khách hàng!";
                return false;
            }
        }

        try
        {
            _logRepo.LogAction(account.AccountId, "Đăng nhập hệ thống", "TaiKhoan");
        }
        catch (Exception ex)
        {
            AppLogger.Error(ex, "Ghi nhật ký đăng nhập");
        }

        message = "Đăng nhập thành công!";
        return true;
    }

    // Đăng ký tài khoản khách mới
    public bool Register(string fullName, string phone, string email, string identityCard, string password, string confirmPassword,
        out string message)
    {
        message = string.Empty;

        if (string.IsNullOrWhiteSpace(fullName))
        {
            message = "Họ tên không được để trống!";
            return false;
        }

        if (string.IsNullOrWhiteSpace(phone) || phone.Length < 9)
        {
            message = "Số điện thoại không hợp lệ (tối thiểu 9 số)!";
            return false;
        }

        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
        {
            message = "Mật khẩu phải từ 6 ký tự trở lên!";
            return false;
        }

        if (password != confirmPassword)
        {
            message = "Mật khẩu xác nhận không khớp!";
            return false;
        }

        var username = !string.IsNullOrWhiteSpace(email) ? email.Trim() : phone.Trim();
        if (_accountRepo.Exists(username))
        {
            message = "Tài khoản với Email hoặc SĐT này đã tồn tại trong hệ thống!";
            return false;
        }

        var newAccount = new Account
        {
            RoleId = AppConstants.MaVTKhachHang,
            Username = username,
            PasswordHash = PasswordHelper.HashPassword(password),
            Status = AppConstants.AccountStatusActive
        };

        var newAccountId = _accountRepo.CreateAccount(newAccount);
        if (newAccountId <= 0)
        {
            message = "Không thể tạo tài khoản vào lúc này!";
            return false;
        }

        var customer = new Customer
        {
            AccountId = newAccountId,
            MembershipRankId = 1,
            FullName = fullName.Trim(),
            Phone = phone.Trim(),
            Email = email?.Trim() ?? string.Empty,
            IdentityCard = identityCard?.Trim() ?? string.Empty,
            LoyaltyPoints = 0
        };

        _customerRepo.AddCustomer(customer);

        try
        {
            _logRepo.LogAction(newAccountId, "Tạo tài khoản khách hàng mới", "TaiKhoan");
        }
        catch (Exception ex)
        {
            AppLogger.Error(ex, "Ghi nhật ký đăng ký tài khoản khách");
        }

        message = "Đăng ký tài khoản thành công! Bạn có thể đăng nhập ngay.";
        return true;
    }

    // Đổi mật khẩu
    public bool ChangePassword(int accountId, string oldPassword, string newPassword, string confirmNewPassword, out string message)
    {
        message = string.Empty;

        if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword))
        {
            message = "Vui lòng nhập đầy đủ mật khẩu!";
            return false;
        }

        if (newPassword.Length < 6)
        {
            message = "Mật khẩu mới phải từ 6 ký tự trở lên!";
            return false;
        }

        if (newPassword != confirmNewPassword)
        {
            message = "Mật khẩu mới xác nhận không khớp!";
            return false;
        }

        var currentAccount = SessionManager.CurrentUser;
        if (currentAccount == null || !PasswordHelper.VerifyPassword(oldPassword, currentAccount.PasswordHash))
        {
            message = "Mật khẩu hiện tại không chính xác!";
            return false;
        }

        // Băm mật khẩu mới trước khi lưu xuống DB
        var newPasswordHash = PasswordHelper.HashPassword(newPassword);
        var result = _accountRepo.ChangePassword(accountId, newPasswordHash);
        if (result)
        {
            currentAccount.PasswordHash = newPasswordHash;
            message = "Đổi mật khẩu thành công!";
            return true;
        }

        message = "Không thể đổi mật khẩu vào lúc này!";
        return false;
    }
}