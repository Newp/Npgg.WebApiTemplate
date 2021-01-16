using System;
using System.Collections.Concurrent;
using System.Runtime.Caching;

namespace WebApiExample.Service
{
    /*
     * WIP
     idempotence는 작업중- 작업완료에 대한 state를 가져야한다.
    작업중에 multiple identical request가 요청될 경우

    1. 작업이 아직 완료되지 않은상태라면? 응답대기를 하거나 status code를 수정하여 client에서 중복된 요청이 진행중임을 알려 처리하도록 돕는다.
    2. 작업이 이미 완료된 api 라면? 지금 캐싱된 데이터를 서버에서 직접 내려 처리한다.

    일반적으로 redis 의 increase를 사용한다면 쉽게 구현이 가능하다.
    request가 왔을때 increase 했을때 1을 받은 request만이 정상적인 api를 호출한다.
     */


    public class IdempotentService
    {
        readonly MemoryCache cache = new MemoryCache("cache_service");
        readonly ConcurrentDictionary<string, int> requestIdList = new ConcurrentDictionary<string, int>();
        readonly CacheItemPolicy policy = new CacheItemPolicy
        {
            SlidingExpiration = TimeSpan.FromSeconds(100), //100초까지만 기다린다.
        };

        public bool TryPreoccupy(string requestId) => requestIdList.TryAdd(requestId, 1);

        public void ReleaseAcquire(string requestId)
        {
            //WIP: requestIdList에서 제거해야한다.
        }

        public void Set(string requestId, object value)
        {
            //var content = JsonConvert.SerializeObject(value);
            
            cache.Set(requestId, value, policy);
        }

        public object Get(string requestId) => cache.Get(requestId);
    }
}
