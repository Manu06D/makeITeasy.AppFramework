using System;
using System.Collections.Generic;
using Module = Autofac.Module;
using System.Reflection;
using Autofac;

namespace makeITeasy.AppFramework.Core.Helpers
{
    public class RegisterAutofacModule : Module
    {
        public List<Assembly> Assemblies { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assemblies.ToArray())
                  .Where(t => t.Name.EndsWith("Service"))
                  .AsImplementedInterfaces()
                  .PropertiesAutowired()
                  .OnActivated(args => AutofacHelper.InjectProperties(args.Context, args.Instance, true));

            builder.RegisterGeneric(typeof(Queries.GenericQueryCommandHandler<>)).AsImplementedInterfaces();
            builder.RegisterGeneric(typeof(Queries.GenericQueryWithProjectCommandHandler<,>)).AsImplementedInterfaces();
        }
    }
}
