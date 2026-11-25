namespace Entity.DTOs;

/// DTO thống kê doanh thu theo tháng để vẽ biểu đồ.
public class MonthlyRevenueDto
{
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal Revenue { get; set; }
    public int BookingCount { get; set; }

    public string MonthLabel => $"Tháng {Month}/{Year}";
}
