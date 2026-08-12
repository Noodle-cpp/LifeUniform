using LifeUniform.Domain.Cart;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace LifeUniform.Web.Services;

/// <summary>
/// После входа восстанавливает корзину из cookie; при выходе очищает её.
/// </summary>
public class CartMergeAuthenticationEvents : CookieAuthenticationEvents
{
    public override async Task SignedIn(CookieSignedInContext context)
    {
        await base.SignedIn(context);

        var cart = context.HttpContext.RequestServices.GetService(typeof(ICartService)) as ICartService;
        if (cart is null)
            return;

        // GetItems() подтянет cookie fallback и запишет обратно в session.
        _ = cart.GetItems();
    }

    public override async Task SigningOut(CookieSigningOutContext context)
    {
        var cart = context.HttpContext.RequestServices.GetService(typeof(ICartService)) as ICartService;
        cart?.Clear();

        await base.SigningOut(context);
    }
}

