namespace Entity.Enums;

/// Trạng thái thanh toán hóa đơn.
public enum InvoiceStatus
{
    Unpaid = 0,     // Chờ thanh toán
    Paid = 1,       // Đã thanh toán đủ
    Cancelled = 2   // Đã hủy hóa đơn
}
