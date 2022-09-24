
using Newtonsoft.Json;

namespace makeITeasy.CarCatalog.Tests
{
    public static class TestHelper
    {
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
