using Cv.Broker.Core.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Test.App.Domain;

namespace Test.App.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RedisPropertiesController : ControllerBase
    {
        private readonly IDatabase database;
     
        public RedisPropertiesController(IDatabase database)
        {
            this.database = database ?? throw new ArgumentNullException(nameof(database));
        }

        [HttpGet("{name}")]
        public async Task<Property> Get(string name)
        {
            var key = GetPropertyKey(name);
            var data = await database.StringGetAsync(key);

            if (data.HasValue)
            {
                var @prop = JsonSerializer.Deserialize<Property>((string?)data, GetJsonOptions());
                return @prop;
            }

            return null;
        }

        [HttpPost]
        public async Task<IActionResult> Add(Property property)
        {
            var key = GetPropertyKey(property.Name);

            var raw = JsonSerializer.Serialize(property, GetJsonOptions());

            await database.StringSetAsync(key, raw, expiry: TimeSpan.FromHours(2400));

            return Ok();
        }

        static JsonSerializerOptions GetJsonOptions()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };

            return options;
        }
        static string GetPropertyKey(string id)
        {
            return $"properties:{id}";
        }
    }
}
