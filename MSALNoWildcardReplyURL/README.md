# Avoiding wildcard reply URLs with MSAL.js

This sample contains an app that avoids usage of a wildcard reply URL.

I wrote this sample to support a blog article that explains this in more detail: [https://joonasw.net/view/avoiding-wildcard-reply-urls-with-msal-js](https://joonasw.net/view/avoiding-wildcard-reply-urls-with-msal-js).

It is a single page application built with the React-Redux ASP.NET Core template.
The MSAL.js library is used to integrate the app with Azure AD.
Now when signing in, by default MSAL.js will assign the `redirect_uri` parameter to be the current URL.
This would require us to either:

1. Define all of the possible paths in Azure AD
1. Define a wildcard reply URL

Neither of these options is particularly good,
so we do a step better here.

In ClientApp/src/components/AuthenticationService we configure MSAL.js:

```js
const ClientId = 'the-app-client-id';
const Authority = 'https://login.microsoftonline.com/the-aad-tenant-id';

var app = new Msal.UserAgentApplication(ClientId, Authority, () => { }, {
  cacheLocation: 'localStorage',
  redirectUri: window.location.origin + "/aad-callback",
  navigateToLoginRequestUrl: false,
  postLogoutRedirectUri: window.location.origin
});
```

By setting `navigateToLoginRequestUrl: false`,
we disable the default behaviour of using the current URL.
Then we set the `redirectUri` to be `/aad-callback` under the current hostname.
This makes it so that all Azure AD responses will come to that route.

The component at ClientApp/src/components/AuthenticatedRoute is used to guard sections of the app
that require authentication.
If the user isn't logged in, this component stores the current path in session storage
and requests MSAL.js to sign the user in.

A component is implemented for the `/aad-callback` route in ClientApp/src/components/AadCallback.
There we fetch the stored route from session storage,
validate it against a whitelist of allowed URLs,
and redirect the user there.

It is very important to validate the local redirect URL
and test your app for open redirect vulnerabilities.
This local redirect URL is user input,
and can be set to anything someone wants.
If you have an open redirect vulnerability,
your users could be subject to phishing attacks.
