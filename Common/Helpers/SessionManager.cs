using Common.Constants;
using Entity.Entities;

namespace Common.Helpers;
/// Quản lý phiên đăng nhập hiện tại và bộ nhớ đệm quyền của vai trò.
public static class SessionManager
{
    private static readonly HashSet<string> _permissions = new(StringComparer.OrdinalIgnoreCase);
    private static int? _loadedRoleId;

    // Hàm tải quyền từ tầng DAL/BLL (đăng ký khi khởi tạo để tránh tham chiếu vòng)
    public static Func<int, IEnumerable<string>>? PermissionLoader { get; set; }

    // Tài khoản đang đăng nhập
    public static Account? CurrentUser { get; set; }

    // Nhân viên đang đăng nhập
    public static Employee? CurrentEmployee { get; set; }

    // Khách hàng đang đăng nhập
    public static Customer? CurrentCustomer { get; set; }

    // Kiểm tra đã đăng nhập hay chưa
    public static bool IsLoggedIn => CurrentUser != null;

    // Danh sách quyền đang giữ trong phiên
    public static IReadOnlyCollection<string> Permissions => _permissions;

    // Nạp danh sách quyền từ bảng VaiTro_Quyen vào bộ nhớ đệm phiên
    public static void SetPermissions(IEnumerable<string>? permissions, int? roleId = null)
    {
        _permissions.Clear();
        if (permissions != null)
        {
            foreach (var p in permissions)
            {
                if (!string.IsNullOrWhiteSpace(p))
                    _permissions.Add(p.Trim());
            }
        }
        _loadedRoleId = roleId ?? CurrentUser?.RoleId;
    }

    // Kiểm tra người dùng hiện tại có quyền theo tên quyền trong VaiTro_Quyen hay không
    public static bool HasPermission(string permissionName)
    {
        if (CurrentUser == null || string.IsNullOrWhiteSpace(permissionName))
            return false;

        if (_loadedRoleId != CurrentUser.RoleId && PermissionLoader != null)
        {
            SetPermissions(PermissionLoader(CurrentUser.RoleId), CurrentUser.RoleId);
        }

        return _permissions.Contains(permissionName.Trim());
    }

    // Kiểm tra người dùng hiện tại có ít nhất một quyền trong danh sách hay không
    public static bool HasAnyPermission(params string[] permissions)
    {
        if (CurrentUser == null || permissions == null || permissions.Length == 0)
            return false;

        foreach (var p in permissions)
        {
            if (HasPermission(p)) return true;
        }
        return false;
    }

    // Kiểm tra quyền cho hàm trả về bool + thông báo
    public static bool CheckPermission(out string message, string errorMessage, params string[] permissions)
    {
        if (HasAnyPermission(permissions))
        {
            message = string.Empty;
            return true;
        }

        message = errorMessage;
        return false;
    }

    // Yêu cầu tối thiểu một quyền cho các hàm truy vấn (ném UnauthorizedAccessException nếu thiếu quyền)
    public static void EnsureAnyPermission(string errorMessage, params string[] permissions)
    {
        if (!HasAnyPermission(permissions))
            throw new UnauthorizedAccessException(errorMessage);
    }

    // Cho phép nhân viên có quyền nghiệp vụ hoặc chính khách hàng sở hữu dữ liệu
    public static bool HasStaffOrOwnerAccess(int ownerCustomerId, params string[] staffPermissions)
    {
        if (HasAnyPermission(staffPermissions))
            return true;

        return HasPermission(AppConstants.QuyenDatPhongOnline)
               && CurrentCustomer != null
               && CurrentCustomer.CustomerId > 0
               && CurrentCustomer.CustomerId == ownerCustomerId;
    }

    // Lưu tài khoản đang đăng nhập
    public static void Login(Account account)
    {
        CurrentUser = account;
        if (PermissionLoader != null && account != null)
        {
            SetPermissions(PermissionLoader(account.RoleId), account.RoleId);
        }
    }

    // Lưu phiên đăng nhập kèm hồ sơ nhân viên/khách
    public static void Login(Account account, Employee? employee, Customer? customer = null)
    {
        CurrentEmployee = employee;
        CurrentCustomer = customer;
        Login(account);
    }

    // Xóa phiên, xóa cache quyền và đăng xuất
    public static void Logout()
    {
        CurrentUser = null;
        CurrentEmployee = null;
        CurrentCustomer = null;
        _permissions.Clear();
        _loadedRoleId = null;
    }

    // Tên hiển thị người dùng
    public static string DisplayName
    {
        get
        {
            if (CurrentEmployee != null && !string.IsNullOrWhiteSpace(CurrentEmployee.FullName))
                return CurrentEmployee.FullName;
            if (CurrentCustomer != null && !string.IsNullOrWhiteSpace(CurrentCustomer.FullName))
                return CurrentCustomer.FullName;
            if (CurrentUser != null && !string.IsNullOrWhiteSpace(CurrentUser.Username))
                return CurrentUser.Username;
            return "Khách";
        }
    }
}
