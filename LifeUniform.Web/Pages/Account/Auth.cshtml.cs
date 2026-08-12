using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LifeUniform.Web.Pages.Account;

public class AuthModel : PageModel
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;

    public AuthModel(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public string Mode { get; private set; } = "login";
    public string? ErrorMessage { get; private set; }

    public IActionResult OnGet(string? mode = null, string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return LocalRedirect(SafeReturnUrl(returnUrl));

        Mode = string.Equals(mode, "register", StringComparison.OrdinalIgnoreCase) ? "register" : "login";
        ReturnUrl = returnUrl;
        ErrorMessage = TempData["AuthError"] as string;
        ViewData["OpenAuthModal"] = Mode;
        ViewData["AuthReturnUrl"] = SafeReturnUrl(ReturnUrl);
        ViewData["AuthError"] = ErrorMessage;
        ViewData["AuthName"] = TempData["AuthName"] as string;
        ViewData["AuthEmail"] = TempData["AuthEmail"] as string;
        ViewData["AuthPassword"] = TempData["AuthPassword"] as string;
        return Page();
    }

    public async Task<IActionResult> OnPostLoginAsync(LoginInput input, string? returnUrl = null)
    {
        ReturnUrl = returnUrl;
        var redirect = SafeReturnUrl(returnUrl);

        if (!ModelState.IsValid || string.IsNullOrWhiteSpace(input.Email) || string.IsNullOrWhiteSpace(input.Password))
        {
            return AuthFail("login", redirect, "Введите email и пароль.", email: input.Email);
        }

        var result = await _signInManager.PasswordSignInAsync(
            input.Email.Trim(),
            input.Password,
            isPersistent: true,
            lockoutOnFailure: false);

        if (result.Succeeded)
            return LocalRedirect(redirect);

        return AuthFail("login", redirect, "Неверный email или пароль.", email: input.Email);
    }

    public async Task<IActionResult> OnPostRegisterAsync(RegisterInput input, string? returnUrl = null)
    {
        ReturnUrl = returnUrl;
        var redirect = SafeReturnUrl(returnUrl);

        if (string.IsNullOrWhiteSpace(input.Name) ||
            string.IsNullOrWhiteSpace(input.Email) ||
            string.IsNullOrWhiteSpace(input.Password))
        {
            return AuthFail(
                "register",
                redirect,
                "Заполните все поля.",
                name: input.Name,
                email: input.Email,
                password: input.Password);
        }

        if (input.Password.Length < 6)
        {
            return AuthFail(
                "register",
                redirect,
                "Пароль должен быть не короче 6 символов.",
                name: input.Name,
                email: input.Email,
                password: input.Password);
        }

        var user = new IdentityUser
        {
            UserName = input.Email.Trim(),
            Email = input.Email.Trim(),
            EmailConfirmed = true
        };

        var create = await _userManager.CreateAsync(user, input.Password);
        if (!create.Succeeded)
        {
            var msg = string.Join(" ", create.Errors.Select(e => e.Description));
            return AuthFail(
                "register",
                redirect,
                msg,
                name: input.Name,
                email: input.Email,
                password: input.Password);
        }

        await _userManager.AddToRoleAsync(user, "Customer");
        if (!string.IsNullOrWhiteSpace(input.Name))
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.GivenName, input.Name.Trim()));
        await _signInManager.SignInAsync(user, isPersistent: true);
        return LocalRedirect(redirect);
    }

    private IActionResult AuthFail(
        string mode,
        string returnUrl,
        string message,
        string? name = null,
        string? email = null,
        string? password = null)
    {
        TempData["AuthError"] = message;
        if (!string.IsNullOrWhiteSpace(name))
            TempData["AuthName"] = name.Trim();
        if (!string.IsNullOrWhiteSpace(email))
            TempData["AuthEmail"] = email.Trim();
        // Keep typed password so user can tweak it after Identity validation errors.
        if (!string.IsNullOrEmpty(password))
            TempData["AuthPassword"] = password;
        return RedirectToPage(new { mode, returnUrl });
    }

    private string SafeReturnUrl(string? returnUrl)
    {
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            return returnUrl;
        return "/";
    }

    public class LoginInput
    {
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterInput
    {
        public string Name { get; set; } = string.Empty;
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}

