using Autofac;
using Autofac.Extensions.DependencyInjection;

using makeITeasy.AppFramework.Models;
using makeITeasy.CarCatalog.dotnet9.WebApp.Components;
using makeITeasy.CarCatalog.dotnet9.WebApp.Modules.StartupModules;

using Radzen;

using System.Reflection;

Assembly[] assembliesToScan =
    [
        makeITeasy.AppFramework.Core.AppFrameworkCore.Assembly,
        makeITeasy.CarCatalog.dotnet9.Core.CarCatalogCore.Assembly,
        AppFrameworkModels.Assembly
    ];

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddRadzenComponents();

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(builder => builder.RegisterModule(new AutofacModule() { AssembliesToScan = assembliesToScan }));

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

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
