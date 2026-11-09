using Entity.DTOs;
using Entity.Entities;

namespace DAL.Interfaces;

public interface IInvoiceRepository : IGenericRepository<Invoice>
{
    Invoice? GetInvoiceByBookingId(int bookingId);
    IEnumerable<MonthlyRevenueDto> GetMonthlyRevenueReport(int year);
}