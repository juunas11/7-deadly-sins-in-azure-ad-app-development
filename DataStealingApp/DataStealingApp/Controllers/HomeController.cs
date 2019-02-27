using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DataStealingApp.Models;
using DataStealingApp.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;

namespace DataStealingApp.Controllers
{
    public class HomeController : Controller
    {
        private static readonly HttpClient HttpClient = new HttpClient();
        private readonly AppSettings _appSettings;

        public HomeController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> DemoOne()
        {
            // Calls the vulnerable API
            var model = await GetDemoPageModel(_appSettings.EmployeeApiBaseUrl);
            return View(model);
        }

        public async Task<IActionResult> DemoTwo()
        {
            // Calls the API that checks scopes, should fail
            var model = await GetDemoPageModel(_appSettings.SafeEmployeeApiBaseUrl);
            return View("DemoOne", model);
        }

        private async Task<DemoModel> GetDemoPageModel(string apiBaseUrl)
        {
            var model = new DemoModel();
            try
            {
                var app = new ConfidentialClientApplication(
                    _appSettings.ClientId,
                    _appSettings.EmployeeApiAuthority,
                    "https://localhost", // redirect URI can be anything in this case
                    new ClientCredential(_appSettings.ClientSecret),
                    userTokenCache: null,
                    appTokenCache: new TokenCache());

                var authRes = await app.AcquireTokenForClientAsync(
                    new[] { _appSettings.EmployeeApiAppIdUri + "/.default" },
                    true);
                model.AccessToken = authRes.AccessToken;

                var req = new HttpRequestMessage(HttpMethod.Get, apiBaseUrl + "/api/employees");
                model.TargetUrl = req.RequestUri.ToString();
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authRes.AccessToken);
                var res = await HttpClient.SendAsync(req);
                if (res.IsSuccessStatusCode)
                {
                    model.Employees = await res.Content.ReadAsAsync<List<Employee>>();
                }
                else
                {
                    model.ErrorStatusCode = res.StatusCode;
                }
            }
            catch
            {
            }

            return model;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
