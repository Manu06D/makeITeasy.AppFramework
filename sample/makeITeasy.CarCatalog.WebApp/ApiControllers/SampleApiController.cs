using makeITeasy.CarCatalog.WebApp.WebAppElements.Attributes;

using Microsoft.AspNetCore.Mvc;

namespace makeITeasy.CarCatalog.WebApp.ApiControllers
{
    [ApiKey]
    public class SampleApiController : ControllerBase
    {
        public IActionResult Hello()
        {
            return new JsonResult("World");
        }
    }
}
