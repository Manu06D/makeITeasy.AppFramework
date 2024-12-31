using Microsoft.OpenApi.Models;

using Scalar.AspNetCore;

using System.Runtime.CompilerServices;

namespace makeITeasy.CarCatalog.dotnet9.WebApp.Modules.StartupModules
{
    public static class ApiStartupModule
    {
        public static void AddApiSupport(this WebApplicationBuilder builder)
        {
            //enable API controllers and add openApi documentation
            builder.Services.AddControllers();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Sample API",
                    Version = "v1",
                    Description = "API to demonstrate Swagger integration."
                });
            });
            builder.Services.AddOpenApi();
        }

        public static void EnableApiSupport(this WebApplication app)
        {
            app.UseRouting();
            app.UseAntiforgery();
            //map API controllers
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            if (app.Environment.IsDevelopment())
            {
                ///swagger/index.html
                app.UseSwagger();
                app.UseSwaggerUI();
                ///scalar/v1
                app.MapOpenApi();
                app.MapScalarApiReference();
            }
        }   
    }
}
