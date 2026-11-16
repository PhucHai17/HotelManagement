using HotelManagementSystem.Models.Entities;

namespace DAL.Repositories;

public class ServiceRepository : IServiceRepository
{
    private IServiceRepository _serviceRepositoryImplementation;
    public List<Service> GetAll()
    {
        return _serviceRepositoryImplementation.GetAll();
    }

    public Service? GetById(int id)
    {
        return _serviceRepositoryImplementation.GetById(id);
    }

    public bool Add(Service entity)
    {
        return _serviceRepositoryImplementation.Add(entity);
    }

    public bool Update(Service entity)
    {
        return _serviceRepositoryImplementation.Update(entity);
    }

    public bool Delete(int id)
    {
        return _serviceRepositoryImplementation.Delete(id);
    }

    public int AddService(Service service)
    {
        return _serviceRepositoryImplementation.AddService(service);
    }

    public int RecordServiceUsage(int bookingDetailId, int serviceId, int quantity, double unitPrice, string usageDate)
    {
        return _serviceRepositoryImplementation.RecordServiceUsage(bookingDetailId, serviceId, quantity, unitPrice, usageDate);
    }

    public List<ServiceUsage> GetUsagesByBookingDetailId(int bookingDetailId)
    {
        return _serviceRepositoryImplementation.GetUsagesByBookingDetailId(bookingDetailId);
    }

    public List<ServiceUsage> GetUsagesByBookingId(int bookingId)
    {
        return _serviceRepositoryImplementation.GetUsagesByBookingId(bookingId);
    }

    public List<BookingDetail> GetCustomerBookingDetailsForService(int customerId)
    {
        return _serviceRepositoryImplementation.GetCustomerBookingDetailsForService(customerId);
    }

    public List<ServiceUsage> GetUsagesByCustomerId(int customerId)
    {
        return _serviceRepositoryImplementation.GetUsagesByCustomerId(customerId);
    }
}