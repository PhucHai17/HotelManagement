namespace DAL.Repositories;

public class ReviewRepository : BaseRepository, IReviewRepository
{
    // Lưu đánh giá mới của khách hàng cho một đơn đặt phòng
    public bool Add(Review review)
    {
        var sql = @"
            INSERT INTO DanhGia (MaKH, MaDP, SoSao, NoiDung)
            VALUES (@MaKH, @MaDP, @SoSao, @NoiDung);";

        var result = DatabaseHelper.ExecuteNonQuery(sql,
            CreateParameter("@MaKH", review.CustomerId),
            CreateParameter("@MaDP", review.BookingId),
            CreateParameter("@SoSao", review.Stars),
            CreateParameter("@NoiDung", review.Content));
        return result > 0;
    }

    // Lấy đánh giá của một đơn đặt phòng (null nếu chưa đánh giá)
    public Review? GetByBookingId(int bookingId)
    {
        var sql = @"
            SELECT dg.MaDG, dg.MaKH, dg.MaDP, dg.SoSao, dg.NoiDung, kh.HoTen
            FROM DanhGia dg
            INNER JOIN KhachHang kh ON dg.MaKH = kh.MaKH
            WHERE dg.MaDP = @MaDP
            LIMIT 1;";

        return QuerySingle(sql, MapToReview, CreateParameter("@MaDP", bookingId));
    }

    private Review MapToReview(DataRow row)
    {
        return new Review
        {
            ReviewId = GetValue<int>(row, "MaDG"),
            CustomerId = GetValue<int>(row, "MaKH"),
            BookingId = GetValue<int>(row, "MaDP"),
            Stars = GetValue<int>(row, "SoSao", 5),
            Content = GetValue<string>(row, "NoiDung", ""),
            CustomerName = GetValue<string>(row, "HoTen", "")
        };
    }
}
