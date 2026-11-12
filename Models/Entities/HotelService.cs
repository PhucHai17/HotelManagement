namespace Entity.Entities;

public class HotelService
{
    public int ServiceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public double UnitPrice { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public override string ToString() => $"{Name} - {UnitPrice:N0} đ/{Unit}";
}