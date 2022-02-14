using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using zblesk_web.Models;

namespace zblesk_web;

public class OwnerOrAdminAuthorizationHandler :
    AuthorizationHandler<MatchingOwnerRequirement, DataHeader>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                   MatchingOwnerRequirement requirement,
                                                   DataHeader data)
    {
        if (data == null)
        {
            context.Fail();
            return Task.CompletedTask;
        }
        if (context.User.Claims.Any(c => c.Type == ClaimTypes.NameIdentifier && c.Value == data.OwnerId)
            || context.User.IsInRole(Roles.Admin))
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}

public class MatchingOwnerRequirement : IAuthorizationRequirement { }
