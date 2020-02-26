using System;
using Microsoft.Extensions.Caching.Memory;

namespace WeatherAPI.Services
{
    public interface IMemoryCacheService
    {
        void SetCache<T>(string key, T value);
        T GetValueFromCache<T>(string key);
    }
    public class MemoryCacheService : IMemoryCacheService
    {
        private readonly IMemoryCache _memoryCache;

        public MemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public void SetCache<T>(string key, T value)
        {
            _memoryCache.Set(key, value, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1) // expire the stale value after 1 day
            });
        }

        public T GetValueFromCache<T>(string key)
        {
            return _memoryCache.Get<T>(key);
        }
    }
}
