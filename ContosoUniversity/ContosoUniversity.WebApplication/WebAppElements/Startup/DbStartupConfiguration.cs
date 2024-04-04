using ContosoUniversity.Infrastructure.Data;
using ContosoUniversity.Infrastructure.Persistence;

using makeITeasy.AppFramework.Models;

using Microsoft.EntityFrameworkCore;

using System.Threading.Channels;

namespace ContosoUniversity.WebApplication.WebAppElements.Startup
{
    public static class DbStartupConfiguration
    {
        public static void ConfigureDatabase(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContextFactory<ContosoUniversityDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("dbConnectionString"))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .AddInterceptors(new DatabaseInterceptor(builder.Services.BuildServiceProvider().GetRequiredService<ChannelWriter<IBaseEntity>>()))
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
                ;
            });
        }
    }
}
