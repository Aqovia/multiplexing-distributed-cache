using Xunit;
using Aqovia.Cache;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace Aqovia.Cache.Tests;

public class MultiplexingDistributedCacheTests
{
    [Fact]
    public void Secondary_Get_Is_Not_Called()
    {
        var primary = new Mock<IDistributedCache>();
        var secondary = new Mock<IDistributedCache>();

        var cache = new MultiplexingDistributedCache(primary.Object, secondary.Object);

        cache.Get("foo");

        primary.Verify(_ => _.Get("foo"), Times.Once());
        primary.VerifyNoOtherCalls();
        secondary.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Secondary_GetAsync_Is_Not_Called()
    {
        var primary = new Mock<IDistributedCache>();
        var secondary = new Mock<IDistributedCache>();

        var cache = new MultiplexingDistributedCache(primary.Object, secondary.Object);

        await cache.GetAsync("foo").ConfigureAwait(false);

        primary.Verify(_ => _.GetAsync("foo", It.IsAny<CancellationToken>()), Times.Once());
        primary.VerifyNoOtherCalls();
        secondary.VerifyNoOtherCalls();
    }

    [Fact]
    public void Secondary_Set_Is_Called_Once()
    {
        var primary = new Mock<IDistributedCache>();
        var secondary = new Mock<IDistributedCache>();

        var cache = new MultiplexingDistributedCache(primary.Object, secondary.Object);

        cache.Set("foo", new byte[0]);

        primary.Verify(_ => _.Set("foo", It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Once());
        secondary.Verify(_ => _.Set("foo", It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Once());

        primary.VerifyNoOtherCalls();
        secondary.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Secondary_SetAsync_Is_Called_Once()
    {
        var primary = new Mock<IDistributedCache>();
        var secondary = new Mock<IDistributedCache>();

        var cache = new MultiplexingDistributedCache(primary.Object, secondary.Object);

        await cache.SetAsync("foo", new byte[0]);

        primary.Verify(_ => _.SetAsync("foo", It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Once());
        secondary.Verify(_ => _.SetAsync("foo", It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Once());

        primary.VerifyNoOtherCalls();
        secondary.VerifyNoOtherCalls();
    }

    [Fact]
    public void Secondary_Refresh_Is_Called_Once()
    {
        var primary = new Mock<IDistributedCache>();
        var secondary = new Mock<IDistributedCache>();

        var cache = new MultiplexingDistributedCache(primary.Object, secondary.Object);

        cache.Refresh("foo");

        primary.Verify(_ => _.Refresh("foo"), Times.Once());
        secondary.Verify(_ => _.Refresh("foo"), Times.Once());

        primary.VerifyNoOtherCalls();
        secondary.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Secondary_RefreshAsync_Is_Called_Once()
    {
        var primary = new Mock<IDistributedCache>();
        var secondary = new Mock<IDistributedCache>();

        var cache = new MultiplexingDistributedCache(primary.Object, secondary.Object);

        await cache.RefreshAsync("foo");

        primary.Verify(_ => _.RefreshAsync("foo", It.IsAny<CancellationToken>()), Times.Once());
        secondary.Verify(_ => _.RefreshAsync("foo", It.IsAny<CancellationToken>()), Times.Once());

        primary.VerifyNoOtherCalls();
        secondary.VerifyNoOtherCalls();
    }

    [Fact]
    public void Secondary_Remove_Is_Called_Once()
    {
        var primary = new Mock<IDistributedCache>();
        var secondary = new Mock<IDistributedCache>();

        var cache = new MultiplexingDistributedCache(primary.Object, secondary.Object);

        cache.Remove("foo");

        primary.Verify(_ => _.Remove("foo"), Times.Once());
        secondary.Verify(_ => _.Remove("foo"), Times.Once());

        primary.VerifyNoOtherCalls();
        secondary.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Secondary_RemoveAsync_Is_Called_Once()
    {
        var primary = new Mock<IDistributedCache>();
        var secondary = new Mock<IDistributedCache>();

        var cache = new MultiplexingDistributedCache(primary.Object, secondary.Object);

        await cache.RemoveAsync("foo");

        primary.Verify(_ => _.RemoveAsync("foo", It.IsAny<CancellationToken>()), Times.Once());
        secondary.Verify(_ => _.RemoveAsync("foo", It.IsAny<CancellationToken>()), Times.Once());

        primary.VerifyNoOtherCalls();
        secondary.VerifyNoOtherCalls();
    }

    [Fact]
    public void No_Error_When_Setting_With_No_Secondary()
    {
        var primary = new Mock<IDistributedCache>();
        Action testCode = () =>
        {
            var cache = new MultiplexingDistributedCache(primary.Object);
            cache.Set("foo", new byte[0]);
        };

        var ex = Record.Exception(testCode);
        Assert.Null(ex);
    }
}
