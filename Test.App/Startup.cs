using Cv.Broker.Core.Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using StackExchange.Redis;
using System;
using System.Collections;

namespace Test.App
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connection = Configuration.GetValue<string>("POSTGRES_CONNECTION");
          
            services.AddDbContext<CoreContext>(options =>
                    options.UseNpgsql(connection));

            services.AddControllers();


            services.AddSwaggerGen(c =>
            {
                c.DescribeAllParametersInCamelCase();
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Test", Version = "v1" });
             
            });

            foreach (DictionaryEntry de in Environment.GetEnvironmentVariables())
                Console.WriteLine("{0} = {1}", de.Key, de.Value);

            // Redis
            services.AddSingleton(sp =>
            {
                var con = Configuration.GetValue<string>("REDIS_CONNECTION");
                var redis = ConnectionMultiplexer.Connect(con);
                return redis.GetDatabase(3);
            });

            // Mongo
            services.AddSingleton(sp =>
            {
                var con = Configuration.GetValue<string>("MONGODB_CONNECTION");
                var client = new MongoClient(con);
                return client;
            });
 
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app, 
            IWebHostEnvironment env,
            CoreContext dataContext)
        {

           dataContext.Database.Migrate();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Test v1"));

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
