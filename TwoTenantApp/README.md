# Two tenant app with lacking authorizations

This sample has two projects:

* TwoTenantApp: has problems in authorization
* TwoTenantAppDoneBetter: same app with the problems fixed

The main issue this showcases is that if you have an "N-tenant app",
that is to say an app that is only accessible by a specific number of tenants,
you need to do issuer validation.
In the general multi-tenant scenario issuer validation is commonly turned off.
In this case it should not be turned off!

The app has two login buttons, one for internal people and one for external people.
These redirect to a tenant-specific login endpoint for internal and external people, respectively.
So two Azure AD tenants are used.
But the app is registered as multi-tenant in Azure AD since we can't specify "allow login with these two".
So a user can just change the tenant id in the URL to whatever they want and login with another tenant.

In TwoTenantApp, they are able to login.
In TwoTenantAppDoneBetter, the login fails.

The reason is that the former disables issuer validation.
The latter defines both tenants as valid issuers:

```cs
o.TokenValidationParameters = new TokenValidationParameters
{
    // NOTE: We should not turn issuer validation off
    // We should instead list the valid issuers
    NameClaimType = "name",
    ValidIssuers = new[] // THIS IS IMPORTANT Only accept tokens from these tenants
    {
        $"https://login.microsoftonline.com/{authSettings.EmployeeTenantId}/v2.0",
        $"https://login.microsoftonline.com/{authSettings.PartnerTenantId}/v2.0"
    }
};
```

The TwoTenantApp also has a problem in the way it decides the user's role (employee/partner),
and ends up giving the partner role to users from any other tenant.

Always specify valid issuers if you know in advance which tenants are allowed!
If you don't know them in advance, add a validation step via the OpenIdConnectEvents.
There you can check the tenant id against a database for example.
In the general multi-tenant case, issuer validation can be disabled,
or you can specify a validator that checks the prefix.

## References

* [Tenancy in Azure Active Directory](https://docs.microsoft.com/en-us/azure/active-directory/develop/single-and-multi-tenant-apps)