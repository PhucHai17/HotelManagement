using Microsoft.Data.Sqlite;

namespace DAL.Database;

public static class DatabaseHelper
{
    // Mở kết nối SQLite
    public static SqliteConnection GetConnection()
    {
        var connection = DBConnection.CreateConnection();

        try
        {
            connection.Open();
            return connection;
        }
        catch
        {
            connection.Dispose();
            throw;
        }
    }

    // Thêm các tham số truy vấn
    private static void AddParameters(
        SqliteCommand command,
        SqliteParameter[]? parameters)
    {
        if (parameters != null && parameters.Length > 0)
        {
            command.Parameters.AddRange(parameters);
        }
    }

    // INSERT, UPDATE, DELETE
    public static int ExecuteNonQuery(
        string sql,
        params SqliteParameter[] parameters)
    {
        using var connection = GetConnection();
        using var command = new SqliteCommand(sql, connection);

        AddParameters(command, parameters);

        return command.ExecuteNonQuery();
    }

    // SELECT một giá trị: COUNT, SUM, MAX...
    public static object? ExecuteScalar(
        string sql,
        params SqliteParameter[] parameters)
    {
        using var connection = GetConnection();
        using var command = new SqliteCommand(sql, connection);

        AddParameters(command, parameters);

        return command.ExecuteScalar();
    }

    // SELECT nhiều dòng dữ liệu
    public static DataTable ExecuteDataTable(
        string sql,
        params SqliteParameter[] parameters)
    {
        var table = new DataTable();

        using var connection = GetConnection();
        using var command = new SqliteCommand(sql, connection);

        AddParameters(command, parameters);

        using var reader = command.ExecuteReader();

        // Đọc dữ liệu thủ công, xử lý cột trùng tên
        var columnIndexes = new List<int>();

        for (int i = 0; i < reader.FieldCount; i++)
        {
            string columnName = reader.GetName(i);

            if (!table.Columns.Contains(columnName))
            {
                table.Columns.Add(columnName, typeof(object));
                columnIndexes.Add(i);
            }
        }

        while (reader.Read())
        {
            var row = table.NewRow();

            for (int i = 0; i < columnIndexes.Count; i++)
            {
                int index = columnIndexes[i];

                row[i] = reader.IsDBNull(index)
                    ? DBNull.Value
                    : reader.GetValue(index);
            }

            table.Rows.Add(row);
        }

        return table;
    }

    // Tạo tham số SQLite
    public static SqliteParameter CreateParameter(
        string name,
        object? value)
    {
        return new SqliteParameter(name, value ?? DBNull.Value);
    }
}
