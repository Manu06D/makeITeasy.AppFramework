using System.Threading.Tasks;
using makeITeasy.AppFramework.Core.Queries;
using makeITeasy.AppFramework.Models;
using makeITeasy.CarCatalog.Core.Domains.CarDomain;
using makeITeasy.CarCatalog.Core.Domains.CarDomain.Queries;
using makeITeasy.CarCatalog.Models;
using makeITeasy.CarCatalog.WebApp.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace makeITeasy.CarCatalog.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICarService _carService;
        private readonly IMediator _mediator;

        public HomeController(ILogger<HomeController> logger, ICarService carService, IMediator mediator)
        {
            _logger = logger;
            _carService = carService;
            this._mediator = mediator;

        }

        public async Task<IActionResult> Index()
        {
            var car =
                await _carService.GetByID((long)1,
                new System.Collections.Generic.List<System.Linq.Expressions.Expression<System.Func<CarCatalog.Models.Car, object>>>()
                { x => x.Brand.Country }
                );

            var baseQuery = new BaseCarQuery() { };

            var output = await _mediator.Send(new GenericQueryWithProjectCommand<Car, CarViewModel>(baseQuery));


            return View();
        }
    }
}
