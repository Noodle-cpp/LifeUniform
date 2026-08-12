using LifeUniform.Domain.Catalog;
using LifeUniform.Domain.Marketing;
using LifeUniform.Domain.Promotions;
using Microsoft.EntityFrameworkCore;

namespace LifeUniform.Infrastructure.Persistence;

public static class SeedData
{
    private static readonly string[] ProductImages =
    [
        "/images/products/scrub-teal.jpg",
        "/images/products/scrub-team.jpg",
        "/images/products/scrub-black.png",
        "/images/products/scrub-navy.png",
        "/images/products/scrub-group.png"
    ];

    private static readonly (string Name, string Hex)[] ColorPalette =
    [
        ("Сиреневый", "#c4a4c8"),
        ("Шалфей", "#8a9a7b"),
        ("Графит", "#4b5563"),
        ("Тёмно-синий", "#1e3a5f"),
        ("Чёрный", "#111111"),
        ("Белый", "#f5f5f5")
    ];

    public static async Task SeedAsync(ApplicationDbContext db, CancellationToken cancellationToken)
    {
        db.ChangeTracker.Clear();

        // Размеры S-5XL должны существовать всегда.
        var sizesExist = await db.Sizes.AnyAsync(cancellationToken);
        if (!sizesExist)
        {
            var sizes = new List<Size>
            {
                new() { Id = Guid.NewGuid(), Label = "S", SortOrder = 10 },
                new() { Id = Guid.NewGuid(), Label = "M", SortOrder = 20 },
                new() { Id = Guid.NewGuid(), Label = "L", SortOrder = 30 },
                new() { Id = Guid.NewGuid(), Label = "XL", SortOrder = 40 },
                new() { Id = Guid.NewGuid(), Label = "2XL", SortOrder = 50 },
                new() { Id = Guid.NewGuid(), Label = "3XL", SortOrder = 60 },
                new() { Id = Guid.NewGuid(), Label = "4XL", SortOrder = 70 },
                new() { Id = Guid.NewGuid(), Label = "5XL", SortOrder = 80 },
            };

            db.Sizes.AddRange(sizes);
            await db.SaveChangesAsync(cancellationToken);
            db.ChangeTracker.Clear();
        }

        // Категории/товары создаём, если их нет.
        if (!await db.Categories.AnyAsync(cancellationToken))
        {
            var categories = new List<Category>
            {
                new() { Id = Guid.NewGuid(), Gender = ProductGender.Women, Name = "Костюмы", Slug = "women-kostyumy", SortOrder = 10 },
                new() { Id = Guid.NewGuid(), Gender = ProductGender.Women, Name = "Топы", Slug = "women-topy", SortOrder = 20 },
                new() { Id = Guid.NewGuid(), Gender = ProductGender.Women, Name = "Брюки", Slug = "women-bryuki", SortOrder = 30 },
                new() { Id = Guid.NewGuid(), Gender = ProductGender.Women, Name = "Халаты", Slug = "women-halaty", SortOrder = 40 },

                new() { Id = Guid.NewGuid(), Gender = ProductGender.Men, Name = "Костюмы", Slug = "men-kostyumy", SortOrder = 10 },
                new() { Id = Guid.NewGuid(), Gender = ProductGender.Men, Name = "Топы", Slug = "men-topy", SortOrder = 20 },
                new() { Id = Guid.NewGuid(), Gender = ProductGender.Men, Name = "Брюки", Slug = "men-bryuki", SortOrder = 30 },
                new() { Id = Guid.NewGuid(), Gender = ProductGender.Men, Name = "Халаты", Slug = "men-halaty", SortOrder = 40 },
            };

            var products = new List<Product>
            {
                MakeProduct(categories[0], "Костюм COIN/JOE", "kostyum-coin-joe-1", 7800m, 7000m, 100, 0),
                MakeProduct(categories[0], "Костюм AeroForm", "kostyum-aeroform-1", 8600m, null, 90, 1),
                MakeProduct(categories[1], "Топ COIN/JOE", "top-coin-joe-1", 5200m, 4800m, 80, 2),
                MakeProduct(categories[2], "Брюки JOE", "bryuki-joe-1", 6200m, null, 75, 3),
                MakeProduct(categories[3], "Халат Comfort", "halat-comfort-1", 7900m, 7200m, 85, 4),

                MakeProduct(categories[4], "Костюм COIN/JOE (м)", "kostyum-coin-joe-men-1", 7900m, 7800m, 95, 0),
                MakeProduct(categories[5], "Топ COIN/JOE (м)", "top-coin-joe-men-1", 4800m, null, 70, 1),
                MakeProduct(categories[6], "Брюки JOE (м)", "bryuki-joe-men-1", 6100m, 5500m, 60, 2),
                MakeProduct(categories[7], "Халат Comfort (м)", "halat-comfort-men-1", 7500m, null, 65, 3),
            };

            db.Categories.AddRange(categories);
            db.Products.AddRange(products);
            await db.SaveChangesAsync(cancellationToken);
            db.ChangeTracker.Clear();
        }

        // Выставляем доступность размеров товарам, если ещё не добавляли.
        // Только FK, без навигаций — иначе EF может пометить уже tracked Size на удаление (Size = null).
        db.ChangeTracker.Clear();

        var allSizeIds = await db.Sizes.AsNoTracking()
            .OrderBy(s => s.SortOrder)
            .Select(s => s.Id)
            .ToListAsync(cancellationToken);

        var productIdsWithSizes = await db.ProductSizeOptions
            .AsNoTracking()
            .Select(so => so.ProductId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var productIdsNeedingSizes = await db.Products
            .AsNoTracking()
            .Where(p => p.IsActive && !productIdsWithSizes.Contains(p.Id))
            .Select(p => p.Id)
            .ToListAsync(cancellationToken);

        if (productIdsNeedingSizes.Count > 0 && allSizeIds.Count > 0)
        {
            var availableSizeOptionsToInsert = new List<ProductSizeOption>();
            foreach (var productId in productIdsNeedingSizes)
            {
                foreach (var sizeId in allSizeIds)
                {
                    availableSizeOptionsToInsert.Add(new ProductSizeOption
                    {
                        ProductId = productId,
                        SizeId = sizeId
                    });
                }
            }

            db.ProductSizeOptions.AddRange(availableSizeOptionsToInsert);
            await db.SaveChangesAsync(cancellationToken);
            db.ChangeTracker.Clear();
        }

        if (!await db.PromotionCodes.AnyAsync(p => p.Code == "WELCOME10", cancellationToken))
        {
            db.PromotionCodes.Add(new PromotionCode
            {
                Id = Guid.NewGuid(),
                Code = "WELCOME10",
                Type = PromotionDiscountType.Percent,
                Value = 10m,
                IsActive = true
            });
            await db.SaveChangesAsync(cancellationToken);
            db.ChangeTracker.Clear();
        }

        // Backfill composition / description / size chart for older seeds (not images)
        var productsToBackfill = await db.Products
            .Where(p => p.IsActive)
            .ToListAsync(cancellationToken);

        var backfillChanged = false;
        foreach (var p in productsToBackfill)
        {
            if (string.IsNullOrWhiteSpace(p.Material))
            {
                p.Material = "65% хлопок, 35% полиэстер";
                backfillChanged = true;
            }

            if (string.IsNullOrWhiteSpace(p.Description))
            {
                p.Description =
                    $"{p.Name} — медицинская униформа LifeUniform. Удобный крой, дышащая ткань, подходит для ежедневной работы в клинике.";
                backfillChanged = true;
            }

            if (string.IsNullOrWhiteSpace(p.SizeChartImageUrl))
            {
                p.SizeChartImageUrl = "/images/size-chart.svg";
                backfillChanged = true;
            }

            if (string.IsNullOrWhiteSpace(p.ShortName))
            {
                var first = p.Name.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? p.Name;
                p.ShortName = first.ToUpperInvariant();
                backfillChanged = true;
            }

            if (string.IsNullOrWhiteSpace(p.Sku))
            {
                p.Sku = $"SKU-{Math.Abs(p.Id.GetHashCode()) % 100000:D5}";
                backfillChanged = true;
            }

            if (string.IsNullOrWhiteSpace(p.CareInstructions))
            {
                p.CareInstructions = "Стирка при 40°C. Не отбеливать. Гладить при средней температуре. Не использовать барабанную сушку.";
                backfillChanged = true;
            }

            if (p.FreeShippingFrom is null)
            {
                p.FreeShippingFrom = 8000m;
                backfillChanged = true;
            }

            // Do not invent or overwrite product photos here — admin uploads and
            // seed MakeProduct own image assignment. Overwriting upload filenames
            // caused "random" images and DbUpdateConcurrencyException.
        }

        if (backfillChanged)
            await db.SaveChangesAsync(cancellationToken);

        db.ChangeTracker.Clear();

        // Цвета товаров — только INSERT по ProductId, без tracked Product
        // (иначе EF может слать UPDATE/DELETE по связанным сущностям → concurrency).
        var productIdsWithoutColors = await db.Products
            .AsNoTracking()
            .Where(p => p.IsActive && !p.ColorOptions.Any())
            .Select(p => p.Id)
            .ToListAsync(cancellationToken);

        if (productIdsWithoutColors.Count > 0)
        {
            var colorRows = new List<ProductColorOption>();
            foreach (var productId in productIdsWithoutColors)
            {
                var colorCount = 2 + Math.Abs(productId.GetHashCode()) % 3; // 2..4
                for (var i = 0; i < colorCount; i++)
                {
                    var palette = ColorPalette[(Math.Abs(productId.GetHashCode()) + i) % ColorPalette.Length];
                    var image = ProductImages[(Math.Abs(productId.GetHashCode()) + i + 1) % ProductImages.Length];
                    colorRows.Add(new ProductColorOption
                    {
                        Id = Guid.NewGuid(),
                        ProductId = productId,
                        Name = palette.Name,
                        Hex = palette.Hex,
                        ImageFileName = image,
                        SortOrder = (i + 1) * 10
                    });
                }
            }

            db.ProductColorOptions.AddRange(colorRows);
            await db.SaveChangesAsync(cancellationToken);
            db.ChangeTracker.Clear();
        }

        // Замена старых цветов палитры, которые плохо читаются в UI
        var recolored = await db.ProductColorOptions
            .Where(c => c.Hex == "#0d9488" || c.Hex == "#c2a878" || c.Name == "Бирюзовый" || c.Name == "Песочный")
            .ToListAsync(cancellationToken);

        if (recolored.Count > 0)
        {
            foreach (var c in recolored)
            {
                if (c.Name is "Бирюзовый" || c.Hex == "#0d9488")
                {
                    c.Name = "Сиреневый";
                    c.Hex = "#c4a4c8";
                }
                else if (c.Name is "Песочный" || c.Hex == "#c2a878")
                {
                    c.Name = "Шалфей";
                    c.Hex = "#8a9a7b";
                }
            }

            await db.SaveChangesAsync(cancellationToken);
            db.ChangeTracker.Clear();
        }

        // Backfill color×size availability for products that don't have a matrix yet.
        var productIdsWithStock = await db.ProductColorSizeStocks
            .AsNoTracking()
            .Select(s => s.ProductId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var productsNeedingStock = await db.Products
            .AsNoTracking()
            .Where(p => p.IsActive && !productIdsWithStock.Contains(p.Id))
            .Select(p => p.Id)
            .ToListAsync(cancellationToken);

        if (productsNeedingStock.Count > 0)
        {
            var colorPairs = await db.ProductColorOptions
                .AsNoTracking()
                .Where(c => productsNeedingStock.Contains(c.ProductId))
                .Select(c => new { c.ProductId, c.Name })
                .ToListAsync(cancellationToken);

            var sizePairs = await db.ProductSizeOptions
                .AsNoTracking()
                .Where(s => productsNeedingStock.Contains(s.ProductId))
                .Select(s => new { s.ProductId, s.SizeId })
                .ToListAsync(cancellationToken);

            var stockRows = new List<ProductColorSizeStock>();
            foreach (var productId in productsNeedingStock)
            {
                var colors = colorPairs.Where(c => c.ProductId == productId).Select(c => c.Name).Distinct().ToList();
                var sizes = sizePairs.Where(s => s.ProductId == productId).Select(s => s.SizeId).Distinct().ToList();
                if (colors.Count == 0 || sizes.Count == 0)
                    continue;

                foreach (var color in colors)
                {
                    foreach (var sizeId in sizes)
                    {
                        stockRows.Add(new ProductColorSizeStock
                        {
                            ProductId = productId,
                            ColorName = color,
                            SizeId = sizeId,
                            IsInStock = true
                        });
                    }
                }
            }

            if (stockRows.Count > 0)
            {
                db.ProductColorSizeStocks.AddRange(stockRows);
                await db.SaveChangesAsync(cancellationToken);
                db.ChangeTracker.Clear();
            }
        }

        // Промо-офферы на главной
        if (!await db.PromoOffers.AnyAsync(cancellationToken))
        {
            db.PromoOffers.AddRange(
                new PromoOffer
                {
                    Id = Guid.NewGuid(),
                    Title = "Спецпредложение",
                    Subtitle = "Скидки на популярные модели медицинской формы",
                    Badge = "−15%",
                    ImageUrl = "/images/hero-banner.png",
                    LinkUrl = "/Catalog",
                    LinkText = "Смотреть каталог",
                    SortOrder = 10,
                    IsActive = true
                },
                new PromoOffer
                {
                    Id = Guid.NewGuid(),
                    Title = "Женская коллекция",
                    Subtitle = "Новые костюмы и топы для клиник",
                    Badge = "Новинки",
                    ImageUrl = "/images/women-collection.png",
                    LinkUrl = "/Catalog?gender=Women",
                    LinkText = "В каталог",
                    SortOrder = 20,
                    IsActive = true
                },
                new PromoOffer
                {
                    Id = Guid.NewGuid(),
                    Title = "Мужская коллекция",
                    Subtitle = "Удобные комплекты на каждый день",
                    Badge = "Хит",
                    ImageUrl = "/images/men-collection.png",
                    LinkUrl = "/Catalog?gender=Men",
                    LinkText = "В каталог",
                    SortOrder = 30,
                    IsActive = true
                });
            await db.SaveChangesAsync(cancellationToken);
        }
    }

    private static Product MakeProduct(
        Category category,
        string name,
        string slug,
        decimal price,
        decimal? discountPrice,
        int popularity,
        int imageIndex)
    {
        var imagePath = ProductImages[imageIndex % ProductImages.Length];
        var shortName = name.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? name;
        var p = new Product
        {
            Id = Guid.NewGuid(),
            Gender = category.Gender,
            CategoryId = category.Id,
            Category = category,
            Name = name,
            ShortName = shortName.ToUpperInvariant(),
            Slug = slug,
            Sku = $"LU-{Math.Abs(slug.GetHashCode()) % 100000:D5}",
            Description =
                $"{name} — медицинская униформа LifeUniform. Удобный крой, дышащая ткань, подходит для ежедневной работы в клинике.",
            Material = "65% хлопок, 35% полиэстер",
            CareInstructions = "Стирка при 40°C. Не отбеливать. Гладить при средней температуре. Не использовать барабанную сушку.",
            SizeChartImageUrl = "/images/size-chart.svg",
            Price = price,
            DiscountPrice = discountPrice,
            IsActive = true,
            IsInStock = true,
            FreeShippingFrom = 8000m,
            PopularityRank = popularity,
            Images = new List<ProductImage>()
        };

        p.Images.Add(new ProductImage
        {
            Id = Guid.NewGuid(),
            ProductId = p.Id,
            Product = p,
            FileNameOriginal = imagePath,
            FileNamePreview = imagePath,
            FileNameWebp = imagePath,
            AltText = name,
            IsPrimary = true,
            SortOrder = 1
        });

        return p;
    }
}
