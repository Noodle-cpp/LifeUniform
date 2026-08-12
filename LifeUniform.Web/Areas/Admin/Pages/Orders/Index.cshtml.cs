using LifeUniform.Application.Orders.Commands;
using LifeUniform.Application.Orders.Dto;
using LifeUniform.Application.Orders.Queries;
using LifeUniform.Domain.Orders;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LifeUniform.Web.Areas.Admin.Pages.Orders;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly IMediator _mediator;

    public IndexModel(IMediator mediator) => _mediator = mediator;

    public IReadOnlyList<OrderDto> Vm { get; private set; } = Array.Empty<OrderDto>();

    public async Task OnGetAsync()
    {
        Vm = await _mediator.Send(new GetOrdersForAdminQuery());
    }

    public async Task<IActionResult> OnPostStatusAsync(Guid orderId, OrderStatus status)
    {
        await _mediator.Send(new UpdateOrderStatusCommand
        {
            OrderId = orderId,
            Status = status
        });
        return RedirectToPage();
    }
}
