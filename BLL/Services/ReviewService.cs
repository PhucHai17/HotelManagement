using BLL.Interfaces;

namespace BLL.Services;

public class ReviewService : IReviewService
{
    private const int MaxContentLength = 500;

    private readonly IReviewRepository _reviewRepo;
    private readonly IBookingRepository _bookingRepo;

    public ReviewService() : this(new ReviewRepository(), new BookingRepository())
    {
    }

    public ReviewService(IReviewRepository reviewRepo, IBookingRepository bookingRepo)
    {
        _reviewRepo = reviewRepo ?? throw new ArgumentNullException(nameof(reviewRepo));
        _bookingRepo = bookingRepo ?? throw new ArgumentNullException(nameof(bookingRepo));
    }

    public Review? GetByBookingId(int bookingId)
    {
        return bookingId <= 0 ? null : _reviewRepo.GetByBookingId(bookingId);
    }

    // Khách hàng gửi đánh giá cho đơn đặt phòng đã hoàn tất
    public bool SubmitReview(int bookingId, int stars, string? content, out string message)
    {
        var customer = SessionManager.CurrentCustomer;
        if (customer == null || customer.CustomerId <= 0)
        {
            message = "Chỉ khách hàng đã đăng nhập mới có thể gửi đánh giá!";
            return false;
        }

        if (stars < 1 || stars > 5)
        {
            message = "Số sao đánh giá phải từ 1 đến 5!";
            return false;
        }

        var text = (content ?? string.Empty).Trim();
        if (text.Length > MaxContentLength)
        {
            message = $"Nội dung đánh giá tối đa {MaxContentLength} ký tự!";
            return false;
        }

        var booking = _bookingRepo.GetById(bookingId);
        if (booking == null)
        {
            message = "Không tìm thấy đơn đặt phòng!";
            return false;
        }

        if (booking.CustomerId != customer.CustomerId)
        {
            message = "Bạn chỉ có thể đánh giá đơn đặt phòng của chính mình!";
            return false;
        }

        if (booking.Status != BookingStatus.CheckedOut)
        {
            message = "Chỉ có thể đánh giá sau khi đã hoàn tất kỳ nghỉ!";
            return false;
        }

        if (_reviewRepo.GetByBookingId(bookingId) != null)
        {
            message = "Bạn đã đánh giá đơn đặt phòng này rồi!";
            return false;
        }

        var ok = _reviewRepo.Add(new Review
        {
            CustomerId = customer.CustomerId,
            BookingId = bookingId,
            Stars = stars,
            Content = text
        });

        message = ok ? "Cảm ơn bạn đã gửi đánh giá!" : "Không thể lưu đánh giá, vui lòng thử lại!";
        return ok;
    }
}
