using System.Reflection;

namespace makeITeasy.AppFramework.Models
{
    public class AppFrameworkModels
    {
        public static Assembly Assembly
        {
            get => typeof(AppFrameworkModels).Assembly;
        }
    }
}
