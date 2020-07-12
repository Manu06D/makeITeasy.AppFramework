using System;
using System.Collections.Generic;
using AutoMapper;
using makeITeasy.AppFramework.Core.Interfaces;

namespace makeITeasy.AppFramework.Core.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            List<Type> types = ReflectionHelper.GetTypeListFromAssemblies(AppDomain.CurrentDomain.GetAssemblies(), typeof(IMapFrom<>));

            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type);

                var methodInfo = type.GetMethod("Mapping") ?? type?.GetInterface("IMapFrom`1")?.GetMethod("Mapping");

                methodInfo?.Invoke(instance, new object[] { this });
            }
        }
    }
}
