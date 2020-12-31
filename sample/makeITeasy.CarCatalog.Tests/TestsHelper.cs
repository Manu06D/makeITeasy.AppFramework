using Newtonsoft.Json;

namespace makeITeasy.CarCatalog.Tests
{
    public static class TestsHelper
    {
        public static T Clone<T>(T entity)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(entity, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
        }
    }
}
