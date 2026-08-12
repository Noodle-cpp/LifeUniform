using System.ComponentModel.DataAnnotations;
using LifeUniform.Application.Marketing.Commands;
using LifeUniform.Application.Marketing.Dto;
using LifeUniform.Application.Marketing.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LifeUniform.Web.Areas.Admin.Pages.Offers;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly IMediator _mediator;

    public IndexModel(IMediator mediator) => _mediator = mediator;

    public IReadOnlyList<PromoOfferDto> Vm { get; private set; } = Array.Empty<PromoOfferDto>();

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ErrorMessage { get; private set; }

    public class InputModel
    {
        public Guid? Id { get; set; }

        [Required, Display(Name = "Заголовок")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Подзаголовок")]
        public string? Subtitle { get; set; }

        [Display(Name = "Бейдж")]
        public string? Badge { get; set; }

        [Required, Display(Name = "URL картинки")]
        public string ImageUrl { get; set; } = "/images/hero-banner.png";

        [Display(Name = "Ссылка")]
        public string? LinkUrl { get; set; } = "/Catalog";

        [Display(Name = "Текст кнопки")]
        public string? LinkText { get; set; } = "Подробнее";

        [Display(Name = "Цена")]
        public decimal? Price { get; set; }

        [Display(Name = "Старая цена")]
        public decimal? OldPrice { get; set; }

        [Display(Name = "Сортировка")]
        public int SortOrder { get; set; }

        [Display(Name = "Активен")]
        public bool IsActive { get; set; } = true;
    }

    public async Task OnGetAsync(Guid? editId)
    {
        Vm = await _mediator.Send(new GetPromoOffersForAdminQuery());
        if (editId is Guid id)
        {
            var item = Vm.FirstOrDefault(x => x.Id == id);
            if (item is not null)
            {
                Input = new InputModel
                {
                    Id = item.Id,
                    Title = item.Title,
                    Subtitle = item.Subtitle,
                    Badge = item.Badge,
                    ImageUrl = item.ImageUrl,
                    LinkUrl = item.LinkUrl,
                    LinkText = item.LinkText,
                    Price = item.Price,
                    OldPrice = item.OldPrice,
                    SortOrder = item.SortOrder,
                    IsActive = item.IsActive
                };
            }
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Vm = await _mediator.Send(new GetPromoOffersForAdminQuery());
        if (!ModelState.IsValid)
            return Page();

        try
        {
            await _mediator.Send(new UpsertPromoOfferCommand
            {
                Id = Input.Id,
                Title = Input.Title,
                Subtitle = Input.Subtitle,
                Badge = Input.Badge,
                ImageUrl = Input.ImageUrl,
                LinkUrl = Input.LinkUrl,
                LinkText = Input.LinkText,
                Price = Input.Price,
                OldPrice = Input.OldPrice,
                SortOrder = Input.SortOrder,
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

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        await _mediator.Send(new DeletePromoOfferCommand { Id = id });
        return RedirectToPage();
    }
}
