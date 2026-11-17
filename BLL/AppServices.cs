using BLL.Interfaces;
using BLL.Services;

namespace BLL;

public static class AppServices
{
    private static Lazy<IAuthService> _auth = CreateLazy<IAuthService>(() => new AuthService());
    private static Lazy<ICustomerService> _customer = CreateLazy<ICustomerService>(() => new CustomerService());
    private static Lazy<IRoomService> _room = CreateLazy<IRoomService>(() => new RoomService());
    private static Lazy<IBookingService> _booking = CreateLazy<IBookingService>(() => new BookingService());
    private static Lazy<IHotelService> _hotel = CreateLazy<IHotelService>(() => new BLL.Services.HotelService());
    private static Lazy<IInvoiceService> _invoice = CreateLazy<IInvoiceService>(() => new InvoiceService());
    private static Lazy<IReportService> _report = CreateLazy<IReportService>(() => new ReportService());

    public static IAuthService Auth => _auth.Value;
    public static ICustomerService Customer => _customer.Value;
    public static IRoomService Room => _room.Value;
    public static IBookingService Booking => _booking.Value;
    public static IHotelService Hotel => _hotel.Value;
    public static IInvoiceService Invoice => _invoice.Value;
    public static IReportService Report => _report.Value;

    
    public static void Reset()
    {
        _auth = CreateLazy<IAuthService>(() => new AuthService());
        _customer = CreateLazy<ICustomerService>(() => new CustomerService());
        _room = CreateLazy<IRoomService>(() => new RoomService());
        _booking = CreateLazy<IBookingService>(() => new BookingService());
        _hotel = CreateLazy<IHotelService>(() => new BLL.Services.HotelService());
        _invoice = CreateLazy<IInvoiceService>(() => new InvoiceService());
        _report = CreateLazy<IReportService>(() => new ReportService());
    }

    private static Lazy<T> CreateLazy<T>(Func<T> factory) => new(factory);
}
