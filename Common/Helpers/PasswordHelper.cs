using System.Security.Cryptography;
using System.Text;

namespace Common.Helpers;

/// Lớp tiện ích băm và kiểm tra mật khẩu bằng SHA-256.
public static class PasswordHelper
{
    /// Băm mật khẩu bằng SHA-256 (chuỗi hex 64 ký tự viết thường).
    public static string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password)) return string.Empty;
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    /// <summary>
    /// Kiểm tra mật khẩu nhập vào với chuỗi hash SHA-256 đã lưu.
    /// </summary>
    public static bool VerifyPassword(string password, string hashedPassword)
    {
        if (string.IsNullOrEmpty(password) || !IsSha256Hash(hashedPassword)) return false;
        return string.Equals(HashPassword(password), hashedPassword, StringComparison.OrdinalIgnoreCase);
    }
    /// Kiểm tra chuỗi có đúng định dạng hash SHA-256 (64 ký tự hex) hay chưa.
    public static bool IsSha256Hash(string? value)
    {
        return !string.IsNullOrEmpty(value) && value.Length == 64 && value.All(Uri.IsHexDigit);
    }
}
