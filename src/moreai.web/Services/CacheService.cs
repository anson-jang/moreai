using Microsoft.Extensions.Caching.Memory;
using System;

namespace moreai.web.Services
{
    public interface ICacheService
    {
        T Get<T>(string key);
        bool Set<T>(string key, T value, TimeSpan? expiration = null);
        void Remove(string key);
    }

    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(30);

        public MemoryCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public T Get<T>(string key)
        {
            return _cache.TryGetValue(key, out T value) ? value : default;
        }

        public bool Set<T>(string key, T value, TimeSpan? expiration = null)
        {
            var cacheExpiration = expiration ?? _defaultExpiration;
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(cacheExpiration)
                .SetAbsoluteExpiration(TimeSpan.FromHours(24));

            _cache.Set(key, value, cacheEntryOptions);
            return true;
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }
    }
}