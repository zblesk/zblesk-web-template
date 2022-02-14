using System.Security.Claims;
using zblesk_web.Models;

namespace zblesk_web.ViewModels;

public class UserInfo
{
    public string UserId { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public bool IsAdmin { get; set; } = false;

    public static UserInfo From(ClaimsPrincipal principal)
            => (principal?.Identity == null || !principal.Identity.IsAuthenticated)
                ? new()
                : new()
                {
                    UserId = principal.GetUserId(),
                    Name = principal.Identity.Name,
                    Email = principal.FindFirstValue(ClaimTypes.Email),
                    IsAdmin = principal?.IsInRole(Roles.Admin) ?? false,
                };

    public static UserInfo From(ApplicationUser user, ClaimsPrincipal principal = null)
        => user == null
        ? new()
        : new()
        {
            UserId = user.Id,
            Name = user.UserName,
            Email = user.Email,
            Phone = user.PhoneNumber,
            IsAdmin = principal?.IsInRole(Roles.Admin) ?? false,
        };

    public static UserInfo From(ApplicationUser user, IList<string> roles)
        => user == null
        ? new()
        : new()
        {
            UserId = user.Id,
            Name = user.UserName,
            Email = user.Email,
            Phone = user.PhoneNumber,
            IsAdmin = roles.Contains(Roles.Admin)
        };
}
