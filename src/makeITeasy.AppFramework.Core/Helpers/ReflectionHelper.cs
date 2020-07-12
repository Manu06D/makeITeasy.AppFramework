using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace makeITeasy.AppFramework.Core.Helpers
{
    public static class ReflectionHelper
    {
        public static List<Type> GetTypeListFromAssembly(Assembly assembly, Type type)
        {
            List<Type> output = new List<Type>();
            if (assembly != null)
            {
                output = assembly.GetExportedTypes()
                    .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == type)).ToList();
            }

            return output;
        }

        public static List<Type> GetTypeListFromAssemblies(IEnumerable<Assembly> assemblies, Type type)
        {
            return assemblies.Where(x => !x.IsDynamic).SelectMany(x => GetTypeListFromAssembly(x, type)).ToList();
        }
    }
}
