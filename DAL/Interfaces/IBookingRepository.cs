namespace DAL.Interfaces;

// Interface repository đặt phòng
public interface IBookingRepository
{
    List<Booking> GetAll();
    List<Booking> Search(string keyword, string status, string fromDate);
    List<Booking> GetByCustomerId(int customerId);
    int CreateBooking(Booking booking, int roomId, double priceAtBooking);
    bool UpdateStatus(int bookingId, string newStatus);
    Booking? GetById(int bookingId);
    List<Booking> GetPendingCheckIns();
    Booking? GetActiveBookingByRoomNumber(string roomNumber);
    int? GetActiveBookingDetailIdByRoomNumber(string roomNumber);
    bool CheckIn(int bookingId, string identityCard, string actualCheckInDate);
    bool CheckOut(int bookingId, string actualCheckOutDate);
}
