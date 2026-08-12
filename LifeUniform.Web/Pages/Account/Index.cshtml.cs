using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LifeUniform.Web.Pages.Account;

[Authorize]
public class IndexModel : PageModel
{
    public void OnGet()
    {
    }
}
