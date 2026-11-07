using Microsoft.Data.Sqlite;

namespace DAL.Database;

public static class Database
{
    private static readonly string DuongDanDatabase =
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "HotelManagement",
            "hotel.db"
        );

    private static readonly string ChuoiKetNoi =
        new SqliteConnectionStringBuilder
        {
            DataSource = DuongDanDatabase,
            Mode = SqliteOpenMode.ReadWriteCreate,
            ForeignKeys = true
        }.ToString();

    public static SqliteConnection TaoKetNoi()
    {
        var thuMuc = Path.GetDirectoryName(DuongDanDatabase);

        if (!Directory.Exists(thuMuc)) Directory.CreateDirectory(thuMuc!);

        return new SqliteConnection(ChuoiKetNoi);
    }
}