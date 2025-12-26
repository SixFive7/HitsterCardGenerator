namespace HitsterCardGenerator.Data;

using LiteDB;
using Microsoft.Extensions.Options;

public class LiteDbContext : ILiteDbContext, IDisposable
{
    public LiteDatabase Database { get; }
    private bool _disposed;

    public LiteDbContext(IOptions<LiteDbOptions> options)
    {
        var connectionString = options.Value.ConnectionString;

        // Extract filename and ensure directory exists
        var filename = ExtractFilename(connectionString);
        var directory = Path.GetDirectoryName(filename);

        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        Database = new LiteDatabase(connectionString);
    }

    private static string ExtractFilename(string connectionString)
    {
        // Handle both "path.db" and "Filename=path.db;..." formats
        if (!connectionString.Contains('='))
            return connectionString;

        var parts = connectionString.Split(';');
        var filenamePart = parts.FirstOrDefault(p =>
            p.TrimStart().StartsWith("Filename=", StringComparison.OrdinalIgnoreCase));
        return filenamePart?.Split('=', 2)[1].Trim() ?? connectionString;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            Database?.Dispose();
            _disposed = true;
        }
    }
}
