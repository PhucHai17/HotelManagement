namespace Entity.Entities;

public class BookingDetail
{
    // mã chi tiết
    public int BookingDetailId { get; set; }
    // mã đơn đặt phòng
    public int BookingId { get; set; }
    // mã phòng
    public int RoomId { get; set; }
    // giá phòng tại thời điểm đặt
    public double PriceAtBooking { get; set; }
    // ngày nhận phòng thực tế
    public string ActualCheckInDate { get; set; } = string.Empty;
    // ngày trả phòng thực tế
    public string ActualCheckOutDate { get; set; } = string.Empty;
    // số phòng 
    public string RoomNumber { get; set; } = string.Empty;
    // tên loại phòng
    public string RoomTypeName { get; set; } = string.Empty;
}