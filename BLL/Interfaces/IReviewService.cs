namespace BLL.Interfaces;

public interface IReviewService
{
    Review? GetByBookingId(int bookingId);
    bool SubmitReview(int bookingId, int stars, string? content, out string message);
}
