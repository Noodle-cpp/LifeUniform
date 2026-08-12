using System.Text.Json;
using LifeUniform.Domain.Cart;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;

namespace LifeUniform.Web.Services;

/// <summary>
/// Корзина в session + cookie-бэкап, чтобы пережить регенерацию session при логине.
/// </summary>
public class SessionCartService : ICartService
{
    private const string SessionKey = "lifeuniform.cart";
    private const string CookieName = "lu.cart";
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDataProtector _protector;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public SessionCartService(IHttpContextAccessor httpContextAccessor, IDataProtectionProvider dataProtection)
    {
        _httpContextAccessor = httpContextAccessor;
        _protector = dataProtection.CreateProtector("LifeUniform.Cart.v1");
    }

    public IReadOnlyList<CartLine> GetItems() => Load().AsReadOnly();

    public void AddOrUpdate(CartLine line)
    {
        var items = Load();
        var existing = items.FirstOrDefault(x =>
            x.ProductId == line.ProductId
            && x.SizeId == line.SizeId
            && string.Equals(x.ColorName ?? "", line.ColorName ?? "", StringComparison.OrdinalIgnoreCase));
        if (existing is null)
            items.Add(line);
        else
        {
            existing.Quantity += line.Quantity;
            if (existing.Quantity < 1)
                existing.Quantity = 1;
        }

        Save(items);
    }

    public void Merge(IEnumerable<CartLine> lines)
    {
        var items = Load();
        foreach (var line in lines)
        {
            var existing = items.FirstOrDefault(x =>
                x.ProductId == line.ProductId
                && x.SizeId == line.SizeId
                && string.Equals(x.ColorName ?? "", line.ColorName ?? "", StringComparison.OrdinalIgnoreCase));
            if (existing is null)
            {
                items.Add(new CartLine
                {
                    ProductId = line.ProductId,
                    ProductName = line.ProductName,
                    ProductSlug = line.ProductSlug,
                    PreviewImageUrl = line.PreviewImageUrl,
                    ColorName = line.ColorName,
                    SizeId = line.SizeId,
                    SizeLabel = line.SizeLabel,
                    UnitPrice = line.UnitPrice,
                    Quantity = line.Quantity < 1 ? 1 : line.Quantity
                });
            }
            else
            {
                existing.Quantity = Math.Max(existing.Quantity, line.Quantity);
                if (existing.Quantity < 1)
                    existing.Quantity = 1;
            }
        }

        Save(items);
    }

    public void UpdateQuantity(Guid productId, Guid sizeId, int quantity, string? colorName = null)
    {
        var items = Load();
        var existing = items.FirstOrDefault(x =>
            x.ProductId == productId
            && x.SizeId == sizeId
            && string.Equals(x.ColorName ?? "", colorName ?? "", StringComparison.OrdinalIgnoreCase));
        if (existing is null)
            return;

        if (quantity <= 0)
            items.Remove(existing);
        else
            existing.Quantity = quantity;

        Save(items);
    }

    public void Remove(Guid productId, Guid sizeId, string? colorName = null)
    {
        var items = Load();
        items.RemoveAll(x =>
            x.ProductId == productId
            && x.SizeId == sizeId
            && string.Equals(x.ColorName ?? "", colorName ?? "", StringComparison.OrdinalIgnoreCase));
        Save(items);
    }

    public void Clear()
    {
        var ctx = _httpContextAccessor.HttpContext;
        if (ctx is null)
            return;

        // Empty session first so Load() won't revive items from the request cookie.
        ctx.Session?.SetString(SessionKey, "[]");
        ctx.Response.Cookies.Delete(CookieName, new CookieOptions
        {
            Path = "/",
            Secure = ctx.Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            HttpOnly = true,
            IsEssential = true
        });
    }

    public int GetItemCount() => Load().Sum(x => x.Quantity);

    public decimal GetItemsTotal() => Load().Sum(x => x.UnitPrice * x.Quantity);

    private List<CartLine> Load()
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        if (session is not null)
        {
            var json = session.GetString(SessionKey);
            if (!string.IsNullOrWhiteSpace(json))
                return JsonSerializer.Deserialize<List<CartLine>>(json, JsonOptions) ?? new List<CartLine>();
        }

        // Fallback: cookie (после логина / смены session id)
        var cookieItems = ReadCookie();
        if (cookieItems.Count > 0 && session is not null)
            Save(cookieItems);

        return cookieItems;
    }

    private void Save(List<CartLine> items)
    {
        var ctx = _httpContextAccessor.HttpContext;
        if (ctx is null)
            return;

        var json = JsonSerializer.Serialize(items, JsonOptions);
        ctx.Session?.SetString(SessionKey, json);

        var protectedPayload = _protector.Protect(json);
        ctx.Response.Cookies.Append(CookieName, protectedPayload, new CookieOptions
        {
            HttpOnly = true,
            IsEssential = true,
            Secure = ctx.Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            MaxAge = TimeSpan.FromDays(14),
            Path = "/"
        });
    }

    private List<CartLine> ReadCookie()
    {
        var ctx = _httpContextAccessor.HttpContext;
        if (ctx is null || !ctx.Request.Cookies.TryGetValue(CookieName, out var raw) || string.IsNullOrWhiteSpace(raw))
            return new List<CartLine>();

        try
        {
            var json = _protector.Unprotect(raw);
            return JsonSerializer.Deserialize<List<CartLine>>(json, JsonOptions) ?? new List<CartLine>();
        }
        catch
        {
            return new List<CartLine>();
        }
    }
}
