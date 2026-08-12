using LifeUniform.Application.Catalog.Dto;
using LifeUniform.Domain.Catalog;

namespace LifeUniform.Application.Catalog.Mapping;

public static class CatalogMapper
{
    private static readonly string PlaceholderImage = "/images/placeholder-product.svg";

    public static string ResolvePreviewUrl(Product product)
    {
        var file = product.PrimaryImage?.FileNamePreview
                   ?? product.Images.OrderBy(i => i.SortOrder).FirstOrDefault()?.FileNamePreview;
        return ResolveImageFile(file) ?? PlaceholderImage;
    }

    public static string? ResolveImageFile(string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return null;

        var value = fileName.Trim();

        // Repair values previously broken by prefixing uploads path onto absolute URLs
        const string brokenPrefix = "/uploads/products/";
        if (value.StartsWith(brokenPrefix, StringComparison.OrdinalIgnoreCase))
        {
            var rest = value[brokenPrefix.Length..];
            if (rest.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                || rest.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
                || rest.StartsWith("//", StringComparison.Ordinal))
            {
                value = rest;
            }
        }

        // Absolute URL (color photo link, CDN, etc.)
        if (value.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            || value.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
            || value.StartsWith("//", StringComparison.Ordinal))
        {
            return value.StartsWith("//", StringComparison.Ordinal) ? $"https:{value}" : value;
        }

        // Already a site-relative path
        if (value.StartsWith('/'))
            return value;

        // Uploaded file name on disk
        return $"/uploads/products/{value}";
    }

    public static ProductColorDto ToProductColor(ProductColorOption c) => new()
    {
        Name = c.Name,
        Hex = c.Hex,
        ImageUrl = ResolveImageFile(c.ImageFileName)
    };

    public static CategoryCardDto ToCategoryCard(Category c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Slug = c.Slug,
        Gender = (int)c.Gender
    };

    public static CategoryAdminCardDto ToCategoryAdminCard(Category c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Slug = c.Slug,
        Gender = (int)c.Gender,
        SortOrder = c.SortOrder,
        IsActive = c.IsActive
    };

    public static CategoryEditDto ToCategoryEdit(Category c) => new()
    {
        Slug = c.Slug,
        Name = c.Name,
        Gender = (int)c.Gender,
        Description = c.Description,
        SortOrder = c.SortOrder,
        IsActive = c.IsActive
    };

    public static ProductCardDto ToProductCard(Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Slug = p.Slug,
        Price = p.Price,
        DiscountPrice = p.DiscountPrice,
        PreviewImageUrl = ResolvePreviewUrl(p),
        Colors = p.ColorOptions
            .OrderBy(c => c.SortOrder)
            .Select(ToProductColor)
            .ToList()
    };

    public static ProductDetailsDto ToProductDetails(Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        ShortName = p.ShortName,
        Slug = p.Slug,
        Sku = p.Sku,
        CategoryId = p.CategoryId,
        CategoryName = p.Category?.Name ?? string.Empty,
        CategorySlug = p.Category?.Slug ?? string.Empty,
        Gender = (int)p.Gender,
        Description = p.Description,
        Material = p.Material,
        CareInstructions = p.CareInstructions,
        Price = p.Price,
        DiscountPrice = p.DiscountPrice,
        IsInStock = p.IsInStock,
        FreeShippingFrom = p.FreeShippingFrom ?? 8000m,
        PreviewImageUrl = ResolvePreviewUrl(p),
        SizeChartImageUrl = string.IsNullOrWhiteSpace(p.SizeChartImageUrl)
            ? "/images/size-chart.svg"
            : p.SizeChartImageUrl,
        Colors = p.ColorOptions
            .OrderBy(c => c.SortOrder)
            .Select(ToProductColor)
            .ToList(),
        GalleryImageUrls = p.Images
            .OrderByDescending(i => i.IsPrimary)
            .ThenBy(i => i.SortOrder)
            .Select(i => ResolveImageFile(i.FileNamePreview))
            .Where(url => !string.IsNullOrWhiteSpace(url))
            .Cast<string>()
            .Distinct()
            .DefaultIfEmpty(PlaceholderImage)
            .ToList(),
        Sizes = p.SizeOptions
            .Where(so => so.Size is not null)
            .Select(so => new SizeCardDto { Id = so.Size.Id, Label = so.Size.Label, IsInStock = true })
            .ToList(),
        ColorSizeStocks = p.ColorSizeStocks
            .Where(s => s.IsInStock)
            .Select(s => new ColorSizeStockDto
            {
                ColorName = s.ColorName,
                SizeId = s.SizeId,
                IsInStock = true
            })
            .ToList(),
        RelatedProducts = Array.Empty<ProductCardDto>()
    };

    public static ProductAdminCardDto ToProductAdminCard(Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Slug = p.Slug,
        Gender = (int)p.Gender,
        CategoryName = p.Category?.Name ?? string.Empty,
        Price = p.Price,
        DiscountPrice = p.DiscountPrice
    };

    public static IReadOnlyList<CategoryCardDto> ToCategoryCards(IEnumerable<Category> items) =>
        items.Select(ToCategoryCard).ToList();

    public static IReadOnlyList<ProductCardDto> ToProductCards(IEnumerable<Product> items) =>
        items.Select(ToProductCard).ToList();
}
