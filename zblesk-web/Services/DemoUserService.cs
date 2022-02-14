﻿using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using zblesk_web.Configs;
using zblesk_web.Models;
using zblesk_web.ViewModels;

namespace zblesk_web.Services;

public class DemoUserService : UserServiceBase
{
    private readonly IStringLocalizer<Strings> _localizer;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public DemoUserService(
        ILogger<UserServiceBase> logger,
        UserManager<ApplicationUser> userManager,
       SignInManager<ApplicationUser> signInManager,
        IStringLocalizer<Strings> localizer,
        ApplicationDbContext dbContext,
        Config config) : base(userManager, logger, dbContext, localizer, config)
    {
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
    }

    public override async Task<ClaimsPrincipal> GetPrincipal(LoginCredentials creds)
        => await _signInManager.CreateUserPrincipalAsync(
            await _userManager.FindByEmailAsync("janka@bookclub.gov"));

    public override Task<bool> InitiatePasswordReset(string email)
        => throw new ForbiddenException(_localizer["forbiddenInDemo"]);

    public override Task<IdentityResult> MakeAdmin(string email)
        => throw new ForbiddenException(_localizer["forbiddenInDemo"]);

    public override Task<UserInfo> Register(LoginCredentials creds, string defaultName = null)
        => throw new ForbiddenException(_localizer["forbiddenInDemo"]);

    public override Task<UserInfo> RegisterFirstAdmin(LoginCredentials creds)
        => throw new ForbiddenException(_localizer["forbiddenInDemo"]);

    public override Task<IdentityResult> ResetPassword(
        string userId,
        string resetToken,
        string newPassword)
        => throw new ForbiddenException(_localizer["forbiddenInDemo"]);

    public override Task<bool> ValidateLogin(LoginCredentials creds)
        => Task.FromResult(true);
}
