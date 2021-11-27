using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper.Contrib.Autofac.DependencyInjection;
using MediatR.Extensions.Autofac.DependencyInjection;

using FluentValidation;

using makeITeasy.AppFramework.Core.Helpers;
using makeITeasy.AppFramework.Core.Infrastructure.Autofac;
using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.AppFramework.Models;
using makeITeasy.AppFramework.Web.Filters;
using makeITeasy.AppFramework.Web.Helpers;
using makeITeasy.CarCatalog.Core.Ports;
using makeITeasy.CarCatalog.Core.Services.Interfaces;
using makeITeasy.CarCatalog.Core.Services;
using makeITeasy.CarCatalog.Infrastructure.Data;
using makeITeasy.CarCatalog.Infrastructure.Persistence;
using makeITeasy.CarCatalog.Infrastructure.Repositories;
using makeITeasy.CarCatalog.Models;

using Microsoft.EntityFrameworkCore;

using Serilog;

using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) => config.WriteTo.Console());

Assembly[] assembliesToScan = new Assembly[]
    {
        makeITeasy.AppFramework.Core.AppFrameworkCore.Assembly,
        makeITeasy.CarCatalog.Core.CarCatalogCore.Assembly,
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

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(
    builder =>
    {
        builder.RegisterModule(new RegisterAutofacModule() { Assemblies = assembliesToScan });
        builder.RegisterAutoMapper(assembliesToScan);
        builder.RegisterMediatR(assembliesToScan);

        builder.RegisterType<CarCatalogContext>();

        builder.RegisterGeneric(typeof(TransactionCarCatalogRepository<>)).As(typeof(IAsyncRepository<>)).InstancePerLifetimeScope()
                .PropertiesAutowired()
                .OnActivated(args => AutofacHelper.InjectProperties(args.Context, args.Instance, true));

        builder.RegisterAssemblyTypes(CarCatalogModels.Assembly).Where(t => t.IsClosedTypeOf(typeof(IValidator<>))).AsImplementedInterfaces();

        builder.RegisterType<AutofacValidatorFactory>().As<IValidatorFactory>().SingleInstance();

        ////specific service/repository
        builder.RegisterType<CarService>().As<ICarService>();
        builder.RegisterType<CarRepository>().As<ICarRepository>();
    }
);

DatatableHelpers.RegisterDatatableService(builder.Services);

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