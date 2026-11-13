namespace Entity.DTOs;

using System;

/// DTO chứa tiêu chí tìm kiếm và lọc phòng trống.
public class SearchRoomRequest
{
    public DateTime CheckInDate { get; set; } = DateTime.Today;
    public DateTime CheckOutDate { get; set; } = DateTime.Today.AddDays(1);
    public int GuestCount { get; set; } = 2;
    public int? RoomTypeId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
}
