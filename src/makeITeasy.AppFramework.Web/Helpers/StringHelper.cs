using System;

namespace makeITeasy.AppFramework.Web.Helpers
{
    public static class StringHelper
    {
        public static String ToCamelCase(this string input)
        {
            if (!String.IsNullOrEmpty(input))
            {
                if (input.Length <= 2)
                {
                    return input?.ToLowerInvariant();
                }

                if (Char.IsLetter((char)input[0]))
                {
                    return String.Concat(Char.ToLowerInvariant(input[0]), input.Substring(1));
                }
            }

            return input;
        }
    }
}
