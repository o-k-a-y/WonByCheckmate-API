using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using API.Data;
using API.Extensions;
using API.Middleware;
using API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace API {
    public class Startup {
        private readonly IConfiguration _config;
        public Startup(IConfiguration config) {
            _config = config;
        }



        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {

            // services.AddDbContext<DataContext>(options => {
            //     options.UseSqlite(_config.GetConnectionString("DefaultConnection"));
            // });


            var serverVersion = new MariaDbServerVersion(new Version(10, 5, 0));

            // Replace 'YourDbContext' with the name of your own DbContext derived class.
            services.AddDbContext<DataContext>(
                dbContextOptions => dbContextOptions
                    .UseMySql(Environment.GetEnvironmentVariable("CHESS_DB"), serverVersion)
                    // .UseMySql(Environment.GetEnvironmentVariable(_config.GetConnectionString("MariaDB")), serverVersion)
                    .EnableSensitiveDataLogging() // <-- These two calls are optional but help
                    .EnableDetailedErrors()       // <-- with debugging (remove for production).
            );
            services.AddScoped<IChessStatsService, ChessStatsService>();

            services.AddControllers();
            // .AddNewtonsoftJson(); // Supports return newtonsoft json objects from controllers
            // https://docs.microsoft.com/en-us/aspnet/core/web-api/advanced/formatting?view=aspnetcore-5.0#add-newtonsoftjson-based-json-format-support
            // .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null); // should make it PascalCase, does not if AddNewtonsoftJson is added. Also I don't even want PascalCase
            // services.AddSwaggerGen(c => {
            //     c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
            // });

            services.AddHttpClient<HttpClient>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            // if (env.IsDevelopment()) {
            //     app.UseDeveloperExceptionPage();
            //     app.UseSwagger();
            //     app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
            // }

            // Only allow valid game configurations
            app.FilterGameConfigs();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(policy => policy.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader());

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });

        }
    }
}
