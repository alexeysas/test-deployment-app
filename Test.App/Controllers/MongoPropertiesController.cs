using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using Test.App.Domain;

namespace Test.App.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MongoPropertiesController : ControllerBase
    {
        private readonly MongoClient client;
      
        public MongoPropertiesController(MongoClient client)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
        }

        [HttpGet("{name}")]
        public IActionResult Get(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("property name can not be null");
            }

            var collection = client.GetDatabase("test")
                 .GetCollection<BsonDocument>("properties");

            var filter = Builders<BsonDocument>.Filter.Eq("Name", name);
            var document = collection.Find(filter).FirstOrDefault();

            if (document == null)
            {
                return NotFound();
            }

            var propEntity = BsonSerializer.Deserialize<PropertyEntity>(document);

            return Ok(new Property()
            {
                Name = propEntity.Name,
                Value = propEntity.Value,
            });
        }

        [HttpPost]
        public async Task<IActionResult> Add(Property property)
        {
            var entity = new PropertyEntity
            {
                Name = property.Name,
                Value = property.Value
            };

            var bsonDocument = entity.ToBsonDocument();

            var collection = client.GetDatabase("test")
                .GetCollection<BsonDocument>("properties");

            await collection.InsertOneAsync(bsonDocument);

            return Ok();
        }


        public class PropertyEntity
        {
            public ObjectId Id { get; set; }
            public string Name { get; set; }
         
            public string Value { get; set; }
        }
    }
}
