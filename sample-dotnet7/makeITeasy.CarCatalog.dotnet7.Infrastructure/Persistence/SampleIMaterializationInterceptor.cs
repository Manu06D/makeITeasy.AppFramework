using makeITeasy.CarCatalog.dotnet7.Models;

using Microsoft.EntityFrameworkCore.Diagnostics;

namespace makeITeasy.CarCatalog.dotnet7.Infrastructure.Persistence
{
    public class SampleIMaterializationInterceptor : IMaterializationInterceptor
    {
        /// <summary>
        /// Has to be registered with AddInterceptors
        /// Will be instanciated every time an entity will be created
        /// </summary>
        /// <param name="materializationData"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public object InitializedInstance(MaterializationInterceptionData materializationData, object entity)
        {
            if(entity is Car car)
            {
                //do stuff
            }

            return entity;
        }
    }
}
