using Cv.Broker.Core.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Test.App.Domain;

namespace Test.App.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PropertiesController : ControllerBase
    {
        private readonly IDatabase database;
        private readonly ILogger<PropertiesController> _logger;
        private readonly CoreContext context;

        public PropertiesController(
            IDatabase database,
            ILogger<PropertiesController> logger) //, CoreContext context)
        {
            this.database = database ?? throw new ArgumentNullException(nameof(database));
            _logger = logger;
            //this.context = context;
        }


        static string GetPropertyKey(string id)
        {
            return $"properties:{id}";
        }

        [HttpGet("{id}")]
        public async Task<Property> Get(string name)
        {
            var key = GetPropertyKey(name);
            var data = await database.StringGetAsync(key);

            if (data.HasValue)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                };
                var @prop = JsonSerializer.Deserialize<Property>((string?)data, options);
                return @prop;
            }

            return null;
        }

        [HttpPost]
        public async Task<IActionResult> Add(Property property)
        {
            var key = GetPropertyKey(property.Name);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
            var raw = JsonSerializer.Serialize(property, options);

            await database.StringSetAsync(key, raw, expiry: TimeSpan.FromHours(2400));

            return Ok();
        }
    }
}
