using BLL.Interfaces;
using HotelManagementSystem.Models.Entities;

namespace BLL.Services;

// Service nghiệp vụ dịch vụ khách sạn
public class HotelService : IHotelService
{
    private readonly IBookingRepository _bookingRepo;
    private readonly IServiceRepository _serviceRepo;

    public HotelService() : this(new BookingRepository(), new ServiceRepository())
    {
    }

    public HotelService(IBookingRepository bookingRepo, IServiceRepository serviceRepo)
    {
        _bookingRepo = bookingRepo ?? throw new ArgumentNullException(nameof(bookingRepo));
        _serviceRepo = serviceRepo ?? throw new ArgumentNullException(nameof(serviceRepo));
    }

    // Lấy tất cả dịch vụ
    public List<Service> GetAll()
    {
        return _serviceRepo.GetAll();
    }

    // Thêm dịch vụ mới
    public bool AddService(Service service, out string message)
    {
        if (!SessionManager.CheckPermission(
                out message,
                "Bạn không có quyền thêm dịch vụ mới!",
                AppConstants.QuyenQuanLyPhong,
                AppConstants.QuyenQuanLyGia))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(service.Name))
        {
            message = "Tên dịch vụ không được để trống!";
            return false;
        }

        if (service.UnitPrice <= 0)
        {
            message = "Đơn giá phải lớn hơn 0!";
            return false;
        }

        if (string.IsNullOrWhiteSpace(service.Unit))
        {
            message = "Vui lòng nhập đơn vị tính (ví dụ: suất, kg, món...)!";
            return false;
        }

        var newId = _serviceRepo.AddService(service);
        if (newId > 0)
        {
            service.ServiceId = newId;
            message = "Thêm dịch vụ mới thành công!";
            return true;
        }

        message = "Thêm dịch vụ thất bại!";
        return false;
    }

    // Ghi dịch vụ cho phòng đang ở
    public bool RecordServiceUsage(string roomNumber, int serviceId, int quantity, out string message)
    {
        if (!SessionManager.CheckPermission(
                out message,
                "Bạn không có quyền ghi nhận dịch vụ cho phòng!",
                AppConstants.QuyenQuanLyPhong,
                AppConstants.QuyenQuanLyDatPhong))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(roomNumber))
        {
            message = "Vui lòng chọn số phòng đang ở!";
            return false;
        }

        if (serviceId <= 0)
        {
            message = "Vui lòng chọn loại dịch vụ!";
            return false;
        }

        if (quantity <= 0)
        {
            message = "Số lượng phải lớn hơn 0!";
            return false;
        }

        var bookingDetailId = _bookingRepo.GetActiveBookingDetailIdByRoomNumber(roomNumber.Trim());
        if (!bookingDetailId.HasValue || bookingDetailId.Value <= 0)
        {
            message = $"Không tìm thấy đơn đặt phòng đang lưu trú của phòng {roomNumber}!";
            return false;
        }

        var allServices = _serviceRepo.GetAll();
        var selectedService = allServices.Find(x => x.ServiceId == serviceId);
        if (selectedService == null)
        {
            message = "Dịch vụ không tồn tại!";
            return false;
        }

        var newUsageId = _serviceRepo.RecordServiceUsage(bookingDetailId.Value, serviceId, quantity, selectedService.UnitPrice,
            DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
        if (newUsageId > 0)
        {
            message = $"Đã thêm {quantity} {selectedService.Unit} '{selectedService.Name}' vào hóa đơn phòng {roomNumber}!";
            return true;
        }

        message = "Lỗi khi ghi nhận dịch vụ!";
        return false;
    }

    // Đặt dịch vụ trực tiếp theo mã chi tiết đặt phòng (cho giao diện khách hàng hoặc nhân viên)
    public bool RecordServiceUsageByBookingDetail(int bookingDetailId, int serviceId, int quantity, out string message)
    {
        if (!SessionManager.CheckPermission(
                out message,
                "Bạn không có quyền đặt dịch vụ!",
                AppConstants.QuyenQuanLyDatPhong,
                AppConstants.QuyenQuanLyPhong,
                AppConstants.QuyenDatPhongOnline))
        {
            return false;
        }

        if (bookingDetailId <= 0)
        {
            message = "Vui lòng chọn phòng đặt hợp lệ!";
            return false;
        }

        if (!SessionManager.HasAnyPermission(AppConstants.QuyenQuanLyDatPhong, AppConstants.QuyenQuanLyPhong))
        {
            var customerId = SessionManager.CurrentCustomer?.CustomerId ?? 0;
            var ownedDetails = customerId > 0
                ? _serviceRepo.GetCustomerBookingDetailsForService(customerId)
                : new List<BookingDetail>();
            if (!ownedDetails.Any(ct => ct.BookingDetailId == bookingDetailId))
            {
                message = "Bạn chỉ có quyền đặt dịch vụ cho đơn đặt phòng của chính mình!";
                return false;
            }
        }

        if (serviceId <= 0)
        {
            message = "Vui lòng chọn dịch vụ!";
            return false;
        }

        if (quantity <= 0)
        {
            message = "Số lượng phải lớn hơn 0!";
            return false;
        }

        var selectedService = _serviceRepo.GetAll().Find(x => x.ServiceId == serviceId);
        if (selectedService == null)
        {
            message = "Dịch vụ không tồn tại!";
            return false;
        }

        var newUsageId = _serviceRepo.RecordServiceUsage(
            bookingDetailId,
            serviceId,
            quantity,
            selectedService.UnitPrice,
            DateTime.Now.ToString("dd/MM/yyyy HH:mm"));

        if (newUsageId > 0)
        {
            message = $"Đã đặt thành công {quantity} {selectedService.Unit} '{selectedService.Name}'!";
            return true;
        }

        message = "Không thể ghi nhận đặt dịch vụ!";
        return false;
    }

    // Lấy danh sách phòng đã đặt của khách để đặt dịch vụ
    public List<BookingDetail> GetCustomerBookingDetailsForService(int customerId)
    {
        if (!SessionManager.HasStaffOrOwnerAccess(customerId, AppConstants.QuyenQuanLyDatPhong, AppConstants.QuyenQuanLyPhong))
            throw new UnauthorizedAccessException("Bạn chỉ có quyền xem danh sách phòng đặt dịch vụ của chính mình!");

        if (customerId <= 0) return new List<BookingDetail>();
        return _serviceRepo.GetCustomerBookingDetailsForService(customerId);
    }

    // Lấy dịch vụ đã dùng theo mã đặt phòng
    public List<ServiceUsage> GetServiceUsagesByBookingId(int bookingId)
    {
        SessionManager.EnsureAnyPermission(
            "Bạn không có quyền xem dịch vụ của đơn đặt phòng!",
            AppConstants.QuyenQuanLyDatPhong,
            AppConstants.QuyenQuanLyPhong,
            AppConstants.QuyenDatPhongOnline);

        if (!SessionManager.HasAnyPermission(AppConstants.QuyenQuanLyDatPhong, AppConstants.QuyenQuanLyPhong))
        {
            var booking = _bookingRepo.GetById(bookingId);
            if (booking == null || !SessionManager.HasStaffOrOwnerAccess(booking.CustomerId, AppConstants.QuyenQuanLyDatPhong, AppConstants.QuyenQuanLyPhong))
                throw new UnauthorizedAccessException("Bạn chỉ có quyền xem dịch vụ của đơn đặt phòng thuộc về mình!");
        }

        return _serviceRepo.GetUsagesByBookingId(bookingId);
    }

    // Lấy lịch sử sử dụng dịch vụ của khách hàng
    public List<ServiceUsage> GetServiceUsagesByCustomerId(int customerId)
    {
        if (!SessionManager.HasStaffOrOwnerAccess(customerId, AppConstants.QuyenQuanLyDatPhong, AppConstants.QuyenQuanLyPhong))
            throw new UnauthorizedAccessException("Bạn chỉ có quyền xem lịch sử dịch vụ của chính mình!");

        if (customerId <= 0) return new List<ServiceUsage>();
        return _serviceRepo.GetUsagesByCustomerId(customerId);
    }
}
