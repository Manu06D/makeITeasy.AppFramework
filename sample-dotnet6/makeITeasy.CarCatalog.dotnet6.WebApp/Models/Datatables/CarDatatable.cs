using System;

using makeITeasy.AppFramework.Web.Models;

namespace makeITeasy.CarCatalog.dotnet6.WebApp.Models.Datatables
{
    public class CarDatatable : DatatableBaseConfiguration<CarDatatableSearchViewModel, CarDatatableViewModel>
    {
        public CarDatatable(string url) : base(url)
        {
            Options.PageLength = 5;
            Options.LoadOnDisplay = true;
            Options.ActivateDoubleClickOnRow = true;
            Options.EnablePaging = true;
            Options.Responsive = true;
        }
    }
}
