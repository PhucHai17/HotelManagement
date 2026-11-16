namespace DAL.Repositories;

public class BookingRepository : IBookingRepository
{
    private IBookingRepository _bookingRepositoryImplementation;
    public List<Booking> GetAll()
    {
        return _bookingRepositoryImplementation.GetAll();
    }

    public List<Booking> Search(string keyword, string status, string fromDate)
    {
        return _bookingRepositoryImplementation.Search(keyword, status, fromDate);
    }

    public List<Booking> GetByCustomerId(int customerId)
    {
        return _bookingRepositoryImplementation.GetByCustomerId(customerId);
    }

    public int CreateBooking(Booking booking, int roomId, double priceAtBooking)
    {
        return _bookingRepositoryImplementation.CreateBooking(booking, roomId, priceAtBooking);
    }

    public bool UpdateStatus(int bookingId, string newStatus)
    {
        return _bookingRepositoryImplementation.UpdateStatus(bookingId, newStatus);
    }

    public Booking? GetById(int bookingId)
    {
        return _bookingRepositoryImplementation.GetById(bookingId);
    }

    public List<Booking> GetPendingCheckIns()
    {
        return _bookingRepositoryImplementation.GetPendingCheckIns();
    }

    public Booking? GetActiveBookingByRoomNumber(string roomNumber)
    {
        return _bookingRepositoryImplementation.GetActiveBookingByRoomNumber(roomNumber);
    }

    public int? GetActiveBookingDetailIdByRoomNumber(string roomNumber)
    {
        return _bookingRepositoryImplementation.GetActiveBookingDetailIdByRoomNumber(roomNumber);
    }

    public bool CheckIn(int bookingId, string identityCard, string actualCheckInDate)
    {
        return _bookingRepositoryImplementation.CheckIn(bookingId, identityCard, actualCheckInDate);
    }

    public bool CheckOut(int bookingId, string actualCheckOutDate)
    {
        return _bookingRepositoryImplementation.CheckOut(bookingId, actualCheckOutDate);
    }
}