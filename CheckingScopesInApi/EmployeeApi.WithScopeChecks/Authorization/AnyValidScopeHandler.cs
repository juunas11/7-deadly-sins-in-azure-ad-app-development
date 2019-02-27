using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeeApi.WithScopeChecks.Authorization
{
    public class AnyValidScopeHandler : AuthorizationHandler<AnyValidScopeRequirement>
    {
        private const string ScopeClaimType = "http://schemas.microsoft.com/identity/claims/scope";

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            AnyValidScopeRequirement requirement)
        {
            if (context.User.HasClaim(c => c.Type == ScopeClaimType)
                && context.User.FindFirstValue(ScopeClaimType)
                    .Split(' ')
                    .Any(scope => DelegatedPermissions.All.Contains(scope)))
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
