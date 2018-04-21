using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Common;
using JetBrains.Annotations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Lykke.Common.Cache
{
    /// <summary>
    ///    Cache using implementation of <see cref="T:Microsoft.Extensions.Caching.Memory.IMemoryCache" /> to store its entries.
    ///    In contrast to IMemoryCache and ConcurrentDictionary this cache implementation prevents multiple calls of data entry factory.
    /// </summary>
    public class OnDemandDataCache<T>
        where T : class 
    {
        private readonly IMemoryCache _innerCache;
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks;
        
        /// <inheritdoc />
        public OnDemandDataCache()
            : this(new MemoryCacheOptions())
        {
            
        }

        /// <inheritdoc />
        public OnDemandDataCache(IOptions<MemoryCacheOptions> optionsAccessor)
            : this(new MemoryCache(optionsAccessor))
        {

        }

        /// <summary>
        ///    Creates a new OnDemandDataCache instance.
        /// </summary>
        /// <param name="memoryCache">
        ///    Underlying memory cache.
        /// </param>
        public OnDemandDataCache(IMemoryCache memoryCache)
        {
            // ReSharper disable once JoinNullCheckWithUsage
            if (memoryCache == null)
            {
                throw new ArgumentNullException(nameof(memoryCache));
            }

            _innerCache = memoryCache;
            _locks = new ConcurrentDictionary<string, SemaphoreSlim>();
        }

        /// <summary>
        ///    Gets the item associated with specified key if present.
        /// </summary>
        /// <param name="key">
        ///    A string identifying the requested entry.
        /// </param>
        /// <returns>
        ///    Cached item, null otherwise.
        /// </returns>
        [CanBeNull]
        public T Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            return _innerCache.Get<T>(key);
        }

        /// <summary>
        ///    Gets the item associated with specified key if present, creates it otherwise.
        /// </summary>
        /// <param name="key">
        ///    A string identifying the requested entry.
        /// </param>
        /// <param name="factory">
        ///    New item factory
        /// </param>
        /// <returns>
        ///    Cached item, or created one.
        /// </returns>
        public T GetOrAdd(string key, Func<string, T> factory)
        {
            var options = new MemoryCacheEntryOptions();

            return GetOrAdd(key, factory, options);
        }

        /// <summary>
        ///    Gets the item associated with specified key if present, creates it otherwise.
        /// </summary>
        /// <param name="key">
        ///    A string identifying the requested entry.
        /// </param>
        /// <param name="factory">
        ///    New item factory
        /// </param>
        /// <param name="absoluteExpiration">
        ///    Absolute expiration date for the cache entry.
        /// </param>
        /// <returns>
        ///    Cached item, or created one.
        /// </returns>
        public T GetOrAdd(string key, Func<string, T> factory, DateTimeOffset absoluteExpiration)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = absoluteExpiration
            };

            return GetOrAdd(key, factory, options);
        }

        /// <summary>
        ///    Gets the item associated with specified key if present, creates it otherwise.
        /// </summary>
        /// <param name="key">
        ///    A string identifying the requested entry.
        /// </param>
        /// <param name="factory">
        ///    New item factory
        /// </param>
        /// <param name="absoluteExpirationRelativeToNow">
        ///    Absolute expiration time, relative to now.
        /// </param>
        /// <returns>
        ///    Cached item, or created one.
        /// </returns>
        public T GetOrAdd(string key, Func<string, T> factory, TimeSpan absoluteExpirationRelativeToNow)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow
            };

            return GetOrAdd(key, factory, options);
        }

        /// <summary>
        ///    Gets the item associated with specified key if present, creates it otherwise.
        /// </summary>
        /// <param name="key">
        ///    A string identifying the requested entry.
        /// </param>
        /// <param name="factory">
        ///    New item factory
        /// </param>
        /// <param name="options">
        ///    Options for new entry.
        /// </param>
        /// <returns>
        ///    Cached item, or created one.
        /// </returns>
        public T GetOrAdd(string key, Func<string, T> factory, MemoryCacheEntryOptions options)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

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

        /// <summary>
        ///    Gets the item associated with specified key if present, creates it otherwise.
        /// </summary>
        /// <param name="key">
        ///    A string identifying the requested entry.
        /// </param>
        /// <param name="factory">
        ///    New item factory
        /// </param>
        /// <returns>
        ///    Cached item, or created one.
        /// </returns>
        public Task<T> GetOrAddAsync(string key, Func<string, Task<T>> factory)
        {
            var options = new MemoryCacheEntryOptions();

            return GetOrAddAsync(key, factory, options);
        }

        /// <summary>
        ///    Gets the item associated with specified key if present, creates it otherwise.
        /// </summary>
        /// <param name="key">
        ///    A string identifying the requested entry.
        /// </param>
        /// <param name="factory">
        ///    New item factory
        /// </param>
        /// <param name="absoluteExpiration">
        ///    Absolute expiration date for the cache entry.
        /// </param>
        /// <returns>
        ///    Cached item, or created one.
        /// </returns>
        public Task<T> GetOrAddAsync(string key, Func<string, Task<T>> factory, DateTimeOffset absoluteExpiration)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = absoluteExpiration
            };

            return GetOrAddAsync(key, factory, options);
        }

        /// <summary>
        ///    Gets the item associated with specified key if present, creates it otherwise.
        /// </summary>
        /// <param name="key">
        ///    A string identifying the requested entry.
        /// </param>
        /// <param name="factory">
        ///    New item factory
        /// </param>
        /// <param name="absoluteExpirationRelativeToNow">
        ///    Absolute expiration time, relative to now.
        /// </param>
        /// <returns>
        ///    Cached item, or created one.
        /// </returns>
        public Task<T> GetOrAddAsync(string key, Func<string, Task<T>> factory, TimeSpan absoluteExpirationRelativeToNow)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow
            };

            return GetOrAddAsync(key, factory, options);
        }

        /// <summary>
        ///    Gets the item associated with specified key if present, creates it otherwise.
        /// </summary>
        /// <param name="key">
        ///    A string identifying the requested entry.
        /// </param>
        /// <param name="factory">
        ///    New item factory
        /// </param>
        /// <param name="options">
        ///    Options for new entry.
        /// </param>
        /// <returns>
        ///    Cached item, or created one.
        /// </returns>
        public async Task<T> GetOrAddAsync(string key, Func<string, Task<T>> factory, MemoryCacheEntryOptions options)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

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

        /// <summary>
        ///    Removes the item, associated with the given key.
        /// </summary>
        /// <param name="key">
        ///    A string identifying the entry.
        /// </param>
        public void Remove(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            _innerCache.Remove(key);
        }

        /// <summary>
        ///    Sets the item associated with specified key.
        /// </summary>
        /// <param name="key">
        ///    A string identifying the requested entry.
        /// </param>
        /// <param name="value">
        ///    Entry value
        /// </param>
        public void Set(string key, T value)
        {
            var options = new MemoryCacheEntryOptions();

            Set(key, value, options);
        }

        /// <summary>
        ///    Sets the item associated with specified key.
        /// </summary>
        /// <param name="key">
        ///    A string identifying the requested entry.
        /// </param>
        /// <param name="value">
        ///    Entry value
        /// </param>
        /// <param name="absoluteExpiration">
        ///    Absolute expiration date for the cache entry.
        /// </param>
        public void Set(string key, T value, DateTimeOffset absoluteExpiration)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = absoluteExpiration
            };

            Set(key, value, options);
        }

        /// <summary>
        ///    Sets the item associated with specified key.
        /// </summary>
        /// <param name="key">
        ///    A string identifying the requested entry.
        /// </param>
        /// <param name="value">
        ///    Entry value
        /// </param>
        /// <param name="absoluteExpirationRelativeToNow">
        ///    Absolute expiration time, relative to now.
        /// </param>
        public void Set(string key, T value, TimeSpan absoluteExpirationRelativeToNow)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow
            };

            Set(key, value, options);
        }

        /// <summary>
        ///    Sets the item associated with specified key.
        /// </summary>
        /// <param name="key">
        ///    A string identifying the requested entry.
        /// </param>
        /// <param name="value">
        ///    Entry value
        /// </param>
        /// <param name="options">
        ///    Options for new entry.
        /// </param>
        public void Set(string key, T value, MemoryCacheEntryOptions options)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _innerCache.Set(key, value, options);
        }

        /// <summary>
        ///    Gets the item associated with this key if present.
        /// </summary>
        /// <param name="key">
        ///    A string identifying the requested entry.
        /// </param>
        /// <param name="value">
        ///    The located value or null.
        /// </param>
        /// <returns>
        ///    True if the key was found.
        /// </returns>
        public bool TryGet(string key, out T value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

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
