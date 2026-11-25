namespace Entity.Enums;
/// Trạng thái đơn đặt phòng.
public enum BookingStatus
{
    Pending = 0,    // Chờ check-in / Đã cọc
    Confirmed = 1,  // Đã xác nhận
    CheckedIn = 2,  // Đang ở (đã check-in)
    CheckedOut = 3, // Đã trả phòng (hoàn tất)
    Cancelled = 4   // Đã hủy đặt phòng
}
