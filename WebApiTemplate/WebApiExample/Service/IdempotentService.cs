
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebApiExample.Service
{
    public class IdempotentService
    {
        readonly MemoryCache cache = new MemoryCache("cache_service");
        readonly CacheItemPolicy policy = new CacheItemPolicy
        {
            SlidingExpiration = TimeSpan.FromSeconds(100), //100초까지만 기다린다.
        };

        public void Set(string requestId, object value)
        {
            //var content = JsonConvert.SerializeObject(value);

            cache.Set(requestId, value, policy);
        }

        public object Get(string requestId) => cache.Get(requestId);
    }
}
