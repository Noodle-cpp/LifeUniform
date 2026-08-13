using LifeUniform.Application.Abstractions.Caching;
using LifeUniform.Application.Catalog.Dto;
using LifeUniform.Application.Catalog.Mapping;
using LifeUniform.Application.Marketing.Queries;
using LifeUniform.Domain.Catalog;
using LifeUniform.Domain.Marketing;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace LifeUniform.Application.Catalog.Queries;

public class GetCatalogHomeHandler : IRequestHandler<GetCatalogHomeQuery, CatalogHomeDto>
{
    private readonly ICatalogRepository _catalogRepository;
    private readonly IPromoOfferRepository _promoOfferRepository;
    private readonly IClientPhotoRepository _clientPhotoRepository;
    private readonly IMemoryCache _cache;

    public GetCatalogHomeHandler(
        ICatalogRepository catalogRepository,
        IPromoOfferRepository promoOfferRepository,
        IClientPhotoRepository clientPhotoRepository,
        IMemoryCache cache)
    {
        _catalogRepository = catalogRepository;
        _promoOfferRepository = promoOfferRepository;
        _clientPhotoRepository = clientPhotoRepository;
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
            var womenProducts = await _catalogRepository.GetPopularProductsAsync(12, cancellationToken, ProductGender.Women);
            var menProducts = await _catalogRepository.GetPopularProductsAsync(12, cancellationToken, ProductGender.Men);
            var offers = await _promoOfferRepository.GetActiveAsync(cancellationToken);
            var photos = await _clientPhotoRepository.GetActiveAsync(cancellationToken);

            return new CatalogHomeDto
            {
                Categories = CatalogMapper.ToCategoryCards(categories),
                PopularProducts = CatalogMapper.ToProductCards(popularProducts).ToList(),
                WomenProducts = CatalogMapper.ToProductCards(womenProducts).ToList(),
                MenProducts = CatalogMapper.ToProductCards(menProducts).ToList(),
                PromoOffers = offers.Select(GetActivePromoOffersHandler.ToDto).ToList(),
                ClientPhotos = photos.Select(GetActiveClientPhotosHandler.ToDto).ToList()
            };
        }) ?? new CatalogHomeDto();

        var dto = new CatalogHomeDto
        {
            Categories = cached.Categories,
            PromoOffers = cached.PromoOffers,
            ClientPhotos = cached.ClientPhotos,
            PopularProducts = cached.PopularProducts.Select(CatalogMapper.CloneCard).ToList(),
            WomenProducts = cached.WomenProducts.Select(CatalogMapper.CloneCard).ToList(),
            MenProducts = cached.MenProducts.Select(CatalogMapper.CloneCard).ToList()
        };

        if (!string.IsNullOrWhiteSpace(request.UserId))
        {
            var favoriteIds = await _catalogRepository.GetFavoriteProductIdsAsync(request.UserId!, cancellationToken);
            MarkFavorites(dto.PopularProducts, favoriteIds);
            MarkFavorites(dto.WomenProducts, favoriteIds);
            MarkFavorites(dto.MenProducts, favoriteIds);
        }

        return dto;
    }

    private static void MarkFavorites(IReadOnlyList<ProductCardDto> products, IReadOnlyCollection<Guid> favoriteIds)
    {
        foreach (var p in products)
            p.IsFavorite = favoriteIds.Contains(p.Id);
    }
}
