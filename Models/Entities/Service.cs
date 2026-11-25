namespace HotelManagementSystem.Models.Entities;

// Thực thể dịch vụ
public class Service
{
    public int ServiceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public double UnitPrice { get; set; }
    public string Unit { get; set; } = string.Empty; // suất, kg, món, lượt, vé
    public string Description { get; set; } = string.Empty;

    public override string ToString() => $"{Name} - {UnitPrice:N0} đ/{Unit}";
}
