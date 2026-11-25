namespace DAL.Interfaces;

// Interface repository đánh giá của khách hàng
public interface IReviewRepository
{
    bool Add(Review review);
    Review? GetByBookingId(int bookingId);
}
