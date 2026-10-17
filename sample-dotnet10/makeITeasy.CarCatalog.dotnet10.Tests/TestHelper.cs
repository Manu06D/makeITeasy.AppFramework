
using Newtonsoft.Json;

namespace makeITeasy.CarCatalog.dotnet10.Tests
{
    public static class TestHelper
    {
        public static string UniqueSuffix()
        {
            return TimeOnly.FromDateTime(DateTime.Now).ToString("ssfffffff") + Random.Shared.Next(1000).ToString("000");
        }

        public static T Clone<T>(T entity)
        {
            return JsonConvert.DeserializeObject<T>(Dump(entity));
        }

        public static string Dump(object entity)
        {
            return JsonConvert.SerializeObject(entity, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }
    }
}
