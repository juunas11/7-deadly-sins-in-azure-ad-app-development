# Checking token permissions in an API

This sample is composed of two projects:

* EmployeeApi
* EmployeeApi.WithScopeChecks

EmployeeApi lacks the checks for permissions and is vulnerable to an attack.
EmployeeApi.WithScopeChecks fixes the issue.

An app in another Azure AD tenant can acquire an access token for this API even if the API is single-tenant.
The token won't contain any permissions.
But it is otherwise valid.
And because of this, tokens must be checked that they contain any delegated permission or application permission.

The DataStealingApp project in the repo illustrates the attack in action.

The main difference is that we require any token to have a valid delegated or application permission:

```cs
services.AddAuthorization(o =>
{
    // By default require that caller has any valid scope/role
    o.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .AddRequirements(new AnyValidScopeRequirement())
        .Build();
    o.AddPolicy(Policies.ListEmployees, p =>
    {
        p.AddRequirements(new EmployeeReadRequirement());
    });
});
services.AddSingleton<IAuthorizationHandler, AnyValidScopeHandler>();
services.AddSingleton<IAuthorizationHandler, EmployeeReadHandler>();
```

The `AnyValidScopeHandler` is responsible for the check, and is in the Authorization folder of the project.

Always check in an API protected by Azure AD that the access token contains at least one valid permission!