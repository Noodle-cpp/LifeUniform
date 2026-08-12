using LifeUniform.Application.Abstractions.Caching;
using LifeUniform.Application.Catalog.Dto;
using LifeUniform.Application.Catalog.Mapping;
using LifeUniform.Application.Marketing.Dto;
using LifeUniform.Domain.Catalog;
using LifeUniform.Domain.Marketing;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace LifeUniform.Application.Catalog.Queries;

public class GetCatalogHomeHandler : IRequestHandler<GetCatalogHomeQuery, CatalogHomeDto>
{
    private readonly ICatalogRepository _catalogRepository;
    private readonly IPromoOfferRepository _promoOfferRepository;
    private readonly IMemoryCache _cache;

    public GetCatalogHomeHandler(
        ICatalogRepository catalogRepository,
        IPromoOfferRepository promoOfferRepository,
        IMemoryCache cache)
    {
        _catalogRepository = catalogRepository;
        _promoOfferRepository = promoOfferRepository;
        _cache = cache;
    }

    public async Task<CatalogHomeDto> Handle(GetCatalogHomeQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = CatalogCacheKeys.HomePrefix + (request.Gender?.ToString() ?? "all");

        var cached = await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);

            var categories = await _catalogRepository.GetCategoriesAsync(request.Gender, cancellationToken);
            var popularProducts = await _catalogRepository.GetPopularProductsAsync(12, cancellationToken);
            var offers = await _promoOfferRepository.GetActiveAsync(cancellationToken);

            return new CatalogHomeDto
            {
                Categories = CatalogMapper.ToCategoryCards(categories),
                PopularProducts = CatalogMapper.ToProductCards(popularProducts).ToList(),
                PromoOffers = offers.Select(o => new PromoOfferDto
                {
                    Id = o.Id,
                    Title = o.Title,
                    Subtitle = o.Subtitle,
                    Badge = o.Badge,
                    ImageUrl = o.ImageUrl,
                    LinkUrl = o.LinkUrl,
                    LinkText = o.LinkText,
                    Price = o.Price,
                    OldPrice = o.OldPrice,
                    SortOrder = o.SortOrder,
                    IsActive = o.IsActive
                }).ToList()
            };
        }) ?? new CatalogHomeDto();

        // Clone popular cards so per-user favorite flags never leak via shared cache instances.
        var dto = new CatalogHomeDto
        {
            Categories = cached.Categories,
            PromoOffers = cached.PromoOffers,
            PopularProducts = cached.PopularProducts.Select(CloneCard).ToList()
        };

        if (!string.IsNullOrWhiteSpace(request.UserId))
        {
            var favoriteIds = await _catalogRepository.GetFavoriteProductIdsAsync(request.UserId!, cancellationToken);
            foreach (var p in dto.PopularProducts)
                p.IsFavorite = favoriteIds.Contains(p.Id);
        }

        return dto;
    }

    private static ProductCardDto CloneCard(ProductCardDto p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Slug = p.Slug,
        Price = p.Price,
        DiscountPrice = p.DiscountPrice,
        PreviewImageUrl = p.PreviewImageUrl,
        Colors = p.Colors,
        IsFavorite = false
    };
}

