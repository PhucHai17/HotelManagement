namespace DAL.Interfaces;

// Interface repository hóa đơn
public interface IInvoiceRepository
{
    List<Invoice> GetAll();
    Invoice? GetById(int invoiceId);
    int CreateInvoice(Invoice invoice);
    bool PayInvoice(int invoiceId, string paymentMethod);
}
