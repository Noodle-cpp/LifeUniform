using FluentValidation;
using LifeUniform.Application.Catalog.Dto;
using LifeUniform.Application.Catalog.Queries;
using LifeUniform.Application.Orders.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace LifeUniform.Web.Pages.Catalog
{
    public class DetailsModel : PageModel
    {
        private readonly IMediator _mediator;

        public ProductDetailsDto Vm { get; private set; } = new();
        public bool IsNotFound { get; private set; }
        public string? CartMessage { get; private set; }
        public bool SizeMissing { get; private set; }
        public string? SelectedColor { get; private set; }
        public bool CartAdded { get; private set; }

        public DetailsModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> OnGetAsync(string slug, string? color = null)
        {
            SelectedColor = color;
            CartAdded = TempData["CartAdded"] as string == "1";
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Vm = await _mediator.Send(new GetProductDetailsQuery { Slug = slug, UserId = userId });
                IsNotFound = false;
                return Page();
            }
            catch (KeyNotFoundException)
            {
                IsNotFound = true;
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAddToCartAsync(
            string slug,
            Guid? sizeId,
            int quantity = 1,
            string? color = null)
        {
            SelectedColor = color;

            if (sizeId is null || sizeId == Guid.Empty)
            {
                await ReloadAsync(slug);
                SizeMissing = true;
                CartMessage = "Выберите размер перед добавлением в корзину.";
                return Page();
            }

            try
            {
                await _mediator.Send(new AddToCartCommand
                {
                    ProductSlug = slug,
                    SizeId = sizeId.Value,
                    Quantity = quantity,
                    ColorName = color
                });
                TempData["CartAdded"] = "1";
                return RedirectToPage(new { slug, color });
            }
            catch (KeyNotFoundException)
            {
                IsNotFound = true;
                return Page();
            }
            catch (ValidationException ex)
            {
                await ReloadAsync(slug);
                var sizeError = ex.Errors.FirstOrDefault(e => e.PropertyName == nameof(AddToCartCommand.SizeId));
                SizeMissing = sizeError is not null;
                CartMessage = sizeError?.ErrorMessage
                    ?? ex.Errors.FirstOrDefault()?.ErrorMessage
                    ?? "Не удалось добавить товар в корзину.";
                return Page();
            }
            catch (InvalidOperationException ex)
            {
                await ReloadAsync(slug);
                CartMessage = ex.Message;
                return Page();
            }
        }

        private async Task ReloadAsync(string slug)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Vm = await _mediator.Send(new GetProductDetailsQuery { Slug = slug, UserId = userId });
        }
    }
}
