using Autofac;
using Autofac.Extensions.DependencyInjection;

using makeITeasy.AppFramework.Models;
using makeITeasy.CarCatalog.dotnet10.WebApp.Components;
using makeITeasy.CarCatalog.dotnet10.WebApp.Modules.StartupModules;

using Microsoft.AspNetCore.HttpLogging;

using Radzen;

using System.Reflection;

public partial class Program
{
    private static Assembly[] AssembliesToScan =
    [
        makeITeasy.AppFramework.Core.AppFrameworkCore.Assembly,
        makeITeasy.CarCatalog.dotnet10.Core.CarCatalogCore.Assembly,
        AppFrameworkModels.Assembly
    ];

    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDatabase(builder.Configuration);
        builder.Services.AddRadzenComponents();
        builder.Services.AddHttpLogging(options =>
        {
            options.LoggingFields = HttpLoggingFields.Request | HttpLoggingFields.Response;
            options.RequestBodyLogLimit = 2048;
            options.ResponseBodyLogLimit = 2048;
            options.RequestHeaders.Add("User-Agent");
        });


        builder.AddLogger();

        builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
        builder.Host.ConfigureContainer<ContainerBuilder>(builder => builder.RegisterModule(new AutofacModule() { AssembliesToScan = AssembliesToScan }));

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

        app.UseHttpsRedirection();

        app.UseAntiforgery();

        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}