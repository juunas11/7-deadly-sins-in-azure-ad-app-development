using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RopcLogin.Models;
using RopcLogin.Options;

namespace RopcLogin.Controllers
{
    public class HomeController : Controller
    {
        private static readonly HttpClient HttpClient = new HttpClient();
        private readonly AuthSettings _authSettings;

        public HomeController(IOptions<AuthSettings> authSettings)
        {
            _authSettings = authSettings.Value;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new HomeIndexModel();

            if (User.Identity.IsAuthenticated)
            {
                model.Employees = await GetEmployeeData();
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<EmployeeApiModel[]> GetEmployeeData()
        {
            var req = new HttpRequestMessage(HttpMethod.Get, $"{_authSettings.EmployeeApiBaseUrl}/api/employees");
            string accessToken = User.FindFirstValue("api_token");
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var res = await HttpClient.SendAsync(req);
            if (!res.IsSuccessStatusCode)
            {
                return null;
            }

            string json = await res.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<EmployeeApiModel[]>(json);
        }
    }
}
