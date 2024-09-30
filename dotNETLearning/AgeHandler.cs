using Microsoft.AspNetCore.Authorization;
using dotNETLearning;
using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace dotNETLearning;

public class AgeHandler : AuthorizationHandler<AgeRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AgeRequirement requirement)
    {
        var yearClaim = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.DateOfBirth);
        if (yearClaim is not null)
        {
            if (int.TryParse(yearClaim.Value, out var year))
            {
                if((DateTime.Now.Year - year) >= requirement.age)
                    context.Succeed(requirement);
            }
        }
        return Task.CompletedTask;
    }
}