using Microsoft.Data.Sqlite;

namespace DAL.Database;

public class DBConnection
{
    public static readonly string DatabasePath = Path.Combine(
        AppContext.BaseDirectory,
        "DAL", "Scripts", "hotelmanagement.db"
    );

    private static readonly string ConnectionString =
        new SqliteConnectionStringBuilder
        {
            DataSource = DatabasePath,
            Mode = SqliteOpenMode.ReadWrite,
            ForeignKeys = true
        }.ToString();

    public static void EnsureDatabaseExists()
    {
        if (!File.Exists(DatabasePath))
        {
            throw new FileNotFoundException(
                "Không tìm thấy file CSDL hotelmanagement.db.",
                DatabasePath
            );
        }
    }

    public static SqliteConnection CreateConnection()
    {
        EnsureDatabaseExists();

        return new SqliteConnection(ConnectionString);
    }
}