using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace fd.infrastructure.core.CacheManager
{
    public class MemoryCacheService : ICacheService
    {
        protected IMemoryCache _cache;

        private readonly ConcurrentDictionary<string, object> _lockDict = new();
        public MemoryCacheService(IMemoryCache cache)
        {
            _cache = cache;

        }
        /// <summary>
        /// 验证缓存项是否存在
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public bool Exists(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            return _cache.Get(key) != null;
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <returns></returns>
        public bool Add(string key, object value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            _cache.Set(key, value);
            return Exists(key);
        }

        public bool AddObject(string key, object value, int expireSeconds = -1, bool isSliding = false)
        {
            if (expireSeconds != -1)
            {
                _cache.Set(key,
                    value,
                    new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(new TimeSpan(0, 0, expireSeconds))
                    );
            }
            else
            {
                _cache.Set(key, value);
            }

            return true;
        }
        public bool Add(string key, string value, int expireSeconds = -1, bool isSliding = false)
        {
            return AddObject(key, value, expireSeconds, isSliding);
        }
        public void LPush(string key, string val)
        {
            var list = GetList(key);
            lock (_lockDict.GetOrAdd(key, _ => new object()))
            {
                list.Insert(0, val); // 插入开头
            }

        }
        public void RPush(string key, string val)
        {
            var list = GetList(key);
            lock (_lockDict.GetOrAdd(key, _ => new object()))
            {
                list.Add(val); // 插入末尾
            }

        }
        public T ListDequeue<T>(string key) where T : class
        {
            var obj = ListDequeue(key);
            if (obj is string str)
            {
                return JsonSerializer.Deserialize<T>(str);
            }
            return null;
        }
        public object ListDequeue(string key)
        {
            var list = GetList(key);
            lock (_lockDict.GetOrAdd(key, _ => new object()))
            {
                if (list.Count == 0) return null;
                var item = list[list.Count - 1]; // 从右边取出
                list.RemoveAt(list.Count - 1);
                return item;
            }
        }
        public void ListRemove(string key, int keepIndex)
        {
            var list = GetList(key);
            lock (_lockDict.GetOrAdd(key, _ => new object()))
            {
                if (keepIndex < list.Count)
                {
                    list.RemoveRange(keepIndex, list.Count - keepIndex);
                }
            }
        }
        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="expiresSliding">滑动过期时长（如果在过期时间内有操作，则以当前时间点延长过期时间）</param>
        /// <param name="expiressAbsoulte">绝对过期时长</param>
        /// <returns></returns>
        public bool Add(string key, object value, TimeSpan expiresSliding, TimeSpan expiressAbsoulte)
        {
            _cache.Set(key, value,
                    new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(expiresSliding)
                    .SetAbsoluteExpiration(expiressAbsoulte)
                    );

            return Exists(key);
        }
        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="expiresIn">缓存时长</param>
        /// <param name="isSliding">是否滑动过期（如果在过期时间内有操作，则以当前时间点延长过期时间）</param>
        /// <returns></returns>
        public bool Add(string key, object value, TimeSpan expiresIn, bool isSliding = false)
        {
            if (isSliding)
                _cache.Set(key, value,
                    new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(expiresIn)
                    );
            else
                _cache.Set(key, value,
                new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(expiresIn)
                );

            return Exists(key);
        }



        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            _cache.Remove(key);

            return !Exists(key);
        }
        /// <summary>
        /// 批量删除缓存
        /// </summary>
        /// <param name="key">缓存Key集合</param>
        /// <returns></returns>
        public void RemoveAll(IEnumerable<string> keys)
        {
            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }

            keys.ToList().ForEach(item => _cache.Remove(item));
        }
        public string Get(string key)
        {
            return _cache.Get(key)?.ToString();
        }
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public T Get<T>(string key) where T : class
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            return _cache.Get(key) as T;
        }

        private List<string> GetList(string key)
        {
            if (!_cache.TryGetValue(key, out List<string> list))
            {
                list = new List<string>();
                _cache.Set(key, list);
            }
            return list;
        }

        public void Dispose()
        {
            if (_cache != null)
                _cache.Dispose();
            GC.SuppressFinalize(this);
        }

        public Task<bool> ExistsAsync(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            // 包装同步 Get 操作的结果
            return Task.FromResult(_cache.Get(key) != null);
        }

        public Task<bool> AddAsync(string key, object value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            _cache.Set(key, value);
            return Task.FromResult(Exists(key));
        }

        public Task<bool> AddObjectAsync(string key, object value, int expireSeconds = -1, bool isSliding = false)
        {
            if (expireSeconds != -1)
            {
                // MemoryCache 总是同步操作
                _cache.Set(key,
                    value,
                    new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(new TimeSpan(0, 0, expireSeconds))
                );
            }
            else
            {
                _cache.Set(key, value);
            }

            return Task.FromResult(true); // 内存缓存添加通常总是成功
        }

        public Task<bool> AddAsync(string key, string value, int expireSeconds = -1, bool isSliding = false)
        {
            return AddObjectAsync(key, value, expireSeconds, isSliding);
        }

        public Task<bool> AddAsync(string key, object value, TimeSpan expiresSliding, TimeSpan expiressAbsoulte)
        {
            _cache.Set(key, value,
                new MemoryCacheEntryOptions()
                .SetSlidingExpiration(expiresSliding)
                .SetAbsoluteExpiration(expiressAbsoulte)
            );

            return Task.FromResult(Exists(key));
        }

        public Task<bool> AddAsync(string key, object value, TimeSpan expiresIn, bool isSliding = false)
        {
            if (isSliding)
                _cache.Set(key, value,
                    new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(expiresIn)
                );
            else
                _cache.Set(key, value,
                new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(expiresIn)
                );

            return Task.FromResult(Exists(key));
        }


        // --- List/Queue Operations (使用 Task.CompletedTask 和 Task.FromResult) ---

        // 内部辅助方法，获取或创建 List<string>。此处必须保证线程安全

        public Task LPushAsync(string key, string val)
        {
            var list = GetList(key);
            // 内存操作，使用同步锁保证线程安全
            lock (_lockDict.GetOrAdd(key, _ => new object()))
            {
                list.Insert(0, val); // 插入开头
            }
            return Task.CompletedTask; // 返回已完成的 Task
        }

        public Task RPushAsync(string key, string val)
        {
            var list = GetList(key);
            lock (_lockDict.GetOrAdd(key, _ => new object()))
            {
                list.Add(val); // 插入末尾
            }
            return Task.CompletedTask; // 返回已完成的 Task
        }

        public Task<T> ListDequeueAsync<T>(string key) where T : class
        {
            var obj = ListDequeueAsync(key).GetAwaiter().GetResult(); // 同步调用 ListDequeueAsync

            if (obj is string str)
            {
                // 反序列化是 CPU 密集型
                return Task.FromResult(JsonSerializer.Deserialize<T>(str));
            }
            return Task.FromResult<T>(null);
        }

        public Task<object> ListDequeueAsync(string key)
        {
            var list = GetList(key);
            object item = null;

            lock (_lockDict.GetOrAdd(key, _ => new object()))
            {
                if (list.Count != 0)
                {
                    item = list[^1]; // 从右边取出
                    list.RemoveAt(list.Count - 1);
                }
            }
            return Task.FromResult(item);
        }

        public Task ListRemoveAsync(string key, int keepIndex)
        {
            var list = GetList(key);
            lock (_lockDict.GetOrAdd(key, _ => new object()))
            {
                if (keepIndex < list.Count)
                {
                    list.RemoveRange(keepIndex, list.Count - keepIndex);
                }
            }
            return Task.CompletedTask;
        }


        // --- Get/Remove 操作 ---

        public Task<bool> RemoveAsync(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            _cache.Remove(key);
            return Task.FromResult(!Exists(key));
        }

        public Task RemoveAllAsync(IEnumerable<string> keys)
        {
            if (keys == null)
                throw new ArgumentNullException(nameof(keys));

            keys.ToList().ForEach(item => _cache.Remove(item));
            return Task.CompletedTask;
        }

        public Task<string> GetAsync(string key)
        {
            // Get 方法返回 object，我们强制转换为 string
            return Task.FromResult(_cache.Get(key)?.ToString());
        }

        public Task<T> GetAsync<T>(string key) where T : class
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            // 返回的 T 需要进行类型转换
            return Task.FromResult(_cache.Get(key) as T);
        }

        public Task<bool> AddAsync(string key, object value, TimeSpan? expiry = null)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (expiry.HasValue)
            {
                // 设置绝对过期时间
                _cache.Set(key, value, new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(expiry.Value));
            }
            else
            {
                // 永不过期
                _cache.Set(key, value);
            }

            // 内存操作是同步的，使用 Task.FromResult 包装结果
            return Task.FromResult(Exists(key));
        }

        // 2. 实现 Task<bool> AddObjectAsync(string key, object value, int expireSeconds = -1)
        public Task<bool> AddObjectAsync(string key, object value, int expireSeconds = -1)
        {
            if (key == null || value == null)
                throw new ArgumentNullException(key == null ? nameof(key) : nameof(value));

            if (expireSeconds != -1)
            {
                // 注意：MemoryCache 默认不提供单独的滑动/绝对过期配置
                // 此处我们将其视为滑动过期，以遵循你原始代码的逻辑（你原代码中使用的是 SetSlidingExpiration）
                _cache.Set(key,
                    value,
                    new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(new TimeSpan(0, 0, expireSeconds))
                );
            }
            else
            {
                // 永不过期
                _cache.Set(key, value);
            }

            return Task.FromResult(true); // 内存缓存添加通常总是成功
        }

        // 3. 实现 Task<bool> AddAsync(string key, string value, int expireSeconds = -1)
        public Task<bool> AddAsync(string key, string value, int expireSeconds = -1)
        {
            // 直接调用上面的 AddObjectAsync，因为 string 也是 object
            return AddObjectAsync(key, value, expireSeconds);
        }
    }
}
