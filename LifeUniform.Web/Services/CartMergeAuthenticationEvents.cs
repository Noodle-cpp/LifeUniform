using LifeUniform.Domain.Cart;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace LifeUniform.Web.Services;

/// <summary>
/// После входа восстанавливает корзину из cookie и переносит гостевое избранное в аккаунт.
/// </summary>
public class CartMergeAuthenticationEvents : CookieAuthenticationEvents
{
    public override async Task SignedIn(CookieSignedInContext context)
    {
        await base.SignedIn(context);

        var cart = context.HttpContext.RequestServices.GetService(typeof(ICartService)) as ICartService;
        if (cart is not null)
            _ = cart.GetItems();

        var userId = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
        var favorites = context.HttpContext.RequestServices.GetService<IFavoriteState>();
        if (favorites is not null && !string.IsNullOrWhiteSpace(userId))
            await favorites.MergeGuestIntoUserAsync(userId, context.HttpContext.RequestAborted);
    }

    public override async Task SigningOut(CookieSigningOutContext context)
    {
        var cart = context.HttpContext.RequestServices.GetService(typeof(ICartService)) as ICartService;
        cart?.Clear();

        await base.SigningOut(context);
    }
}

