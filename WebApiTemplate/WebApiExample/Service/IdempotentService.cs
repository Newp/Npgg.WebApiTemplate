
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
        MemoryCache cache = new MemoryCache("cache_service");
        
        public void Set<T>(string requestId, T value, TimeService expire)
        {
            var content = JsonConvert.SerializeObject(value);

            
        }
    }
}
