using System;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;

public static class UniqueIdGenerator
{
    private static readonly Random _random = new Random();
    private static readonly object _lock = new object();
    private static IMemoryCache _cache;

    public static void Configure(IMemoryCache memoryCache)
    {
        _cache = memoryCache;
    }

    public static int GenerateUniqueId(ProductDbContext dbContext)
    {
        lock (_lock)
        {
            int newId;
            do
            {
                newId = _random.Next(100000, 999999); // Generate a 6-digit number
            }
            while (_cache.TryGetValue(newId, out _) || dbContext.Products.Any(p => p.Id == newId)); // Check cache & database

            // Store in cache to prevent immediate duplicate generation
            _cache.Set(newId, true, TimeSpan.FromMinutes(10));

            return newId;
        }
    }
}
