using Cv.Broker.Core.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Test.App.Domain;

namespace Test.App.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeesController : ControllerBase
    {

        private readonly ILogger<EmployeesController> _logger;
        private readonly CoreContext context;

        public EmployeesController(ILogger<EmployeesController> logger, CoreContext context)
        {
            _logger = logger;
            this.context = context;
        }

        [HttpGet]
        public IEnumerable<Employee> Get()
        {
            return context.Employees.ToArray();
        }


        [HttpPost]
        public IActionResult Add(Employee employee)
        {
            context.Employees.Add(employee);
            context.SaveChanges();

            return Ok();
        }
    }
}
