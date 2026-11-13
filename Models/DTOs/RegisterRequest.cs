namespace Entity.DTOs;

/// DTO chứa thông tin đăng ký tài khoản khách.
public class RegisterRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string CitizenId { get; set; } = string.Empty;
}
