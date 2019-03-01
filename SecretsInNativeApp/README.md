# Storing secrets in native app

This sample is a Windows Forms app that stores a client secret in its code.
It calls the EmployeeApi using these credentials.
In the demo it is shown how easy it is to extract this secret from the executable.

You can simply compile the app and run the [*strings*](https://docs.microsoft.com/en-us/sysinternals/downloads/strings)
utility from Windows SysInternals on the executable.
You should find the client secret along with all the other pieces of text stored in the binary.

**DO NOT** store credentials in native apps!
This includes:

* Desktop apps
* Mobile apps
* Single Page apps
* React Native etc. apps

Any app that runs on a device you do not control is a native app.
If the API you are calling requires authentication,
you must authenticate the user.

## References

* [Azure AD v2.0 Protocols - OAuth 2.0 authorization code flow](https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-oauth2-auth-code-flow)