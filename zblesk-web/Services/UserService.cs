using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Security.Claims;
using System.Web;
using zblesk_web.Configs;
using zblesk_web.Models;
using zblesk_web.ViewModels;

namespace zblesk_web.Services;

public sealed class UserService : UserServiceBase
{
    private readonly ILogger<UserService> _logger;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IStringLocalizer<Strings> _localizer;
    private readonly IStringLocalizer<NewsletterStrings> _newsletterLocalizer;
    private readonly IServiceProvider _serviceProvider;
    private readonly IMailerService _mailer;
    private readonly MatrixService _matrix;

    public UserService(
       ILogger<UserService> logger,
       SignInManager<ApplicationUser> signInManager,
       IStringLocalizer<Strings> localizer,
       IStringLocalizer<NewsletterStrings> newsletterLocalizer,
       IMailerService mailer,
       IServiceProvider serviceProvider,
       UserManager<ApplicationUser> userManager,
       MatrixService matrix,
       ApplicationDbContext dbContext,
       Config config) : base(userManager, logger, dbContext, localizer, config)
    {
        _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _newsletterLocalizer = newsletterLocalizer ?? throw new ArgumentNullException(nameof(newsletterLocalizer));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _mailer = mailer ?? throw new ArgumentNullException(nameof(mailer));
        _matrix = matrix;
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
    }

    public override async Task<UserInfo> Register(
        LoginCredentials creds,
        string defaultName = null)
    {
        if (string.IsNullOrWhiteSpace(defaultName))
        {
            defaultName = creds.Email[..creds.Email.IndexOf('@')];
        }
        var user = new ApplicationUser
        {
            UserName = defaultName,
            Email = creds.Email,
            Language = _config.DefaultCulture,
        };
        _logger.LogInformation("Registering user {email}", user.Email);
        var result = await _userManager.CreateAsync(user, creds.Password);
        if (result.Succeeded)
        {
            _logger.LogInformation("User created");
            LoadUsernameCache();
            return UserInfo.From(user);
        }
        return null;
    }

    private async Task SendDefaultLanguageNotification(ApplicationUser user)
    {
        var origCulture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture = _config.DefaultCultureInfo;
        await _matrix.SendMessage(
            _newsletterLocalizer.Format("newUserAdded",
            user.UserName,
            $"{_config.BaseUrl}/my/{user.Email}"));
        CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture = origCulture;
    }

    public override async Task<IdentityResult> MakeAdmin(string email)
    {
        _logger.LogInformation("Making admin of {email}", email);
        var user = await _signInManager.UserManager.FindByEmailAsync(email);
        var identityResult = await _userManager.AddToRoleAsync(user, "true");
        return identityResult;
    }

    public override async Task<UserInfo> RegisterFirstAdmin(LoginCredentials creds)
    {
        if (GetAllUserIds().Count != 0)
        {
            throw new ForbiddenException(_localizer["usersAlreadyExist"]);
        }
        _logger.LogInformation("Creating first admin with {email}", creds.Email);
        var newUser = await Register(creds);
        if (newUser == null)
        {
            _logger.LogError("First admin creation failed");
            throw new Exception("First admin creation failed");
        }
        var isAdmin = await MakeAdmin(newUser.Email);
        if (isAdmin.Succeeded)
        {
            return newUser;
        }
        _logger.LogError("First admin creation failed. \n\nAdmin success: {@isAdmin}", isAdmin);
        throw new Exception("Failed to make admin");
    }

    public override async Task<bool> ValidateLogin(LoginCredentials creds)
    {
        var user = await _signInManager.UserManager.FindByEmailAsync(creds.Email);
        if (user == null)
        {
            return false;
        }
        var signInRes = await _signInManager.CheckPasswordSignInAsync(user, creds.Password, lockoutOnFailure: false);
        return signInRes.Succeeded;
    }

    public override async Task<ClaimsPrincipal> GetPrincipal(LoginCredentials creds)
    {
        var user = await _signInManager.UserManager.FindByEmailAsync(creds.Email);
        return await _signInManager.CreateUserPrincipalAsync(user);
    }

    public override async Task<bool> InitiatePasswordReset(string email)
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
        var subject = _newsletterLocalizer.Format("passwordResetSubject");
        var body = _newsletterLocalizer.Format("passwordResetBody", callbackUrl);
        await _mailer.SendMail(subject, body, user.Email);
        return true;
    }

    public override async Task<IdentityResult> ResetPassword(string userId, string resetToken, string newPassword)
    {
        _logger.LogInformation("Resetting password for user {userId}", userId);
        var user = await _userManager.FindByIdAsync(userId);
        return await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
    }
}
