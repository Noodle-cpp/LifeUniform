using FluentValidation;
using LifeUniform.Application.Catalog.Dto;
using LifeUniform.Application.Catalog.Queries;
using LifeUniform.Application.Orders.Commands;
using LifeUniform.Domain.Cart;
using LifeUniform.Web.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace LifeUniform.Web.Pages.Catalog
{
    public class DetailsModel : PageModel
    {
        private readonly IMediator _mediator;
        private readonly ICartService _cart;
        private readonly IFavoriteState _favorites;

        public ProductDetailsDto Vm { get; private set; } = new();
        public bool IsNotFound { get; private set; }
        public string? CartMessage { get; private set; }
        public bool SizeMissing { get; private set; }
        public string? SelectedColor { get; private set; }
        public Guid? SelectedSizeId { get; private set; }
        public bool CartAdded { get; private set; }

        public DetailsModel(IMediator mediator, ICartService cart, IFavoriteState favorites)
        {
            _mediator = mediator;
            _cart = cart;
            _favorites = favorites;
        }

        public async Task<IActionResult> OnGetAsync(string slug, string? color = null, Guid? size = null)
        {
            SelectedColor = color;
            SelectedSizeId = size;
            CartAdded = TempData["CartAdded"] as string == "1";
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Vm = await _mediator.Send(new GetProductDetailsQuery { Slug = slug, UserId = userId });
                ApplyGuestFavorites(userId);
                if (SelectedSizeId is Guid sizeId && Vm.Sizes.All(s => s.Id != sizeId))
                    SelectedSizeId = null;
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
            SelectedSizeId = sizeId;
            var json = WantsJson();

            if (sizeId is null || sizeId == Guid.Empty)
            {
                const string missingSize = "Выберите размер перед добавлением в корзину.";
                if (json)
                    return CartJson(ok: false, sizeMissing: true, message: missingSize, statusCode: 400);

                await ReloadAsync(slug);
                SizeMissing = true;
                CartMessage = missingSize;
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
                if (json)
                    return CartJson(ok: true, cartCount: _cart.GetItems().Sum(x => x.Quantity));

                TempData["CartAdded"] = "1";
                return RedirectToPage(new { slug, color });
            }
            catch (KeyNotFoundException)
            {
                if (json)
                    return CartJson(ok: false, notFound: true, statusCode: 404);

                IsNotFound = true;
                return Page();
            }
            catch (ValidationException ex)
            {
                var sizeError = ex.Errors.FirstOrDefault(e => e.PropertyName == nameof(AddToCartCommand.SizeId));
                var message = sizeError?.ErrorMessage
                    ?? ex.Errors.FirstOrDefault()?.ErrorMessage
                    ?? "Не удалось добавить товар в корзину.";
                if (json)
                    return CartJson(ok: false, sizeMissing: sizeError is not null, message: message, statusCode: 400);

                await ReloadAsync(slug);
                SizeMissing = sizeError is not null;
                CartMessage = message;
                return Page();
            }
            catch (InvalidOperationException ex)
            {
                if (json)
                    return CartJson(ok: false, message: ex.Message, statusCode: 400);

                await ReloadAsync(slug);
                CartMessage = ex.Message;
                return Page();
            }
        }

        private JsonResult CartJson(
            bool ok,
            int? cartCount = null,
            bool sizeMissing = false,
            bool notFound = false,
            string? message = null,
            int statusCode = 200)
            => new(new { ok, cartCount, sizeMissing, notFound, message }) { StatusCode = statusCode };

        private bool WantsJson()
        {
            var accept = Request.Headers.Accept.ToString();
            if (accept.Contains("application/json", StringComparison.OrdinalIgnoreCase))
                return true;
            return string.Equals(
                Request.Headers["X-Requested-With"].ToString(),
                "XMLHttpRequest",
                StringComparison.OrdinalIgnoreCase);
        }

        private async Task ReloadAsync(string slug)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Vm = await _mediator.Send(new GetProductDetailsQuery { Slug = slug, UserId = userId });
            ApplyGuestFavorites(userId);
        }

        private void ApplyGuestFavorites(string? userId)
        {
            if (!string.IsNullOrWhiteSpace(userId))
                return;

            var ids = _favorites.GetGuestIds();
            Vm.IsFavorite = ids.Contains(Vm.Id);
            _favorites.ApplyGuest(Vm.RelatedProducts);
        }
    }
}
