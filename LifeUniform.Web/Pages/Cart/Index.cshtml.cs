using LifeUniform.Application.Orders.Commands;
using LifeUniform.Application.Orders.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LifeUniform.Web.Pages.Cart;

public class IndexModel : PageModel
{
    private readonly IMediator _mediator;

    public IndexModel(IMediator mediator) => _mediator = mediator;

    public LifeUniform.Application.Orders.Dto.CartDto Vm { get; private set; } = new();

    public async Task OnGetAsync()
    {
        Vm = await _mediator.Send(new GetCartQuery());
    }

    public async Task<IActionResult> OnPostUpdateAsync(Guid productId, Guid sizeId, int quantity, string? colorName = null)
    {
        await _mediator.Send(new UpdateCartItemCommand
        {
            ProductId = productId,
            SizeId = sizeId,
            ColorName = colorName,
            Quantity = quantity
        });
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRemoveAsync(Guid productId, Guid sizeId, string? colorName = null)
    {
        await _mediator.Send(new RemoveCartItemCommand
        {
            ProductId = productId,
            SizeId = sizeId,
            ColorName = colorName
        });
        return RedirectToPage();
    }
}
