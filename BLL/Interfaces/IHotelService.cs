using HotelManagementSystem.Models.Entities;

namespace BLL.Interfaces;

public interface IHotelService
{
    List<Service> GetAll();
    bool AddService(Service service, out string message);
    bool RecordServiceUsage(string roomNumber, int serviceId, int quantity, out string message);
    bool RecordServiceUsageByBookingDetail(int bookingDetailId, int serviceId, int quantity, out string message);
    List<BookingDetail> GetCustomerBookingDetailsForService(int customerId);
    List<ServiceUsage> GetServiceUsagesByBookingId(int bookingId);
    List<ServiceUsage> GetServiceUsagesByCustomerId(int customerId);
    bool SaveService(Service dv, out string s);
}