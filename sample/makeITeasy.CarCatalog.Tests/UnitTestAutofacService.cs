using Autofac;
using Autofac.Core;

namespace makeITeasy.CarCatalog.Tests
{
    public class UnitTestAutofacService<T> where T : IModule, new()
    {
        protected IContainer container;

        public UnitTestAutofacService()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new T());
            container = builder.Build();
        }

        protected TEntity Resolve<TEntity>()
        {
            return container.Resolve<TEntity>();
        }
    }
}
