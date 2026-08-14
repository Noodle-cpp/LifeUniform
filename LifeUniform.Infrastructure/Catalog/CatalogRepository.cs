using LifeUniform.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using LifeUniform.Infrastructure.Persistence;

namespace LifeUniform.Infrastructure.Catalog;

public class CatalogRepository : ICatalogRepository
{
    private readonly ApplicationDbContext _db;

    public CatalogRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Category>> GetCategoriesAsync(ProductGender? gender, CancellationToken cancellationToken)
    {
        var query = _db.Categories
            .AsNoTracking()
            .Where(c => c.IsActive);

        if (gender is not null)
            query = query.Where(c => c.Gender == gender.Value);

        return await query
            .OrderBy(c => c.SortOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetPopularProductsAsync(int take, CancellationToken cancellationToken, ProductGender? gender = null)
    {
        var query = _db.Products
            .AsNoTracking()
            .Where(p => p.IsActive);

        if (gender is not null)
            query = query.Where(p => p.Gender == gender.Value);

        return await query
            .Include(p => p.Images)
            .Include(p => p.ColorOptions)
            .Include(p => p.SizeOptions)
                .ThenInclude(so => so.Size)
            .OrderByDescending(p => p.PopularityRank)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetProductsByIdsAsync(IReadOnlyList<Guid> ids, CancellationToken cancellationToken)
    {
        if (ids.Count == 0)
            return Array.Empty<Product>();

        return await _db.Products
            .AsNoTracking()
            .Where(p => ids.Contains(p.Id))
            .Include(p => p.Images)
            .Include(p => p.ColorOptions)
            .Include(p => p.SizeOptions)
                .ThenInclude(so => so.Size)
            .ToListAsync(cancellationToken);
    }

    public async Task<Product?> GetProductBySlugAsync(string slug, CancellationToken cancellationToken)
    {
        return await _db.Products
            .AsNoTracking()
            .Where(p => p.IsActive && p.Slug == slug)
            .Include(p => p.Images)
            .Include(p => p.ColorOptions)
            .Include(p => p.SizeOptions)
                .ThenInclude(so => so.Size)
            .Include(p => p.ColorSizeStocks)
            .Include(p => p.Category)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Guid?> GetProductIdBySlugAsync(string slug, CancellationToken cancellationToken)
    {
        return await _db.Products
            .AsNoTracking()
            .Where(p => p.IsActive && p.Slug == slug)
            .Select(p => (Guid?)p.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Product?> GetProductWithSizesBySlugAsync(string slug, CancellationToken cancellationToken)
    {
        return await _db.Products
            .Where(p => p.IsActive && p.Slug == slug)
            .Include(p => p.Images)
            .Include(p => p.SizeOptions)
                .ThenInclude(so => so.Size)
            .Include(p => p.ColorOptions)
            .Include(p => p.ColorSizeStocks)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Category>> GetCategoriesForAdminAsync(CancellationToken cancellationToken)
    {
        return await _db.Categories
            .AsNoTracking()
            .OrderBy(c => c.SortOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<Category?> GetCategoryBySlugAsync(string slug, CancellationToken cancellationToken)
    {
        return await _db.Categories
            .FirstOrDefaultAsync(c => c.Slug == slug, cancellationToken);
    }

    public async Task UpsertCategoryAsync(Category category, CancellationToken cancellationToken)
    {
        var existing = await _db.Categories.FirstOrDefaultAsync(c => c.Slug == category.Slug, cancellationToken);
        if (existing is null)
        {
            category.Id = Guid.NewGuid();
            _db.Categories.Add(category);
        }
        else
        {
            existing.Gender = category.Gender;
            existing.Name = category.Name;
            existing.Description = category.Description;
            existing.SortOrder = category.SortOrder;
            existing.IsActive = category.IsActive;
        }

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<Product?> GetProductWithImagesAndSizesBySlugAsync(string slug, CancellationToken cancellationToken)
    {
        return await _db.Products
            .Where(p => p.Slug == slug)
            .Include(p => p.Images)
            .Include(p => p.SizeOptions)
                .ThenInclude(so => so.Size)
            .Include(p => p.ColorOptions)
            .Include(p => p.ColorSizeStocks)
            .Include(p => p.Category)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task UpsertProductAsync(
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
        CancellationToken cancellationToken)
    {
        var product = await _db.Products
            .Include(p => p.Images)
            .Include(p => p.SizeOptions)
            .Include(p => p.ColorOptions)
            .Include(p => p.ColorSizeStocks)
            .FirstOrDefaultAsync(p => p.Slug == slug, cancellationToken);

        var nowProductId = product?.Id ?? Guid.NewGuid();

        if (product is null)
        {
            product = new Product
            {
                Id = nowProductId,
                Slug = slug,
                Gender = gender,
                CategoryId = categoryId,
                Name = name,
                ShortName = shortName,
                Sku = sku,
                Description = description,
                Material = material,
                CareInstructions = careInstructions,
                SizeChartImageUrl = sizeChartImageUrl,
                IsInStock = isInStock,
                FreeShippingFrom = freeShippingFrom,
                Price = price,
                DiscountPrice = discountPrice,
                PopularityRank = popularityRank,
                IsActive = true
            };

            _db.Products.Add(product);
        }
        else
        {
            product.Gender = gender;
            product.CategoryId = categoryId;
            product.Name = name;
            product.ShortName = shortName;
            product.Sku = sku;
            product.Description = description;
            product.Material = material;
            product.CareInstructions = careInstructions;
            product.SizeChartImageUrl = sizeChartImageUrl;
            product.IsInStock = isInStock;
            product.FreeShippingFrom = freeShippingFrom;
            product.Price = price;
            product.DiscountPrice = discountPrice;
            product.PopularityRank = popularityRank;
            product.IsActive = true;
        }

        var existingSizeOptions = await _db.ProductSizeOptions
            .Where(x => x.ProductId == nowProductId)
            .ToListAsync(cancellationToken);
        _db.ProductSizeOptions.RemoveRange(existingSizeOptions);
        foreach (var sizeId in sizeIds.Distinct())
        {
            _db.ProductSizeOptions.Add(new ProductSizeOption
            {
                ProductId = nowProductId,
                SizeId = sizeId
            });
        }

        // Удаляем текущие цвета и добавляем заново (только FK, без навигации Product).
        var existingColors = await _db.ProductColorOptions
            .Where(c => c.ProductId == nowProductId)
            .ToListAsync(cancellationToken);
        _db.ProductColorOptions.RemoveRange(existingColors);
        var sort = 0;
        foreach (var color in colors.Where(c => !string.IsNullOrWhiteSpace(c.Name)))
        {
            sort += 10;
            _db.ProductColorOptions.Add(new ProductColorOption
            {
                Id = Guid.NewGuid(),
                ProductId = nowProductId,
                Name = color.Name.Trim(),
                Hex = string.IsNullOrWhiteSpace(color.Hex) ? "#cccccc" : color.Hex.Trim(),
                ImageFileName = NormalizeStoredImageRef(color.ImageUrl),
                SortOrder = sort
            });
        }

        // Color × size stock matrix
        var existingStocks = await _db.ProductColorSizeStocks
            .Where(s => s.ProductId == nowProductId)
            .ToListAsync(cancellationToken);
        _db.ProductColorSizeStocks.RemoveRange(existingStocks);

        var colorNames = colors
            .Where(c => !string.IsNullOrWhiteSpace(c.Name))
            .Select(c => c.Name.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var sizeIdSet = sizeIds.Distinct().ToHashSet();

        foreach (var stock in colorSizeStocks.Where(s => s.IsInStock))
        {
            var colorName = stock.ColorName?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(colorName) || !colorNames.Contains(colorName))
                continue;
            if (!sizeIdSet.Contains(stock.SizeId))
                continue;

            // Canonical color name from colors list
            var canonical = colors.First(c =>
                string.Equals(c.Name.Trim(), colorName, StringComparison.OrdinalIgnoreCase)).Name.Trim();

            _db.ProductColorSizeStocks.Add(new ProductColorSizeStock
            {
                ProductId = nowProductId,
                ColorName = canonical,
                SizeId = stock.SizeId,
                IsInStock = true
            });
        }

        if (imageFileNamesToAdd.Count > 0)
        {
            var maxSort = product.Images.Count == 0
                ? 0
                : product.Images.Max(i => i.SortOrder);
            var hasPrimary = product.Images.Any(i => i.IsPrimary);

            foreach (var fileName in imageFileNamesToAdd.Where(f => !string.IsNullOrWhiteSpace(f)))
            {
                maxSort += 10;
                var makePrimary = !hasPrimary;
                hasPrimary = true;
                _db.ProductImages.Add(new ProductImage
                {
                    Id = Guid.NewGuid(),
                    ProductId = nowProductId,
                    FileNameOriginal = fileName,
                    FileNamePreview = fileName,
                    FileNameWebp = fileName,
                    AltText = imageAltText ?? name,
                    IsPrimary = makePrimary,
                    SortOrder = maxSort
                });
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteProductImageAsync(string slug, Guid imageId, CancellationToken cancellationToken)
    {
        var product = await _db.Products
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Slug == slug, cancellationToken);
        if (product is null)
            return;

        var image = product.Images.FirstOrDefault(i => i.Id == imageId);
        if (image is null)
            return;

        var wasPrimary = image.IsPrimary;
        _db.ProductImages.Remove(image);
        if (wasPrimary)
        {
            var next = product.Images
                .Where(i => i.Id != imageId)
                .OrderBy(i => i.SortOrder)
                .FirstOrDefault();
            if (next is not null)
                next.IsPrimary = true;
        }

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Guid>> GetFavoriteProductIdsAsync(string userId, CancellationToken cancellationToken)
    {
        return await _db.UserFavoriteProducts
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => x.ProductId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsProductFavoriteAsync(string userId, Guid productId, CancellationToken cancellationToken)
    {
        return await _db.UserFavoriteProducts
            .AsNoTracking()
            .AnyAsync(x => x.UserId == userId && x.ProductId == productId, cancellationToken);
    }

    public async Task<bool> ToggleProductFavoriteAsync(string userId, Guid productId, CancellationToken cancellationToken)
    {
        var existing = await _db.UserFavoriteProducts
            .FirstOrDefaultAsync(x => x.UserId == userId && x.ProductId == productId, cancellationToken);

        if (existing is null)
        {
            _db.UserFavoriteProducts.Add(new UserFavoriteProduct
            {
                UserId = userId,
                ProductId = productId
            });
            try
            {
                await _db.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (DbUpdateException)
            {
                // Parallel double-add: already favorited
                _db.ChangeTracker.Clear();
                return true;
            }
        }

        _db.UserFavoriteProducts.Remove(existing);
        try
        {
            await _db.SaveChangesAsync(cancellationToken);
            return false;
        }
        catch (DbUpdateConcurrencyException)
        {
            _db.ChangeTracker.Clear();
            return false;
        }
    }

    public async Task<IReadOnlyList<Size>> GetAllSizesAsync(CancellationToken cancellationToken)
    {
        return await _db.Sizes
            .AsNoTracking()
            .Where(s => s.IsActive)
            .OrderBy(s => s.SortOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task SetProductSizesAsync(Guid productId, IReadOnlyList<Guid> sizeIds, CancellationToken cancellationToken)
    {
        // Удаляем текущие опции и добавляем новые.
        var existing = await _db.ProductSizeOptions
            .Where(so => so.ProductId == productId)
            .ToListAsync(cancellationToken);

        _db.ProductSizeOptions.RemoveRange(existing);

        var normalized = sizeIds.Distinct().ToList();
        foreach (var sizeId in normalized)
        {
            _db.ProductSizeOptions.Add(new ProductSizeOption
            {
                ProductId = productId,
                SizeId = sizeId
            });
        }

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<Product> Items, int TotalCount)> GetProductsAsync(
        ProductGender? gender,
        IReadOnlyList<Guid>? categoryIds,
        string? search,
        IReadOnlyList<string>? colors,
        int skip,
        int take,
        CancellationToken cancellationToken)
    {
        var query = _db.Products
            .AsNoTracking()
            .Where(p => p.IsActive);

        if (gender is not null)
            query = query.Where(p => p.Gender == gender.Value);

        if (categoryIds is { Count: > 0 })
        {
            var ids = categoryIds.Distinct().ToList();
            query = query.Where(p => ids.Contains(p.CategoryId));
        }

        string? searchLower = null;
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            searchLower = term.ToLowerInvariant();
            var useInMemory = string.Equals(
                _db.Database.ProviderName,
                "Microsoft.EntityFrameworkCore.InMemory",
                StringComparison.Ordinal);

            // Contains by name / short name / sku / category; description only for longer queries
            if (useInMemory)
            {
                query = query.Where(p =>
                    p.Name.ToLower().Contains(searchLower)
                    || (p.ShortName != null && p.ShortName.ToLower().Contains(searchLower))
                    || (p.Sku != null && p.Sku.ToLower().Contains(searchLower))
                    || (p.Category != null && p.Category.Name.ToLower().Contains(searchLower))
                    || (term.Length >= 3 && p.Description != null && p.Description.ToLower().Contains(searchLower)));
            }
            else
            {
                var pattern = $"%{EscapeLikePattern(term)}%";
                query = query.Where(p =>
                    EF.Functions.ILike(p.Name, pattern)
                    || (p.ShortName != null && EF.Functions.ILike(p.ShortName, pattern))
                    || (p.Sku != null && EF.Functions.ILike(p.Sku, pattern))
                    || (p.Category != null && EF.Functions.ILike(p.Category.Name, pattern))
                    || (term.Length >= 3 && p.Description != null && EF.Functions.ILike(p.Description, pattern)));
            }
        }

        if (colors is { Count: > 0 })
        {
            var colorSet = colors
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Select(c => c.Trim().ToLowerInvariant())
                .Distinct()
                .ToList();

            if (colorSet.Count > 0)
            {
                query = query.Where(p => p.ColorOptions.Any(o =>
                    colorSet.Contains(o.Name.ToLower())
                    || colorSet.Contains(o.Hex.ToLower())));
            }
        }

        var total = await query.CountAsync(cancellationToken);

        var ordered = searchLower is null
            ? query.OrderByDescending(p => p.PopularityRank)
            : query
                .OrderByDescending(p => p.Name.ToLower().Contains(searchLower))
                .ThenByDescending(p => p.ShortName != null && p.ShortName.ToLower().Contains(searchLower))
                .ThenByDescending(p => p.Category != null && p.Category.Name.ToLower().Contains(searchLower))
                .ThenByDescending(p => p.PopularityRank);

        var items = await ordered
            .Include(p => p.Images)
            .Include(p => p.ColorOptions)
            .Include(p => p.Category)
            .Include(p => p.SizeOptions)
                .ThenInclude(so => so.Size)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    private static string EscapeLikePattern(string value) =>
        value
            .Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("%", "\\%", StringComparison.Ordinal)
            .Replace("_", "\\_", StringComparison.Ordinal);

    public async Task<IReadOnlyList<ProductColorOption>> GetDistinctColorsAsync(
        ProductGender? gender,
        IReadOnlyList<Guid>? categoryIds,
        CancellationToken cancellationToken)
    {
        var query = _db.Products
            .AsNoTracking()
            .Where(p => p.IsActive);

        if (gender is not null)
            query = query.Where(p => p.Gender == gender.Value);

        if (categoryIds is { Count: > 0 })
        {
            var ids = categoryIds.Distinct().ToList();
            query = query.Where(p => ids.Contains(p.CategoryId));
        }

        return await query
            .SelectMany(p => p.ColorOptions)
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetRelatedProductsAsync(
        Guid productId,
        Guid categoryId,
        int take,
        CancellationToken cancellationToken)
    {
        return await _db.Products
            .AsNoTracking()
            .Where(p => p.IsActive && p.CategoryId == categoryId && p.Id != productId)
            .Include(p => p.Images)
            .Include(p => p.ColorOptions)
            .Include(p => p.SizeOptions)
                .ThenInclude(so => so.Size)
            .OrderByDescending(p => p.PopularityRank)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetProductsForAdminAsync(CancellationToken cancellationToken)
    {
        return await _db.Products
            .AsNoTracking()
            .Where(p => p.IsActive)
            .Include(p => p.Category)
            .OrderByDescending(p => p.PopularityRank)
            .ToListAsync(cancellationToken);
    }

    private static string? NormalizeStoredImageRef(string? imageUrl)
    {
        // Persist the resolved/canonical form (keeps https:// links intact).
        return LifeUniform.Application.Catalog.Mapping.CatalogMapper.ResolveImageFile(imageUrl);
    }
}

