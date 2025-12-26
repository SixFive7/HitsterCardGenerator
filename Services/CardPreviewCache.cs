using Microsoft.Extensions.Caching.Memory;

namespace HitsterCardGenerator.Services;

/// <summary>
/// In-memory cache for generated card preview images
/// </summary>
public class CardPreviewCache
{
    private readonly IMemoryCache _cache;
    private static readonly MemoryCacheEntryOptions CacheOptions = new MemoryCacheEntryOptions()
        .SetSlidingExpiration(TimeSpan.FromMinutes(10))
        .SetAbsoluteExpiration(TimeSpan.FromHours(1));

    public CardPreviewCache(IMemoryCache cache)
    {
        _cache = cache;
    }

    /// <summary>
    /// Gets a cached image or generates and caches a new one
    /// </summary>
    /// <param name="cacheKey">Unique cache key for the image</param>
    /// <param name="generator">Function to generate the image if not cached</param>
    /// <returns>PNG image bytes</returns>
    public byte[] GetOrCreate(string cacheKey, Func<byte[]> generator)
    {
        return _cache.GetOrCreate(cacheKey, entry =>
        {
            entry.SetOptions(CacheOptions);
            return generator();
        }) ?? generator();
    }

    /// <summary>
    /// Generates a cache key for a front card
    /// </summary>
    public static string FrontCardKey(string trackId, string? backgroundColor)
    {
        return $"card_front_{trackId}_{backgroundColor ?? "default"}";
    }

    /// <summary>
    /// Generates a cache key for a back card
    /// </summary>
    public static string BackCardKey(string trackId, int year, string? backgroundColor)
    {
        return $"card_back_{trackId}_{year}_{backgroundColor ?? "default"}";
    }
}
