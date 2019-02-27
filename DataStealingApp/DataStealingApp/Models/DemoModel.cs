using System.Collections.Generic;
using System.Net;

namespace DataStealingApp.Models
{
    public class DemoModel
    {
        public string AccessToken { get; set; }
        public string TargetUrl { get; set; }
        public List<Employee> Employees { get; set; }
        public HttpStatusCode ErrorStatusCode { get; set; }
    }
}
