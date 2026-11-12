using System.Globalization;
using Common.Constaints;


namespace Common.Helpers;

public static class FormatHelper
{
    private static readonly CultureInfo VietnameCulture =
        new("vi-VN");
    public static string FormatCurrency(decimal amount)
    {
        return amount.ToString("N0", VietnameCulture) + " ₫";
    }
    public static string FormatDate(DateTime date)
    {
        return date.ToString(
            AppConstants.DateFormat);
    }

    public static DateTime ParseDate(string text)
    {
        if (DateTime.TryParseExact(text, AppConstants.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var exact))
            return exact;
        if (DateTime.TryParse(text, out var loose))
            return loose;
        return DateTime.Today;
    }
    public static int CalculateNights(DateTime checkIn, DateTime checkOut)
    {
        var days = (int)(checkOut.Date - checkIn.Date).TotalDays;
        return days > 0 ? days : 1;
    }
}