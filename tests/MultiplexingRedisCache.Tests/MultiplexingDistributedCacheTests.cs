using Xunit;
using Aqovia.MultiplexingDistributedCache;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;

namespace MultiplexingRedisCache.Tests;

public class MultiplexingDistributedCacheTests
{
    [Fact]
    public void Primary_Get_Is_Called_Once()
    {
        var primary = new Mock<IDistributedCache>();
        var secondary = new Mock<IDistributedCache>();

        var cache = new MultiplexingDistributedCache(primary.Object, secondary.Object);

        cache.Get("foo");

        primary.Verify(_ => _.Get("foo"), Times.Once());
    }

    [Fact]
    public void Secondary_Set_Is_Called_Once()
    {
        var primary = new Mock<IDistributedCache>();
        var secondary = new Mock<IDistributedCache>();

        var cache = new MultiplexingDistributedCache(primary.Object, secondary.Object);

        cache.Set("foo", new byte[0]);

        secondary.Verify(_ => _.Set("foo", It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Once());
    }
}
