using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeeApi.WithScopeChecks.Authorization
{
    public class EmployeeReadHandler : AuthorizationHandler<EmployeeReadRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, EmployeeReadRequirement requirement)
        {
            if (context.User.HasClaim(c => c.Type == Claims.ScopeClaimType)
                && context.User.FindAll(Claims.ScopeClaimType)
                    .Any(scope => scope.Value == DelegatedPermissions.ReadEmployees))
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
