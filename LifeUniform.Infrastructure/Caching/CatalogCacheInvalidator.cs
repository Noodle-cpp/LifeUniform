using LifeUniform.Application.Abstractions.Caching;
using Microsoft.Extensions.Caching.Memory;

namespace LifeUniform.Infrastructure.Caching;

public class CatalogCacheInvalidator : ICatalogCacheInvalidator
{
    private readonly IMemoryCache _cache;

    public CatalogCacheInvalidator(IMemoryCache cache) => _cache = cache;

    public void InvalidateCatalog()
    {
        // Home keys include optional gender suffix — clear known variants.
        _cache.Remove(CatalogCacheKeys.Categories);
        _cache.Remove(CatalogCacheKeys.HomePrefix + "all");
        _cache.Remove(CatalogCacheKeys.HomePrefix + "Women");
        _cache.Remove(CatalogCacheKeys.HomePrefix + "Men");
        _cache.Remove(CatalogCacheKeys.HomePrefix + "1");
        _cache.Remove(CatalogCacheKeys.HomePrefix + "2");
        _cache.Remove(CatalogCacheKeys.Popular);
    }
}
