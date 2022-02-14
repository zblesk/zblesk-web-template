using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using zblesk_web.Models;
using zblesk_web.Services;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Localization;
using zblesk_web.Configs;

namespace zblesk_web.Controllers;

[Authorize(Policy = "ModOnly")]
[Route("api/admin")]
public class AdminController : Controller
{
    private readonly ILogger _logger;
    private readonly UserServiceBase _users;
    private readonly IMailerService _mailer;
    private static readonly Random _random = new();
    private readonly Config _config;
    private readonly BackupService _backup;
    private readonly IStringLocalizer<Strings> _localizer;

    public AdminController(
        ILogger<AdminController> logger,
        UserServiceBase users,
        IMailerService mailer,
        BackupService backup,
        IStringLocalizer<Strings> localizer,
        Config config)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _users = users ?? throw new ArgumentNullException(nameof(users));
        _mailer = mailer ?? throw new ArgumentNullException(nameof(mailer));
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _backup = backup ?? throw new ArgumentNullException(nameof(backup));
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
    }

    [HttpPost]
    [Route("createuser")]
    public async Task<IActionResult> CreateUser([FromBody] JObject payload)
    {
        var email = payload["email"].ToString();
        var name = payload.ContainsKey("name")
            ? payload["name"].ToString()
            : null;
        _logger.LogInformation("Adding user {email}", email);
        var password = Enumerable.Range(1, _config.DefaultPasswordMinLength)
            .Aggregate(
            "",
            (pwd, _) => $"{pwd}{_config.PasswordGenerationChars[_random.Next(_config.PasswordGenerationChars.Length)]}");

        var user = await _users.Register(
            new LoginCredentials { Email = email, Password = password },
            name);
        if (user != null)
        {
            await _mailer.SendMail(
                _localizer.Format("newUserSubject", user.Name),
                _localizer.Format("newUserPostBodyMarkdown",
                    _config.BaseUrl,
                    user.Email,
                    password),
                email);
            return Json(user);
        }
        return ValidationProblem("Registration failed");
    }

    [HttpPost("createbackup")]
    public Task CreateBackup()
        => _backup.CreateBackup();
}
