using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper.Contrib.Autofac.DependencyInjection;
using MediatR.Extensions.Autofac.DependencyInjection;

using FluentValidation;

using makeITeasy.AppFramework.Core.Helpers;
using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.AppFramework.Models;
using makeITeasy.AppFramework.Web.Filters;
using makeITeasy.AppFramework.Web.Helpers;
using makeITeasy.CarCatalog.dotnet7.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

using Serilog;

using System.Reflection;
using Microsoft.AspNetCore.ResponseCompression;
using makeITeasy.CarCatalog.dotnet7.Infrastructure.Repositories;
using makeITeasy.CarCatalog.dotnet7.Infrastructure.Data;
using makeITeasy.CarCatalog.dotnet7.Models;
using makeITeasy.CarCatalog.dotnet7.Core.Ports;
using makeITeasy.CarCatalog.dotnet7.Core.Services.Interfaces;
using makeITeasy.CarCatalog.dotnet7.Core.Services;
using makeITeasy.CarCatalog.dotnet7.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) => config.WriteTo.Console().WriteTo.Debug());

Assembly[] assembliesToScan = new Assembly[]
    {
        makeITeasy.AppFramework.Core.AppFrameworkCore.Assembly,
        CarCatalogCore.Assembly,
        AppFrameworkModels.Assembly
    };

builder.Services.AddControllersWithViews();
builder.Services.AddOptions();
builder.Services.AddScoped<DatatableExceptionFilter>();
builder.Services.AddDbContextFactory<CarCatalogContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("dbConnectionString"))

    .EnableDetailedErrors()
    .EnableSensitiveDataLogging()
;
    options.AddInterceptors(new DatabaseInterceptor());
});
builder.Services.AddServerSideBlazor( o => { o.DetailedErrors = true; }).AddCircuitOptions(o => { o.DetailedErrors = true; /* _environment.IsDevelopment(); */ });

builder.Services.AddValidatorsFromAssembly(typeof(Car).Assembly);

builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" });
});

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(
    builder =>
    {
        builder.RegisterModule(new RegisterAutofacModule() { Assemblies = assembliesToScan });
        builder.RegisterAutoMapper(assemblies: assembliesToScan);
        builder.RegisterMediatR(assembliesToScan);

        builder.RegisterType<CarCatalogContext>();

        builder.RegisterGeneric(typeof(TransactionCarCatalogRepository<>)).As(typeof(IAsyncRepository<>)).InstancePerLifetimeScope()
                .PropertiesAutowired()
                .OnActivated(args => AutofacHelper.InjectProperties(args.Context, args.Instance, true));

        ////specific service/repository
        builder.RegisterType<CarService>().As<ICarService>();
        builder.RegisterType<CarRepository>().As<ICarRepository>();
    }
);

builder.Services.RegisterDatatablesService();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseResponseCompression();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapBlazorHub();

app.Run();