using Microsoft.AspNetCore.Authorization;

namespace EmployeeApi.WithScopeChecks.Authorization
{
    public class AnyValidScopeRequirement : IAuthorizationRequirement
    {
    }
}
