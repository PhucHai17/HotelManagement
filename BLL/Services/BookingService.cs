using BLL.Interfaces;

namespace BLL.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepo;
    private readonly IRoomRepository _roomRepo;

    public BookingService() : this(new BookingRepository(), new RoomRepository())
    {
    }

    public BookingService(IBookingRepository bookingRepo, IRoomRepository roomRepo)
    {
        _bookingRepo = bookingRepo ?? throw new ArgumentNullException(nameof(bookingRepo));
        _roomRepo = roomRepo ?? throw new ArgumentNullException(nameof(roomRepo));
    }

    public List<Booking> GetAll()
    {
        SessionManager.EnsureAnyPermission(
            "Bạn không có quyền xem toàn bộ danh sách đặt phòng!",
            AppConstants.QuyenQuanLyDatPhong);
        return _bookingRepo.GetAll();
    }

    public List<Booking> Search(string keyword, string status, string fromDate)
    {
        SessionManager.EnsureAnyPermission(
            "Bạn không có quyền tìm kiếm đặt phòng!",
            AppConstants.QuyenQuanLyDatPhong);
        return _bookingRepo.Search(keyword, status, fromDate);
    }

    public Booking? GetById(int bookingId)
    {
        SessionManager.EnsureAnyPermission(
            "Bạn không có quyền xem chi tiết đặt phòng!",
            AppConstants.QuyenQuanLyDatPhong,
            AppConstants.QuyenDatPhongOnline);

        var booking = _bookingRepo.GetById(bookingId);
        if (booking != null && !SessionManager.HasStaffOrOwnerAccess(booking.CustomerId, AppConstants.QuyenQuanLyDatPhong))
            throw new UnauthorizedAccessException("Bạn chỉ có quyền xem đơn đặt phòng của chính mình!");

        return booking;
    }

    public List<Booking> GetByCustomerId(int customerId)
    {
        var isOwner = SessionManager.CurrentCustomer != null &&
                      SessionManager.CurrentCustomer.CustomerId == customerId;
        var hasStaffAccess = SessionManager.HasPermission(AppConstants.QuyenQuanLyDatPhong);

        if (!isOwner && !hasStaffAccess)
            throw new UnauthorizedAccessException("Bạn chỉ có quyền xem đơn đặt phòng của chính mình!");

        return _bookingRepo.GetByCustomerId(customerId);
    }

    public List<Booking> GetPendingCheckIns()
    {
        SessionManager.EnsureAnyPermission(
            "Bạn không có quyền xem danh sách chờ check-in!",
            AppConstants.QuyenQuanLyDatPhong);
        return _bookingRepo.GetPendingCheckIns();
    }

    public Booking? GetActiveBookingByRoomNumber(string roomNumber)
    {
        SessionManager.EnsureAnyPermission(
            "Bạn không có quyền xem thông tin phòng đang lưu trú!",
            AppConstants.QuyenQuanLyDatPhong,
            AppConstants.QuyenQuanLyPhong);
        return _bookingRepo.GetActiveBookingByRoomNumber(roomNumber);
    }

    // Tạo đơn đặt phòng có kiểm tra quyền và dữ liệu hợp lệ
    public bool CreateBooking(Booking booking, int roomId, double priceAtBooking, out string message)
    {
        if (booking == null)
        {
            message = "Thông tin đơn đặt phòng không hợp lệ!";
            return false;
        }

        if (booking.CustomerId <= 0)
        {
            message = "Thông tin khách hàng không hợp lệ!";
            return false;
        }

        var isOwner = SessionManager.CurrentCustomer != null &&
                      SessionManager.CurrentCustomer.CustomerId == booking.CustomerId;
        var hasStaffAccess = SessionManager.HasPermission(AppConstants.QuyenQuanLyDatPhong);

        if (!isOwner && !hasStaffAccess)
        {
            message = "Bạn chỉ có quyền đặt phòng cho chính tài khoản của mình!";
            return false;
        }

        if (!FormatHelper.TryParseDate(booking.CheckInDate, out var checkIn) ||
            !FormatHelper.TryParseDate(booking.CheckOutDate, out var checkOut))
        {
            message = "Ngày nhận phòng hoặc ngày trả phòng không hợp lệ!";
            return false;
        }

        if (checkOut.Date <= checkIn.Date)
        {
            message = "Ngày trả phòng phải sau ngày nhận phòng!";
            return false;
        }

        if (roomId <= 0)
        {
            message = "Vui lòng chọn phòng cần đặt!";
            return false;
        }

        var room = _roomRepo.GetById(roomId);
        if (room == null)
        {
            message = "Không tìm thấy thông tin phòng đã chọn!";
            return false;
        }

        if (room.Status != AppConstants.PhongTrong)
        {
            message = $"Phòng {room.RoomNumber} hiện không trống ({room.Status}), không thể đặt!";
            return false;
        }

        if (hasStaffAccess)
        {
            if (booking.Status != BookingStatus.Pending && booking.Status != BookingStatus.Confirmed)
                booking.Status = BookingStatus.Confirmed;
            if (string.IsNullOrWhiteSpace(booking.Channel))
                booking.Channel = "Trực tiếp";
        }
        else
        {
            booking.Status = BookingStatus.Pending;
            if (string.IsNullOrWhiteSpace(booking.Channel))
                booking.Channel = "Trực tuyến";
        }

        var bookingId = _bookingRepo.CreateBooking(booking, roomId, priceAtBooking);
        if (bookingId > 0)
        {
            booking.BookingId = bookingId;
            message = $"Đặt phòng thành công! Mã đơn: DP{bookingId:D3}";
            return true;
        }

        message = "Đặt phòng thất bại, vui lòng kiểm tra lại!";
        return false;
    }

    // Duyệt đơn đặt phòng đang chờ duyệt sang trạng thái đã xác nhận
    public bool ConfirmBooking(int bookingId, out string message)
    {
        if (!SessionManager.CheckPermission(
                out message,
                "Bạn không có quyền duyệt đơn đặt phòng!",
                AppConstants.QuyenQuanLyDatPhong))
            return false;

        if (bookingId <= 0)
        {
            message = "Mã đơn đặt phòng không hợp lệ!";
            return false;
        }

        var booking = _bookingRepo.GetById(bookingId);
        if (booking == null)
        {
            message = "Không tìm thấy đơn đặt phòng!";
            return false;
        }

        if (booking.Status != BookingStatus.Pending)
        {
            message = "Chỉ có thể duyệt đơn đặt phòng đang ở trạng thái Chờ duyệt!";
            return false;
        }

        var result = _bookingRepo.UpdateStatus(bookingId, AppConstants.DPDaXacNhan);
        if (result)
        {
            message = $"Đã duyệt đơn đặt phòng DP{bookingId:D3}!";
            return true;
        }

        message = "Không thể duyệt đơn đặt phòng!";
        return false;
    }

    // Hủy đơn đặt phòng và hoàn trả trạng thái phòng trống
    public bool CancelBooking(int bookingId, out string message)
    {
        if (bookingId <= 0)
        {
            message = "Mã đơn đặt phòng không hợp lệ!";
            return false;
        }

        var booking = _bookingRepo.GetById(bookingId);
        if (booking == null)
        {
            message = "Không tìm thấy đơn đặt phòng!";
            return false;
        }

        var isOwner = SessionManager.CurrentCustomer != null &&
                      SessionManager.CurrentCustomer.CustomerId == booking.CustomerId;
        var hasStaffAccess = SessionManager.HasPermission(AppConstants.QuyenQuanLyDatPhong);

        if (!isOwner && !hasStaffAccess)
        {
            message = "Bạn chỉ có quyền hủy đơn đặt phòng của chính mình!";
            return false;
        }

        if (booking.Status == BookingStatus.Cancelled)
        {
            message = "Đơn đặt phòng này đã bị hủy trước đó!";
            return false;
        }

        if (booking.Status == BookingStatus.CheckedIn || booking.Status == BookingStatus.CheckedOut)
        {
            message = "Phòng đã nhận hoặc đã hoàn tất, không thể hủy!";
            return false;
        }

        if (!hasStaffAccess && booking.Status != BookingStatus.Pending)
        {
            message = "Chỉ có thể hủy đơn đặt phòng đang ở trạng thái Chờ duyệt!";
            return false;
        }

        var result = _bookingRepo.UpdateStatus(bookingId, AppConstants.DPDaHuy);
        if (result)
        {
            if (!string.IsNullOrWhiteSpace(booking.RoomNumber))
            {
                _roomRepo.UpdateStatusByRoomNumber(booking.RoomNumber, AppConstants.PhongTrong);
            }

            message = $"Đã hủy đơn đặt phòng DP{bookingId:D3}!";
            return true;
        }

        message = "Không thể hủy đơn đặt phòng!";
        return false;
    }
}
