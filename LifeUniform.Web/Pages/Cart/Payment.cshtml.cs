using LifeUniform.Application.Orders.Commands;
using LifeUniform.Application.Orders.Dto;
using LifeUniform.Application.Orders.Queries;
using LifeUniform.Domain.Orders;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LifeUniform.Web.Pages.Cart;

public class PaymentModel : PageModel
{
    private readonly IMediator _mediator;

    public PaymentModel(IMediator mediator) => _mediator = mediator;

    public OrderDto? Order { get; private set; }
    public bool IsNotFound { get; private set; }
    public string? Message { get; private set; }

    public async Task<IActionResult> OnGetAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            IsNotFound = true;
            return Page();
        }

        Order = await _mediator.Send(new GetOrderByPaymentTokenQuery { PaymentToken = token });
        IsNotFound = Order is null;
        return Page();
    }

    public async Task<IActionResult> OnPostConfirmAsync(string token)
    {
        try
        {
            await _mediator.Send(new ConfirmPaymentCommand { PaymentToken = token });
            Order = await _mediator.Send(new GetOrderByPaymentTokenQuery { PaymentToken = token });
            Message = "Оплата подтверждена (заглушка). Заказ переведён в статус «Оплачен».";
            return Page();
        }
        catch (KeyNotFoundException)
        {
            IsNotFound = true;
            return Page();
        }
        catch (InvalidOperationException ex)
        {
            Order = await _mediator.Send(new GetOrderByPaymentTokenQuery { PaymentToken = token });
            Message = ex.Message;
            return Page();
        }
    }
}
