namespace Entity.Entities;

public class Promotion
{
    // mã khuyến mãi
    public int PromotionId { get; set; }
    // mã code
    public string Code { get; set; } = string.Empty;
    // phần trăm giảm
    public double DiscountPercent { get; set; }
    // ngày bắt đầu
    public string StartDate { get; set; } = string.Empty;
    // ngày kết thúc
    public string EndDate { get; set; } = string.Empty;
    public override string ToString() => $"{Code} (-{DiscountPercent}%)";
}