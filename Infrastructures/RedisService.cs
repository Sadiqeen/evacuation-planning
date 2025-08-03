using System.Text.Json;
using EvacuationPlanning.Infrastructures.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace EvacuationPlanning.Infrastructures
{
    public class RedisService : IRedisService
    {
        private readonly IDistributedCache _cache;

        public RedisService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task SetAsync<T>(string key, T value)
        {
            DistributedCacheEntryOptions options = new();
            await _cache.SetStringAsync(key, JsonSerializer.Serialize(value), options);
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var value = await _cache.GetStringAsync(key);

            if (string.IsNullOrEmpty(value))
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(value);
        }
        
        public async Task RemoveAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }
    }
}