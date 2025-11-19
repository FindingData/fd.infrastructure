using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using StackExchange.Redis;
using System.Text.Json;


namespace fd.infrastructure.core.CacheManager
{
    public class RedisCacheService : ICacheService, IDisposable
    {
        private readonly IDatabase _db;
        private readonly ConnectionMultiplexer _redis;

        public RedisCacheService(IConfiguration configuration)
        {
            var connStr = configuration.GetValue<string>("Redis:ConnectionString");
            _redis = ConnectionMultiplexer.Connect(connStr);
            _db = _redis.GetDatabase();
        }

        public bool Exists(string key)
        {
            return _db.KeyExists(key);
        }

        public bool Add(string key, object value)
        {
            return Add(key, value, TimeSpan.FromHours(1));
        }

        public bool AddObject(string key, object value, int expireSeconds = -1, bool isSliding = false)
        {
            var expiry = expireSeconds > 0 ? TimeSpan.FromSeconds(expireSeconds) : (TimeSpan?)null;
            var json = JsonSerializer.Serialize(value);
            return _db.StringSet(key, json, expiry);
        }

        public bool Add(string key, string value, int expireSeconds = -1, bool isSliding = false)
        {
            var expiry = expireSeconds > 0 ? TimeSpan.FromSeconds(expireSeconds) : (TimeSpan?)null;
            return _db.StringSet(key, value, expiry);
        }

        public void LPush(string key, string val)
        {
            _db.ListLeftPush(key, val);
        }

        public void RPush(string key, string val)
        {
            _db.ListRightPush(key, val);
        }

        public T ListDequeue<T>(string key) where T : class
        {
            var val = _db.ListRightPop(key);
            return val.HasValue ? JsonSerializer.Deserialize<T>(val!) : null;
        }

        public object ListDequeue(string key)
        {
            var val = _db.ListRightPop(key);
            return val.HasValue ? val.ToString() : null;
        }

        public void ListRemove(string key, int keepIndex)
        {
            var len = _db.ListLength(key);
            if (len > keepIndex)
            {
                for (long i = 0; i < len - keepIndex; i++)
                    _db.ListRightPop(key);
            }
        }

        public bool Add(string key, object value, TimeSpan expiresSliding, TimeSpan expiressAbsoulte)
        {
            // Redis 不区分 sliding/absolute，统一处理为 absolute
            return _db.StringSet(key, JsonSerializer.Serialize(value), expiressAbsoulte);
        }

        public bool Add(string key, object value, TimeSpan expiresIn, bool isSliding = false)
        {
            return _db.StringSet(key, JsonSerializer.Serialize(value), expiresIn);
        }

        public bool Remove(string key)
        {
            return _db.KeyDelete(key);
        }

        public void RemoveAll(IEnumerable<string> keys)
        {
            foreach (var key in keys)
            {
                _db.KeyDelete(key);
            }
        }

        public string Get(string key)
        {
            return _db.StringGet(key);
        }

        public T Get<T>(string key) where T : class
        {
            var val = _db.StringGet(key);
            return val.HasValue ? JsonSerializer.Deserialize<T>(val!) : null;
        }

        // --- Caching Operations ---

        public async Task<bool> ExistsAsync(string key)
        {
            return await _db.KeyExistsAsync(key); // 【异步化】
        }

        public Task<bool> AddAsync(string key, object value, TimeSpan? expiry = null)
        {
            // 内部调用 AddObjectAsync，使用 default expireSeconds = -1
            return AddObjectAsync(key, value, expiry.HasValue ? (int)expiry.Value.TotalSeconds : -1);
        }

        public async Task<bool> AddObjectAsync(string key, object value, int expireSeconds = -1)
        {
            var expiry = expireSeconds > 0 ? TimeSpan.FromSeconds(expireSeconds) : (TimeSpan?)null;
            // JsonSerializer.Serialize 是 CPU 密集型，保持同步
            var json = JsonSerializer.Serialize(value);

            // 【异步化】
            return await _db.StringSetAsync(key, json, expiry);
        }

        public async Task<bool> AddAsync(string key, string value, int expireSeconds = -1)
        {
            var expiry = expireSeconds > 0 ? TimeSpan.FromSeconds(expireSeconds) : (TimeSpan?)null;
            // 【异步化】
            return await _db.StringSetAsync(key, value, expiry);
        }

        public async Task<bool> RemoveAsync(string key)
        {
            // 【异步化】
            return await _db.KeyDeleteAsync(key);
        }

        public async Task RemoveAllAsync(IEnumerable<string> keys)
        {
            // StackExchange.Redis 提供了 KeyDelete 批量删除的异步版本
            // 也可以使用 _db.KeyDeleteAsync(keys.Select(k => (RedisKey)k).ToArray());
            var tasks = keys.Select(key => _db.KeyDeleteAsync(key));
            await Task.WhenAll(tasks);
        }

        public async Task<string> GetAsync(string key)
        {
            // 【异步化】
            return await _db.StringGetAsync(key);
        }

        public async Task<T> GetAsync<T>(string key) where T : class
        {
            // 【异步化】
            var val = await _db.StringGetAsync(key);
            // 反序列化是 CPU 密集型，保持同步
            return val.HasValue ? JsonSerializer.Deserialize<T>(val!) : null;
        }

        // --- List/Queue Operations ---

        public async Task LPushAsync(string key, string val)
        {
            // 【异步化】
            await _db.ListLeftPushAsync(key, val);
        }

        public async Task RPushAsync(string key, string val)
        {
            // 【异步化】这是您最需要的更改
            await _db.ListRightPushAsync(key, val);
        }

        public async Task<T> ListDequeueAsync<T>(string key) where T : class
        {
            // 【异步化】
            var val = await _db.ListRightPopAsync(key);
            // 反序列化保持同步
            return val.HasValue ? JsonSerializer.Deserialize<T>(val!) : null;
        }

        public async Task<object> ListDequeueAsync(string key)
        {
            // 【异步化】
            var val = await _db.ListRightPopAsync(key);
            return val.HasValue ? val.ToString() : null;
        }

        public async Task ListRemoveAsync(string key, int keepIndex)
        {
            // ListLengthAsync 是 I/O 操作
            var len = await _db.ListLengthAsync(key);

            if (len > keepIndex)
            {
                // 注意：ListRightPopAsync 应该被多次调用，最好的做法是使用 batch 或 pipeline 
                // 但为了简洁和直接替换，我们使用循环（虽然效率不高，但能工作）
                // 更好的性能：使用 ListTrimAsync 来保持列表长度
                await _db.ListTrimAsync(key, 0, keepIndex - 1);
            }
        }


        public void Dispose()
        {
            if (_redis != null && _redis.IsConnected)
            {
                _redis.Dispose();
            }
            GC.SuppressFinalize(this);
        }
    }
}
