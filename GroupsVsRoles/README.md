# Group claims vs Roles for authorization

This sample contains two projects:

* AppWithGroups: Uses group membership claims to authorize admins
* AppWithRoles: Uses app roles to do the same thing

The sample shows one of the downsides of using group claims.
If a user has more than 200 groups, they will be unable to access the admin section even if they are
a member of the admin group.
This is due to token size limitations.
If a user has more than 200 groups, the group ids will not be included in tokens.
You can use the CreateAndAssignGroups.ps1 script to create groups to test this behavior yourself.
You will also have to set groupMembershipClaims to "SecurityGroup" in the application manifest in Azure AD:

```json
{
	"groupMembershipClaims": "SecurityGroup"
}
```

The AppWithRoles sample requires that a role is defined in the application manifest in Azure AD:

```json
{
	"appRoles": [
		{
			"allowedMemberTypes": [
				"User"
			],
			"description": "Administrators can access Admin-only section",
			"displayName": "Administrators",
			"id": "d1613ef0-097a-44a0-b1d2-c13c02231a97",
			"isEnabled": true,
			"origin": "Application",
			"value": "admin"
		}
	]
}
```

The role also needs to be then assigned to your user/user's group.

The main difference is that with group claims the authorization policy is setup as:

```cs
services.AddAuthorization(o =>
{
    o.AddPolicy(Policies.AdminOnly, p =>
    {
        // Require group membership
        p.RequireClaim("groups", Configuration["Auth:AdminGroupId"]);
    });
});
```

While with role claims it is:

```cs
services.AddAuthorization(o =>
{
    o.AddPolicy(Policies.AdminOnly, p =>
    {
        // Require role
        p.RequireClaim(ClaimTypes.Role, Configuration["Auth:AdminRoleValue"]);
    });
});
```

Instead of group claims, you can also query the groups' members from MS Graph API.
I prefer using app roles, but using groups in this way is not the end of the world.

## References

* [How to: Add app roles in your application and receive them in the token](https://docs.microsoft.com/en-us/azure/active-directory/develop/howto-add-app-roles-in-azure-ad-apps)
* [Authorization in a web app using Azure AD application roles & role claims (Sample)](https://azure.microsoft.com/en-us/resources/samples/active-directory-dotnet-webapp-roleclaims/)
* [Authorization in a web app using Azure AD groups & group claims (Sample)](https://azure.microsoft.com/en-us/resources/samples/active-directory-dotnet-webapp-groupclaims/)
* [List Group members via Microsoft Graph API](https://docs.microsoft.com/en-us/graph/api/group-list-members?view=graph-rest-1.0)