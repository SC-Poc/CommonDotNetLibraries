using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Lykke.Common.Cache
{
    public class OnDemandDataCache<T>
        where T : class 
    {
        private readonly IMemoryCache _innerCache;
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks;

        public OnDemandDataCache()
            : this(new MemoryCacheOptions())
        {
            
        }

        public OnDemandDataCache(IOptions<MemoryCacheOptions> optionsAccessor)
        {
            _innerCache = new MemoryCache(optionsAccessor);
            _locks = new ConcurrentDictionary<string, SemaphoreSlim>();
        }

        public OnDemandDataCache(IMemoryCache memoryCache)
        {
            _innerCache = memoryCache;
        }


        public T GetOrAdd(string key, Func<string, T> factory)
        {
            var options = new MemoryCacheEntryOptions();

            return GetOrAdd(key, factory, options);
        }

        public T GetOrAdd(string key, Func<string, T> factory, DateTimeOffset absoluteExpiration)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = absoluteExpiration
            };

            return GetOrAdd(key, factory, options);
        }

        public T GetOrAdd(string key, Func<string, T> factory, TimeSpan absoluteExpirationRelativeToNow)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow
            };

            return GetOrAdd(key, factory, options);
        }

        public T GetOrAdd(string key, Func<string, T> factory, MemoryCacheEntryOptions options)
        {
            var value = _innerCache.Get<T>(key);

            if (value != null)
            {
                return value;
            }
            
            var @lock = GetOrAddLock(key);

            @lock.Wait();

            try
            {
                value = _innerCache.Get<T>(key);

                if (value != null)
                {
                    return value;
                }

                value = factory(key);

                _innerCache.Set(key, value, options);

                return value;
            }
            finally
            {
                @lock.Release();
            }
        }

        public Task<T> GetOrAddAsync(string key, Func<string, Task<T>> factory)
        {
            var options = new MemoryCacheEntryOptions();

            return GetOrAddAsync(key, factory, options);
        }

        public Task<T> GetOrAddAsync(string key, Func<string, Task<T>> factory, DateTimeOffset absoluteExpiration)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = absoluteExpiration
            };

            return GetOrAddAsync(key, factory, options);
        }

        public Task<T> GetOrAddAsync(string key, Func<string, Task<T>> factory, TimeSpan absoluteExpirationRelativeToNow)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow
            };

            return GetOrAddAsync(key, factory, options);
        }

        public async Task<T> GetOrAddAsync(string key, Func<string, Task<T>> factory, MemoryCacheEntryOptions options)
        {
            var value = _innerCache.Get<T>(key);

            if (value != null)
            {
                return value;
            }

            var @lock = GetOrAddLock(key);

            await @lock.WaitAsync();

            try
            {
                value = _innerCache.Get<T>(key);

                if (value != null)
                {
                    return value;
                }

                value = await factory(key);

                _innerCache.Set(key, value, options);

                return value;
            }
            finally
            {
                @lock.Release();
            }
        }

        public void Remove(string key)
        {
            _innerCache.Remove(key);
        }

        public void Set(string key, T value)
        {
            var options = new MemoryCacheEntryOptions();

            Set(key, value, options);
        }

        public void Set(string key, T value, DateTimeOffset absoluteExpiration)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = absoluteExpiration
            };

            Set(key, value, options);
        }

        public void Set(string key, T value, TimeSpan absoluteExpirationRelativeToNow)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow
            };

            Set(key, value, options);
        }
        
        public void Set(string key, T value, MemoryCacheEntryOptions options)
        {
            _innerCache.Set(key, value, options);
        }

        public bool TryGet(string key, out T value)
        {
            value = _innerCache.Get<T>(key);

            return value != null;
        }

        private SemaphoreSlim GetOrAddLock(string key)
        {
            return _locks.GetOrAdd
            (
                key.CalculateHexHash32(3),
                x => new SemaphoreSlim(1)
            );
        }
    }
}
