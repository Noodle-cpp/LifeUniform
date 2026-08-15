using System.Security.Claims;

namespace LifeUniform.Web.Services;

public static class AccountIdentityHelper
{
    public const string RuPhonePattern = @"^\+7 \(\d{3}\) \d{3}-\d{2}-\d{2}$";

    public static string DisplayName(ClaimsPrincipal user)
    {
        var given = user.FindFirstValue(ClaimTypes.GivenName);
        if (!string.IsNullOrWhiteSpace(given))
            return given.Trim();

        var email = user.Identity?.Name ?? string.Empty;
        var at = email.IndexOf('@');
        if (at > 0)
            return email[..at];
        return string.IsNullOrWhiteSpace(email) ? "Профиль" : email;
    }

    public static string Email(ClaimsPrincipal user) =>
        user.Identity?.Name ?? string.Empty;

    public static bool IsBlankPhone(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return true;
        var t = phone.Trim();
        return t is "+7" or "+7 ";
    }
}
