using System;
using System.Collections.Generic;
using EmployeeApi.Models;
using EmployeeApi.WithScopeChecks.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        // In-memory hard-coded data for testing
        private static readonly List<Employee> Employees = new List<Employee>
        {
            new Employee
            {
                Id = 1,
                FirstName = "First",
                LastName = "Person",
                BirthDate = new DateTime(1978, 1, 1)
            },
            new Employee
            {
                Id = 2,
                FirstName = "Second",
                LastName = "Person",
                BirthDate = new DateTime(1986, 5, 15)
            }
        };

        // GET api/employees
        [HttpGet]
        [Authorize(Policies.ListEmployees)]
        public ActionResult<IEnumerable<Employee>> Get()
        {
            return Employees;
        }
    }
}
