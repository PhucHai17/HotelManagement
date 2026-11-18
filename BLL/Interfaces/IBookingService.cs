namespace BLL.Interfaces;

public interface IBookingService
{
    List<Booking> GetAll();
    List<Booking> Search(string keyword, string status, string fromDate);
    Booking? GetById(int bookingId);
    List<Booking> GetByCustomerId(int customerId);
    List<Booking> GetPendingCheckIns();
    Booking? GetActiveBookingByRoomNumber(string roomNumber);
    bool CreateBooking(Booking booking, int roomId, double priceAtBooking, out string message);
    bool ConfirmBooking(int bookingId, out string message);
    bool CancelBooking(int bookingId, out string message);
}
