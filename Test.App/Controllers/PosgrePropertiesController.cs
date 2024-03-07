using Cv.Broker.Core.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Test.App.Domain;

namespace Test.App.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostgreePropertiesController : ControllerBase
    {
        private readonly CoreContext db;

        public PostgreePropertiesController(CoreContext db)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> Get(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("property name can not be null");
            }

            var res = await db.Properties.FirstOrDefaultAsync(x => x.Name == name);
        
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Property property)
        {
            if (property == null || property.Name == null)
            {
                return BadRequest("property name can not be null");
            }

            db.Properties.Add(property);
            await db.SaveChangesAsync();

            return Ok();
        }
    }
}
