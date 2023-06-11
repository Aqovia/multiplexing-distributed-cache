# Aqovia Multiplexing Distributed Cache

Aqovia.MultiplexingDistributedCache is a library that implements a IDistributedCache that multiplexes between two other implementations of IDistributedCache, primary and secondary. While the secondary is optional that makes this library act as only a primary cache.

## How to use

You need to install [the nuget package](https://www.nuget.org/packages/Aqovia.MultiplexingDistributedCache).

You will inject an object of MultiplexingDistributedCache in Startup.
For that you need to pass a primary IDistributedCache and an optional secondary IDistributedCache objects.

In the following example we use [StackExchange.Redis implementation of IDistributedCache](https://www.nuget.org/packages/StackExchange.Redis) as the primary cache and an [In Memory cache](https://www.nuget.org/packages/Microsoft.Extensions.Caching.Memory) for the secondary.

In Startup.cs:

```cs
using Aqovia.Cache;
using StackExchange.Redis;
using Microsoft.Extensions.Caching.Memory;
```

then in its `ConfigureServices` method:

```cs
public void ConfigureServices(IServiceCollection services)
{
    ...
    var primaryRedisConnectionString = Configuration.GetConnectionString("PrimaryRedisCacheConnectionString");
    var primaryRedisConfig = ConfigurationOptions.Parse(primaryRedisConnectionString);
    var primaryRedisConnectionMultiplexer = ConnectionMultiplexer.Connect(primaryRedisConfig);
    var primaryRedisCache = new RedisCache(new RedisCacheOptions() { ConfigurationOptions = primaryRedisConfig });

    var secondaryRedisCache = new MemoryCache(new MemoryCacheOptions() { });

    services.AddSingleton<IDistributedCache>(
        new MultiplexingDistributedCache(primaryRedisCache, secondaryRedisCache));
    services.AddSession();
    ...
}
```

You can also have only a Primary cache:

```cs
public void ConfigureServices(IServiceCollection services)
{
    ...
    var primaryRedisConnectionString = Configuration.GetConnectionString("PrimaryRedisCacheConnectionString");
    var primaryRedisConfig = ConfigurationOptions.Parse(primaryRedisConnectionString);
    var primaryRedisConnectionMultiplexer = ConnectionMultiplexer.Connect(primaryRedisConfig);
    var primaryRedisCache = new RedisCache(new RedisCacheOptions() { ConfigurationOptions = primaryRedisConfig });

    services.AddSingleton<IDistributedCache>(new MultiplexingDistributedCache(primaryRedisCache));
    services.AddSession();
    ...
}
```

This is useful when you want to have optional secondary based on your config settings:

```cs
public void ConfigureServices(IServiceCollection services)
{
    ...
    var primaryRedisConnectionString = Configuration.GetConnectionString("PrimaryRedisCacheConnectionString");
    var primaryRedisConfig = ConfigurationOptions.Parse(primaryRedisConnectionString);
    var primaryRedisConnectionMultiplexer = ConnectionMultiplexer.Connect(primaryRedisConfig);
    var primaryRedisCache = new RedisCache(new RedisCacheOptions() { ConfigurationOptions = primaryRedisConfig });

    IDistributedCache secondaryRedisCache = null;
    var secondaryRedisConnectionString = Configuration.GetConnectionString("SecondaryRedisCacheConnectionString");
    if (!string.IsNullOrEmpty(secondaryRedisConnectionString) &&
        !secondaryRedisConnectionString.Equals(primaryRedisConnectionString))
    {
        var secondaryRedisConfig = ConfigurationOptions.Parse(secondaryRedisConnectionString);
        var secondaryRedisConnectionMultiplexer = ConnectionMultiplexer.Connect(secondaryRedisConfig);
        secondaryRedisCache = new RedisCache(new RedisCacheOptions() { ConfigurationOptions = secondaryRedisConfig });
    }

    services.AddSingleton<IDistributedCache>(
        new MultiplexingDistributedCache(primaryRedisCache, secondaryRedisCache));
    services.AddSession();
    ...
}
```

## Example Usage Scenario

You are using one Redis instance and you have to migrate to a new Redis instance, for reasons like:

* You want to use different tier of Redis and you can't upgrade it.
* Your Redis instance is in one Cloud Infrastructure or one tenant, and you want to migrate to a Redis in another.

Or any similar reasons. And you want to do this without losing your users' sessions.

### Your Migration Plan

1. Provision your new Redis instance
2. Change your application to use Aqovia.MultiplexingDistributedCache with the primary to be your current Redis instance, and secondary to be the new Redis instance
3. Wait for a while until the first session after you have done the action 2, is expired. This means both Redis instances should have the same data.
4. Change your application, so that the primary cache is your new Redis instance and secondary is the old one.
5. Change your application, so that the primary cache is your new Redis instance and you don't have a secondary cache. At this stage your app will work like before but with the new Redis instance.
6. Now you can remove your old Redis instance.

## How it works

Multiplexing Distributed Cache is an implementation of IDistributedCache that is multiplexing to two other implementation of IDistributedCache. Read will be always from the Primary cache, but other implemented methods of IDistributedCache will be done on both caches.

## Feedback

feedbacks are always welcomed, please open an issue for any problem or bug found, and the suggestions are also welcomed.
