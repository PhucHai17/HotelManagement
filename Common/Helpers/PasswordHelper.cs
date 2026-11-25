using System.Security.Cryptography;
using System.Text;

namespace Common.Helpers;

/// Băm và kiểm tra mật khẩu bằng PBKDF2-SHA256 có muối (salt).
/// Vẫn đọc được hash SHA-256 cũ để người dùng hiện có đăng nhập được, sau đó hệ thống tự nâng cấp.
public static class PasswordHelper
{
    private const string Prefix = "pbkdf2";
    private const int Iterations = 120_000;
    private const int SaltSize = 16;
    private const int HashSize = 32;

    /// Băm mật khẩu: pbkdf2$số vòng lặp$salt(base64)$hash(base64).
    public static string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password)) return string.Empty;

        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);
        return $"{Prefix}${Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    /// Kiểm tra mật khẩu nhập vào với chuỗi hash đã lưu (PBKDF2 hoặc SHA-256 cũ).
    public static bool VerifyPassword(string password, string hashedPassword)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword)) return false;

        if (hashedPassword.StartsWith(Prefix + "$", StringComparison.Ordinal))
        {
            var parts = hashedPassword.Split('$');
            if (parts.Length != 4 || !int.TryParse(parts[1], out var iterations) || iterations <= 0) return false;

            try
            {
                var salt = Convert.FromBase64String(parts[2]);
                var expected = Convert.FromBase64String(parts[3]);
                var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expected.Length);
                return CryptographicOperations.FixedTimeEquals(actual, expected);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        if (!IsSha256Hash(hashedPassword)) return false;

        var legacy = Encoding.UTF8.GetBytes(Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(password))).ToLowerInvariant());
        var stored = Encoding.UTF8.GetBytes(hashedPassword.ToLowerInvariant());
        return CryptographicOperations.FixedTimeEquals(legacy, stored);
    }

    /// Hash SHA-256 cũ cần được nâng cấp lên PBKDF2.
    public static bool NeedsRehash(string? hashedPassword) => IsSha256Hash(hashedPassword);

    /// Chuỗi đã là hash hợp lệ (PBKDF2 hoặc SHA-256), không phải mật khẩu thô.
    public static bool IsHashed(string? value)
    {
        return IsSha256Hash(value) || (!string.IsNullOrEmpty(value) && value.StartsWith(Prefix + "$", StringComparison.Ordinal));
    }

    /// Kiểm tra chuỗi có đúng định dạng hash SHA-256 (64 ký tự hex) hay chưa.
    public static bool IsSha256Hash(string? value)
    {
        return !string.IsNullOrEmpty(value) && value.Length == 64 && value.All(Uri.IsHexDigit);
    }
}
