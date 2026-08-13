using System.Security.Claims;
using System.Text.Json;
using LifeUniform.Application.Catalog.Dto;
using LifeUniform.Domain.Catalog;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;

namespace LifeUniform.Web.Services;

/// <summary>
/// Избранное: у авторизованных — в БД, у гостей — session + cookie.
/// </summary>
public class SessionFavoriteService : IFavoriteState
{
    private const string SessionKey = "lifeuniform.favorites";
    private const string CookieName = "lu.fav";
    private readonly IHttpContextAccessor _http;
    private readonly ICatalogRepository _catalog;
    private readonly IDataProtector _protector;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public SessionFavoriteService(
        IHttpContextAccessor http,
        ICatalogRepository catalog,
        IDataProtectionProvider dataProtection)
    {
        _http = http;
        _catalog = catalog;
        _protector = dataProtection.CreateProtector("LifeUniform.Favorites.v1");
    }

    public IReadOnlyCollection<Guid> GetGuestIds() => LoadGuest().AsReadOnly();

    public async Task<IReadOnlyCollection<Guid>> GetIdsAsync(CancellationToken cancellationToken)
    {
        var userId = UserId();
        if (!string.IsNullOrWhiteSpace(userId))
            return await _catalog.GetFavoriteProductIdsAsync(userId, cancellationToken);

        return GetGuestIds();
    }

    public async Task<bool> ToggleAsync(Guid productId, CancellationToken cancellationToken)
    {
        var userId = UserId();
        if (!string.IsNullOrWhiteSpace(userId))
            return await _catalog.ToggleProductFavoriteAsync(userId, productId, cancellationToken);

        var items = LoadGuest();
        var added = !items.Contains(productId);
        if (added)
            items.Add(productId);
        else
            items.Remove(productId);

        SaveGuest(items);
        return added;
    }

    public async Task MergeGuestIntoUserAsync(string userId, CancellationToken cancellationToken)
    {
        var guestIds = LoadGuest();
        if (guestIds.Count == 0)
            return;

        foreach (var id in guestIds)
        {
            if (!await _catalog.IsProductFavoriteAsync(userId, id, cancellationToken))
                await _catalog.ToggleProductFavoriteAsync(userId, id, cancellationToken);
        }

        SaveGuest(new List<Guid>());
    }

    public void ApplyGuest(IEnumerable<ProductCardDto> products)
    {
        var ids = GetGuestIds();
        foreach (var p in products)
            p.IsFavorite = ids.Contains(p.Id);
    }

    private string? UserId() =>
        _http.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

    private List<Guid> LoadGuest()
    {
        var session = _http.HttpContext?.Session;
        if (session is not null)
        {
            var json = session.GetString(SessionKey);
            if (!string.IsNullOrWhiteSpace(json))
                return JsonSerializer.Deserialize<List<Guid>>(json, JsonOptions) ?? new List<Guid>();
        }

        var cookieItems = ReadCookie();
        if (cookieItems.Count > 0 && session is not null)
            SaveGuest(cookieItems);

        return cookieItems;
    }

    private void SaveGuest(List<Guid> items)
    {
        var ctx = _http.HttpContext;
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

    private List<Guid> ReadCookie()
    {
        var ctx = _http.HttpContext;
        if (ctx is null || !ctx.Request.Cookies.TryGetValue(CookieName, out var raw) || string.IsNullOrWhiteSpace(raw))
            return new List<Guid>();

        try
        {
            var json = _protector.Unprotect(raw);
            return JsonSerializer.Deserialize<List<Guid>>(json, JsonOptions) ?? new List<Guid>();
        }
        catch
        {
            return new List<Guid>();
        }
    }
}
