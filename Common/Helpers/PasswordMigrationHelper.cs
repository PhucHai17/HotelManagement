
using Common.Helpers;
using Microsoft.Data.Sqlite;

namespace Common.Helpers;

public static class PasswordMigrationHelper
{
    public static void MigratePlainTextPasswords(string connectionString)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        using var transaction = connection.BeginTransaction();

        try
        {
            var accounts = new List<(int Id, string Password)>();

            // Đọc toàn bộ mật khẩu hiện có
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = """
                    SELECT MaTK, MatKhauHash
                    FROM TaiKhoan;
                    """;

                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    accounts.Add((
                        reader.GetInt32(0),
                        reader.GetString(1)
                    ));
                }
            }

            // Chỉ hash những mật khẩu còn ở dạng thô (chưa phải SHA-256/PBKDF2)
            foreach (var account in accounts)
            {
                if (PasswordHelper.IsHashed(account.Password))
                    continue;

                string newHash =
                    PasswordHelper.HashPassword(account.Password);

                using var updateCommand = connection.CreateCommand();

                updateCommand.Transaction = transaction;
                updateCommand.CommandText = """
                    UPDATE TaiKhoan
                    SET MatKhauHash = @Hash
                    WHERE MaTK = @Id;
                    """;

                updateCommand.Parameters.AddWithValue("@Hash", newHash);
                updateCommand.Parameters.AddWithValue("@Id", account.Id);

                updateCommand.ExecuteNonQuery();
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
