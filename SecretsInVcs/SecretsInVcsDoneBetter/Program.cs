using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Newtonsoft.Json;

namespace SecretsInVcsDoneBetter
{
    class Program
    {
        private static AppSettings _settings;

        static async Task Main(string[] args)
        {
            var config = BuildConfig();
            _settings = config.Get<AppSettings>();
            string accessToken = await AcquireAccessTokenAsync();
            Employee[] employees = await GetEmployeeDataAsync(accessToken);
            await SyncEmployeeDataAsync(employees);
        }

        private static async Task<string> AcquireAccessTokenAsync()
        {
            var app = new ConfidentialClientApplication(
                _settings.ClientId,
                _settings.Authority,
                "https://localhost",
                new ClientCredential(_settings.ClientSecret),
                userTokenCache: null,
                appTokenCache: new TokenCache());
            var result = await app.AcquireTokenForClientAsync(new[] { $"{_settings.EmployeeApiAppIdUri}/.default" });
            return result.AccessToken;
        }

        private static async Task<Employee[]> GetEmployeeDataAsync(string accessToken)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var res = await client.GetAsync($"{_settings.EmployeeApiBaseUrl}/api/employees");
                string json = await res.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Employee[]>(json);
            }
        }

        private static async Task SyncEmployeeDataAsync(Employee[] employees)
        {
            foreach (var employee in employees)
            {
                Console.WriteLine($"Syncing employee {employee.FirstName} {employee.LastName}");
                // TODO implement sync
            }
        }

        private static IConfiguration BuildConfig()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            // Build config so we can get KeyVault URL from config
            var config = builder.Build();
            // Add KeyVault as configuration source
            var keyVaultUrl = config["KeyVaultUrl"];
            builder.AddAzureKeyVault(keyVaultUrl);
            return builder.Build();
        }
    }
}
