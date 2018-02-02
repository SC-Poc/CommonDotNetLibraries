using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Common
{
    [PublicAPI]
    public class CachedDataDictionary<TKey, TValue> where TValue:class
    {
        private IReadOnlyDictionary<TKey, TValue> _cashe = new Dictionary<TKey, TValue>();
        private DateTime _lastRefreshDateTime;

        private readonly Func<Task<Dictionary<TKey, TValue>>> _getData;
        private readonly TimeSpan _expirationPeriod;
        private readonly CachedDataDictionaryUpdateStrategy _updateStrategy;
        private readonly SemaphoreSlim _updateSync;
        
        public CachedDataDictionary(
            Func<Task<Dictionary<TKey, TValue>>> getData, 
            int validDataInSeconds = 60*5, 
            CachedDataDictionaryUpdateStrategy updateStrategy = CachedDataDictionaryUpdateStrategy.UseSynchronizedUpdates) :
            
            this(getData, TimeSpan.FromSeconds(validDataInSeconds), updateStrategy)
        {
        }

        public CachedDataDictionary(
            Func<Task<Dictionary<TKey, TValue>>> getData, 
            TimeSpan expirationPeriod,
            CachedDataDictionaryUpdateStrategy updateStrategy = CachedDataDictionaryUpdateStrategy.UseSynchronizedUpdates)
        {
            _getData = getData;
            _expirationPeriod = expirationPeriod;
            _updateStrategy = updateStrategy;

            _updateSync = new SemaphoreSlim(1, 1);
        }

        public void Invalidate()
        {
            _cashe = null;
        }

        public bool HaveToRefreshCash()
        {
            return HaveToRefreshCash(DateTime.UtcNow);
        }

        public async Task<IReadOnlyDictionary<TKey, TValue>> GetDictionaryAsync()
        {
            return await GetCache();
        }

        public async Task<TValue> GetItemAsync(TKey key)
        {
            var cache = await GetCache();

            cache.TryGetValue(key, out var value);

            return value;
        }

        public async Task<IEnumerable<TValue>> Values()
        {
            var cache = await GetCache();

            return cache.Values;
        }

        public TValue GetItem(TKey key)
        {
            return GetItemAsync(key).GetAwaiter().GetResult();
        }

        private Task<IReadOnlyDictionary<TKey, TValue>> GetCache()
        {
            switch (_updateStrategy)
            {
                case CachedDataDictionaryUpdateStrategy.AllowConcurrentUpdates:
                    return GetCacheWithConcurrentUpdate();

                case CachedDataDictionaryUpdateStrategy.UseSynchronizedUpdates:
                    return GetCacheWithSynchronizedUpdate();

                default:
                    throw new ArgumentOutOfRangeException(nameof(_updateStrategy), _updateStrategy, "");
            }
        }

        private async Task<IReadOnlyDictionary<TKey, TValue>> GetCacheWithConcurrentUpdate()
        {
            if (HaveToRefreshCash())
            {
                _cashe = await _getData();

                _lastRefreshDateTime = DateTime.UtcNow;
            }

            return _cashe;
        }

        private async Task<IReadOnlyDictionary<TKey, TValue>> GetCacheWithSynchronizedUpdate()
        {
            var now = DateTime.UtcNow;

            // Double check lock
            if (HaveToRefreshCash(now))
            {
                await _updateSync.WaitAsync();

                try
                {
                    if (HaveToRefreshCash(now))
                    {
                        _cashe = await _getData();

                        _lastRefreshDateTime = DateTime.UtcNow;
                    }
                }
                finally
                {
                    _updateSync.Release();
                }
            }

            return _cashe;
        }

        private bool HaveToRefreshCash(DateTime atTheMoment)
        {
            if (_cashe == null)
                return true;

            return atTheMoment - _lastRefreshDateTime > _expirationPeriod;
        }
    }
}
