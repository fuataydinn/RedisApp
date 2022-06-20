using RedisApplication.IServices;
using StackExchange.Redis;
using System.Text.Json;

namespace RedisApplication.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly IConnectionMultiplexer _redisCon;

        private readonly IDatabase _cache;
        private TimeSpan ExpireTime=>TimeSpan.FromDays(1);

        public RedisCacheService(IConnectionMultiplexer redisCon)
        {
            _redisCon = redisCon;
            _cache = redisCon.GetDatabase();
        }

        public async Task Clear(string key)
        {
            await _cache.KeyDeleteAsync(key);
        }

        public void ClearAll()
        {
           var enpoints=_redisCon.GetEndPoints(true);
            foreach(var enpoint in enpoints)
            {
                var server=_redisCon.GetServer(enpoint);
                server.FlushAllDatabases();
            }
        }

        public async Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> action) where T : class
        {
            var result = await _cache.StringGetAsync(key);
            if (result.IsNull)
            {
                result = JsonSerializer.SerializeToUtf8Bytes(await action());  
                await SetValueAsync(key,result); 
            }
            return JsonSerializer.Deserialize<T>(result);
        }
        public async Task<string> GetValueAsync(string key)
        {
            return await _cache.StringGetAsync(key);
        }

        public async Task<bool> SetValueAsync(string key, string value)
        {
          return await _cache.StringSetAsync(key, value,ExpireTime);
        }

        public T GetOrAdd<T>(string key, Func<T> action) where T : class
        {
            var result=_cache.StringGet(key);
            if (result.IsNull)
            {
                result = JsonSerializer.SerializeToUtf8Bytes(action());
                _cache.StringSet(key, result,ExpireTime);
            }
            return JsonSerializer.Deserialize<T>(result);

            //Bu servisde en çok kullanacağımız GetorAdd() fonksiyonu ana işlemlerimizi nerdeyse tek işlemde yapıyor.
            //Ayrıca 1 gün olarak da expire süresi de belirledik bu süre bitince key silinecek.
        }

    }
}
