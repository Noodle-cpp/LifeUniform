using LifeUniform.Application.Marketing.Dto;

namespace LifeUniform.Application.Catalog.Dto;

public class CatalogHomeDto
{
    public IReadOnlyList<CategoryCardDto> Categories { get; set; } = Array.Empty<CategoryCardDto>();
    public IReadOnlyList<ProductCardDto> PopularProducts { get; set; } = Array.Empty<ProductCardDto>();
    public IReadOnlyList<PromoOfferDto> PromoOffers { get; set; } = Array.Empty<PromoOfferDto>();
}

