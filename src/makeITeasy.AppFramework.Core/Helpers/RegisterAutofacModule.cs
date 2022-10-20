using System;
using Module = Autofac.Module;
using System.Reflection;
using Autofac;

namespace makeITeasy.AppFramework.Core.Helpers
{
    public class RegisterAutofacModule : Module
    {
        public Assembly[]? Assemblies { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            if (Assemblies != null)
            {
                builder.RegisterAssemblyTypes(Assemblies)
                      .Where(t => t.Name.EndsWith("Service"))
                      .AsImplementedInterfaces()
                      .PropertiesAutowired()
                      .OnActivated(args => AutofacHelper.InjectProperties(args.Context, args.Instance, true));
            }

            builder.RegisterGeneric(typeof(Queries.GenericQueryCommandHandler<>)).AsImplementedInterfaces();
            builder.RegisterGeneric(typeof(Queries.GenericQueryWithProjectCommandHandler<,>)).AsImplementedInterfaces();
            builder.RegisterGeneric(typeof(Queries.GenericFindUniqueCommandHandler<>)).AsImplementedInterfaces();
            builder.RegisterGeneric(typeof(Queries.GenericFindUniqueWithProjectCommandHandler<,>)).AsImplementedInterfaces();
            builder.RegisterGeneric(typeof(Commands.UpdateEntityCommandHandler<>)).AsImplementedInterfaces();
            builder.RegisterGeneric(typeof(Commands.UpdatePartialEntityCommandHandler<>)).AsImplementedInterfaces();
            builder.RegisterGeneric(typeof(Commands.CreateEntityCommandHandler<>)).AsImplementedInterfaces();
            builder.RegisterGeneric(typeof(Commands.DeleteEntityCommandHandler<>)).AsImplementedInterfaces();
        }
    }
}
