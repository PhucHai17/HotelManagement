namespace BLL.Interfaces;

// Interface dịch vụ hóa đơn
public interface IInvoiceService
{
    List<Invoice> GetAll();
    Invoice? GetById(int invoiceId);
    bool Pay(int invoiceId, string paymentMethod, out string message);
}
