using Microsoft.Data.Sqlite;

namespace DAL.Repositories;

public abstract class BaseRepository
{
    //tao tham so (tranh injection)
    protected SqliteParameter CreateParameter(string name, object? value)
    {
        return new SqliteParameter(name, value ?? DBNull.Value);
    }
    protected T GetValue<T>(
        DataRow row,
        string column,
        T defaultValue = default!)
    {
        if (!row.Table.Columns.Contains(column) || row.IsNull(column))
            return defaultValue;

        object value = row[column];

        Type targetType = Nullable.GetUnderlyingType(typeof(T))
                          ?? typeof(T);

        try
        {
            if (targetType.IsEnum)
                return (T)Enum.ToObject(targetType, value);

            if (targetType == typeof(bool))
                return (T)(object)Convert.ToBoolean(value);

            return (T)Convert.ChangeType(value, targetType);
        }
        catch (Exception ex) when (
            ex is InvalidCastException ||
            ex is FormatException ||
            ex is OverflowException)
        {
            return defaultValue;
        }
    }
    protected T GetValue<T>(SqliteDataReader reader, string column, T defaultValue = default!)
    {
        try
        {
            var ordinal = reader.GetOrdinal(column);
            if (!reader.IsDBNull(ordinal)) return (T)Convert.ChangeType(reader.GetValue(ordinal), typeof(T));
        }
        catch
        {
            return defaultValue;
        }
        return defaultValue;
    }
     //Đọc cột kiểu int có thể null
    protected int? GetNullableInt(DataRow row, string column)
    {
        return row.IsNull(column) ? null : GetValue<int>(row, column);
    }

    //Chạy SELECT và chuyển từng dòng thành đối tượng bằng hàm map
    protected List<T> QueryList<T>(string sql, Func<DataRow, T> map, params SqliteParameter[] parameters)
    {
        var table = DatabaseHelper.ExecuteDataTable(sql, parameters);
        var result = new List<T>();
        foreach (DataRow row in table.Rows) result.Add(map(row));
        return result;
    }

    //Chạy SELECT và lấy dòng đầu tiên
    protected T? QuerySingle<T>(string sql, Func<DataRow, T> map, params SqliteParameter[] parameters) where T : class
    {
        var table = DatabaseHelper.ExecuteDataTable(sql, parameters);
        return table.Rows.Count > 0 ? map(table.Rows[0]) : null;
    }

    //Chạy lệnh INSERT ... ;
    protected int InsertAndGetId(
        string sql,
        params SqliteParameter[] parameters)
    {
        using var connection = DatabaseHelper.GetConnection();
        using var command = new SqliteCommand(sql, connection);

        command.Parameters.AddRange(parameters);

        object? result = command.ExecuteScalar();

        return Convert.ToInt32(result);
    }

    // Thực thi truy vấn và đọc danh sách qua SqliteDataReader
    protected List<T> ExecuteReadList<T>(string sql, Func<SqliteDataReader, T> convert, params SqliteParameter[] parameters)
    {
        var list = new List<T>();
        using var connection = DatabaseHelper.GetConnection();
        using var command = new SqliteCommand(sql, connection);
        if (parameters != null && parameters.Length > 0) command.Parameters.AddRange(parameters);
        using var reader = command.ExecuteReader();
        while (reader.Read()) list.Add(convert(reader));
        return list;
    }

    //Quản lý giao dịch nguyên tử (SqliteTransaction) khi thao tác nhiều bảng cùng lúc
    protected void ExecuteInTransaction(
        Action<SqliteConnection, SqliteTransaction> action)
    {
        using var connection = DatabaseHelper.GetConnection();
        using var transaction = connection.BeginTransaction();

        try
        {
            action(connection, transaction);
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}