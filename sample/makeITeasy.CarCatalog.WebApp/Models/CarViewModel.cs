using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.CarCatalog.Models;

namespace makeITeasy.CarCatalog.WebApp.Models
{
    public class CarViewModel : IMapFrom<Car>
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public bool IsModernCar { get; set; }
    }
}
