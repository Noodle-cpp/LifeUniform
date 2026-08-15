using System.Globalization;
using System.Security.Claims;
using LifeUniform.Application.Orders.Dto;
using LifeUniform.Application.Orders.Queries;
using LifeUniform.Domain.Cart;
using LifeUniform.Domain.Orders;
using LifeUniform.Web.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LifeUniform.Web.Pages.Account;

[Authorize]
public class IndexModel : PageModel
{
    private static readonly CultureInfo Ru = CultureInfo.GetCultureInfo("ru-RU");

    private readonly IMediator _mediator;
    private readonly ICartService _cart;
    private readonly IFavoriteState _favorites;

    public IndexModel(IMediator mediator, ICartService cart, IFavoriteState favorites)
    {
        _mediator = mediator;
        _cart = cart;
        _favorites = favorites;
    }

    public string? DisplayName { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public int OrderCount { get; private set; }
    public int FavoriteCount { get; private set; }
    public int CartCount { get; private set; }
    public decimal CartTotal { get; private set; }
    public OrderDto? LastOrder { get; private set; }

    public string Greeting => string.IsNullOrWhiteSpace(DisplayName)
        ? "Здравствуйте"
        : $"Здравствуйте, {DisplayName}";

    public string OrdersLabel => RuCount(OrderCount, "заказ", "заказа", "заказов");
    public string FavoritesLabel => FavoriteCount == 0
        ? "Пока пусто"
        : RuCount(FavoriteCount, "товар", "товара", "товаров");
    public string CartLabel => CartCount == 0
        ? "Корзина пуста"
        : $"{RuCount(CartCount, "товар", "товара", "товаров")} · {CartTotal.ToString("N0", Ru)} ₽";

    public string? LastOrderDate => LastOrder?.CreatedAt.ToLocalTime().ToString("d MMMM yyyy", Ru);
    public string? LastOrderStatus => LastOrder is null ? null : StatusLabel(LastOrder.Status);

    public async Task OnGetAsync()
    {
        Email = User.Identity?.Name ?? string.Empty;
        var given = User.FindFirstValue(ClaimTypes.GivenName);
        DisplayName = string.IsNullOrWhiteSpace(given) ? null : given.Trim();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        var orders = await _mediator.Send(new GetOrdersForUserQuery { UserId = userId });
        OrderCount = orders.Count;
        LastOrder = orders.Count == 0
            ? null
            : orders.OrderByDescending(o => o.CreatedAt).First();

        FavoriteCount = (await _favorites.GetIdsAsync(HttpContext.RequestAborted)).Count;
        var cartItems = _cart.GetItems();
        CartCount = cartItems.Sum(x => x.Quantity);
        CartTotal = _cart.GetItemsTotal();
    }

    private static string RuCount(int n, string one, string few, string many)
    {
        var n10 = n % 10;
        var n100 = n % 100;
        if (n10 == 1 && n100 != 11) return $"{n} {one}";
        if (n10 is >= 2 and <= 4 && n100 is < 12 or > 14) return $"{n} {few}";
        return $"{n} {many}";
    }

    private static string StatusLabel(OrderStatus status) => status switch
    {
        OrderStatus.PendingPayment => "Ожидает оплаты",
        OrderStatus.Paid => "Оплачен",
        OrderStatus.Shipped => "Отправлен",
        OrderStatus.Cancelled => "Отменён",
        _ => status.ToString()
    };
}
