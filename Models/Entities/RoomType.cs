namespace Entity.Entities;

public class RoomType
{
    public int RoomTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public double BasePrice { get; set; }
    public int Capacity { get; set; } = 2;
    public string Description { get; set; } = string.Empty;
    public string Amenities { get; set; } = "Wi-Fi, Điều hòa, TV, Ban công, Minibar, Bồn tắm";
    public string ImagePath { get; set; } = string.Empty;
    public override string ToString() => $"{Name} - {BasePrice:N0} đ/đêm";
}