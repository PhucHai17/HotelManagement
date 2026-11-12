namespace Entity.Entities;

public class Payment
{
    public int PaymentId { get; set; }
    public int InvoiceId { get; set; }
    public double Amount { get; set; }
    public string Method { get; set; } = "Tiền mặt";
    public string TransactionType { get; set; } = "Thanh toán hóa đơn";
    public string PaymentDate { get; set; } = string.Empty;
}