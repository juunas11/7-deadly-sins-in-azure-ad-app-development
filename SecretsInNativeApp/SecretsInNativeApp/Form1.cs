using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Windows.Forms;

namespace SecretsInNativeApp
{
    public partial class Form1 : Form
    {
        private const string Authority = "https://login.microsoftonline.com/your-tenant-id/v2.0";
        private const string ClientId = "your-app-client-id";
        private const string Secret = "your-app-client-secret";
        private const string RedirectUri = "https://localhost";
        private const string Scope = "your-app-id-uri-or-client-id/.default"; // e.g. client id + /.default
        private const string ApiBaseUrl = "https://localhost:44316"; // No scope checks
        //private const string ApiBaseUrl = "https://localhost:44314"; // With scope checks
        private static readonly HttpClient HttpClient = new HttpClient();

        public Form1()
        {
            InitializeComponent();
        }

        private async void btnCallApi_Click(object sender, EventArgs eventArgs)
        {
            txtResults.Text = "Loading data...";
            // Acquire token with client credentials --(NEVER DO THIS FROM A NATIVE APP)--
            var credentials = new ClientCredential(Secret);
            var app = new ConfidentialClientApplication(
                ClientId, Authority, RedirectUri, credentials, null, new TokenCache());
            var result = await app.AcquireTokenForClientAsync(new [] { Scope });
            var token = result.AccessToken;

            // Call API
            var req = new HttpRequestMessage(HttpMethod.Get, ApiBaseUrl + "/api/employees");
            req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var res = await HttpClient.SendAsync(req);
            string json = await res.Content.ReadAsStringAsync();

            // Parse results into data model
            var employees = JsonConvert.DeserializeObject<EmployeeData[]>(json);

            // Display results in text box
            txtResults.Text = string.Join(Environment.NewLine, employees.Select(e => e.FirstName + " " + e.LastName));
        }
    }
}
