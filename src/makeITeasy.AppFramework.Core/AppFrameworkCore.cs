using System.Reflection;

namespace makeITeasy.AppFramework.Core
{
    public class AppFrameworkCore
    {
        public static Assembly Assembly
        {
            get => typeof(AppFrameworkCore).Assembly;
        }
    }
}
