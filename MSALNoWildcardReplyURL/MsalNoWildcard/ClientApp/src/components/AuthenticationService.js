import * as Msal from 'msal';

const ClientId = 'your-app-client-id';
const Authority = 'https://login.microsoftonline.com/your-aad-tenant-id';

var app = new Msal.UserAgentApplication(ClientId, Authority, () => { }, {
  cacheLocation: 'localStorage',
  redirectUri: window.location.origin + "/aad-callback",
  navigateToLoginRequestUrl: false,
  postLogoutRedirectUri: window.location.origin
});

export const isLoggedIn = () => !!app.getUser();
export const login = () => app.loginRedirect();
export const logout = () => app.logout();
export const getUser = () => app.getUser();