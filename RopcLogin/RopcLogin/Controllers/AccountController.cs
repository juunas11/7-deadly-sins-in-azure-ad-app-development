using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RopcLogin.Models;
using RopcLogin.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RopcLogin.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class AccountController : Controller
    {
        private static readonly HttpClient HttpClient = new HttpClient();
        private AuthSettings _authSettings;

        public AccountController(IOptions<AuthSettings> authSettings)
        {
            _authSettings = authSettings.Value;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            // Please never actually do any of this if you can avoid it
            var (idToken, accessToken) = await AcquireTokensWithRopc(model);

            ClaimsPrincipal user = ConstructUser(idToken, accessToken);

            return SignIn(user, new AuthenticationProperties
            {
                IsPersistent = true,
                RedirectUri = "/"
            }, CookieAuthenticationDefaults.AuthenticationScheme);
        }

        [HttpPost]
        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        private async Task<(string idToken, string accessToken)> AcquireTokensWithRopc(LoginModel model)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, _authSettings.TokenEndpoint)
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["grant_type"] = "password",
                    ["client_id"] = _authSettings.ClientId,
                    ["client_secret"] = _authSettings.ClientSecret,
                    ["username"] = model.Username,
                    ["password"] = model.Password,
                    ["scope"] = $"openid profile {_authSettings.EmployeeApiAppIdUri}/.default"
                })
            };

            var res = await HttpClient.SendAsync(req);
            string json = await res.Content.ReadAsStringAsync();
            if (!res.IsSuccessStatusCode)
            {
                throw new Exception("Failed to acquire token: " + json);
            }

            var result = (JObject)JsonConvert.DeserializeObject(json);
            return (result.Value<string>("id_token"), result.Value<string>("access_token"));
        }

        private ClaimsPrincipal ConstructUser(string idToken, string accessToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var decodedToken = handler.ReadJwtToken(idToken);

            var desiredClaims = new HashSet<string>
            {
                "name",
                "oid",
                "preferred_username",
                "sub",
                "tid"
            };
            var claims = decodedToken.Claims.Where(c => desiredClaims.Contains(c.Type));
            claims = claims.Concat(new[] { new Claim("api_token", accessToken) });

            var identity = new ClaimsIdentity(claims, "ropc", "name", ClaimTypes.Role);
            return new ClaimsPrincipal(identity);
        }
    }
}
