using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeeApi.WithScopeChecks.Authorization
{
    public class EmployeeReadHandler : AuthorizationHandler<EmployeeReadRequirement>
    {
        private const string ScopeClaimType = "http://schemas.microsoft.com/identity/claims/scope";

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, EmployeeReadRequirement requirement)
        {
            if (context.User.HasClaim(c => c.Type == ScopeClaimType)
                && context.User.FindFirstValue(ScopeClaimType)
                    .Split(' ')
                    .Any(scope => scope == DelegatedPermissions.ReadEmployees))
            {
                // Caller has valid delegated permission
                context.Succeed(requirement);
            }
            else if (context.User.HasClaim(c => c.Type == ClaimTypes.Role)
                && context.User.FindAll(ClaimTypes.Role)
                    .Any(role => role.Value == ApplicationPermissions.ReadEmployees))
            {
                // Caller has valid app permission
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
