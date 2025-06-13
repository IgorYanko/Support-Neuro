using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using NeuroApp.Interfaces;

namespace NeuroApp.Services
{
    public class MemoryCacheService : ICacheService
    {
        private readonly ConcurrentDictionary<string, CacheItem> _cache = new();
        private readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(5);

        private class CacheItem
        {
            public object Value { get; set; }
            public DateTime Expiration { get; set; }
        }

        public Task<T> GetAsync<T>(string key)
        {
            if (_cache.TryGetValue(key, out var cacheItem))
            {
                if (cacheItem.Expiration > DateTime.UtcNow)
                {
                    return Task.FromResult((T)cacheItem.Value);
                }
                else
                {
                    _cache.TryRemove(key, out _);
                }
            }

            return Task.FromResult(default(T));
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var cacheItem = new CacheItem
            {
                Value = value,
                Expiration = DateTime.UtcNow + (expiration ?? _defaultExpiration)
            };

            _cache[key] = cacheItem;
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key)
        {
            _cache.TryRemove(key, out _);
            return Task.CompletedTask;
        }

        public Task ClearAsync()
        {
            _cache.Clear();
            return Task.CompletedTask;
        }
    }
} 