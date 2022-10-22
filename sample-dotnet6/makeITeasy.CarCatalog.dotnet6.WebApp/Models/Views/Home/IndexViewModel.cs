using Microsoft.AspNetCore.Mvc.Rendering;

namespace makeITeasy.CarCatalog.dotnet6.WebApp.Models.Views.Home
{
    public class IndexViewModel
    {
        public List<SelectListItem> AllBrandNames { get; set; }

        public List<SelectListItem> AllBrandIds { get; set; }
    }
}
