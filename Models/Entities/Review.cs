namespace Entity.Entities;

public class Review
{
    // mã đánh giá
    public int ReviewId { get; set; }
    // mã khách hàng
    public int CustomerId { get; set; }
    // mã đơn đặt phòng
    public int BookingId { get; set; }
    // số sao
    public int Stars { get; set; } = 5;
    // nội dung
    public string Content { get; set; } = string.Empty;
    // tên khách
    public string CustomerName { get; set; } = string.Empty;
}