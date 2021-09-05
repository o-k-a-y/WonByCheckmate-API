using API.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

// Validates game configurations
namespace API.Middleware {
    public class GameConfigsMiddleware {
        private readonly RequestDelegate _next;

        public GameConfigsMiddleware(RequestDelegate next) {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context, IChessStatsService svc) {
            
            string gameConfigs = context.Request.Query["configs"];

            // TODO: Actually implement the middleware and handle errors
            // if (gameConfigs != null) {
            //     string[] configArr = gameConfigs.Split(',');
            //     Console.WriteLine(configArr);
            //     foreach (string config in configArr) {
            //         string[] configParts = config.Split(':');
            //         if (configParts.Length != 3) {
            //             return NotFound();
            //         }

            //         if (!ValidGameConfigurations.Contains(new Config(configParts[0], configParts[1], configParts[2]))) {
            //             return NotFound();
            //         }
            //         Console.WriteLine(config);
            //     }
            // }

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}