
using System;

namespace makeITeasy.AppFramework.Core.Helpers
{
    public static class TypeFinder
    {
        public static Type? FindType(string typeFullName)
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type? type = asm.GetType(typeFullName, throwOnError: false, ignoreCase: false);

                if (type != null)
                {
                    return type;
                }
            }

            return null;
        }
    }
}
