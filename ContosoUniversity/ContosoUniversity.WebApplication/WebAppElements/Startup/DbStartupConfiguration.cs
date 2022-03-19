using ContosoUniversity.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.WebApplication.WebAppElements.Startup
{
    public static class DbStartupConfiguration
    {
        public static void ConfigureDatabase(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContextFactory<ContosoUniversityDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("dbConnectionString"))

                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
                ;
            });
        }
    }
}
