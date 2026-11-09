namespace Common.Helpers;

public class PasswordHelper
{
    public static  string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Mật khâ không được để trống! ", nameof(password));
        }

        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
    }
    
    public static bool VerifyPassword(string password, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(passwordHash))
        {
            return false;
        }

        try
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
        catch (ArgumentException)
        {
            return false;
        }
    }
}