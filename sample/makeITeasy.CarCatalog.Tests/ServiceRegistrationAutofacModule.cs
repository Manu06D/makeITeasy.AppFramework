using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using FluentValidation;
using makeITeasy.AppFramework.Core.Helpers;
using makeITeasy.AppFramework.Core.Infrastructure.Autofac;
using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.AppFramework.Core.Services;
using makeITeasy.AppFramework.Models;
using makeITeasy.CarCatalog.Core.Domains.CarDomain;
using makeITeasy.CarCatalog.Infrastructure.Data;
using makeITeasy.CarCatalog.Infrastructure.Persistence;
using makeITeasy.CarCatalog.Models;
using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
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
            Assembly[] assembliesToScan = new Assembly[]{
                    typeof(BaseEntityService<Car>).GetTypeInfo().Assembly,
                    typeof(CarService).GetTypeInfo().Assembly,
                    typeof(BaseQuery<>).GetTypeInfo().Assembly};


            services.AddMediatR(assembliesToScan);
            services.AddAutoMapper(new Assembly[] {
                typeof(MappingProfile).Assembly, //Automatic IMapFrom mapping   
                Assembly.GetExecutingAssembly() //Custom Mapping
            });

            services.AddLogging(opt =>
            {
                opt.SetMinimumLevel(LogLevel.Debug);
                opt.AddDebug();

            });

            var sqlLiteMemoryConnection = new SqliteConnection("DataSource=:memory:");
            sqlLiteMemoryConnection.Open();

            services.AddDbContext<CarCatalogContext>(options =>
            {
                options.UseSqlite(sqlLiteMemoryConnection);
                //options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=CarCatalog2;Trusted_Connection=True;MultipleActiveResultSets=true");
                options.EnableSensitiveDataLogging(true);
            });

            builder.Populate(services);

            builder.RegisterModule(new RegisterAutofacModule() { Assemblies = new List<Assembly>() { typeof(RegisterAutofacModule).Assembly, typeof(CarService).Assembly } });

            builder.RegisterAssemblyTypes(typeof(CarCatalog.Models.Car).Assembly)
                .Where(t => t.IsClosedTypeOf(typeof(IValidator<>))).AsImplementedInterfaces();

            builder.RegisterType<CarCatalogContext>();

            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IAsyncRepository<>)).InstancePerLifetimeScope()
            .PropertiesAutowired()
            .OnActivated(args => AutofacHelper.InjectProperties(args.Context, args.Instance, true));
            builder.RegisterType<AutofacValidatorFactory>().As<IValidatorFactory>().SingleInstance();
        }
    }
}
