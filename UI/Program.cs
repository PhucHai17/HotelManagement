using Common.Constants;
using Common.Helpers;
using DAL.Database;
using UI.Forms;
using UI.Helpers;

namespace UI;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += (_, e) => HandleException(e.Exception);
        AppDomain.CurrentDomain.UnhandledException += (_, e) => HandleException(e.ExceptionObject as Exception ?? new Exception("Lỗi không xác định."));
        TaskScheduler.UnobservedTaskException += (_, e) => { AppLogger.Error(e.Exception, "Lỗi tác vụ nền"); e.SetObserved(); };
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.SetDefaultFont(new Font(AppConstants.FontChuChinh, 10f));

        try
        {
            DBConnection.EnsureDatabaseExists();
            PasswordMigrationHelper.MigratePlainTextPasswords(DBConnection.ConnectionString);
            Application.Run(new LoginForm { Icon = HotelProIcon.Instance, ShowInTaskbar = true });
        }
        catch (Exception ex)
        {
            AppLogger.Error(ex, "Khởi tạo ứng dụng");
            MessageBox.Show($"Không thể khởi tạo ứng dụng: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static void HandleException(Exception ex)
    {
        AppLogger.Error(ex, "Lỗi toàn cục");
        MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
