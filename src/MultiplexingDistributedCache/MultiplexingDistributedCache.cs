using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace Aqovia.Cache
{
    public class MultiplexingDistributedCache : IDistributedCache
    {
        private readonly IDistributedCache _primary;
        private readonly IDistributedCache _secondary;
        public MultiplexingDistributedCache(IDistributedCache primary, IDistributedCache secondary = default)
        {
            _primary = primary;
            _secondary = secondary;
        }

        public byte[] Get(string key)
        {
            return _primary.Get(key);
        }

        public async Task<byte[]> GetAsync(string key, CancellationToken token = default)
        {
            return await _primary.GetAsync(key, token).ConfigureAwait(false);
        }

        public void Refresh(string key)
        {
            _primary.Refresh(key);
            _secondary?.Refresh(key);
        }

        public async Task RefreshAsync(string key, CancellationToken token = default)
        {
            await (_secondary?.RefreshAsync(key, token) ?? Task.CompletedTask).ConfigureAwait(false);
            await _primary.RefreshAsync(key, token).ConfigureAwait(false);
        }

        public void Remove(string key)
        {
            _primary.Remove(key);
            _secondary?.Remove(key);
        }

        public async Task RemoveAsync(string key, CancellationToken token = default)
        {
            await (_secondary?.RemoveAsync(key, token) ?? Task.CompletedTask).ConfigureAwait(false);
            await _primary.RemoveAsync(key, token).ConfigureAwait(false);
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            _primary.Set(key, value, options);
            _secondary?.Set(key, value, options);
        }

        public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
        {
            await (_secondary?.SetAsync(key, value, options, token) ?? Task.CompletedTask).ConfigureAwait(false);
            await _primary.SetAsync(key, value, options, token).ConfigureAwait(false);
        }
    }
}
