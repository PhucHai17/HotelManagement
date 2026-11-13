using System.Globalization;
using Common.Constants;

namespace Common.Helpers;


public static class FormatHelper
{
    private static readonly CultureInfo VietnameseCulture =
        CultureInfo.GetCultureInfo(AppConstants.CurrencyCulture);
    
    /// Định dạng tiền tệ kèm chữ đ.
    public static string FormatCurrency(decimal amount)
    {
        return amount.ToString("N0", VietnameseCulture) + " đ";
    }
    /// Định dạng tiền tệ (kiểu double).
    public static string FormatCurrency(double amount)
    {
        return amount.ToString("N0", VietnameseCulture) + " đ";
    }
    
    /// Định dạng ngày dd/MM/yyyy.
    public static string FormatDate(DateTime date)
    {
        return date.ToString(
            AppConstants.DateFormat,
            VietnameseCulture);
    }
    
    /// Định dạng ngày giờ.
    public static string FormatDateTime(DateTime dateTime)
    {
        return dateTime.ToString(
            AppConstants.DateTimeFormat,
            VietnameseCulture);
    }

    /// Định dạng giờ.
    public static string FormatTime(DateTime time)
    {
        return time.ToString(
            AppConstants.TimeFormat,
            VietnameseCulture);
    }

    /// Kiểm tra và chuyển chuỗi ngày hợp lệ về DateTime (hỗ trợ dd/MM/yyyy và yyyy-MM-dd).
    public static bool TryParseDate(string? dateStr, out DateTime result)
    {
        result = default;
        if (string.IsNullOrWhiteSpace(dateStr)) return false;

        var trimmed = dateStr.Trim();
        string[] formats = { AppConstants.DateFormat, "yyyy-MM-dd", "d/M/yyyy", "yyyy-M-d" };
        if (DateTime.TryParseExact(trimmed, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            return true;

        return DateTime.TryParse(trimmed, VietnameseCulture, DateTimeStyles.None, out result)
               || DateTime.TryParse(trimmed, CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
    }
    
    /// Chuyển chuỗi ngày về DateTime.
    public static DateTime ParseDate(string dateStr)
    {
        return TryParseDate(dateStr, out var result) ? result : DateTime.Today;
    }
    /// Tính số đêm lưu trú (tối thiểu 1 đêm).
    public static int CalculateNights(DateTime checkIn, DateTime checkOut)
    {
        var days = (int)(checkOut.Date - checkIn.Date).TotalDays;
        return days > 0 ? days : 1;
    }
}
