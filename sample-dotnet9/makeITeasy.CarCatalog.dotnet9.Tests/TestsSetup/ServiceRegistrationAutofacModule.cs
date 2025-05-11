using System.Reflection;

using Autofac;
using Autofac.Extensions.DependencyInjection;

using AutoMapper.Contrib.Autofac.DependencyInjection;

using FluentValidation;

using makeITeasy.AppFramework.Core.Helpers;
using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.AppFramework.Models;
using makeITeasy.CarCatalog.dotnet9.Core.Ports;
using makeITeasy.CarCatalog.dotnet9.Core.Services;
using makeITeasy.CarCatalog.dotnet9.Core.Services.Interfaces;
using makeITeasy.CarCatalog.dotnet9.Infrastructure.Data;
using makeITeasy.CarCatalog.dotnet9.Infrastructure.Persistence;
using makeITeasy.CarCatalog.dotnet9.Infrastructure.Repositories;
using makeITeasy.CarCatalog.dotnet9.Models;

using MediatR;
using MediatR.Extensions.Autofac.DependencyInjection;
using MediatR.Extensions.Autofac.DependencyInjection.Builder;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Module = Autofac.Module;

namespace makeITeasy.CarCatalog.dotnet9.Tests.TestsSetup
{
    public class ServiceRegistrationAutofacModule : Module
    {
        public string? DatabaseConnectionString { get; set; }
        public DatabaseType DatabaseType { get; set; }

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
                    Assembly.GetExecutingAssembly(),
                    typeof(Car).Assembly
            };


            //services.AddDbContext<CarCatalogContext>(options =>
            _ = services.AddDbContextFactory<CarCatalogContext>(options =>
                        {
                            options.AddInterceptors(new DatabaseInterceptor());

                            if (DatabaseType == DatabaseType.MsSql)
                            {
                                options.UseSqlServer(DatabaseConnectionString);
                            }
                            else if (DatabaseType == DatabaseType.SqlLite)
                            {
                                var sqlLiteMemoryConnection = new SqliteConnection("DataSource=:memory:");
                                sqlLiteMemoryConnection.Open();
                                options.UseSqlite(sqlLiteMemoryConnection);
                            }
                            else if (DatabaseType == DatabaseType.CosmosDb)
                            {
                                options.UseCosmos(
                                    "https://localhost:8081",
                                    "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
                                    databaseName: "OrdersDB");
                            }

                            options.EnableSensitiveDataLogging(true);
                            options.EnableDetailedErrors();
                            options.ConfigureWarnings(x => x.Ignore(RelationalEventId.AmbientTransactionWarning));
                        });

            services.AddValidatorsFromAssemblies(assembliesToScan);
            builder.Populate(services);

            builder.RegisterModule(new RegisterAutofacModule() { Assemblies = assembliesToScan });
            builder.RegisterAutoMapper(assemblies: assembliesToScan);

            var mediatrConfiguration = MediatRConfigurationBuilder.Create(assembliesToScan)
                    .WithAllOpenGenericHandlerTypesRegistered()
                    .WithRegistrationScope(RegistrationScope.Scoped) // currently only supported values are `Transient` and `Scoped`
                    .Build();
            builder.RegisterMediatR(mediatrConfiguration);

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
