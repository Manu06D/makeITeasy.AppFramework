using Microsoft.AspNetCore.HttpLogging;

using Serilog;
using Serilog.Context;

namespace makeITeasy.CarCatalog.dotnet10.WebApp.Modules.StartupModules
{
    public static class LoggerModule
    {
        public static void AddLogger(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<IHttpLoggingInterceptor, CustomLoggingInterceptor>();

            builder.Services.AddSerilog((services, lc) => lc
                .ReadFrom.Configuration(builder.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .WriteTo.Console());
        }
    }

    public class CustomLoggingInterceptor : IHttpLoggingInterceptor
    {
        public ValueTask OnRequestAsync(HttpLoggingInterceptorContext context)
        {
            context.HttpContext.Request.Headers.Remove("X-API-Key");

            return ValueTask.CompletedTask;
        }

        public ValueTask OnResponseAsync(HttpLoggingInterceptorContext context)
        {
            context.HttpContext.Response.Headers.Remove("Set-Cookie");

            return ValueTask.CompletedTask;
        }
    }
}
