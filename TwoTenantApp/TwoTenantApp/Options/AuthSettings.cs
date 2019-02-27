namespace TwoTenantApp.Options
{
    public class AuthSettings
    {
        public string ClientId { get; set; }
        public string EmployeeAuthorizationEndpoint => $"https://login.microsoftonline.com/{EmployeeTenantId}/oauth2/v2.0/authorize";
        public string PartnerAuthorizationEndpoint => $"https://login.microsoftonline.com/{PartnerTenantId}/oauth2/v2.0/authorize";
        public string EmployeeTenantId { get; set; }
        public string PartnerTenantId { get; set; }
    }
}
