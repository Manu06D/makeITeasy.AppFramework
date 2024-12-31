using Autofac;

using AutoMapper.Contrib.Autofac.DependencyInjection;

using makeITeasy.AppFramework.Core.Helpers;
using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.CarCatalog.dotnet9.Core.Ports;
using makeITeasy.CarCatalog.dotnet9.Core.Services;
using makeITeasy.CarCatalog.dotnet9.Core.Services.Interfaces;
using makeITeasy.CarCatalog.dotnet9.Infrastructure.Data;
using makeITeasy.CarCatalog.dotnet9.Infrastructure.Persistence;
using makeITeasy.CarCatalog.dotnet9.Infrastructure.Repositories;

using MediatR.Extensions.Autofac.DependencyInjection;
using MediatR.Extensions.Autofac.DependencyInjection.Builder;

namespace makeITeasy.CarCatalog.dotnet9.WebApp.Modules.StartupModules
{
    public class AutofacModule : Module
    {
        public System.Reflection.Assembly[]? AssembliesToScan { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule(new RegisterAutofacModule() { Assemblies = AssembliesToScan });
            builder.RegisterAutoMapper(assemblies: AssembliesToScan);
            var mediatrConfiguration = MediatRConfigurationBuilder.Create(AssembliesToScan)
                    .WithAllOpenGenericHandlerTypesRegistered()
                    .WithRegistrationScope(RegistrationScope.Scoped) // currently only supported values are `Transient` and `Scoped`
                    .Build();
            builder.RegisterMediatR(mediatrConfiguration);

            builder.RegisterType<CarCatalogContext>();

            builder.RegisterGeneric(typeof(TransactionCarCatalogRepository<>)).As(typeof(IAsyncRepository<>)).InstancePerLifetimeScope()
                    .PropertiesAutowired()
                    .OnActivated(args => AutofacHelper.InjectProperties(args.Context, args.Instance, true));

            ////specific service/repository
            builder.RegisterType<CarService>().As<ICarService>();
            builder.RegisterType<CarRepository>().As<ICarRepository>();
        }
    }
}
