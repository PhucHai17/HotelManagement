using BLL.Interfaces;
using BLL.Services;

namespace BLL;

public static class AppServices
{
    private static Lazy<IAuthService> _auth = CreateLazy<IAuthService>(() => new AuthService());

    public static IAuthService Auth => _auth.Value;

    // Tạo mới lại cache service khi cần reset trong kiểm thử
    public static void Reset()
    {
        _auth = CreateLazy<IAuthService>(() => new AuthService());
    }

    private static Lazy<T> CreateLazy<T>(Func<T> factory) => new(factory);
}
