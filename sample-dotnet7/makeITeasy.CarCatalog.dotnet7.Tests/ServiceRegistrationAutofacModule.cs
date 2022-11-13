using System.Reflection;

using Autofac;
using Autofac.Extensions.DependencyInjection;

using AutoMapper.Contrib.Autofac.DependencyInjection;

using FluentValidation;

using makeITeasy.AppFramework.Core.Helpers;
using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.AppFramework.Models;
using makeITeasy.CarCatalog.dotnet7.Infrastructure.Data;
using makeITeasy.CarCatalog.dotnet7.Infrastructure.Persistence;
using makeITeasy.CarCatalog.dotnet7.Infrastructure.Repositories;
using makeITeasy.CarCatalog.dotnet7.Models;

using MediatR;
using MediatR.Extensions.Autofac.DependencyInjection;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Module = Autofac.Module;
using makeITeasy.CarCatalog.dotnet7.Core.Ports;
using makeITeasy.CarCatalog.dotnet7.Core.Services.Interfaces;
using makeITeasy.CarCatalog.dotnet7.Core.Services;

namespace makeITeasy.CarCatalog.dotnet7.Tests
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
makeITeasy.CarCatalog.dotnet7.Core.CarCatalogCore.Assembly,                //Service Assembly
                    AppFrameworkModels.Assembly,                 //Models Assembly
                    Assembly.GetExecutingAssembly(),
                    typeof(Car).Assembly
            };

            var sqlLiteMemoryConnection = new SqliteConnection("DataSource=:memory:");
            sqlLiteMemoryConnection.Open();

            //services.AddDbContext<CarCatalogContext>(options =>
            _ = services.AddDbContextFactory<CarCatalogContext>(options =>
                        {
                            options.UseSqlite(sqlLiteMemoryConnection);
                            options.AddInterceptors(new DatabaseInterceptor());
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

            services.AddValidatorsFromAssemblies(assembliesToScan);
            builder.Populate(services);

            builder.RegisterModule(new RegisterAutofacModule() { Assemblies = assembliesToScan });
            builder.RegisterAutoMapper(assemblies: assembliesToScan);
            builder.RegisterMediatR(assembliesToScan);

            builder.RegisterType<CarCatalogContext>();

            builder.RegisterGeneric(typeof(LoggingBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterType<MediatRLog>().SingleInstance();

            builder.RegisterGeneric(typeof(TransactionCarCatalogRepository<>)).As(typeof(IAsyncRepository<>)).InstancePerLifetimeScope()
                .PropertiesAutowired()
                .OnActivated(args => AutofacHelper.InjectProperties(args.Context, args.Instance, true));

            //specific service/repository
            builder.RegisterType<CarService>().As<ICarService>()
                //let's inject the properties (IDateTimeProvider)
                .OnActivated(args => AutofacHelper.InjectProperties(args.Context, args.Instance, true));
            ;
            builder.RegisterType<CarRepository>().As<ICarRepository>();

            builder.RegisterType<UnitOfWork>().As(typeof(IUnitOfWork));
        }
    }
}
