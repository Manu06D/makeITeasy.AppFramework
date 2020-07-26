using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.CarCatalog.Models;

namespace makeITeasy.CarCatalog.WebApp.Models
{
    public class CarEditViewModel : IMapFrom<Car>
    {
        public long Id { get; set; }

        [Required]
        [MinLength(3)]
        public String Name { get; set; }

        public List<Brand> Brands { get; set; }

        public int BrandID { get; set; }

        public int ReleaseYear { get; set; }
    }
}
