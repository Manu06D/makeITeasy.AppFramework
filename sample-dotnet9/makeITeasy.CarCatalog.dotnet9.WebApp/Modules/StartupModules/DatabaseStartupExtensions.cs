using makeITeasy.CarCatalog.dotnet9.Infrastructure.Data;
using makeITeasy.CarCatalog.dotnet9.Infrastructure.Persistence;
using makeITeasy.CarCatalog.dotnet9.WebApp.Modules.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;

namespace makeITeasy.CarCatalog.dotnet9.WebApp.Modules.StartupModules
{
    public static class DatabaseStartupExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, ConfigurationManager configuration)
        {
            //way 1 to map options
            services.Configure<DatabaseOptions>("DatabaseOptions", configuration);

            //way 2 to map options
            DatabaseOptions? dbOptions = configuration.GetSection("DatabaseOptions")?.Get<DatabaseOptions>();

            //way 3
            services
                .AddOptions<DatabaseOptions>()
                .Bind(configuration.GetSection("DatabaseOptions"))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            _ = services.AddDbContextFactory<CarCatalogContext>(options =>
            {
                bool enableSensitiveDataLogging;

                //way 1 - 3
                var databaseOptions = services.BuildServiceProvider().GetService<IOptions<DatabaseOptions>>()!.Value;
                enableSensitiveDataLogging = databaseOptions.EnableSensitiveDataLogging;

                //way 2
                enableSensitiveDataLogging = dbOptions?.EnableSensitiveDataLogging ?? false;

                options.UseSqlServer(configuration.GetConnectionString("dbConnectionString"));
                options.AddInterceptors(new DatabaseInterceptor());
                options.EnableSensitiveDataLogging(enableSensitiveDataLogging);
                options.ConfigureWarnings(x => x.Ignore(RelationalEventId.AmbientTransactionWarning));
            });

            return services;
        }
    }
}
