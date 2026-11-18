using Microsoft.Data.Sqlite;

namespace ScholarWeb.Data;

public static class DatabasePath
{
    public const string DefaultConnectionString = "Data Source=Data/scholarweb.db";

    public static string ResolveConnectionString(string contentRootPath, string? connectionString)
    {
        var connectionStringBuilder = new SqliteConnectionStringBuilder(
            string.IsNullOrWhiteSpace(connectionString) ? DefaultConnectionString : connectionString);

        if (!Path.IsPathRooted(connectionStringBuilder.DataSource))
        {
            connectionStringBuilder.DataSource = Path.GetFullPath(
                Path.Combine(contentRootPath, connectionStringBuilder.DataSource));
        }

        var databaseDirectory = Path.GetDirectoryName(connectionStringBuilder.DataSource);
        if (!string.IsNullOrWhiteSpace(databaseDirectory))
        {
            Directory.CreateDirectory(databaseDirectory);
        }

        return connectionStringBuilder.ToString();
    }
}
