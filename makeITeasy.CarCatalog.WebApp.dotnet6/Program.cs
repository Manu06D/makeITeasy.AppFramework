using Autofac;
using makeITeasy.AppFramework.Core.Helpers;
using makeITeasy.AppFramework.Models;

using Serilog;

using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) => config.WriteTo.Console());


Assembly[] assembliesToScan = new Assembly[]{
    makeITeasy.AppFramework.Core.AppFrameworkCore.Assembly,
    makeITeasy.CarCatalog.Core.CarCatalogCore.Assembly,
    AppFrameworkModels.Assembly
};

builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
    {
        builder.RegisterModule(new RegisterAutofacModule() { Assemblies = assembliesToScan });
    }
);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddOptions();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
