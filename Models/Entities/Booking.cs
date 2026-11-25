using Entity.Enums;

namespace Entity.Entities;

public class Booking 
{
    // mã đơn đặt phòng
    public int BookingId { get; set; }
    // Mm khách hàng đặt
    public int CustomerId { get; set; }
    // mã nhân viên tạo đơn
    public int? EmployeeId { get; set; }
    // mã khuyến mãi áp dụng
    public int? PromotionId { get; set; }
    // ngày nhận phòng dự kiến
    public string CheckInDate { get; set; } = string.Empty;
    // ngày trả phòng dự kiến
    public string CheckOutDate { get; set; } = string.Empty;
    // Số khách
    public int GuestCount { get; set; } = 1;
    // tien cọc
    public double Deposit { get; set; } = 0.0;
    // kênh đặt phòng
    public string Channel { get; set; } = "Trực tiếp";
    // trạng thái
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public string CustomerName { get; set; } = string.Empty;
    // sđt
    public string CustomerPhone { get; set; } = string.Empty;
    //CCCD 
    public string CustomerNationalId { get; set; } = string.Empty;
    public string RoomNumber { get; set; } = string.Empty;
    // tên loại phòng
    public string RoomTypeName { get; set; } = string.Empty;
    // tổng tiền
    public double TotalAmount { get; set; }
    public string DisplayCode => $"DP{BookingId:D3}";
}