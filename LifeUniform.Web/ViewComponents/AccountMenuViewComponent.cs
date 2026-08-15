using LifeUniform.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace LifeUniform.Web.ViewComponents;

public class AccountMenuViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var user = HttpContext.User;
        if (user.Identity?.IsAuthenticated != true)
            return Content(string.Empty);

        return View(new AccountMenuVm
        {
            DisplayName = AccountIdentityHelper.DisplayName(user),
            Email = AccountIdentityHelper.Email(user)
        });
    }
}

public class AccountMenuVm
{
    public string DisplayName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}
