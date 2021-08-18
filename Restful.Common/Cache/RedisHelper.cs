using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSRedis;

namespace Restful.Common.Cache
{
    public class Redis
    {
        /// <summary>
        /// Redis操作
        /// </summary>
        public static CSRedisClient Current { get; private set; }

        public static void Initialization()
        {
            Current = new CSRedisClient(ConfigurationUtil.RedisConnectionString);
            RedisHelper.Initialization(Current);
        }

        /// <summary>
        /// 删除匹配的缓存
        /// </summary>
        /// <param name="pattern">通配符</param>
        public static long DelPattern(string pattern)
        {
            string[] keys = RedisHelper.Keys($"*{pattern}*");
            //keys = keys.Select(k => k.Replace("lib_", "")).ToArray();
            return RedisHelper.Del(keys);
        }
    }
}
