# Data stealing app

This app demonstrates an attack that can be done on APIs which do not properly check for delegated/application permissions in tokens.

It acquires an access token from another Azure AD tenant to an API in that tenant.
It then calls the APIs in the CheckingScopesInApi solution, which you can find in this repo.

It does require three things:

1. API Azure AD tenant id
1. API identifier in Azure AD (client id or App ID URI)
1. API URL