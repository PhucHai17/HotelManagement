using BLL.Interfaces;
using BLL.Services;

namespace BLL;

public static class AppServices
{
    private static Lazy<IAuthService> _auth = CreateLazy<IAuthService>(() => new AuthService());
    private static Lazy<ICustomerService> _customer = CreateLazy<ICustomerService>(() => new CustomerService());
    private static Lazy<IRoomService> _room = CreateLazy<IRoomService>(() => new RoomService());

    public static IAuthService Auth => _auth.Value;
    public static ICustomerService Customer => _customer.Value;
    public static IRoomService Room => _room.Value;

    // Tạo mới lại cache service khi cần reset trong kiểm thử
    public static void Reset()
    {
        _auth = CreateLazy<IAuthService>(() => new AuthService());
        _customer = CreateLazy<ICustomerService>(() => new CustomerService());
        _room = CreateLazy<IRoomService>(() => new RoomService());
    }

    private static Lazy<T> CreateLazy<T>(Func<T> factory) => new(factory);
}
