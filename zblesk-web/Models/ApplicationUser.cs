using Microsoft.AspNetCore.Identity;

namespace zblesk_web.Models;

public class ApplicationUser : IdentityUser
{
    // Add your user-specific info here
    public string Language { get; internal set; }
    public string AvatarUrl { get; internal set; }
}
