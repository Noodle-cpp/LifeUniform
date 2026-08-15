using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.RegularExpressions;
using LifeUniform.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LifeUniform.Web.Pages.Account;

[Authorize]
public class ProfileModel : PageModel
{
    private static readonly Regex RuPhone = new(AccountIdentityHelper.RuPhonePattern, RegexOptions.Compiled);

    private readonly UserManager<IdentityUser> _users;
    private readonly SignInManager<IdentityUser> _signIn;

    public ProfileModel(UserManager<IdentityUser> users, SignInManager<IdentityUser> signIn)
    {
        _users = users;
        _signIn = signIn;
    }

    [BindProperty]
    public ProfileInput Profile { get; set; } = new();

    [BindProperty]
    public PasswordInput Password { get; set; } = new();

    public string Email { get; private set; } = string.Empty;
    public string? SavedMessage { get; private set; }
    public string? ProfileError { get; private set; }
    public string? PasswordError { get; private set; }
    public string? PasswordSaved { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _users.GetUserAsync(User);
        if (user is null)
            return Challenge();

        Email = user.Email ?? User.Identity?.Name ?? string.Empty;
        Profile.Name = User.FindFirstValue(ClaimTypes.GivenName) ?? string.Empty;
        if (string.IsNullOrWhiteSpace(Profile.Name))
        {
            var claims = await _users.GetClaimsAsync(user);
            Profile.Name = claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value ?? string.Empty;
        }

        Profile.Phone = string.IsNullOrWhiteSpace(user.PhoneNumber) ? "+7 " : user.PhoneNumber;
        SavedMessage = TempData["ProfileSaved"] as string;
        PasswordSaved = TempData["PasswordSaved"] as string;
        return Page();
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        var user = await _users.GetUserAsync(User);
        if (user is null)
            return Challenge();

        Email = user.Email ?? User.Identity?.Name ?? string.Empty;
        var name = (Profile.Name ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            ProfileError = "Укажите имя.";
            Profile.Phone = Profile.Phone ?? "+7 ";
            return Page();
        }

        if (name.Length > 100)
        {
            ProfileError = "Имя слишком длинное.";
            return Page();
        }

        var phone = Profile.Phone?.Trim() ?? string.Empty;
        if (AccountIdentityHelper.IsBlankPhone(phone))
        {
            phone = string.Empty;
        }
        else if (!RuPhone.IsMatch(phone))
        {
            ProfileError = "Введите телефон в формате +7 (900) 000-00-00.";
            return Page();
        }

        var claims = await _users.GetClaimsAsync(user);
        var existing = claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName);
        if (existing is null)
            await _users.AddClaimAsync(user, new Claim(ClaimTypes.GivenName, name));
        else if (!string.Equals(existing.Value, name, StringComparison.Ordinal))
            await _users.ReplaceClaimAsync(user, existing, new Claim(ClaimTypes.GivenName, name));

        var phoneResult = await _users.SetPhoneNumberAsync(user, string.IsNullOrEmpty(phone) ? null : phone);
        if (!phoneResult.Succeeded)
        {
            ProfileError = phoneResult.Errors.FirstOrDefault()?.Description ?? "Не удалось сохранить телефон.";
            return Page();
        }

        await _signIn.RefreshSignInAsync(user);
        TempData["ProfileSaved"] = "Данные профиля сохранены.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostPasswordAsync()
    {
        var user = await _users.GetUserAsync(User);
        if (user is null)
            return Challenge();

        Email = user.Email ?? User.Identity?.Name ?? string.Empty;
        Profile.Name = User.FindFirstValue(ClaimTypes.GivenName) ?? string.Empty;
        Profile.Phone = string.IsNullOrWhiteSpace(user.PhoneNumber) ? "+7 " : user.PhoneNumber;

        if (string.IsNullOrWhiteSpace(Password.Current) ||
            string.IsNullOrWhiteSpace(Password.New) ||
            string.IsNullOrWhiteSpace(Password.Confirm))
        {
            PasswordError = "Заполните все поля для смены пароля.";
            return Page();
        }

        if (Password.New.Length < 6)
        {
            PasswordError = "Новый пароль должен быть не короче 6 символов.";
            return Page();
        }

        if (!string.Equals(Password.New, Password.Confirm, StringComparison.Ordinal))
        {
            PasswordError = "Новый пароль и подтверждение не совпадают.";
            return Page();
        }

        var result = await _users.ChangePasswordAsync(user, Password.Current, Password.New);
        if (!result.Succeeded)
        {
            PasswordError = result.Errors.Any(e => e.Code == "PasswordMismatch")
                ? "Неверный текущий пароль."
                : string.Join(" ", result.Errors.Select(e => e.Description));
            return Page();
        }

        await _signIn.RefreshSignInAsync(user);
        TempData["PasswordSaved"] = "Пароль изменён.";
        return RedirectToPage();
    }

    public class ProfileInput
    {
        [Display(Name = "Имя")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Телефон")]
        public string Phone { get; set; } = "+7 ";
    }

    public class PasswordInput
    {
        public string Current { get; set; } = string.Empty;
        public string New { get; set; } = string.Empty;
        public string Confirm { get; set; } = string.Empty;
    }
}
