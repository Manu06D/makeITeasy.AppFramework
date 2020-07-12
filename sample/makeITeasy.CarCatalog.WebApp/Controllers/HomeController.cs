using System.Threading.Tasks;
using makeITeasy.CarCatalog.Core.Domains.CarDomain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace makeITeasy.CarCatalog.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICarService _carService;

        public HomeController(ILogger<HomeController> logger, ICarService carService)
        {
            _logger = logger;
            _carService = carService;
        }

        public async Task<IActionResult> Index()
        {
            var car = 
                await _carService.GetByID((long)1, 
                new System.Collections.Generic.List<System.Linq.Expressions.Expression<System.Func<CarCatalog.Models.Car, object>>>()
                { x => x.Brand.Country }
                );

            return View();
        }
    }
}
