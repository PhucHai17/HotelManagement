namespace Entity.Constants;

public static class RoomStatus
{
    public const string Available = "Trống";
    public const string Occupied = "Có khách";
    public const string Reserved = "Đã đặt";
    public const string Cleaning = "Đang dọn";
}

// Trạng thái đơn đặt phòng
public static class BookingStatus
{
    public const string Pending = "Chờ duyệt";
    public const string Confirmed = "Đã xác nhận";
    public const string CheckedIn = "Đang ở";
    public const string Completed = "Đã hoàn tất";
    public const string Cancelled = "Đã hủy";
}

// Trạng thái hóa đơn
public static class InvoiceStatus
{
    public const string Unpaid = "Chưa thanh toán";
    public const string Paid = "Đã thanh toán";
}

// Kênh đặt phòng
public static class BookingChannel
{
    public const string Direct = "Trực tiếp";
    public const string Online = "Trực tuyến";
}

// Loại việc cần làm trong ngày
public static class TaskType
{
    public const string CheckIn = "Check-in";
    public const string CheckOut = "Check-out";
    public const string Cleaning = "Dọn phòng";
}

// Trạng thái tài khoản mặc định
public static class AccountStatus
{
    public const string Active = "Hoạt động";
}

// Mã vai trò
public static class RoleIds
{
    public const int Admin = 1;
    public const int Receptionist = 2;
    public const int Customer = 3;
}

// Tên vai trò hiển thị
public static class RoleNames
{
    public const string Admin = "Quản lý";
    public const string Receptionist = "Lễ tân";
    public const string Customer = "Khách hàng";
}
