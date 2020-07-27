using System;
using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.AppFramework.Web.Attributes;
using makeITeasy.CarCatalog.Models;

namespace makeITeasy.CarCatalog.WebApp.Models.Datatables
{
    public class CarDatatableViewModel : IMapFrom<Car>
    {
        [TableColumn(Name = nameof(ID), Title = "ID", IsRowId = true, Visible = true)]
        public long ID { get; set; }

        [TableColumn(Name = nameof(Name), Title = "Nom")]
        public String Name { get; set; }

        [TableColumn(Name = nameof(BrandName), Title = "Marque", SortDataSource = "Brand.Name")]

        public String BrandName { get; set; }

        [TableColumn(Name = nameof(BrandCountryCountryCode), Title = "Pays", SortDataSource = "Brand.Country.CountryCode")]

        public String BrandCountryCountryCode { get; set; }

        [TableColumn(Name = nameof(IsModernCar), Title = "Actual", Sortable = true)]
        public bool IsModernCar { get; set; }

        [TableColumn(Name = nameof(Edit), Title = "")]
        public String Edit { get; set; } = String.Empty;
    }
}
