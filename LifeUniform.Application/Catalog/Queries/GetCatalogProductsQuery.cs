using LifeUniform.Application.Catalog.Dto;
using LifeUniform.Application.Catalog.Mapping;
using LifeUniform.Domain.Catalog;
using MediatR;

namespace LifeUniform.Application.Catalog.Queries;

public class GetCatalogProductsQuery : IRequest<CatalogProductsPageDto>
{
    public ProductGender? Gender { get; init; }
    public IReadOnlyList<Guid> CategoryIds { get; init; } = Array.Empty<Guid>();
    public string? Search { get; init; }
    public IReadOnlyList<string> Colors { get; init; } = Array.Empty<string>();
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 12;
    public string? UserId { get; init; }
}

public class CatalogProductsPageDto
{
    public IReadOnlyList<ProductCardDto> Items { get; set; } = Array.Empty<ProductCardDto>();
    public IReadOnlyList<CategoryCardDto> Categories { get; set; } = Array.Empty<CategoryCardDto>();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
    public ProductGender? Gender { get; set; }
    public IReadOnlyList<Guid> CategoryIds { get; set; } = Array.Empty<Guid>();
    public string? Search { get; set; }
    public IReadOnlyList<string> Colors { get; set; } = Array.Empty<string>();
    public IReadOnlyList<ProductColorDto> AvailableColors { get; set; } = Array.Empty<ProductColorDto>();
}

public class GetCatalogProductsHandler : IRequestHandler<GetCatalogProductsQuery, CatalogProductsPageDto>
{
    private readonly ICatalogRepository _catalog;

    public GetCatalogProductsHandler(ICatalogRepository catalog) => _catalog = catalog;

    public async Task<CatalogProductsPageDto> Handle(GetCatalogProductsQuery request, CancellationToken cancellationToken)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 48 ? 12 : request.PageSize;
        var skip = (page - 1) * pageSize;
        var categoryIds = request.CategoryIds?.Where(id => id != Guid.Empty).Distinct().ToList()
            ?? new List<Guid>();
        var colors = request.Colors?
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Select(c => c.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList()
            ?? new List<string>();

        var (items, total) = await _catalog.GetProductsAsync(
            request.Gender,
            categoryIds,
            request.Search,
            colors,
            skip,
            pageSize,
            cancellationToken);

        var categories = await _catalog.GetCategoriesAsync(request.Gender, cancellationToken);
        var availableColors = await _catalog.GetDistinctColorsAsync(request.Gender, categoryIds, cancellationToken);

        IReadOnlyCollection<Guid> favoriteIds = Array.Empty<Guid>();
        if (!string.IsNullOrWhiteSpace(request.UserId))
            favoriteIds = await _catalog.GetFavoriteProductIdsAsync(request.UserId!, cancellationToken);

        var cards = CatalogMapper.ToProductCards(items)
            .Select(p =>
            {
                p.IsFavorite = favoriteIds.Contains(p.Id);
                return p;
            })
            .ToList();

        return new CatalogProductsPageDto
        {
            Items = cards,
            Categories = CatalogMapper.ToCategoryCards(categories),
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
            Gender = request.Gender,
            CategoryIds = categoryIds,
            Search = request.Search,
            Colors = colors,
            AvailableColors = availableColors
                .GroupBy(c => c.Hex + "|" + c.Name, StringComparer.OrdinalIgnoreCase)
                .Select(g => CatalogMapper.ToProductColor(g.First()))
                .ToList()
        };
    }
}
