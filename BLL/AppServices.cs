using BLL.Interfaces;
using BLL.Services;

namespace BLL;

public static class AppServices
{
    private static Lazy<IAuthService> _auth = CreateLazy<IAuthService>(() => new AuthService());
    private static Lazy<ICustomerService> _customer = CreateLazy<ICustomerService>(() => new CustomerService());

    public static IAuthService Auth => _auth.Value;
    public static ICustomerService Customer => _customer.Value;

    // Tạo mới lại cache service khi cần reset trong kiểm thử
    public static void Reset()
    {
        _auth = CreateLazy<IAuthService>(() => new AuthService());
        _customer = CreateLazy<ICustomerService>(() => new CustomerService());
    }

    private static Lazy<T> CreateLazy<T>(Func<T> factory) => new(factory);
}
