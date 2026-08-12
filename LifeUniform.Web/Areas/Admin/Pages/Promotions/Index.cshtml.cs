using System.ComponentModel.DataAnnotations;
using LifeUniform.Application.Promotions.Commands;
using LifeUniform.Application.Promotions.Dto;
using LifeUniform.Application.Promotions.Queries;
using LifeUniform.Domain.Promotions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LifeUniform.Web.Areas.Admin.Pages.Promotions;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly IMediator _mediator;

    public IndexModel(IMediator mediator) => _mediator = mediator;

    public IReadOnlyList<PromotionCodeDto> Vm { get; private set; } = Array.Empty<PromotionCodeDto>();

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ErrorMessage { get; private set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Укажите код")]
        [Display(Name = "Код")]
        public string Code { get; set; } = string.Empty;

        [Display(Name = "Тип")]
        public PromotionDiscountType Type { get; set; } = PromotionDiscountType.Percent;

        [Range(0.01, 100000)]
        [Display(Name = "Значение")]
        public decimal Value { get; set; }

        [Display(Name = "Мин. сумма заказа")]
        public decimal? MinOrderAmount { get; set; }

        [Display(Name = "Активен")]
        public bool IsActive { get; set; } = true;
    }

    public async Task OnGetAsync()
    {
        Vm = await _mediator.Send(new GetPromotionsForAdminQuery());
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Vm = await _mediator.Send(new GetPromotionsForAdminQuery());

        if (!ModelState.IsValid)
            return Page();

        try
        {
            await _mediator.Send(new UpsertPromotionCommand
            {
                Code = Input.Code.Trim().ToUpperInvariant(),
                Type = Input.Type,
                Value = Input.Value,
                MinOrderAmount = Input.MinOrderAmount,
                IsActive = Input.IsActive
            });

            return RedirectToPage();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            return Page();
        }
    }

    public async Task<IActionResult> OnPostToggleAsync(Guid id, bool isActive)
    {
        await _mediator.Send(new SetPromotionActiveCommand { Id = id, IsActive = isActive });
        return RedirectToPage();
    }
}
