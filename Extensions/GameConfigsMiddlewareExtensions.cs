// Allow the use of app.FilterGameConfigs(); in Startup.cs instead of app.UseMiddleware<GameConfigsMiddleware>();
using API.Middleware;
using Microsoft.AspNetCore.Builder;

namespace API.Extensions {
    public static class GameConfigsMiddlewareExtensions {
        public static IApplicationBuilder FilterGameConfigs(this IApplicationBuilder builder) {
            return builder.UseMiddleware<GameConfigsMiddleware>();
        }
    }
}