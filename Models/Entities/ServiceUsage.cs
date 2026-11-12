namespace Entity.Entities;

public class ServiceUsage
{
    public int UsageId { get; set; }
    public int BookingDetailId { get; set; }
    public int ServiceId { get; set; }
    public int Quantity { get; set; } = 1;
    public double UnitPrice { get; set; }
    public string UsageDate { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    // donvitinh
    public string Unit { get; set; } = string.Empty;
    public string RoomNumber { get; set; } = string.Empty;
    // thành tiền = số lượng x đơn giá
    public double LineTotal => Quantity * UnitPrice;
}