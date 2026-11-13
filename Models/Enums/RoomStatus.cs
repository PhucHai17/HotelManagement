namespace Entity.Enums;

/// Trạng thái hoạt động của phòng.
public enum RoomStatus
{
    Available = 0,   // Trống
    Occupied = 1,    // Đang có khách ở
    Cleaning = 2,    // Đang dọn dẹp
    Maintenance = 3, // Đang bảo trì
    Reserved = 4     // Đã đặt trước
}
