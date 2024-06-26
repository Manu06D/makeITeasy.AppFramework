﻿using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper.Contrib.Autofac.DependencyInjection;
using MediatR.Extensions.Autofac.DependencyInjection;

using ContosoUniversity.Core.Services;
using ContosoUniversity.Infrastructure.Data;

using FluentValidation;

using makeITeasy.AppFramework.Core.Helpers;

using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.AppFramework.Models;

using System.Reflection;
using ContosoUniversity.Infrastructure;
using MediatR.Extensions.Autofac.DependencyInjection.Builder;

namespace ContosoUniversity.WebApplication.WebAppElements.Startup
{
    public static class AutofacStartupConfiguration
    {
        public static void ConfigureAutofac(this WebApplicationBuilder builder)
        {
            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

            Assembly modelAssembly = typeof(StudentService).Assembly;
            Assembly[] assembliesToScan = new Assembly[]
                {
                    makeITeasy.AppFramework.Core.AppFrameworkCore.Assembly,
                    modelAssembly,
                    AppFrameworkModels.Assembly
                };

            builder.Host.ConfigureContainer<ContainerBuilder>(
            builder =>
            {
                builder.RegisterModule(new RegisterAutofacModule() { Assemblies = assembliesToScan });
                builder.RegisterAutoMapper(assemblies:assembliesToScan);

                var mediatrConfiguration = MediatRConfigurationBuilder.Create(assembliesToScan)
                        .WithAllOpenGenericHandlerTypesRegistered()
                        .WithRegistrationScope(RegistrationScope.Scoped) // currently only supported values are `Transient` and `Scoped`
                        .Build();
                builder.RegisterMediatR(mediatrConfiguration);

                builder.RegisterType<ContosoUniversityDbContext>();

                builder.RegisterGeneric(typeof(UniversityCatalogRepository<>)).As(typeof(IAsyncRepository<>)).InstancePerLifetimeScope()
                        .PropertiesAutowired()
                        .OnActivated(args => AutofacHelper.InjectProperties(args.Context, args.Instance, true));

            }
        );
        }
    }
}
