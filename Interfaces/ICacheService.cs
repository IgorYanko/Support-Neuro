using System;
using System.Threading.Tasks;

namespace NeuroApp.Interfaces
{
    public interface ICacheService
    {
        Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null);
        Task RemoveAsync(string key);
        Task ClearAsync();
    }
} 