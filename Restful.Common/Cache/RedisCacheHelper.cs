using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restful.Common.Cache
{
    public class RedisCacheHelper : ICacheService
    {
        public void Add<V>(string key, V value)
        {
            Redis.Current.Set(key, value);
        }

        public void Add<V>(string key, V value, int cacheDurationInSeconds)
        {
            Redis.Current.Set(key, value, cacheDurationInSeconds);
        }

        public bool ContainsKey<V>(string key)
        {
            return Redis.Current.Exists(key);
        }

        public V Get<V>(string key)
        {
            return Redis.Current.Get<V>(key);
        }

        public IEnumerable<string> GetAllKey<V>()
        {
            return Redis.Current.Keys("*");
        }

        public V GetOrCreate<V>(string cacheKey, Func<V> create, int cacheDurationInSeconds = int.MaxValue)
        {
            //if (ContainsKey<V>(cacheKey))
            //{
            //    return Get<V>(cacheKey);
            //}
            //else
            //{
            var result = create();
            //Add(cacheKey, result, cacheDurationInSeconds);
            return result;
            //  }
        }

        public void Remove<V>(string key)
        {
            Redis.Current.Del(key);
        }

    }
}