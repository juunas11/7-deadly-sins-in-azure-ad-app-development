namespace EmployeeApi.WithScopeChecks.Authorization
{
    public static class ApplicationPermissions
    {
        public static string ReadEmployees = "Employees.Read.All";
        public static string[] All = new[] { ReadEmployees };
    }
}
