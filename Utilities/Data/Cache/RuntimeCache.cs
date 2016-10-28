using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Caching;

namespace Utilities.Data.Cache
{
    public interface IRuntimeCache
    {
        object Get(string key);

        bool Add(string key, object value);
        
        object Remove(string key);

        object AddOrGetExisting(string key, Func<object> getter);
    }

    public class RuntimeCache : IRuntimeCache
    {
        public MemoryCache Cache { get; set; }

        public RuntimeCache()
        {
            Cache = MemoryCache.Default;
        }

        public object Get(string key)
        {
            return Cache.Get(key);
        }

        public bool Add(string key, object value)
        {
            return Cache.Add(key, value, new CacheItemPolicy()
            {
                SlidingExpiration = TimeSpan.FromMinutes(20)
            });
        }

        public object Remove(string key)
        {
            return Cache.Remove(key);
        }

        public object AddOrGetExisting(string key, Func<object> getter)
        {
            var result = Get(key);

            if (result == null)
            {
                result = getter.Invoke();

                Add(key, result);
            }

            return result;
        }
    }
}
