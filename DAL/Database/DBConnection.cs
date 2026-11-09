using System.Data;
using System.Data.SQLite;

namespace DAL.Database;

public class DBConnection
{
    private readonly string _connectionString;

    public DBConnection(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CreateConnection()
    {
        var connection = new SQLiteConnection(_connectionString);
        connection.Open();
        return connection;
    }
}