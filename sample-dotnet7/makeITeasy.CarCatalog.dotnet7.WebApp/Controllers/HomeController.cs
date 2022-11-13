using AutoMapper;

using makeITeasy.AppFramework.Core.Commands;
using makeITeasy.AppFramework.Core.Queries;
using makeITeasy.CarCatalog.dotnet7.Models;
using makeITeasy.CarCatalog.dotnet7.WebApp.Models;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;
using makeITeasy.CarCatalog.dotnet7.Core.Services.Queries.BrandQueries;
using makeITeasy.CarCatalog.dotnet7.Core.Services.Queries.CarQueries;

namespace makeITeasy.CarCatalog.dotnet7.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public HomeController(ILogger<HomeController> logger, IMediator mediator, IMapper mapper)
        {
            _logger = logger;
            _mediator = mediator;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult IndexWithBlazor()
        {
            return View();
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id > 0)
            {
                CarEditViewModel model =
                    await _mediator.Send(new GenericFindUniqueWithProjectCommand<Car, CarEditViewModel>(
                            new BaseCarQuery() { ID = id, Includes = new List<System.Linq.Expressions.Expression<Func<Car, object>>>() { x => x.Brand } }));

                QueryResult<Brand> output = await _mediator.Send(new GenericQueryCommand<Brand>(new BaseBrandQuery()));

                model.Brands = output.Results.ToList();

                return base.PartialView(model);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] CarEditViewModel model)
        {
            //if (ModelState.IsValid)
            //{
                CommandResult<Car> result = await _mediator.Send(new UpdateEntityCommand<Car>(_mapper.Map<Car>(model)));

                if (result.Result == CommandState.Success)
                {
                    return Ok(model);
                }
            //}

            return StatusCode(StatusCodes.Status500InternalServerError, "An error has occured");
        }

        public async Task<IActionResult> CarDetails(int id)
        {
            Car result = await _mediator.Send(new GenericFindUniqueCommand<Car>(new BaseCarQuery() { ID = id, IncludeStrings = new List<string>() { "Brand" } }));

            return PartialView(result);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}