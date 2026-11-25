using Entity.Enums;

namespace Entity.Entities;

public class Invoice
{
    public int InvoiceId { get; set; }
    public int BookingId { get; set; }
    public int? EmployeeId { get; set; }
    public int? ShiftId { get; set; }
    public double RoomCharge { get; set; }
    public double ServiceCharge { get; set; }
    public double Discount { get; set; }
    public double VatAmount { get; set; }
    public double TotalAmount { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Unpaid;
    public string IssuedDate { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string RoomNumber { get; set; } = string.Empty;
    public string DisplayCode => $"HD{InvoiceId:D3}";
}

