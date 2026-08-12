using FluentValidation;
using FluentValidation.AspNetCore;
using LifeUniform.Application.Abstractions.Caching;
using LifeUniform.Application.Abstractions.Delivery;
using LifeUniform.Application.Abstractions.Erp;
using LifeUniform.Application.Abstractions.Images;
using LifeUniform.Application.Abstractions.Payment;
using LifeUniform.Application.Catalog.Queries;
using LifeUniform.Application.Common.Behaviors;
using LifeUniform.Domain.Promotions;
using LifeUniform.Infrastructure.Caching;
using LifeUniform.Infrastructure.Delivery;
using LifeUniform.Infrastructure.Erp;
using LifeUniform.Infrastructure.Payment;
using LifeUniform.Infrastructure.Promotions;
using LifeUniform.Infrastructure.Storage;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LifeUniform.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Razor Pages
        builder.Services.AddRazorPages();

        // Postgres + EF Core
        builder.Services.AddDbContext<LifeUniform.Infrastructure.Persistence.ApplicationDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

        // Identity (registration/login + roles)
        builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<LifeUniform.Infrastructure.Persistence.ApplicationDbContext>()
            .AddDefaultTokenProviders()
            .AddDefaultUI();

        builder.Services.AddScoped<LifeUniform.Web.Services.CartMergeAuthenticationEvents>();
        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Auth";
            options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            options.EventsType = typeof(LifeUniform.Web.Services.CartMergeAuthenticationEvents);
        });

        // AuthZ
        builder.Services.AddAuthorization();

        // Session cart
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession(options =>
        {
            options.Cookie.Name = ".LifeUniform.Session";
            options.IdleTimeout = TimeSpan.FromDays(14);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<LifeUniform.Domain.Cart.ICartService, LifeUniform.Web.Services.SessionCartService>();

        // CQRS + validation pipeline
        builder.Services.AddMemoryCache();
        builder.Services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(GetCatalogHomeQuery).Assembly));
        builder.Services.AddValidatorsFromAssembly(typeof(GetCatalogHomeQuery).Assembly);
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        // Mapping
        // Repositories & adapters
        builder.Services.AddScoped<LifeUniform.Domain.Catalog.ICatalogRepository, LifeUniform.Infrastructure.Catalog.CatalogRepository>();
        builder.Services.AddScoped<LifeUniform.Domain.Orders.IOrderRepository, LifeUniform.Infrastructure.Orders.OrderRepository>();
        builder.Services.AddScoped<IPromotionRepository, PromotionRepository>();
        builder.Services.AddScoped<LifeUniform.Domain.Marketing.IPromoOfferRepository, LifeUniform.Infrastructure.Marketing.PromoOfferRepository>();
        builder.Services.AddScoped<IImageStorage, DiskImageStorage>();
        builder.Services.AddScoped<ICatalogCacheInvalidator, CatalogCacheInvalidator>();
        builder.Services.AddScoped<IPaymentFacade, StubPaymentFacade>();
        builder.Services.AddScoped<IDeliveryCalculator, StubDeliveryCalculator>();
        builder.Services.AddScoped<IErpCatalogImporter, StubErpCatalogImporter>();
        builder.Services.AddScoped<IErpOrderExporter, StubErpOrderExporter>();
        builder.Services.AddScoped<IErpSyncService, StubErpSyncService>();

        // FluentValidation (Razor Pages)
        builder.Services.AddFluentValidationAutoValidation();

        var app = builder.Build();

        // Dev-seed для первого запуска.
        if (app.Environment.IsDevelopment())
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<LifeUniform.Infrastructure.Persistence.ApplicationDbContext>();
            db.Database.Migrate();

            SeedDevData(scope.ServiceProvider).GetAwaiter().GetResult();
        }

        static async Task SeedDevData(IServiceProvider serviceProvider)
        {
            // 1) Identity (роли + dev-admin)
            using (var innerScope = serviceProvider.CreateScope())
            {
                var roleManager = innerScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = innerScope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                await EnsureRoleAsync(roleManager, "Admin");
                await EnsureRoleAsync(roleManager, "Customer");

                // Dev admin: email/password берём из env, чтобы не хардкодить пароли.
                var adminEmail = Environment.GetEnvironmentVariable("LIFEUNIFORM_ADMIN_EMAIL") ?? "admin@lifeuniform.local";
                var adminPassword = Environment.GetEnvironmentVariable("LIFEUNIFORM_ADMIN_PASSWORD") ?? "Admin123!";

                var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
                if (existingAdmin is null)
                {
                    var adminUser = new IdentityUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true
                    };

                    var createResult = await userManager.CreateAsync(adminUser, adminPassword);
                    if (createResult.Succeeded)
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // 2) Каталог + промо — отдельный scope, чистый DbContext
            using (var catalogScope = serviceProvider.CreateScope())
            {
                var db = catalogScope.ServiceProvider
                    .GetRequiredService<LifeUniform.Infrastructure.Persistence.ApplicationDbContext>();
                await LifeUniform.Infrastructure.Persistence.SeedData.SeedAsync(db, CancellationToken.None);
            }
        }

        static async Task EnsureRoleAsync(RoleManager<IdentityRole> roleManager, string roleName)
        {
            if (await roleManager.RoleExistsAsync(roleName))
                return;

            await roleManager.CreateAsync(new IdentityRole(roleName));
        }

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios. See https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseSession();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseStaticFiles();

        app.MapStaticAssets();
        app.MapRazorPages()
           .WithStaticAssets();

        app.Run();
    }
}
