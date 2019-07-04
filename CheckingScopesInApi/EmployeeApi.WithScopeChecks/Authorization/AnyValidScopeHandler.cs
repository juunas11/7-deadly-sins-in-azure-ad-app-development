using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeeApi.WithScopeChecks.Authorization
{
    public class AnyValidScopeHandler : AuthorizationHandler<AnyValidScopeRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            AnyValidScopeRequirement requirement)
        {
            if (context.User.HasClaim(c => c.Type == Claims.ScopeClaimType)
                && context.User.FindAll(Claims.ScopeClaimType)
                    .Any(scope => DelegatedPermissions.All.Contains(scope.Value)))
            {
                // Caller has valid delegated permission
                context.Succeed(requirement);
            }
            else if (context.User.HasClaim(c => c.Type == ClaimTypes.Role)
                && context.User.FindAll(ClaimTypes.Role)
                    .Any(role => ApplicationPermissions.All.Contains(role.Value)))
            {
                // Caller has valid app permission
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
