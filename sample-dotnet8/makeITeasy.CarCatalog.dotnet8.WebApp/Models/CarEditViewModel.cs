using System.ComponentModel.DataAnnotations;

using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.CarCatalog.dotnet8.Models;

namespace makeITeasy.CarCatalog.dotnet8.WebApp.Models
{
    public class CarEditViewModel : IMapFrom<Car>
    {
        public long Id { get; set; }

        [Required]
        [MinLength(3)]
        public string Name { get; set; }

        public List<Brand> Brands { get; set; }

        public int BrandID { get; set; }

        public int ReleaseYear { get; set; }
        public byte[] RowVersion { get; set; }
        public string DatabaseRowVersion
        {
            get { return Convert.ToBase64String(RowVersion); }
            set { RowVersion = Convert.FromBase64String(value); }
        }
    }
}
