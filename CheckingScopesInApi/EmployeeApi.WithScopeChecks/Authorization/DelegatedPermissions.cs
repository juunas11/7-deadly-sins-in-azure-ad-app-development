namespace EmployeeApi.WithScopeChecks.Authorization
{
    public static class DelegatedPermissions
    {
        public static string ReadEmployees = "Employees.Read";
        public static string[] All = new[] { ReadEmployees };
    }
}
