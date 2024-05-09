
namespace makeITeasy.CarCatalog.dotnet8.Models
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
    }
}
