using System.Reflection;

namespace DAL.Database;

public static class DatabaseInitializer
{
    public static void KhoiTao()
    {
        using var ketNoi = Database.TaoKetNoi();
        ketNoi.Open();

        using var stream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("DAL.Database.CreateDatabase.sql");

        if (stream == null) throw new FileNotFoundException("Khong tim thay file CreateDatabase.sql");

        using var reader = new StreamReader(stream);
        var noiDungSQL = reader.ReadToEnd();

        using var lenh = ketNoi.CreateCommand();
        lenh.CommandText = noiDungSQL;
        lenh.ExecuteNonQuery();
    }
}