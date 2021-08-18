using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;

namespace Restful.Common.Cache
{
    /// <summary>
    /// 缓存接口实现
    /// </summary>
    public class MemoryCacheManager : ICacheService
    {
        protected readonly IMemoryCache _cache;

        public MemoryCacheManager(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void Add<V>(string key, V value)
        {
            if (!string.IsNullOrEmpty(key))
            {
                _cache.Set(key, value);
            }
        }

        public void Add<V>(string key, V value, int cacheDurationInSeconds)
        {
            if (!string.IsNullOrEmpty(key))
            {
                _cache.Set(key, value, DateTimeOffset.Now.AddMinutes(cacheDurationInSeconds));
            }
        }

        public bool ContainsKey<V>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            object cache;
            return _cache.TryGetValue(key, out cache);
        }

        public V Get<V>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return default(V);
            }
            if (ContainsKey<V>(key))
            {
                return (V)_cache.Get(key); ;
            }
            return default(V);
        }

        public IEnumerable<string> GetAllKey<V>()
        {
            throw new NotImplementedException();
        }

        public V GetOrCreate<V>(string cacheKey, Func<V> create, int cacheDurationInSeconds = int.MaxValue)
        {
            if (ContainsKey<V>(cacheKey))
            {
                return Get<V>(cacheKey);
            }
            else
            {
                var result = create();
                Add(cacheKey, result, cacheDurationInSeconds);
                return result;
            }
        }

        public void Remove<V>(string key)
        {
            if (!string.IsNullOrWhiteSpace(key) && ContainsKey<V>(key))
            {
                _cache.Remove(key);
            }
        }
    }
}
