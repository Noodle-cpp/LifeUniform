using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LifeUniform.Application.Catalog.Commands;
using LifeUniform.Application.Catalog.Dto;
using LifeUniform.Application.Catalog.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LifeUniform.Web.Areas.Admin.Pages.Catalog
{
    [Authorize(Roles = "Admin")]
    public class EditSizesModel : PageModel
    {
        private readonly IMediator _mediator;

        public ProductSizeEditDto Vm { get; private set; } = new();
        public bool IsNotFound { get; private set; }

        [BindProperty]
        public List<Guid> SelectedSizeIds { get; set; } = new();

        [BindProperty]
        public string Slug { get; set; } = string.Empty;

        public EditSizesModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task OnGetAsync(string slug)
        {
            Slug = slug;
            try
            {
                Vm = await _mediator.Send(new GetProductSizesForEditQuery { Slug = slug });
                IsNotFound = false;
            }
            catch (KeyNotFoundException)
            {
                IsNotFound = true;
            }
        }

        public async Task<IActionResult> OnPostAsync(string slug)
        {
            Slug = slug;
            try
            {
                await _mediator.Send(new UpdateProductSizesCommand
                {
                    Slug = slug,
                    SizeIds = SelectedSizeIds
                });

                return RedirectToPage(new { slug });
            }
            catch (KeyNotFoundException)
            {
                IsNotFound = true;
                return Page();
            }
        }
    }
}

