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
