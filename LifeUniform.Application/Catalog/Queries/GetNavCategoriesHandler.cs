using LifeUniform.Application.Abstractions.Caching;
using LifeUniform.Application.Catalog.Dto;
using LifeUniform.Application.Catalog.Mapping;
using LifeUniform.Domain.Catalog;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace LifeUniform.Application.Catalog.Queries;

public class GetNavCategoriesHandler : IRequestHandler<GetNavCategoriesQuery, NavMenuDto>
{
    private readonly ICatalogRepository _catalog;
    private readonly IMemoryCache _cache;

    public GetNavCategoriesHandler(ICatalogRepository catalog, IMemoryCache cache)
    {
        _catalog = catalog;
        _cache = cache;
    }

    public async Task<NavMenuDto> Handle(GetNavCategoriesQuery request, CancellationToken cancellationToken)
    {
        var cached = await _cache.GetOrCreateAsync(CatalogCacheKeys.Categories, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);
            var categories = await _catalog.GetCategoriesAsync(null, cancellationToken);
            var cards = CatalogMapper.ToCategoryCards(categories);
            return new NavMenuDto
            {
                Women = cards.Where(c => c.Gender == (int)ProductGender.Women).ToList(),
                Men = cards.Where(c => c.Gender == (int)ProductGender.Men).ToList()
            };
        });

        return cached ?? new NavMenuDto();
    }
}
