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
public class EditModel : PageModel
{
    private readonly IMediator _mediator;

    public EditModel(IMediator mediator) => _mediator = mediator;

    public bool IsNotFound { get; private set; }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ErrorMessage { get; private set; }

    public class InputModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Укажите код")]
        [Display(Name = "Код")]
        public string Code { get; set; } = string.Empty;

        [Display(Name = "Тип")]
        public PromotionDiscountType Type { get; set; }

        [Range(0.01, 100000)]
        [Display(Name = "Значение")]
        public decimal Value { get; set; }

        [Display(Name = "Мин. сумма заказа")]
        public decimal? MinOrderAmount { get; set; }

        [Display(Name = "Лимит использований")]
        public int? MaxRedemptions { get; set; }

        [Display(Name = "Активен")]
        public bool IsActive { get; set; } = true;

        public int RedemptionCount { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var dto = await _mediator.Send(new GetPromotionByIdQuery { Id = id });
        if (dto is null)
        {
            IsNotFound = true;
            return Page();
        }

        Input = Map(dto);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            await _mediator.Send(new UpsertPromotionCommand
            {
                Id = Input.Id,
                Code = Input.Code.Trim().ToUpperInvariant(),
                Type = Input.Type,
                Value = Input.Value,
                MinOrderAmount = Input.MinOrderAmount,
                MaxRedemptions = Input.MaxRedemptions,
                IsActive = Input.IsActive
            });
            return RedirectToPage("/Promotions/Index", new { area = "Admin" });
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            return Page();
        }
    }

    private static InputModel Map(PromotionCodeDto dto) => new()
    {
        Id = dto.Id,
        Code = dto.Code,
        Type = (PromotionDiscountType)dto.Type,
        Value = dto.Value,
        MinOrderAmount = dto.MinOrderAmount,
        MaxRedemptions = dto.MaxRedemptions,
        IsActive = dto.IsActive,
        RedemptionCount = dto.RedemptionCount
    };
}
