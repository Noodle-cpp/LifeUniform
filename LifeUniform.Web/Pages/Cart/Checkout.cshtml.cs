using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using LifeUniform.Application.Orders.Commands;
using LifeUniform.Application.Orders.Dto;
using LifeUniform.Application.Orders.Queries;
using LifeUniform.Domain.Orders;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LifeUniform.Web.Pages.Cart;

public class CheckoutModel : PageModel
{
    private readonly IMediator _mediator;

    public CheckoutModel(IMediator mediator) => _mediator = mediator;

    public CartDto Cart { get; private set; } = new();

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ErrorMessage { get; private set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Укажите имя")]
        [Display(Name = "ФИО")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Укажите телефон")]
        [Display(Name = "Телефон")]
        [RegularExpression(@"^\+7 \(\d{3}\) \d{3}-\d{2}-\d{2}$", ErrorMessage = "Введите телефон в формате +7 (900) 000-00-00")]
        public string CustomerPhone { get; set; } = "+7 ";

        [Required(ErrorMessage = "Укажите email")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string CustomerEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Укажите адрес доставки")]
        [Display(Name = "Адрес доставки")]
        public string DeliveryAddress { get; set; } = string.Empty;

        [Display(Name = "Способ доставки")]
        public DeliveryMethod DeliveryMethod { get; set; } = DeliveryMethod.Cdek;

        [Display(Name = "Промокод")]
        public string? PromoCode { get; set; }

        [Display(Name = "Комментарий к заказу")]
        [MaxLength(500)]
        public string? Comment { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        Cart = await _mediator.Send(new GetCartQuery());
        if (Cart.Items.Count == 0)
            return RedirectToPage("/Cart/Index");

        if (User.Identity?.IsAuthenticated == true && string.IsNullOrWhiteSpace(Input.CustomerEmail))
            Input.CustomerEmail = User.Identity.Name ?? string.Empty;

        if (string.IsNullOrWhiteSpace(Input.CustomerPhone) || Input.CustomerPhone.Trim() == "+7")
            Input.CustomerPhone = "+7 ";

        return Page();
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

        Cart = await _mediator.Send(new GetCartQuery());
        if (Cart.Items.Count == 0)
            return RedirectToPage("/Cart/Index");

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostAsync(bool agreeTerms = false)
    {
        Cart = await _mediator.Send(new GetCartQuery());
        if (Cart.Items.Count == 0)
            return RedirectToPage("/Cart/Index");

        if (!agreeTerms)
            ModelState.AddModelError(string.Empty, "Нужно согласие с условиями оферты и политикой конфиденциальности.");

        if (!ModelState.IsValid)
            return Page();

        try
        {
            var order = await _mediator.Send(new CreateOrderCommand
            {
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                CustomerName = Input.CustomerName,
                CustomerPhone = Input.CustomerPhone,
                CustomerEmail = Input.CustomerEmail,
                DeliveryAddress = string.IsNullOrWhiteSpace(Input.Comment)
                    ? Input.DeliveryAddress
                    : $"{Input.DeliveryAddress}\nКомментарий: {Input.Comment.Trim()}",
                DeliveryMethod = Input.DeliveryMethod,
                PromoCode = string.IsNullOrWhiteSpace(Input.PromoCode) ? null : Input.PromoCode.Trim()
            });

            return RedirectToPage("/Cart/Payment", new { token = order.PaymentToken });
        }
        catch (InvalidOperationException ex)
        {
            ErrorMessage = ex.Message;
            return Page();
        }
    }
}
