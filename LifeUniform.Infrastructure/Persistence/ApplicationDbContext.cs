using LifeUniform.Domain.Catalog;
using LifeUniform.Domain.Marketing;
using LifeUniform.Domain.Orders;
using LifeUniform.Domain.Promotions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LifeUniform.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<ProductColorOption> ProductColorOptions => Set<ProductColorOption>();
    public DbSet<ProductColorSizeStock> ProductColorSizeStocks => Set<ProductColorSizeStock>();
    public DbSet<Size> Sizes => Set<Size>();
    public DbSet<ProductSizeOption> ProductSizeOptions => Set<ProductSizeOption>();
    public DbSet<UserFavoriteProduct> UserFavoriteProducts => Set<UserFavoriteProduct>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<PromotionCode> PromotionCodes => Set<PromotionCode>();
    public DbSet<PromoOffer> PromoOffers => Set<PromoOffer>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Category>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).HasMaxLength(200).IsRequired();
            b.Property(x => x.Slug).HasMaxLength(200).IsRequired();
            b.Property(x => x.Description).HasMaxLength(2000);

            b.HasIndex(x => new { x.Gender, x.Slug }).IsUnique();

            b.HasMany(x => x.Products)
                .WithOne(x => x.Category)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Product>(b =>
        {
            b.HasKey(x => x.Id);

            b.Property(x => x.Name).HasMaxLength(250).IsRequired();
            b.Property(x => x.ShortName).HasMaxLength(100);
            b.Property(x => x.Slug).HasMaxLength(250).IsRequired();
            b.HasIndex(x => x.Slug).IsUnique();
            b.Property(x => x.Sku).HasMaxLength(50);
            b.HasIndex(x => x.Sku);

            b.Property(x => x.Description).HasMaxLength(4000);
            b.Property(x => x.Material).HasMaxLength(500);
            b.Property(x => x.CareInstructions).HasMaxLength(2000);
            b.Property(x => x.SizeChartImageUrl).HasMaxLength(500);
            b.Property(x => x.FreeShippingFrom).HasPrecision(18, 2);

            b.Property(x => x.Price).HasPrecision(18, 2).IsRequired();
            b.Property(x => x.DiscountPrice).HasPrecision(18, 2);

            b.HasMany(x => x.Images)
                .WithOne(x => x.Product)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(x => x.ColorOptions)
                .WithOne(x => x.Product)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(x => x.ColorSizeStocks)
                .WithOne(x => x.Product)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<ProductImage>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.FileNameOriginal).HasMaxLength(500).IsRequired();
            b.Property(x => x.FileNamePreview).HasMaxLength(500).IsRequired();
            b.Property(x => x.FileNameWebp).HasMaxLength(500).IsRequired();
            b.Property(x => x.AltText).HasMaxLength(500);
            b.HasIndex(x => new { x.ProductId, x.IsPrimary });
        });

        builder.Entity<ProductColorOption>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).HasMaxLength(100).IsRequired();
            b.Property(x => x.Hex).HasMaxLength(20).IsRequired();
            b.Property(x => x.ImageFileName).HasMaxLength(500);
            b.HasIndex(x => new { x.ProductId, x.SortOrder });
        });

        builder.Entity<ProductColorSizeStock>(b =>
        {
            b.HasKey(x => new { x.ProductId, x.ColorName, x.SizeId });
            b.Property(x => x.ColorName).HasMaxLength(100).IsRequired();

            b.HasOne(x => x.Size)
                .WithMany()
                .HasForeignKey(x => x.SizeId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(x => new { x.ProductId, x.ColorName });
        });

        builder.Entity<PromoOffer>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Title).HasMaxLength(200).IsRequired();
            b.Property(x => x.Subtitle).HasMaxLength(500);
            b.Property(x => x.Badge).HasMaxLength(100);
            b.Property(x => x.ImageUrl).HasMaxLength(500).IsRequired();
            b.Property(x => x.LinkUrl).HasMaxLength(500);
            b.Property(x => x.LinkText).HasMaxLength(100);
            b.Property(x => x.Price).HasPrecision(18, 2);
            b.Property(x => x.OldPrice).HasPrecision(18, 2);
            b.HasIndex(x => new { x.IsActive, x.SortOrder });
        });

        builder.Entity<Size>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Label).HasMaxLength(20).IsRequired();
            b.HasIndex(x => x.Label).IsUnique();
        });

        builder.Entity<ProductSizeOption>(b =>
        {
            b.HasKey(x => new { x.ProductId, x.SizeId });

            b.HasOne(x => x.Product)
                .WithMany(p => p.SizeOptions)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Size)
                .WithMany()
                .HasForeignKey(x => x.SizeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<UserFavoriteProduct>(b =>
        {
            b.HasKey(x => new { x.UserId, x.ProductId });
            b.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Order>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Number).HasMaxLength(40).IsRequired();
            b.HasIndex(x => x.Number).IsUnique();
            b.HasIndex(x => x.PaymentToken).IsUnique();

            b.Property(x => x.CustomerName).HasMaxLength(200).IsRequired();
            b.Property(x => x.CustomerPhone).HasMaxLength(50).IsRequired();
            b.Property(x => x.CustomerEmail).HasMaxLength(200).IsRequired();
            b.Property(x => x.DeliveryAddress).HasMaxLength(500).IsRequired();
            b.Property(x => x.PaymentToken).HasMaxLength(100).IsRequired();
            b.Property(x => x.PromoCode).HasMaxLength(50);
            b.Property(x => x.DiscountAmount).HasPrecision(18, 2);

            b.Property(x => x.ItemsTotal).HasPrecision(18, 2);
            b.Property(x => x.DeliveryFee).HasPrecision(18, 2);
            b.Property(x => x.GrandTotal).HasPrecision(18, 2);

            b.HasMany(x => x.Items)
                .WithOne(x => x.Order)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<OrderItem>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.ProductName).HasMaxLength(250).IsRequired();
            b.Property(x => x.ProductSlug).HasMaxLength(250).IsRequired();
            b.Property(x => x.SizeLabel).HasMaxLength(20).IsRequired();
            b.Property(x => x.UnitPrice).HasPrecision(18, 2);
        });

        builder.Entity<PromotionCode>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Code).HasMaxLength(50).IsRequired();
            b.HasIndex(x => x.Code).IsUnique();
            b.Property(x => x.Value).HasPrecision(18, 2);
            b.Property(x => x.MinOrderAmount).HasPrecision(18, 2);
        });
    }
}

