using HotelManagementSystem.Models.Entities;

namespace DAL.Interfaces;

// Interface repository dịch vụ
public interface IServiceRepository : IGenericRepository<Service>
{
    int AddService(Service service);
    int RecordServiceUsage(int bookingDetailId, int serviceId, int quantity, double unitPrice, string usageDate);
    List<ServiceUsage> GetUsagesByBookingDetailId(int bookingDetailId);
    List<ServiceUsage> GetUsagesByBookingId(int bookingId);
    List<BookingDetail> GetCustomerBookingDetailsForService(int customerId);
    List<ServiceUsage> GetUsagesByCustomerId(int customerId);
}
