using System;
using System.Collections.Concurrent;
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

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
        {
            if (_cache.TryGetValue(key, out var item))
            {
                if (DateTime.Now < item.Expiration)
                {
                    return (T)item.Value;
                }
            }

            var value = await factory();
            var expirationTime = DateTime.Now.Add(expiration ?? _defaultExpiration);

            _cache.AddOrUpdate(
                key,
                new CacheItem { Value = value, Expiration = expirationTime },
                (_, __) => new CacheItem { Value = value, Expiration = expirationTime }
            );

            return value;
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