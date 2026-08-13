using System.ComponentModel.DataAnnotations;
using LifeUniform.Application.Marketing.Commands;
using LifeUniform.Application.Marketing.Dto;
using LifeUniform.Application.Marketing.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LifeUniform.Web.Areas.Admin.Pages.ClientPhotos;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly IMediator _mediator;

    public IndexModel(IMediator mediator) => _mediator = mediator;

    public IReadOnlyList<ClientPhotoDto> Vm { get; private set; } = Array.Empty<ClientPhotoDto>();

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ErrorMessage { get; private set; }

    public class InputModel
    {
        public Guid? Id { get; set; }

        [Required, Display(Name = "Заголовок отзыва")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Текст отзыва")]
        public string? ReviewText { get; set; }

        [Required, Display(Name = "URL картинки")]
        public string ImageUrl { get; set; } = string.Empty;

        [Display(Name = "Оценка"), Range(1, 5)]
        public int Rating { get; set; } = 5;

        [Display(Name = "Сортировка")]
        public int SortOrder { get; set; }

        [Display(Name = "Активен")]
        public bool IsActive { get; set; } = true;
    }

    public async Task OnGetAsync(Guid? editId)
    {
        Vm = await _mediator.Send(new GetClientPhotosForAdminQuery());
        if (editId is Guid id)
        {
            var item = Vm.FirstOrDefault(x => x.Id == id);
            if (item is not null)
            {
                Input = new InputModel
                {
                    Id = item.Id,
                    Title = item.Title,
                    ReviewText = item.ReviewText,
                    ImageUrl = item.ImageUrl,
                    Rating = item.Rating,
                    SortOrder = item.SortOrder,
                    IsActive = item.IsActive
                };
            }
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Vm = await _mediator.Send(new GetClientPhotosForAdminQuery());
        if (!ModelState.IsValid)
            return Page();

        try
        {
            await _mediator.Send(new UpsertClientPhotoCommand
            {
                Id = Input.Id,
                Title = Input.Title,
                ReviewText = Input.ReviewText,
                ImageUrl = Input.ImageUrl,
                Rating = Input.Rating,
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
        await _mediator.Send(new DeleteClientPhotoCommand { Id = id });
        return RedirectToPage();
    }
}
