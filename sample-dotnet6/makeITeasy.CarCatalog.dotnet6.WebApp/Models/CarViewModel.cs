using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.CarCatalog.dotnet6.Models;

namespace makeITeasy.CarCatalog.dotnet6.WebApp.Models
{
    public class CarViewModel : IMapFrom<Car>
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public bool IsModernCar { get; set; }
    }
}
