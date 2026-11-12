namespace Entity.Entities;

public class MembershipTier
{
    public int TierId { get; set; }
    public string Name { get; set; } = "Thường";
    public int MinimumPoints { get; set; } = 0;
    public double DiscountPercent { get; set; } = 0.0;
}