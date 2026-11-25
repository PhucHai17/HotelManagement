using System.Diagnostics;
using System.Text;

namespace Common.Helpers;

public static class AppLogger
{
    private static readonly object SyncLock = new();

    public static string LogFilePath =>
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "hotelpro-error.log");

    // Ghi thông tin ngoại lệ kèm ngữ cảnh xảy ra lỗi
    public static void Error(Exception ex, string? context = null)
    {
        if (ex == null) return;

        var sb = new StringBuilder();
        sb.AppendLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [ERROR] {(string.IsNullOrWhiteSpace(context) ? "Lỗi hệ thống" : context)}");
        sb.AppendLine($"  Loại lỗi: {ex.GetType().FullName}");
        sb.AppendLine($"  Thông điệp: {ex.Message}");
        if (ex.InnerException != null)
            sb.AppendLine($"  Lỗi gốc (Inner): {ex.InnerException.GetType().Name} - {ex.InnerException.Message}");
        if (!string.IsNullOrWhiteSpace(ex.StackTrace))
            sb.AppendLine(ex.StackTrace);

        WriteLogEntry(sb.ToString());
    }

    // Ghi thông điệp lỗi dạng văn bản
    public static void Error(string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return;
        WriteLogEntry($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [ERROR] {message}{Environment.NewLine}");
    }

    private static void WriteLogEntry(string content)
    {
        Debug.WriteLine(content);

        try
        {
            var directory = Path.GetDirectoryName(LogFilePath);
            if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            lock (SyncLock)
            {
                File.AppendAllText(LogFilePath, content + Environment.NewLine, Encoding.UTF8);
            }
        }
        catch
        {
            // Bỏ qua lỗi I/O khi ghi file log để tránh phát sinh lỗi dây chuyền
        }
    }
}
