using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using FluentValidation;
using makeITeasy.AppFramework.Core.Helpers;
using makeITeasy.AppFramework.Core.Infrastructure.Autofac;
using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.CarCatalog.Core.Domains.CarDomain;
using makeITeasy.CarCatalog.Infrastructure.Data;
using makeITeasy.CarCatalog.Infrastructure.Persistence;
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

        public void ConfigureServices(IServiceCollection services)
        {
            _ = services.AddAutoMapper(new Assembly[] {
                typeof(MappingProfile).Assembly, //Automatic IMapFrom mapping
                Assembly.GetExecutingAssembly() //Custom Mapping
            });
            
            _ = services.AddOptions();

            services.AddControllersWithViews();

            _ = services.AddDbContext<CarCatalogContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("dbConnectionString"))
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
                ;
            });

            Assembly[] assembliesToScan = new Assembly[]
            {
                    typeof(CarService).GetTypeInfo().Assembly,
            };

            services.AddMediatR(assembliesToScan);
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new RegisterAutofacModule() { Assemblies = new Assembly[]() { typeof(RegisterAutofacModule).Assembly, typeof(CarService).Assembly } });

            builder.RegisterAssemblyTypes(typeof(CarCatalog.Models.Car).Assembly)
                .Where(t => t.IsClosedTypeOf(typeof(IValidator<>))).AsImplementedInterfaces();

            builder.RegisterType<CarCatalogContext>();

            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IAsyncRepository<>)).InstancePerLifetimeScope()
            .PropertiesAutowired()
            .OnActivated(args => AutofacHelper.InjectProperties(args.Context, args.Instance, true));
            builder.RegisterType<AutofacValidatorFactory>().As<IValidatorFactory>().SingleInstance();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            this.AutofacContainer = app.ApplicationServices.GetAutofacRoot();

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
