using Serilog;

namespace makeITeasy.CarCatalog.dotnet9.WebApp.Modules.StartupModules
{
    public static class LoggerModule
    {
        public static void AddLogger(this WebApplicationBuilder builder)
        {
            builder.Services.AddSerilog((services, lc) => lc
                .ReadFrom.Configuration(builder.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .WriteTo.Console());
        }
    }
}
