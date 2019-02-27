using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace TwoTenantAppDoneBetter.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult EmployeeLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = "/"
            };
            properties.Parameters.Add("userType", "employee");
            return Challenge(properties);
        }

        [HttpPost]
        public IActionResult PartnerLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = "/"
            };
            properties.Parameters.Add("userType", "partner");
            return Challenge(properties);
        }

        [HttpPost]
        public IActionResult SignOut() =>
            SignOut(CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme);

        [HttpGet]
        public IActionResult SignedOut() => View();
    }
}
