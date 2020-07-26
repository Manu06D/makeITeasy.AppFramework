﻿using makeITeasy.AppFramework.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace makeITeasy.AppFramework.Web.Areas.Datatable
{
    [ViewComponent(Name = "Datatable")]
    public class DatatablViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(IDatatableBaseConfiguration configuration)
        {
            return View(configuration);
        }
    }
}
