using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Web;
using zblesk_web.Configs;
using zblesk_web.Models;
using Microsoft.Extensions.Localization;

namespace zblesk_web.Services;

public class AccountService
{
    private readonly ILogger<AccountService> _logger;

    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    private readonly JwtSecurityTokenHandler _tokenHandler = new();
    private readonly IMailerService _mailer;
    private readonly Config _config;
    private readonly ApplicationDbContext _dbContext;
    private readonly IStringLocalizer<NewsletterStrings> _newsletterLocalizer;

    public AccountService(
       UserManager<ApplicationUser> userManager,
       SignInManager<ApplicationUser> signInManager,
       ILogger<AccountService> logger,
       IConfiguration config,
       IStringLocalizer<NewsletterStrings> newsletterLocalizer,
       IMailerService mailer,
       ApplicationDbContext dbContext)
    {
        if (config == null)
            throw new ArgumentNullException(nameof(config));

        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _config = new Config();
        config.Bind(_config);
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _newsletterLocalizer = newsletterLocalizer ?? throw new ArgumentNullException(nameof(newsletterLocalizer));
        _mailer = mailer ?? throw new ArgumentNullException(nameof(mailer));
    }

    public async Task<ApplicationUser> Register(LoginCredentials creds)
    {
        var user = new ApplicationUser
        {
            UserName = creds.Email[..creds.Email.IndexOf('@')],
            Email = creds.Email,
        };
        _logger.LogInformation("Registering user {email}", user.Email);
        var result = await _userManager.CreateAsync(user, creds.Password);
        if (result.Succeeded)
        {
            _logger.LogInformation("User created");
            await _userManager.AddToRoleAsync(user, Roles.User);

            if (GetUserCount() < 2)
            {
                _logger.LogInformation("No users so far. Adding admin privileges.");
                await _userManager.AddToRoleAsync(user, Roles.Admin);
            }
            return user;
        }
        return null;
    }

    public async Task<bool> ValidateLogin(LoginCredentials creds)
    {
        var user = await _signInManager.UserManager.FindByEmailAsync(creds.Email);
        if (user == null)
        {
            return false;
        }
        var signInRes = await _signInManager.CheckPasswordSignInAsync(user, creds.Password, lockoutOnFailure: false);
        return signInRes.Succeeded;
    }

    public async Task<ClaimsPrincipal> GetPrincipal(LoginCredentials creds)
    {
        var user = await _signInManager.UserManager.FindByEmailAsync(creds.Email);
        return await _signInManager.CreateUserPrincipalAsync(user);
    }

    public string GetToken(ClaimsPrincipal principal)
        => _tokenHandler.WriteToken(
            new JwtSecurityToken(
                "zbleskweb",
                "zbleskweb",
                principal.Claims,
                expires: DateTime.UtcNow.AddDays(190),
                signingCredentials: _config.SigningCreds));

    public async Task<bool> InitiatePasswordReset(string email)
    {
        _logger.LogInformation("Initiating password reset for {email}", email);
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return false;
        }
        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        var callbackUrl = string.Format("{0}/passwordreset/{1}/{2}",
                                        _config.BaseUrl,
                                        HttpUtility.UrlEncode(user.Id),
                                        HttpUtility.UrlEncode(resetToken));
        var subject = _newsletterLocalizer["passwordResetSubject"];
        var body = _newsletterLocalizer.Format("passwordResetBody", callbackUrl);
        await _mailer.SendMail(subject, body, user.Email);
        _logger.LogWarning("reset hesla body {b}", body);
        return true;
    }

    public async Task<IdentityResult> ResetPassword(string userId, string resetToken, string newPassword)
    {
        _logger.LogInformation("Resetting password for user {userId}", userId);
        var user = await _userManager.FindByIdAsync(userId);
        return await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
    }

    public Task<ApplicationUser> GetUserByPrincipal(ClaimsPrincipal principal) => _userManager.GetUserAsync(principal);

    public int GetUserCount() => _dbContext.Users.Count();
}
