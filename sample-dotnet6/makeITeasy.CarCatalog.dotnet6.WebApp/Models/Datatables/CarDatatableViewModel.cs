using System;

using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.AppFramework.Web.Attributes;
using makeITeasy.CarCatalog.dotnet6.Models;

namespace makeITeasy.CarCatalog.dotnet6.WebApp.Models.Datatables
{
    public class CarDatatableViewModel : IMapFrom<Car>
    {
        [TableColumn(Name = nameof(ID), Title = "ID", IsRowId = true, Priority = 1, Visible = false)]
        public long ID { get; set; }

        [TableColumn(Name = nameof(Name), Title = "Name", Priority = 2)]
        public string Name { get; set; }

        [TableColumn(Name = nameof(BrandName), Title = "Brand", SortDataSource = "Brand.Name", Priority = 3)]

        public string BrandName { get; set; }

        [TableColumn(Name = nameof(ReleaseYear), Title = "Year", SortDataSource = "Brand.Country.Name")]

        public string ReleaseYear { get; set; }

        [TableColumn(Name = nameof(BrandCountryName), Title = "Country", SortDataSource = "Brand.Country.Name")]

        public string BrandCountryName { get; set; }

        [TableColumn(Name = nameof(IsModernCar), Title = "Is Actual", Sortable = false, Priority = 4)]
        public bool IsModernCar { get; set; }

        [TableColumn(Name = nameof(IsItalianCar), Title = "Is Italian", Sortable = false, Priority = 5)]
        public bool IsItalianCar { get; set; }

        [TableColumn(Name = nameof(Edit), Title = "", Priority = 2)]
        public string Edit { get; set; } = string.Empty;
    }
}
