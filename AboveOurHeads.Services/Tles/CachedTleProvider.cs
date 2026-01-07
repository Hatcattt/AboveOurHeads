using AboveOurHeads.Core.Interfaces;
using AboveOurHeads.Core.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace AboveOurHeads.Services.Tles
{
    public class CachedTleProvider : ITleProvider
    {
        // TODO: Fallback sur cache expiré si le inner provider échoue ou autre problème

        private readonly ILogger<CachedTleProvider> _logger;
        private readonly ITleProvider _innerProvider;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheDuration;

        public CachedTleProvider(
            ILogger<CachedTleProvider> logger,
            ITleProvider innerProvider,
            IMemoryCache cache,
            TimeSpan cacheDuration)
        {
            _logger = logger;
            _innerProvider = innerProvider;
            _cache = cache;
            _cacheDuration = cacheDuration;
        }

        public async Task<TleData?> GetTleAsync(int noradId)
        {
            if (_cacheDuration <= TimeSpan.Zero)
            {
                _logger.LogInformation(
                    "Cache disabled (duration={CacheDuration}), fetching directly",
                    _cacheDuration);
                return await _innerProvider.GetTleAsync(noradId);
            }

            if (_cache.TryGetValue(noradId, out TleData? cachedData))
            {
                _logger.LogInformation(
                    "TLE data for NoradId={NoradId} retrieved from cache",
                    noradId);
                return cachedData;
            }

            _logger.LogInformation(
                "Cache miss for NoradId={NoradId}, fetching from provider (type={ProviderType})",
                noradId,
                _innerProvider.GetType().Name);

            var data = await _innerProvider.GetTleAsync(noradId);

            if (data == null)
            {
                return null;
            }

            _cache.Set(
                noradId,
                data,
                new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = _cacheDuration
                });

            _logger.LogInformation(
                "TLE data for NoradId={NoradId} cached for {CacheDuration}",
                noradId,
                _cacheDuration);

            return data;
        }
    }
}
