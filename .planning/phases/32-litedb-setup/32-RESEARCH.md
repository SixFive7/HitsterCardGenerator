# Phase 32: LiteDB Setup - Research Findings

## Overview

This document captures research findings for implementing LiteDB as the persistence layer in an ASP.NET Core web application, focusing on singleton patterns, dependency injection, document references, and common pitfalls.

---

## 1. Dependency Injection Setup

### Recommended Pattern: DbContext Wrapper

The recommended approach is to create a context class that wraps `LiteDatabase` and register it as a singleton:

```csharp
// Interface
public interface ILiteDbContext
{
    LiteDatabase Database { get; }
}

// Implementation
public class LiteDbContext : ILiteDbContext
{
    public LiteDatabase Database { get; }

    public LiteDbContext(IOptions<LiteDbOptions> options)
    {
        Database = new LiteDatabase(options.Value.ConnectionString);
    }
}

// Configuration
public class LiteDbOptions
{
    public string ConnectionString { get; set; } = string.Empty;
}
```

### Service Registration

```csharp
// In Program.cs
builder.Services.Configure<LiteDbOptions>(
    builder.Configuration.GetSection("LiteDbOptions"));
builder.Services.AddSingleton<ILiteDbContext, LiteDbContext>();
```

### Why Singleton?

Per LiteDB creator Mauricio David: *"If your application works in a single process (like mobile apps, asp.net websites) prefer to use a single database instance and share across all threads."*

- LiteDB is thread-safe internally (uses `ReaderWriterLockSlim`)
- Single instance avoids file locking conflicts
- Better performance than opening/closing for each request

---

## 2. Connection String Configuration

### Full Connection String Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Filename` | string | (required) | Full/relative path. Supports `:memory:` or `:temp:` |
| `Connection` | string | `direct` | `direct` (exclusive) or `shared` (multi-process) |
| `Password` | string | null | AES encryption password |
| `InitialSize` | string/long | 0 | Initial file size (supports KB, MB, GB) |
| `ReadOnly` | bool | false | Open in read-only mode |
| `Upgrade` | bool | false | Auto-upgrade older database versions |

### Connection Modes

**Direct Mode (Recommended for web apps)**
- Engine maintains exclusive file access until `Dispose()`
- Faster and cacheable
- Cannot be opened by other processes

**Shared Mode**
- Engine closes file after each operation using mutex locks
- Enables multi-process access (useful for app pool recycling)
- Higher overhead

### Recommended Connection String for This Project

```
Filename=./data/hitster.db;Connection=direct
```

Use `Connection=shared` only if app pool recycling is a concern (IIS deployments).

---

## 3. Document References

### Two Approaches

#### Option A: Store IDs (Manual References)
```csharp
public class Playlist
{
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public List<ObjectId> TrackIds { get; set; } = new();  // Manual refs
}
```
- More explicit control
- Requires manual joins/lookups
- Better for our use case (track reuse across playlists)

#### Option B: DbRef with [BsonRef] Attribute
```csharp
public class Playlist
{
    public ObjectId Id { get; set; }
    public string Name { get; set; }

    [BsonRef("tracks")]
    public List<Track> Tracks { get; set; } = new();  // Auto-resolved
}
```
- Requires `.Include(x => x.Tracks)` on queries
- Each include = additional disk read
- Can cause circular reference issues

### Recommendation

Use **manual ObjectId references** (Option A) for `Playlist.TrackIds`:
- Simpler to reason about
- Better performance for large track lists
- Avoids accidental circular references
- Tracks can be shared across playlists easily

---

## 4. Repository Pattern

### Pattern Structure

```csharp
public interface IPlaylistRepository
{
    Task<Playlist?> GetByIdAsync(ObjectId id);
    Task<IEnumerable<Playlist>> GetByBrowserIdAsync(string browserId);
    Task<ObjectId> CreateAsync(Playlist playlist);
    Task UpdateAsync(Playlist playlist);
    Task DeleteAsync(ObjectId id);
}

public class PlaylistRepository : IPlaylistRepository
{
    private readonly ILiteDbContext _context;

    public PlaylistRepository(ILiteDbContext context)
    {
        _context = context;
    }

    private ILiteCollection<Playlist> Collection =>
        _context.Database.GetCollection<Playlist>("playlists");

    public Task<Playlist?> GetByIdAsync(ObjectId id)
    {
        return Task.FromResult(Collection.FindById(id));
    }

    // ... other methods
}
```

### Note on Async

LiteDB is synchronous. Async methods should wrap calls in `Task.FromResult()` or `Task.Run()` for API consistency, though this adds minor overhead.

---

## 5. Directory Creation

LiteDB auto-creates the database file but NOT parent directories.

```csharp
public class LiteDbContext : ILiteDbContext
{
    public LiteDatabase Database { get; }

    public LiteDbContext(IOptions<LiteDbOptions> options)
    {
        var path = options.Value.ConnectionString;

        // Extract filename from connection string
        var filename = ExtractFilename(path);
        var directory = Path.GetDirectoryName(filename);

        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        Database = new LiteDatabase(path);
    }

    private static string ExtractFilename(string connectionString)
    {
        // Handle both "path.db" and "Filename=path.db;..." formats
        if (!connectionString.Contains('='))
            return connectionString;

        var parts = connectionString.Split(';');
        var filenamePart = parts.FirstOrDefault(p =>
            p.TrimStart().StartsWith("Filename=", StringComparison.OrdinalIgnoreCase));
        return filenamePart?.Split('=')[1].Trim() ?? connectionString;
    }
}
```

---

## 6. Disposal Pattern

### ASP.NET Core DI Handles Disposal

When registered as singleton, ASP.NET Core's DI container automatically disposes `IDisposable` services when the host shuts down.

```csharp
public class LiteDbContext : ILiteDbContext, IDisposable
{
    public LiteDatabase Database { get; }
    private bool _disposed;

    public LiteDbContext(IOptions<LiteDbOptions> options)
    {
        // ... initialization
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
```

The DI container will call `Dispose()` when the application shuts down.

---

## 7. Common Pitfalls

### Pitfall 1: Multiple LiteDatabase Instances
**Problem:** Creating multiple `LiteDatabase` instances for the same file causes locking conflicts.
**Solution:** Always use singleton pattern with shared instance.

### Pitfall 2: SynchronizationLockException
**Problem:** "The read lock is being released without being held" error.
**Cause:** Usually from improper instance management or concurrent access issues.
**Solution:** Use single singleton instance, ensure thread-safe access.

### Pitfall 3: App Pool Recycling (IIS)
**Problem:** During IIS app pool recycle, old and new processes may conflict.
**Solution:** Use `Connection=shared` in connection string for IIS deployments. For Docker/Kestrel, `direct` is fine.

### Pitfall 4: Missing Data Directory
**Problem:** LiteDB throws if parent directory doesn't exist.
**Solution:** Create directory in context constructor before opening database.

### Pitfall 5: Not Disposing on Shutdown
**Problem:** Data may not be fully flushed to disk.
**Solution:** Let DI container handle disposal, or ensure clean shutdown.

### Pitfall 6: Async Confusion
**Problem:** LiteDB is synchronous, wrapping in `Task.Run()` adds thread pool overhead.
**Solution:** Use sync methods directly, or accept minor overhead for API consistency.

---

## 8. Proposed Implementation Structure

```
Models/
  Playlist.cs          # Playlist entity with ObjectId
  Track.cs             # Track entity with ObjectId (may extend Song)

Data/
  ILiteDbContext.cs    # Interface for database context
  LiteDbContext.cs     # Singleton wrapper for LiteDatabase
  LiteDbOptions.cs     # Configuration class

Repositories/
  IPlaylistRepository.cs
  PlaylistRepository.cs
  ITrackRepository.cs
  TrackRepository.cs
```

---

## 9. Configuration Example

### appsettings.json
```json
{
  "LiteDbOptions": {
    "ConnectionString": "Filename=./data/hitster.db;Connection=direct"
  }
}
```

### For Docker deployment
Mount volume at `/app/data` to persist database across container restarts.

---

## 10. Sources

- [Using LiteDB in an ASP.NET Core API - George Kosmidis](https://blog.georgekosmidis.net/using-litedb-in-an-asp-net-core-api.html)
- [LiteDB Connection String Documentation](https://www.litedb.org/docs/connection-string/)
- [LiteDB Repository Pattern Wiki](https://github.com/litedb-org/LiteDB/wiki/Repository-Pattern)
- [Injecting LiteDb as a Service in ASP.NET Core - Codehaks](https://codehaks.github.io/2018/10/01/injecting-litedb-as-a-service-in-asp.net-core.html/)
- [ASP.NET Core DI Thread Safety Issue #873](https://github.com/litedb-org/LiteDB/issues/873)
- [App Pool Recycling Issue #950](https://github.com/litedb-org/LiteDB/issues/950)
- [Four Ways to Dispose IDisposables in ASP.NET Core - Andrew Lock](https://andrewlock.net/four-ways-to-dispose-idisposables-in-asp-net-core/)
- [Microsoft Learn - Dependency Injection in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-10.0)

---

## Key Decisions for Phase 32

1. **Singleton Pattern**: Use `ILiteDbContext` wrapper registered as singleton
2. **Connection Mode**: Use `Connection=direct` for Docker/Kestrel deployment
3. **Document References**: Store `List<ObjectId>` for TrackIds (manual references)
4. **Directory Creation**: Create `./data/` directory in context constructor
5. **Configuration**: Use `IOptions<LiteDbOptions>` pattern for connection string
6. **Disposal**: Implement `IDisposable`, let DI container manage lifecycle
