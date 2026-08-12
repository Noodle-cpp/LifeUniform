using LifeUniform.Application.Catalog.Queries;
using LifeUniform.Application.Marketing.Queries;
using LifeUniform.Application.Orders.Dto;
using LifeUniform.Application.Orders.Queries;
using LifeUniform.Application.Promotions.Queries;
using LifeUniform.Domain.Orders;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LifeUniform.Web.Areas.Admin.Pages;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly IMediator _mediator;

    public IndexModel(IMediator mediator) => _mediator = mediator;

    public int ProductsCount { get; private set; }
    public int CategoriesCount { get; private set; }
    public int OrdersCount { get; private set; }
    public int PendingOrdersCount { get; private set; }
    public int ActivePromosCount { get; private set; }
    public int ActiveOffersCount { get; private set; }
    public decimal OrdersRevenue { get; private set; }
    public IReadOnlyList<OrderDto> RecentOrders { get; private set; } = Array.Empty<OrderDto>();

    public async Task OnGetAsync()
    {
        var products = await _mediator.Send(new GetProductsForAdminQuery());
        var categories = await _mediator.Send(new GetCategoriesForAdminQuery());
        var orders = await _mediator.Send(new GetOrdersForAdminQuery());
        var promos = await _mediator.Send(new GetPromotionsForAdminQuery());
        var offers = await _mediator.Send(new GetPromoOffersForAdminQuery());

        ProductsCount = products.Count;
        CategoriesCount = categories.Count;
        OrdersCount = orders.Count;
        PendingOrdersCount = orders.Count(o => o.Status == OrderStatus.PendingPayment);
        ActivePromosCount = promos.Count(p => p.IsActive);
        ActiveOffersCount = offers.Count(o => o.IsActive);
        OrdersRevenue = orders
            .Where(o => o.Status is OrderStatus.Paid or OrderStatus.Shipped)
            .Sum(o => o.GrandTotal);
        RecentOrders = orders.OrderByDescending(o => o.CreatedAt).Take(6).ToList();
    }
}
