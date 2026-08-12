using System.Security.Claims;
using LifeUniform.Application.Orders.Dto;
using LifeUniform.Application.Orders.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LifeUniform.Web.Pages.Account;

[Authorize]
public class OrdersModel : PageModel
{
    private readonly IMediator _mediator;

    public OrdersModel(IMediator mediator) => _mediator = mediator;

    public IReadOnlyList<OrderDto> Vm { get; private set; } = Array.Empty<OrderDto>();

    public async Task OnGetAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        Vm = await _mediator.Send(new GetOrdersForUserQuery { UserId = userId });
    }
}
