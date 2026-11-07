using DAL.Database;

namespace UI;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        
        ApplicationConfiguration.Initialize();
        DatabaseInitializer.KhoiTao();
        Application.Run(new Form1());
    }
}