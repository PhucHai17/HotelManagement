using Entity.Entities;
using Entity.Enums;

namespace DAL.Interfaces;

public interface IBookingRepository : IGenericRepository<Booking>
{
    Booking? GetBookingWithDetails(int bookingId);
    IEnumerable<Booking> GetBookingsByCustomer(int customerId);
    IEnumerable<Booking> GetBookingsByStatus(BookingStatus status);
    bool UpdateStatus(int bookingId, BookingStatus status);
    int CreateBookingTransaction(Booking booking, List<BookingDetail> details);
}