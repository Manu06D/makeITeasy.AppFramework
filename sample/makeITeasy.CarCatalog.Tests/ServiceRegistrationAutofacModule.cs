using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper.Contrib.Autofac.DependencyInjection;
using FluentValidation;
using makeITeasy.AppFramework.Core.Helpers;
using makeITeasy.AppFramework.Core.Infrastructure.Autofac;
using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.AppFramework.Models;
using makeITeasy.CarCatalog.Core.Ports;
using makeITeasy.CarCatalog.Core.Services;
using makeITeasy.CarCatalog.Core.Services.Interfaces;
using makeITeasy.CarCatalog.Infrastructure.Data;
using makeITeasy.CarCatalog.Infrastructure.Persistence;
using makeITeasy.CarCatalog.Infrastructure.Repositories;
using makeITeasy.CarCatalog.Models;
using MediatR;
using MediatR.Extensions.Autofac.DependencyInjection;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Module = Autofac.Module;

namespace makeITeasy.CarCatalog.Tests
{
    public class ServiceRegistrationAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var services = new ServiceCollection();

            services.AddLogging(opt =>
            {
                opt.SetMinimumLevel(LogLevel.Debug);
                opt.AddDebug();

            });

            Assembly[] assembliesToScan = new Assembly[]{
                    AppFramework.Core.AppFrameworkCore.Assembly, //Framework Assembly
                    Core.CarCatalogCore.Assembly,                //Service Assembly
                    AppFrameworkModels.Assembly,                 //Models Assembly
                    Assembly.GetExecutingAssembly()};

            var sqlLiteMemoryConnection = new SqliteConnection("DataSource=:memory:");
            sqlLiteMemoryConnection.Open();

            //services.AddDbContext<CarCatalogContext>(options =>
            _ = services.AddDbContextFactory<CarCatalogContext>(options =>
                        {
                            options.UseSqlite(sqlLiteMemoryConnection);
                            //options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=CarCatalog3;Trusted_Connection=True;MultipleActiveResultSets=true");
                            options.EnableSensitiveDataLogging(true);
                            options.ConfigureWarnings(x => x.Ignore(RelationalEventId.AmbientTransactionWarning));
                        });

            //_ = services.AddDbContextFactory<CarCatalogContext>(options =>
            //{
            //    options.UseCosmos(
            //        "https://localhost:8081",
            //        "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
            //        databaseName: "OrdersDB");
            //    options.EnableSensitiveDataLogging(true);
            //    options.EnableDetailedErrors();
            //});

            builder.Populate(services);

            builder.RegisterModule(new RegisterAutofacModule() { Assemblies = assembliesToScan });
            builder.RegisterAutoMapper(assembliesToScan);
            builder.RegisterMediatR(assembliesToScan);

            builder.RegisterType<CarCatalogContext>();

            builder.RegisterGeneric(typeof(LoggingBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterType<MediatRLog>().SingleInstance();

            builder.RegisterGeneric(typeof(CarCatalogRepository<>)).As(typeof(IAsyncRepository<>)).InstancePerLifetimeScope()
                .PropertiesAutowired()
                .OnActivated(args => AutofacHelper.InjectProperties(args.Context, args.Instance, true));

            //specific service/repository
            builder.RegisterType<CarService>().As<ICarService>();
            builder.RegisterType<CarRepository>().As<ICarRepository>();

            builder.RegisterAssemblyTypes(CarCatalogModels.Assembly).Where(t => t.IsClosedTypeOf(typeof(IValidator<>))).AsImplementedInterfaces();
            builder.RegisterType<AutofacValidatorFactory>().As<IValidatorFactory>().SingleInstance();

            builder.RegisterType<UnitOfWork>().As(typeof(IUnitOfWork));
        }
    }
}
