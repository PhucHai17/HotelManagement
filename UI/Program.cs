
using Common.Constants;
using Common.Helpers;
using DAL.Database;
using UI.Forms;

namespace UI;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        // Xử lý lỗi toàn cục
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

        Application.ThreadException += (_, e) =>
            HandleException(e.Exception);

        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
            HandleException(e.ExceptionObject as Exception
                            ?? new Exception("Lỗi không xác định."));

        TaskScheduler.UnobservedTaskException += (_, e) =>
        {
            AppLogger.Error(e.Exception, "Lỗi tác vụ nền");
            e.SetObserved();
        };

        // Cấu hình giao diện
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.SetDefaultFont(new Font(AppConstants.FontChuChinh, 10f));

        // Khởi tạo database
        try
        {
            DBConnection.EnsureDatabaseExists();
            
        }
        catch (Exception ex)
        {
            AppLogger.Error(ex, "Khởi tạo database");

            MessageBox.Show(
                $"Không thể khởi tạo database: {ex.Message}",
                "Lỗi hệ thống",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }

        // Mở màn hình đăng nhập
        Application.Run(new LoginForm());
    }

    private static void HandleException(Exception ex)
    {
        AppLogger.Error(ex, "Lỗi toàn cục");

        MessageBox.Show(
            $"Đã xảy ra lỗi: {ex.Message}",
            "Lỗi hệ thống",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error
        );
    }
}
