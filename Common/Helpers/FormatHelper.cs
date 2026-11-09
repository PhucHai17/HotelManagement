using System.Globalization;
using Common.Constaints;


namespace Common.Helpers;

public static class FormatHelper
{
    private static readonly CultureInfo VietnameseCulture =
        CultureInfo.GetCultureInfo(AppConstants.CurrencyCulture);


    /// Định dạng tiền tệ Việt Nam.
    public static string FormatCurrency(decimal amount)
    {
        return amount.ToString("N0", VietnameseCulture) + " ₫";
    }
    /// Định dạng ngày tháng.
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
}