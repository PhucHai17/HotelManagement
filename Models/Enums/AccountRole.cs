namespace Entity.Enums;

/// Vai trò tài khoản người dùng.
public enum AccountRole
{
    Admin = 1,        // Quản trị viên toàn quyền
    Receptionist = 2, // Lễ tân khách sạn
    Customer = 3,     // Khách đặt phòng trực tuyến
    Manager = 4       // Quản lý khách sạn
}
