using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper.Contrib.Autofac.DependencyInjection;
using FluentValidation;
using makeITeasy.AppFramework.Core.Helpers;
using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.AppFramework.Models;
using makeITeasy.AppFramework.Web.Filters;
using makeITeasy.AppFramework.Web.Helpers;
using makeITeasy.CarCatalog.Core.Ports;
using makeITeasy.CarCatalog.Core.Services;
using makeITeasy.CarCatalog.Core.Services.Interfaces;
using makeITeasy.CarCatalog.Infrastructure.Data;
using makeITeasy.CarCatalog.Infrastructure.Persistence;
using makeITeasy.CarCatalog.Infrastructure.Repositories;
using makeITeasy.CarCatalog.Models;
using MediatR.Extensions.Autofac.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace makeITeasy.CarCatalog.WebApp
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private readonly IWebHostEnvironment _environment;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            _environment = environment;
        }

        public ILifetimeScope AutofacContainer { get; private set; }

        private readonly Assembly[] assembliesToScan = new Assembly[]{
                    AppFramework.Core.AppFrameworkCore.Assembly,
                    Core.CarCatalogCore.Assembly,
                    AppFrameworkModels.Assembly,
                    typeof(Car).Assembly
            };

        public void ConfigureServices(IServiceCollection services)
        {
            _ = services.AddOptions();
            services.AddScoped<DatatableExceptionFilter>();

            services.AddControllersWithViews().AddNewtonsoftJson();

            services.AddValidatorsFromAssemblies(assembliesToScan);

            services.AddServerSideBlazor().AddCircuitOptions(o =>
            {
                o.DetailedErrors = _environment.IsDevelopment();
            }); 

            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" });
            });

            _ = services.AddDbContextFactory<CarCatalogContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("dbConnectionString"))

                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
                ;
                options.AddInterceptors(new DatabaseInterceptor());
            });

            DatatableHelpers.RegisterDatatablesService(services);
        }

        public void ConfigureContainer(ContainerBuilder builder)
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
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseResponseCompression();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapBlazorHub();
            });
        }
    }
}
