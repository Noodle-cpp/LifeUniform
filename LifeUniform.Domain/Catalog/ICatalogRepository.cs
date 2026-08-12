namespace LifeUniform.Domain.Catalog;

public interface ICatalogRepository
{
    Task<IReadOnlyList<Category>> GetCategoriesAsync(ProductGender? gender, CancellationToken cancellationToken);

    Task<IReadOnlyList<Product>> GetPopularProductsAsync(int take, CancellationToken cancellationToken);

    Task<IReadOnlyList<Product>> GetProductsByIdsAsync(IReadOnlyList<Guid> ids, CancellationToken cancellationToken);

    Task<Product?> GetProductBySlugAsync(string slug, CancellationToken cancellationToken);


    Task<Product?> GetProductWithSizesBySlugAsync(string slug, CancellationToken cancellationToken);

    Task<IReadOnlyList<Size>> GetAllSizesAsync(CancellationToken cancellationToken);

    Task SetProductSizesAsync(Guid productId, IReadOnlyList<Guid> sizeIds, CancellationToken cancellationToken);

    Task<IReadOnlyList<Product>> GetProductsForAdminAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<Category>> GetCategoriesForAdminAsync(CancellationToken cancellationToken);

    Task<Category?> GetCategoryBySlugAsync(string slug, CancellationToken cancellationToken);

    Task UpsertCategoryAsync(Category category, CancellationToken cancellationToken);

    Task<Product?> GetProductWithImagesAndSizesBySlugAsync(string slug, CancellationToken cancellationToken);

    Task UpsertProductAsync(
        string slug,
        ProductGender gender,
        Guid categoryId,
        decimal price,
        decimal? discountPrice,
        string name,
        string? shortName,
        string? sku,
        string? description,
        string? material,
        string? careInstructions,
        string? sizeChartImageUrl,
        bool isInStock,
        decimal? freeShippingFrom,
        int popularityRank,
        IReadOnlyList<Guid> sizeIds,
        IReadOnlyList<(string Name, string Hex, string? ImageUrl)> colors,
        IReadOnlyList<(string ColorName, Guid SizeId, bool IsInStock)> colorSizeStocks,
        IReadOnlyList<string> imageFileNamesToAdd,
        string? imageAltText,
        CancellationToken cancellationToken);

    Task DeleteProductImageAsync(string slug, Guid imageId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Guid>> GetFavoriteProductIdsAsync(string userId, CancellationToken cancellationToken);

    Task<bool> IsProductFavoriteAsync(string userId, Guid productId, CancellationToken cancellationToken);

    /// <summary>Returns whether the product is favorite after toggle.</summary>
    Task<bool> ToggleProductFavoriteAsync(string userId, Guid productId, CancellationToken cancellationToken);

    Task<(IReadOnlyList<Product> Items, int TotalCount)> GetProductsAsync(
        ProductGender? gender,
        Guid? categoryId,
        string? search,
        int skip,
        int take,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Product>> GetRelatedProductsAsync(
        Guid productId,
        Guid categoryId,
        int take,
        CancellationToken cancellationToken);
}

