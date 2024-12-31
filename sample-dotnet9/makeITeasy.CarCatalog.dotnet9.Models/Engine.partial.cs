
namespace makeITeasy.CarCatalog.dotnet9.Models
{
    public partial class Engine
    {
        public object DatabaseID => Id;

        public EngineDetails Details { get; set; }
    }

    public class EngineDetails
    {
        public int PowerHorse { get; set; }
        public bool HasTurbo { get; set; }
        public List<Characteristic> Characteristics { get; set; }
    }

    public class Characteristic
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
