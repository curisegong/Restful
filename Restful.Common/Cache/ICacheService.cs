using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restful.Common.Cache
{
    public interface ICacheService
    {
        void Add<V>(string key, V value);

        void Add<V>(string key, V value, int cacheDurationInSeconds);

        bool ContainsKey<V>(string key);

        V Get<V>(string key);

        IEnumerable<string> GetAllKey<V>();

        void Remove<V>(string key);

        V GetOrCreate<V>(string cacheKey, Func<V> create, int cacheDurationInSeconds = int.MaxValue);
    }
}
