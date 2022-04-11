using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Web;
using zblesk_web.Hubs;
using zblesk_web.Models;
using zblesk_web.Services;
using zblesk_web.ViewModels;

namespace zblesk_web.Controllers;

[AllowAnonymous]
[Route("api/accounts")]
public class AccountController : Controller
{
    private readonly AccountService _users;
    private readonly IHubContext<EventHub, IEventHub> _eventHub;

    public AccountController(
       AccountService users,
       IHubContext<EventHub, IEventHub> eventHub)
    {
        _users = users;
        _eventHub = eventHub ?? throw new ArgumentNullException(nameof(eventHub));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Token([FromBody] LoginCredentials creds)
    {
        if (!await _users.ValidateLogin(creds))
        {
            return Unauthorized(new
            {
                error = "Login failed"
            });
        }
        var principal = await _users.GetPrincipal(creds);
        var token = _users.GetToken(principal);
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        Task.Delay(4000).ContinueWith(t =>
            _eventHub.Clients.All.Notify("Look, a delayed notification, showing SignalR now works."));
#pragma warning restore CS4014 
        return Json(new { user = UserInfo.From(await _users.GetUserByPrincipal(principal)), token });
    }

    [HttpGet("context")]
    public async Task<JsonResult> Context() => Json(UserInfo.From(await _users.GetUserByPrincipal(User)));

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] LoginCredentials creds)
    {
        try
        {
            var user = await _users.Register(creds);
            if (user != null)
            {
                return Json(UserInfo.From(user));
            }
        }
        catch (Exception ex)
        {
            return ValidationProblem("Registration failed: " + ex.Message);
        }
        return ValidationProblem("Registration failed");
    }

    [HttpPost("passwordreset/token/{userId}")]
    public async Task<IActionResult> ResetPassword(string userId, [FromBody] JsonElement payload)
    {
        var result = await _users.ResetPassword(
            HttpUtility.UrlDecode(userId),
            payload.GetProperty("resetToken").GetString(),
            payload.GetProperty("password").GetString());
        if (!result.Succeeded)
        {
            return StatusCode(403, result.Errors.Aggregate("", (a, e) => $"{a}{e.Description} "));
        }
        return Ok();
    }

    [HttpPost("passwordreset/{email}")]
    public Task<bool> InitiatePasswordReset(string email) => _users.InitiatePasswordReset(email);
}
