using System;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using FluentValidation;
using makeITeasy.AppFramework.Core.Helpers;
using makeITeasy.AppFramework.Core.Infrastructure.Autofac;
using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.AppFramework.Models;
using makeITeasy.AppFramework.Web.Helpers;
using makeITeasy.CarCatalog.Core.Ports;
using makeITeasy.CarCatalog.Core.Services;
using makeITeasy.CarCatalog.Core.Services.Interfaces;
using makeITeasy.CarCatalog.Infrastructure.Data;
using makeITeasy.CarCatalog.Infrastructure.Persistence;
using makeITeasy.CarCatalog.Infrastructure.Repositories;
using makeITeasy.CarCatalog.Models;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace makeITeasy.CarCatalog.WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public ILifetimeScope AutofacContainer { get; private set; }

        private readonly Assembly[] assembliesToScan = new Assembly[]{
                    AppFramework.Core.AppFrameworkCore.Assembly,
                    Core.CarCatalogCore.Assembly,
                    AppFrameworkModels.Assembly
            };

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMediatR(assembliesToScan);

            services.AddAutoMapper(new Assembly[] {
                AppFramework.Core.AppFrameworkCore.Assembly, //Automatic IMapFrom mapping   
                Assembly.GetExecutingAssembly() //Custom Mapping
            });

            _ = services.AddOptions();

            services.AddControllersWithViews().AddNewtonsoftJson(); 

            _ = services.AddDbContext<CarCatalogContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("dbConnectionString"))
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
                ;
            });

            DatatableHelpers.RegisterDatatableService(services);
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {

            builder.RegisterModule(new RegisterAutofacModule() { Assemblies = assembliesToScan });

            builder.RegisterType<CarCatalogContext>();

            builder.RegisterGeneric(typeof(CarCatalogRepository<>)).As(typeof(IAsyncRepository<>)).InstancePerLifetimeScope()
                    .PropertiesAutowired()
                    .OnActivated(args => AutofacHelper.InjectProperties(args.Context, args.Instance, true));

            builder.RegisterAssemblyTypes(CarCatalogModels.Assembly).Where(t => t.IsClosedTypeOf(typeof(IValidator<>))).AsImplementedInterfaces();
            
            builder.RegisterType<AutofacValidatorFactory>().As<IValidatorFactory>().SingleInstance();

            //specific service/repository
            builder.RegisterType<CarService>().As<ICarService>();
            builder.RegisterType<CarRepository>().As<ICarRepository>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            AutofacContainer = app.ApplicationServices.GetAutofacRoot();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
