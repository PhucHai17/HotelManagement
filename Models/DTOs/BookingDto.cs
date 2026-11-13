namespace Entity.DTOs;

using System;

/// <summary>
/// DTO mang thông tin đặt phòng để hiển thị bảng và xuất hóa đơn.
/// </summary>
public class BookingDto
{
    public int BookingId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string RoomNumber { get; set; } = string.Empty;
    public string RoomTypeName { get; set; } = string.Empty;
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal DepositAmount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
