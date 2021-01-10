using System;
using makeITeasy.AppFramework.Web.Models;

namespace makeITeasy.CarCatalog.WebApp.Models.Datatables
{
    public class CarDatatable : DatatableBaseConfiguration<CarDatatableSearchViewModel, CarDatatableViewModel>
    {
        public CarDatatable(String url) : base(url)
        {
            this.Options.PageLength = 5;
            this.Options.LoadOnDisplay = true;
            this.Options.ActivateDoubleClickOnRow = true;
            this.Options.EnablePaging = true;
            this.Options.Responsive = true;
        }
    }
}
